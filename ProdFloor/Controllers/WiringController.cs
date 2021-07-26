using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Wiring;

namespace ProdFloor.Controllers
{
    public class WiringController : Controller
    {
        private IJobRepository jobRepo;
        private IWiringRepository wiringRepo;
        private IItemRepository itemRepo;
        private ITestingRepository testRepo;
        private UserManager<AppUser> userManager;
        private IHostingEnvironment _env;
        public int PageSize = 7;
        string appDataFolder => _env.WebRootPath.ToString() + @"\AppData\";

        public WiringController(IWiringRepository repo,
            IJobRepository repo2,
            IItemRepository repo3,
            ITestingRepository repo4,
            UserManager<AppUser> userMgr,
            IHostingEnvironment env)
        {
            jobRepo = repo2;
            wiringRepo = repo;
            itemRepo = repo3;
            userManager = userMgr;
            testRepo = repo4;
            _env = env;
        }

        public ViewResult List(int page = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;

            IQueryable<Wiring> wiringJobList = wiringRepo.Wirings
                .Where(m => m.WirerID == currentUser.EngID);

            IQueryable<PO> poList = jobRepo.POs.Where(m =>
                wiringJobList.Any(n => n.POID == m.POID));

            IQueryable<Job> jobList = jobRepo.Jobs.Where(m =>
                poList.Any(n => n.JobID == m.JobID));


            WiringViewModel viewModel = new WiringViewModel
            {
                WiringJobList = wiringJobList.OrderBy(p => p.WirerID)
                                .Skip((page - 1) * PageSize)
                                .Take(PageSize).ToList(),
                JobList = jobList.ToList(),
                JobTypeList = itemRepo.JobTypes.ToList(),
                StationsList = testRepo.Stations.ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = wiringJobList.Count()
                }
            };

            return View(viewModel);
        }

        public IActionResult NewWiringJob(int PONumb)
        {
            WiringViewModel viewModel = new WiringViewModel();
            DashboardIndexViewModel viewHomeModel = new DashboardIndexViewModel();
            AppUser currentUser = GetCurrentUser().Result;
            viewHomeModel.POJobSearch = PONumb;

            PO po = jobRepo.POs.FirstOrDefault(m => m.PONumb == PONumb);
            if (po == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PO no existe";

                return RedirectToAction("SearchByPO", "Home", viewHomeModel);
            }

            Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == po.JobID);
            if (job == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el Job no existe";

                return RedirectToAction("SearchByPO", "Home", viewHomeModel);
            }

