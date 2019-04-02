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

                //Rellena las listas que se llenaran para la comparacion
                List<Step> Steps = testingRepo.Steps.OrderBy(m => m.Order).ToList();
                List<TriggeringFeature> TriggersWithNameNull = testingRepo.TriggeringFeatures.Where(m => m.Name == null).ToList();
                List<TriggeringFeature> TriggersWithOutNameNull = testingRepo.TriggeringFeatures.Where(m => m.Name != null).ToList();
                var JobFeatures = testingRepo.TestFeatures.FirstOrDefault(m => m.TestJobID == testJobView.TestFeature.TestJobID);
                //Checa si la lista de steps no esta vacia
                if (Steps.Count > 0)
                {
                    //inicia el contador del consecutivo
                    int consecutivo = 1;

                    //Checa cada step de la lista
                    foreach (Step step in Steps)
                    {
                        //Obtiene el primer trigger del step actual step
                        TriggeringFeature TriggerInStep = step._TriggeringFeatures.FirstOrDefault();

                        //si su name es nulo significa que es un step por default, debido a esto lo agrega a step for Job
                        if (TriggerInStep.Name == null)
                        {
                            StepsForJob stepForJob = new StepsForJob
                            {
                                StepID = step.StepID,
                                TestJobID = JobFeatures.TestJobID,
                                Start = DateTime.Now,
                                Stop = DateTime.Now,
                                Elapsed = new DateTime(1, 1, 1),
                                Consecutivo = consecutivo
                            };

                            testingRepo.SaveStepsForJob(stepForJob);
                            consecutivo++;
                        }
                        /*si su name no es nulo significa que es un trigger optativo, debido a esto se comparara sus features con los del job
                        y si concuerdan se anadira a steps for job*/
                        else if (TriggerInStep.Name != null)
                        {
                            //Crea una lista con todos los triggers del step actual
                            var triggers = testingRepo.TriggeringFeatures.Where(m => m.StepID == step.StepID).ToList();
                            //checa que la lista de triggers no este vacia
                            if (triggers.Count > 0)
                            {
                                int count = triggers.Count;
                                int countAux = 0;
                                //Checa que cada feature de la lista concuerde con los features del testjob
                                foreach (TriggeringFeature trigger in triggers)
                                {
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
                                        default: break;
                                    }
                                }
                                //Si se vuelve valido agrega el step a la lista de steps for job
                                if (count == countAux)
                                {
                                    StepsForJob stepForJob = new StepsForJob
                                    {
                                        StepID = step.StepID,
                                        TestJobID = JobFeatures.TestJobID,
                                        Start = DateTime.Now,
                                        Stop = DateTime.Now,
                                        Elapsed = new DateTime(1, 1, 1),
                                        Consecutivo = consecutivo
                                    };
                                    testingRepo.SaveStepsForJob(stepForJob);
                                    consecutivo++;
                                }
                            }
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
                //Despues de terminar de hacer la lista de steps para job se manda el primero a la siguiente vista
                var stepsFor = testingRepo.StepsForJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestFeature.TestJobID && m.Consecutivo == 1);
                var stepInfo = testingRepo.Steps.FirstOrDefault(m => m.StepID == stepsFor.StepID);
                var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestJob.TestJobID);
                var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);
                return View("StepsForJob",new TestJobViewModel { StepsForJob = stepsFor, Step = stepInfo, Job = job, TestJob = testjobinfo});
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
                var currentStepForJob = StepsForJobList.FirstOrDefault(m => m.Consecutivo == viewModel.StepsForJob.Consecutivo);
                currentStepForJob.Complete = true; testingRepo.SaveStepsForJob(currentStepForJob); 
                var nextStepFor = StepsForJobList.FirstOrDefault(m => m.Consecutivo == next);
                var stepInfo = testingRepo.Steps.FirstOrDefault(m => m.StepID == nextStepFor.StepID);
                var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == viewModel.TestJob.TestJobID);
                var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);
                return View("StepsForJob", new TestJobViewModel { StepsForJob = nextStepFor, Step = stepInfo, Job = job, TestJob = testjobinfo });

            }
            else if(viewModel.StepsForJob.Consecutivo == (next + 1))
            {
                var currentStepForJob = StepsForJobList.FirstOrDefault(m => m.Consecutivo == viewModel.StepsForJob.Consecutivo);
                var previusStepFor = StepsForJobList.FirstOrDefault(m => m.Consecutivo == next && m.Complete == true);
                var stepInfo = testingRepo.Steps.FirstOrDefault(m => m.StepID == previusStepFor.StepID);
                var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == viewModel.TestJob.TestJobID);
                var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);
                return View("StepsForJob", new TestJobViewModel { StepsForJob = previusStepFor, Step = stepInfo, Job = job, TestJob = testjobinfo });
            }
            else if (next == 777)
            {
                ///Va a mandar a la pagina de Stops
                return View("StepsForJob", viewModel);
            }
            else if( next > StepsForJobList.Count())
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