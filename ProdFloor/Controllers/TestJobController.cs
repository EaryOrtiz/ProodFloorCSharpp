using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.TestJob;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TestJobController : Controller
    {
        private IJobRepository jobRepo;
        private ITestingRepository testingRepo;
        private UserManager<AppUser> userManager;
        public int PageSize = 10;

        public TestJobController(ITestingRepository repo, IJobRepository repo2, UserManager<AppUser> userMgr)
        {
            jobRepo = repo2;
            testingRepo = repo;
            userManager = userMgr;
        }

        public ViewResult List(int page = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            TestJobViewModel testJobView = new TestJobViewModel
            {
                TestJobList = testingRepo.TestJobs
                .Where(m => m.TechnicianID == currentUser.EngID)
                .OrderBy(p => p.TechnicianID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = testingRepo.TestJobs.Count()
                }
            };
            return View(testJobView);
        }

        public ViewResult NewTestJob()
        {
            return View(new TestJobViewModel());
        }

        [HttpPost]
        public IActionResult SearchJob(TestJobViewModel viewModel)
         {
            AppUser currentUser = GetCurrentUser().Result;
            var jobSearch = jobRepo.Jobs.AsQueryable();
            TestJobViewModel testJobSearchAux = new TestJobViewModel { };

            if (viewModel.POJobSearch > 0)
            {
                var _jobSearch = jobSearch.First(m => m.PO == viewModel.POJobSearch);

                if (_jobSearch != null && _jobSearch.Status != "Incomplete")
                {

                    TestJob testJob = new TestJob {JobID = _jobSearch.JobID,TechnicianID = currentUser.EngID, Status = "Working on it"};
                    testingRepo.SaveTestJob(testJob);

                    var currentTestJob = testingRepo.TestJobs
                        .FirstOrDefault(p => p.TestJobID == testingRepo.TestJobs.Max(x => x.TestJobID));


                    TestJobViewModel testJobView = new TestJobViewModel
                    {
                        TestJob = currentTestJob,
                    };

                    return View("NewTestFeatures", testJobView);

                }
                else
                {
                    TempData["message"] = $"There seems to be errors in the form. Please validate....";
                    TempData["alert"] = $"alert-danger";
                    return RedirectToAction("NewTestFeatures", testJobSearchAux);
                }
            }
            else
            {
                return View(testJobSearchAux);
            }
        }

        [HttpPost]
        public IActionResult NewTestFeatures(TestJobViewModel testJobView)
        {
            //Checa que la lista de features no este vacia o nula
            if (testJobView.TestFeature != null)
            {
                //guarda la lista de features
                testingRepo.SaveTestFeature(testJobView.TestFeature);

                var Steps = testingRepo.Steps.GroupBy(m => m.Order).ToList();
                var JobFeatures = testingRepo.TestFeatures.FirstOrDefault(m => m.TestJobID == testJobView.TestFeature.TestJobID);
                //Checa si la lista de estps no esta vacia
                if (Steps.Count > 0)
                {
                    List<Step> StepList = new List<Step>();
                    //Se checa cada step que este en la lista
                    foreach (Step step in Steps)
                    {
                        var triggers = testingRepo.TriggeringFeatures.Where(m => m.StepID == step.StepID).ToList();
                        int consecutivo = 1;
                        //checa que la lista de triggers no este vacia
                        if (triggers.Count > 0)
                        {
                            bool stepIsValid = false;
                            int count = triggers.Count;
                            //Checa que cada feature de la lista concuerde con los features del testjob
                            foreach (TriggeringFeature trigger in triggers)
                            {
                                int countAux = 0;
                                switch (trigger.Name)
                                {
                                    case "Overlay": if (trigger.IsSelected == JobFeatures.Overlay) { countAux++; } break;
                                    case "Group": if (trigger.IsSelected == JobFeatures.Group) { countAux++; } break;
                                    case "PC": if (trigger.IsSelected == JobFeatures.PC) { countAux++; } break;
                                    case "BrakeCoilVoltageMoreThan10": if (trigger.IsSelected == JobFeatures.BrakeCoilVoltageMoreThan10) { countAux++; } break;
                                    case "MBrake": if (trigger.IsSelected == JobFeatures.MBrake) { countAux++; } break;
                                    case "EMCO": if (trigger.IsSelected == JobFeatures.EMCO) { countAux++; } break;
                                    case "R6": if (trigger.IsSelected == JobFeatures.R6) { countAux++; } break;
                                    case "Local": if (trigger.IsSelected == JobFeatures.Local) { countAux++; } break;
                                    case "ShortFloor": if (trigger.IsSelected == JobFeatures.ShortFloor) { countAux++; } break;
                                    case "Custom": if (trigger.IsSelected == JobFeatures.Custom) { countAux++; } break;
                                    case "MRL": if (trigger.IsSelected == JobFeatures.MRL) { countAux++; } break;
                                    case "CTL2": if (trigger.IsSelected == JobFeatures.CTL2) { countAux++; } break;
                                }
                                //Si la cantidad de features coinciden con la del los triggers entoces se volvera true
                                if (count == countAux) stepIsValid = true;
                            }
                            //Si se vuelve valido agrega el step a la lista de steps for job
                            if (stepIsValid == true)
                            {
                                StepsForJob stepForJob = new StepsForJob
                                {
                                    StepID = step.StepID,
                                    TestJobID = JobFeatures.TestJobID,
                                    Start = DateTime.Now,
                                    Stop = DateTime.Now,
                                    Elapsed = new DateTime(0, 0, 0),
                                    Consecutivo = consecutivo
                                };

                                testingRepo.SaveStepsForJob(stepForJob);
                            }
                        }
                    }
                }
                //Despues de terminar de hacer la lista de steps para job se manda el primero a la siguiente vista
                var stepsFor = testingRepo.StepsForJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestFeature.TestJobID && m.Consecutivo == 1);
                var stepAux = testingRepo.Steps.FirstOrDefault(m => m.StepID == stepsFor.StepID);
                var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJobView.TestJob.JobID);
                return View("StepsForJob",new TestJobViewModel { StepsForJob = stepsFor, Step = stepAux, Job = job});
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult StepsForJob(TestJobViewModel viewModel, int next)
        {
            var StepsForJobList = testingRepo.StepsForJobs.Where(m => m.TestJobID == viewModel.TestFeature.TestJobID).ToList();
            if (next == 0)
            {
                return View("StepsForJob", viewModel);

            }else if (viewModel.StepsForJob.Consecutivo == (next - 1))
            {
                var currentStepForJob = StepsForJobList.FirstOrDefault(m => m.Consecutivo == viewModel.StepsForJob.Consecutivo); currentStepForJob.Complete = true;
                var nextStepFor = StepsForJobList.FirstOrDefault(m => m.Consecutivo == next && m.Complete == false);
                var stepAux = testingRepo.Steps.FirstOrDefault(m => m.StepID == nextStepFor.StepID);
                var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == viewModel.TestJob.JobID);
                return View("StepsForJob", new TestJobViewModel { StepsForJob = nextStepFor, Step = stepAux, Job = job });

            }else if(viewModel.StepsForJob.Consecutivo == (next + 1))
            {

            }else if( next > StepsForJobList.Count())
            {
                //******Se setearan varias cosas del testJob
                //Imprimir ahora si las tablas
                return RedirectToAction(nameof(List));
            }
            
            return View(NotFound());
        }

        private async Task<AppUser> GetCurrentUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            return user;
        }

    }
}