            viewModel.Job = job;
            viewModel.PO = po;
            viewModel.JobTypeName = itemRepo.JobTypes.FirstOrDefault(m => m.JobTypeID == job.JobTypeID).Name;
            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == po.POID);

            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.POID == po.POID);
            if (wiring == null)
            {
                viewModel.WiringJob = new Wiring();
                viewModel.WiringJob.POID = po.POID;
            }
            else if (currentUser.EngID != wiring.WirerID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP a sido reasigando";

                return RedirectToAction("PXPDashboard");
            }
            else if (statusPO.Status == "Waiting for PXP")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el WiringJob con PO #{po.PONumb} ya esta terminado";

                return RedirectToAction("PXPDashboard");
            }
            else
            {
                viewModel.WiringJob = wiring;
            }


            return View(viewModel);
        }

        [HttpPost]
        public IActionResult NewWiringJob(WiringViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;

            viewModel.WiringJob.WirerID = currentUser.EngID;
            viewModel.WiringJob.StartDate = DateTime.Now;
            viewModel.WiringJob.CompletedDate = DateTime.Now;
            wiringRepo.SaveWiring(viewModel.WiringJob);

            viewModel.WiringJob = wiringRepo.Wirings.FirstOrDefault(m => m.POID == viewModel.WiringJob.POID);

            WirersInvolved involved = wiringRepo.WirersInvolveds
                                                 .Where(m => m.WiringID == viewModel.WiringJob.WiringID)
                                                 .FirstOrDefault(m => m.WirerID == currentUser.EngID);
            if (involved == null)
            {
                WirersInvolved wirersInvolved = new WirersInvolved();
                wirersInvolved.WiringID = viewModel.WiringJob.WiringID;
                wirersInvolved.WirerID = currentUser.EngID;
                wiringRepo.SaveWirersInvolved(wirersInvolved);
            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == viewModel.WiringJob.POID);

            statusPO.Status = "WR: Adding Features";
            jobRepo.SaveStatusPO(statusPO);

            viewModel.FeatureList = new List<WiringFeatures>();

            return View("AddFeatures", viewModel);
        }

        [HttpPost]
        public IActionResult AddFeature(WiringViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == viewModel.WiringJob.WiringID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);

            if (wiring == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el WiringJob no existe";

                return RedirectToAction("List");
            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);

            if (currentUser.EngID != wiring.WirerID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el WiringJob a sido reasigando";

                return RedirectToAction("List");
            }
            else if (statusPO.Status == "Waiting for PXP")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el WiringJob con PO #{po.PONumb} ya esta terminado";

                return RedirectToAction("List");
            }

            viewModel.Feature.WiringID = wiring.WiringID;
            wiringRepo.SaveWiringFeatures(viewModel.Feature);

            WirersInvolved involved = wiringRepo.WirersInvolveds
                                                 .Where(m => m.WiringID == wiring.WiringID)
                                                 .FirstOrDefault(m => m.WirerID == currentUser.EngID);
            if (involved == null)
            {
                WirersInvolved wirersInvolved = new WirersInvolved();
                wirersInvolved.WiringID = wiring.WiringID;
                wirersInvolved.WirerID = currentUser.EngID;
                wiringRepo.SaveWirersInvolved(wirersInvolved);
            }

            viewModel.WiringJob = wiring;
            viewModel.PO = po;
            viewModel.Job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == po.JobID);
            viewModel.FeatureList = wiringRepo.WiringFeatures.Where(m => m.WiringID == wiring.WiringID).ToList();

            TempData["message"] = $"Nueva caracteristica añadida con exito.";
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SaveFeatures(WiringViewModel viewModel)
        {
            bool Admin = GetCurrentUserRole("Admin").Result;
            bool productionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            int wirerID = GetCurrentUser().Result.EngID;

            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == viewModel.WiringJob.WiringID);

            if(wiring == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El WiringJob no existe, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

            if (wiring.WirerID != wirerID && !productionAdmin && !Admin)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El WiringJob fue reasignado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

            List<WiringStepForJob> OldStepsForJob = wiringRepo.WiringStepsForJobs.Where(m => m.WiringID == wiring.WiringID).ToList();
            if (OldStepsForJob.Count == 0)
            {
                foreach (WiringStepForJob step in MakeStepsForJobList(viewModel))
                {
                    if (step != null) 
                            wiringRepo.SaveWiringStepForJob(step);
                }

            }
            else
            {
                List<WiringStepForJob> NewStepsForJob = MakeStepsForJobList(viewModel);

                foreach (WiringStepForJob OldStep in OldStepsForJob)
                {
                    if (!(NewStepsForJob.Any(s => s.WiringStepID == OldStep.WiringStepID)))
                    {
                        OldStep.Obsolete = true;
                        wiringRepo.SaveWiringStepForJob(OldStep);
                    }
                }

                foreach (WiringStepForJob Newstep in NewStepsForJob)
                {
                    if (!(OldStepsForJob.Any(s => s.WiringStepID == Newstep.WiringStepID)))
                    {
                        wiringRepo.SaveWiringStepForJob(Newstep);
                    }
                }

                List<WiringStepForJob> StepsForJobUpdated = wiringRepo.WiringStepsForJobs.Where(m => m.WiringID == wiring.WiringID).ToList();
                int PreviusStepNumber = 1;
                foreach (WiringStepForJob step in StepsForJobUpdated)
                {
                    step.Consecutivo = PreviusStepNumber;
                    wiringRepo.SaveWiringStepForJob(step);
                    PreviusStepNumber++;
                }
            }

            WirersInvolved involved = wiringRepo.WirersInvolveds
                                                 .Where(m => m.WiringID == wiring.WiringID)
                                                 .FirstOrDefault(m => m.WirerID == wirerID);
            if (involved == null)
            {
                WirersInvolved wirersInvolved = new WirersInvolved();
                wirersInvolved.WiringID = wiring.WiringID;
                wirersInvolved.WirerID = wirerID;
                wiringRepo.SaveWirersInvolved(wirersInvolved);
            }

            return View(viewModel);
        }

        public List<WiringStepForJob> MakeStepsForJobList(WiringViewModel viewModel)
        {
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == viewModel.WiringJob.WiringID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);
            Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == po.JobID);

            List<WiringStepForJob> stepsForJob = new List<WiringStepForJob>();
            List<WiringFeatures> features = wiringRepo.WiringFeatures.Where(m => m.WiringID == wiring.WiringID).ToList(); 
            List<WiringStep> steps = wiringRepo.WiringSteps.Where(m => m.JobTypeID == job.JobTypeID).OrderBy(m => m.Order).ToList();
            int consecutivo = 1;

            if (steps == null || steps.Count == 0)
                return stepsForJob;

            foreach(WiringStep step in steps)
            {
                List<WiringTriggeringFeature> triggers = wiringRepo.WiringTriggeringFeatures
                                                                   .Where(m => m.WiringStepID == step.WiringStepID)
                                                                   .ToList();

                if(triggers.Count == 0 || (triggers.Count == 1 && triggers[0].WiringOptionID == 0 ))
                {
                    WiringStepForJob stepForJob = new WiringStepForJob
                    {
                        WiringStepID = step.WiringStepID,
                        WiringID = wiring.WiringID,
                        Start = DateTime.Now,
                        Stop = DateTime.Now,
                        Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                        Consecutivo = consecutivo,
                        AuxStationID = wiring.StationID,
                        AuxWirerID = wiring.WirerID
                    };

                    stepsForJob.Add(stepForJob);
                    consecutivo++;
                }
                else
                {
                    int triggersExpected = triggers.Count;
                    int triggersCounted = 0;

                    foreach(WiringTriggeringFeature trigger in triggers)
                    {
                        if (trigger.IsSelected == true && features.Any(m => m.WiringOptionID == trigger.WiringOptionID))
                            triggersCounted++;
                        else if (trigger.IsSelected == false && features.Any(m => m.WiringOptionID == trigger.WiringOptionID == false))
                            triggersCounted++;
                        else
                            continue;
                    }

                    if(triggersExpected == triggersCounted)
                    {
                        WiringStepForJob stepForJob = new WiringStepForJob
                        {
                            WiringStepID = step.WiringStepID,
                            WiringID = wiring.WiringID,
                            Start = DateTime.Now,
                            Stop = DateTime.Now,
                            Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                            Consecutivo = consecutivo,
                            AuxStationID = wiring.StationID,
                            AuxWirerID = wiring.WirerID
                        };

                        stepsForJob.Add(stepForJob);
                        consecutivo++;
                    }
                }
            }



            return stepsForJob;
        }

        public IActionResult ContinueStep(int wiringID)
        {
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == wiringID);
            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);

            if (wiring == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El WiringJob no existe, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

            bool Admin = GetCurrentUserRole("Admin").Result;
            bool productionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            bool isNotSameEngineer = GetCurrentUser().Result.EngID != wiring.WirerID;
            bool istCompleted = statusPO.Status == "Waiting for PXP";

            if (istCompleted || (isNotSameEngineer &&  !productionAdmin && !Admin ))
            {
                if(istCompleted) 
                    TempData["message"] = $"Error, El WiringJob ya esta completado, intente de nuevo o contacte al Admin";
                else 
                    TempData["message"] = $"Error, El WiringJob fue reasignado, intente de nuevo o contacte al Admin";

                TempData["alert"] = $"alert-danger";
                return RedirectToAction("Index", "Home");
            }

            List<WiringStepForJob> StepsForJob = wiringRepo.WiringStepsForJobs.Where(m => m.WiringID == wiring.WiringID).ToList();
            List<WiringStop> AllStopsFromWiringJob = wiringRepo.WiringStops.Where(m => m.WiringID == wiring.WiringID).ToList();
            List<WiringStop> StopsFromWiringJob = wiringRepo.WiringStops.Where(m => m.WiringID == wiring.WiringID && m.Critical == false)
                                                                   .Where(m => m.Reason1 != 980 & m.Reason1 != 981 && m.Reason2 == 0).ToList();

            if (!StepsForJob.Any(m => m.Complete == true) && AllStopsFromWiringJob.Count == 0)
            {

            }


            return View();
        }

        ///Aditional functions
        private async Task<bool> GetCurrentUserRole(string role)
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            bool isInRole = await userManager.IsInRoleAsync(user, role);

            return isInRole;
        }

        private async Task<AppUser> GetCurrentUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            return user;
        }
    }
}
