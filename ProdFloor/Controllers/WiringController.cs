using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        public IActionResult AdminDashboard(string Clean, string jobNumber, string jobnumb = "", int MyJobsPage = 1, int PendingToCrossJobPage = 1, int OnCrossJobPage = 1)
        {
            if (!string.IsNullOrEmpty(jobnumb)) jobNumber = jobnumb;
            if (!string.IsNullOrEmpty(jobNumber)) jobnumb = jobNumber;
            if (!string.IsNullOrEmpty(Clean))
            {
                RedirectToAction("SearchTestJob", "TestJob");
                jobnumb = "";
            }

            List<Wiring> WiringsOnProgress = new List<Wiring>();
            List<Wiring> WiringsCompleted = new List<Wiring>();
            List<Wiring> WiringsToBeStart = new List<Wiring>();
            List<Wiring> wiringList = new List<Wiring>();
            List<Wiring> PoList = new List<Wiring>();
            List<Wiring> StatusPOList = new List<Wiring>();
            List<Job> jobList = jobRepo.Jobs.Where(m => m.JobNum.Contains(jobnumb)).ToList();

            foreach (Job job in jobList)
            {
                PO poByJob = jobRepo.POs.FirstOrDefault(m => m.JobID == job.JobID);
                Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.POID == poByJob.POID);

                if (wiring != null) wiringList.Add(wiring);
            }

            //WiringsToBeStart
            List<StatusPO> statusPOsToBeStart = jobRepo.StatusPOs.Where(m => m.Status == "WR: Adding features").ToList();

            //WiringsOnProgress
            List<StatusPO> statusPOsOnProgress = jobRepo.StatusPOs.Where(s => s.Status == "Wiring on progress" || s.Status == "WR: Reassignment"
                                                                           || s.Status == "WR: Stopped" || s.Status == "WR: Shift End").ToList();

            //WiringsCompleted
            List<StatusPO> statusPOsCompleted = jobRepo.StatusPOs.Where(m => m.Status == "Waiting for PXP" || m.Status == "PXP on progress").ToList();

            if (wiringList != null && wiringList.Count > 0)
            {
                WiringsToBeStart = wiringList.Where(m => statusPOsToBeStart.Any(n => n.POID == m.POID)).ToList();

                WiringsOnProgress = wiringList.Where(m => statusPOsOnProgress.Any(n => n.POID == m.POID)).ToList();

                WiringsCompleted = wiringList.Where(m => statusPOsCompleted.Any(n => n.POID == m.POID))
                                             .Where(m => (m.CompletedDate.AddDays(-2) < (DateTime.Now))).OrderBy(s => s.CompletedDate).ToList();

            }
            else
            {
                WiringsToBeStart = wiringRepo.Wirings.Where(m => statusPOsToBeStart.Any(n => n.POID == m.POID)).ToList();

                WiringsOnProgress = wiringRepo.Wirings.Where(m => statusPOsOnProgress.Any(n => n.POID == m.POID)).ToList();

                WiringsCompleted = wiringRepo.Wirings.Where(m => statusPOsToBeStart.Any(n => n.POID == m.POID))
                                             .Where(m => (m.CompletedDate.AddDays(-2) < (DateTime.Now))).OrderBy(s => s.CompletedDate).ToList();
            }

            WiringViewModel viewModel = new WiringViewModel
            {
                WiringJobIncompletedList = WiringsToBeStart
                    .OrderBy(p => p.WirerID)
                    .Skip((MyJobsPage - 1) * 5)
                    .Take(5).ToList(),
                WiringJobWorkingOnItList = WiringsOnProgress
                    .OrderBy(p => p.WirerID)
                    .Skip((PendingToCrossJobPage - 1) * 5)
                    .Take(5).ToList(),
                WiringJobCompletedList = WiringsCompleted
                    .OrderBy(p => p.WirerID)
                    .Skip((OnCrossJobPage - 1) * 5)
                    .Take(5).ToList(),

                JobList = jobRepo.Jobs.ToList(),
                POList = jobRepo.POs.ToList(),
                JobTypeList = itemRepo.JobTypes.ToList(),
                StationsList = testRepo.Stations.ToList(),
                StatusPOList = jobRepo.StatusPOs.ToList(),
                StepList = wiringRepo.WiringSteps.ToList(),
                StepsForJobList = wiringRepo.WiringStepsForJobs.ToList(),
                StopList = wiringRepo.WiringStops.Where(m => m.Reason1 != 980 && m.Reason1 != 981 && m.Reason2 == 0).ToList(),
                
                PagingInfoIncompleted = new PagingInfo
                {
                    CurrentPage = MyJobsPage,
                    ItemsPerPage = 5,
                    JobNumb = jobnumb,
                    TotalItems = WiringsToBeStart.Count()
                },
                PagingInfoWorkingOnIt = new PagingInfo
                {
                    CurrentPage = PendingToCrossJobPage,
                    JobNumb = jobnumb,
                    ItemsPerPage = 5,
                    TotalItems = WiringsOnProgress.Count()
                },
                PagingInfoCompleted = new PagingInfo
                {
                    CurrentPage = OnCrossJobPage,
                    ItemsPerPage = 5,
                    JobNumb = jobnumb,
                    TotalItems = WiringsCompleted.Count()
                },

            };

            if (string.IsNullOrEmpty(jobNumber))
                return View(viewModel);
            if (wiringList.Count > 0 && wiringList[0] != null)
                return View(viewModel);

            TempData["message"] = $"Does not exist any job with the JobNum #{jobNumber}, please try again.";
            TempData["alert"] = $"alert-danger";
            return View(viewModel);
        }

        public ViewResult List(int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;

            List<Wiring> MyWiringList = wiringRepo.Wirings.Where(j => j.WirerID == currentUser.EngID)
                                                                   .ToList();

            List<StatusPO> MyStatusPOList = jobRepo.StatusPOs
                .Where(m => MyWiringList.Any(n => n.POID == m.POID))
                .Where(s => s.Status == "Wiring on progress" || s.Status ==  "WR: Reassignment" 
                         || s.Status == "WR: Stopped" || s.Status == "WR: Shift End" || s.Status == "WR: Adding features")
                .ToList();

            List<PO> OnProgressPOsList = jobRepo.POs.Where(m => MyStatusPOList.Any(n => n.POID == m.POID))
                                                    .ToList();

            List<Job> MyjobsList = jobRepo.Jobs.Where(m => OnProgressPOsList.Any(n => n.JobID == m.JobID))
                                                .OrderBy(n => n.LatestFinishDate).ToList();

            MyWiringList = MyWiringList.Where(m => MyStatusPOList.Any(n => n.POID == m.POID))
                                                   .OrderBy(p => p.StartDate).ToList();

            List<Job> DummyOnCrossJobsList = new List<Job>();

            List<Job> DummyPendingToCrossJobList = new List<Job>();

            return View(new DashboardIndexViewModel
            {
                MyWirings = MyWiringList.Skip((MyJobsPage - 1) * 5).Take(5),
                MyJobsPagingInfo = new PagingInfo
                {
                    CurrentPage = MyJobsPage,
                    ItemsPerPage = 5,
                    TotalItems = MyWiringList.Count(),
                },
                POs = OnProgressPOsList,
                StatusPOs = MyStatusPOList,
                MyJobs = MyjobsList,
                JobTypesList = itemRepo.JobTypes.ToList(),
                OnCrossJobs = DummyOnCrossJobsList,
                StationList = testRepo.Stations.ToList(),
                OnCrossJobsPagingInfo = new PagingInfo
                {
                    CurrentPage = OnCrossJobPage,
                    ItemsPerPage = PageSize,
                    TotalItems = DummyOnCrossJobsList.Count(),
                    sort = "deafult"
                },
                PendingToCrossJobs = DummyPendingToCrossJobList,
                PendingToCrossJobsPagingInfo = new PagingInfo
                {
                    CurrentPage = PendingToCrossJobPage,
                    ItemsPerPage = PageSize,
                    TotalItems = DummyPendingToCrossJobList.Count(),
                    sort = "deafult"
                },
            });
        }

        public ViewResult MyWirings(int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;

            List<Wiring> MyWiringList = wiringRepo.Wirings.Where(j => j.WirerID == currentUser.EngID).ToList();

            List<StatusPO> MyStatusPOList = jobRepo.StatusPOs
                                                .Where(m => MyWiringList.Any(n => n.POID == m.POID)).ToList();

            List<PO> OnProgressPOsList = jobRepo.POs.Where(m => MyStatusPOList.Any(n => n.POID == m.POID))
                                                    .ToList();

            List<Job> MyjobsList = jobRepo.Jobs.Where(m => OnProgressPOsList.Any(n => n.JobID == m.JobID))
                                                .OrderBy(n => n.LatestFinishDate).ToList();

            List<Job> DummyOnCrossJobsList = new List<Job>();
            List<Job> DummyPendingToCrossJobList = new List<Job>();

            return View(new WiringViewModel
            {
                WiringJobList = MyWiringList.Skip((MyJobsPage - 1) * 5).Take(5).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = MyJobsPage,
                    ItemsPerPage = 5,
                    TotalItems = MyWiringList.Count(),
                },
                JobList = jobRepo.Jobs.ToList(),
                StatusPOList = jobRepo.StatusPOs.ToList(),
                POList = jobRepo.POs.ToList(),
                JobTypeList = itemRepo.JobTypes.ToList(),
                StationsList = testRepo.Stations.ToList(),

            });
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
            if (wiring != null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el wiringJob ya existe";
                return RedirectToAction("SearchByPO", "Home", viewHomeModel);
            }
            else
            {
                viewModel.Wiring = new Wiring();
                viewModel.Wiring.POID = po.POID;
            }

            

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult NewWiringJob(WiringViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;

            viewModel.Wiring.POID = viewModel.PO.POID;
            viewModel.Wiring.WirerID = currentUser.EngID;
            viewModel.Wiring.StartDate = DateTime.Now;
            viewModel.Wiring.CompletedDate = DateTime.Now;
            wiringRepo.SaveWiring(viewModel.Wiring);

            viewModel.Wiring = wiringRepo.Wirings.FirstOrDefault(m => m.POID == viewModel.Wiring.POID);

            WirersInvolved involved = wiringRepo.WirersInvolveds
                                                 .Where(m => m.WiringID == viewModel.Wiring.WiringID)
                                                 .FirstOrDefault(m => m.WirerID == currentUser.EngID);
            if (involved == null)
            {
                WirersInvolved wirersInvolved = new WirersInvolved();
                wirersInvolved.WiringID = viewModel.Wiring.WiringID;
                wirersInvolved.WirerID = currentUser.EngID;
                wiringRepo.SaveWirersInvolved(wirersInvolved);
            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == viewModel.Wiring.POID);

            statusPO.Status = "WR: Adding features";
            jobRepo.SaveStatusPO(statusPO);

            viewModel.PO = jobRepo.POs.FirstOrDefault(m => m.POID == viewModel.Wiring.POID);
            viewModel.Job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == viewModel.PO.JobID);
            viewModel.FeatureList = new List<WiringFeatures>();

            return RedirectToAction("EditFeatures", new { WiringID = viewModel.Wiring.WiringID});
        }

        public IActionResult AddFeature(int WiringID)
        {
            VerifyWiringJobStatus(WiringID);

            WiringViewModel viewModel = new WiringViewModel();
            viewModel.Wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == WiringID);
            viewModel.Feature = new WiringFeatures();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddFeature(WiringViewModel viewModel)
        {
            VerifyWiringJobStatus(viewModel.Wiring.WiringID);

            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == viewModel.Wiring.WiringID);

            bool isWOptionRepeated = wiringRepo.WiringFeatures.Any(m => m.WiringID == viewModel.Wiring.WiringID
                                                                    && m.WiringOptionID == viewModel.Feature.WiringOptionID);

            if (isWOptionRepeated)
            {
                WiringOption option = wiringRepo.WiringOptions.FirstOrDefault(m => m.WiringOptionID == viewModel.Feature.WiringOptionID);
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"La caracteristica {option.Description} ya fue añadida previamente";
                return RedirectToAction("EditFeatures", new { WiringID = viewModel.Wiring.WiringID });
            }

            viewModel.Feature.WiringID = wiring.WiringID;
            viewModel.Feature.Quantity = 1;
            wiringRepo.SaveWiringFeatures(viewModel.Feature);

            TempData["message"] = $"Nueva caracteristica añadida con exito.";
            return RedirectToAction("EditFeatures", new { WiringID = viewModel.Wiring.WiringID });
        }

        public IActionResult EditFeatures(int WiringID)
        {
            VerifyWiringJobStatus(WiringID);

            List<WiringFeatures> features = wiringRepo.WiringFeatures.Where(m => m.WiringID == WiringID).ToList();

            WiringViewModel viewModel = new WiringViewModel();
            viewModel.Wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == WiringID);
            viewModel.PO = jobRepo.POs.FirstOrDefault(m => m.POID == viewModel.Wiring.POID);
            viewModel.Job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == viewModel.Wiring.POID);
            viewModel.FeatureList = features ?? new List<WiringFeatures>();

            return View(viewModel);
        }

        public IActionResult DeleteFeature(int ID)
        {
            WiringFeatures feature = wiringRepo.WiringFeatures.FirstOrDefault(m => m.WiringFeaturesID == ID);
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == feature.WiringID);

            VerifyWiringJobStatus(wiring.WiringID);

            WiringFeatures deletedFeature = wiringRepo.DeleteWiringFeatures(ID);
            WiringOption wiringOption = wiringRepo.WiringOptions.FirstOrDefault(m => m.WiringOptionID == deletedFeature.WiringOptionID);
            if (deletedFeature != null)
            {
                TempData["message"] = $"{wiringOption.Description} ha sido eliminado";
            }

            return RedirectToAction("EditFeatures", new { WiringID = wiring.WiringID });
        }

        [HttpPost]
        public IActionResult SaveFeatures(WiringViewModel viewModel)
        {
            VerifyWiringJobStatus(viewModel.Wiring.WiringID);

            bool Admin = GetCurrentUserRole("Admin").Result;
            bool productionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            int wirerID = GetCurrentUser().Result.EngID;

            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == viewModel.Wiring.WiringID);
            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);


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

                List<WiringStepForJob> StepsForJobUpdated = wiringRepo.WiringStepsForJobs.Where(m => m.WiringID == wiring.WiringID
                                                                                    && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
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

            statusPO.Status = "Wiring on progress";
            jobRepo.SaveStatusPO(statusPO);

            wiring.StartDate = DateTime.Now;
            wiring.CompletedDate = DateTime.Now;
            wiringRepo.SaveWiring(wiring);

            if (Admin | productionAdmin)
                return RedirectToAction("AdminDashboard");
            
            
            return RedirectToAction("ContinueStep", new { WiringID = wiring.WiringID });
        }

        public List<WiringStepForJob> MakeStepsForJobList(WiringViewModel viewModel)
        {
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == viewModel.Wiring.WiringID);
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
            VerifyWiringJobStatus(wiringID);

            WiringViewModel viewModel = new WiringViewModel();
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == wiringID);
            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);

            bool Admin = GetCurrentUserRole("Admin").Result;
            bool productionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            bool isNotSameEngineer = GetCurrentUser().Result.EngID != wiring.WirerID;
            bool istCompleted = statusPO.Status == "Waiting for PXP";

            List<WiringStepForJob> StepsForJobList = wiringRepo.WiringStepsForJobs.Where(m => m.WiringID == wiring.WiringID && m.Obsolete == false)
                                                                            .OrderBy(m => m.Consecutivo).ToList();

            List<WiringStep> StepsInfoList = wiringRepo.WiringSteps.Where(m => StepsForJobList.Any(n => n.WiringStepID == m.WiringStepID)).ToList();

            viewModel.Wiring = wiring;
            viewModel.PO = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);
            viewModel.JobNum = jobRepo.Jobs.FirstOrDefault(m => m.JobID == viewModel.PO.JobID).JobNum;
            viewModel.currentStep = StepsForJobList.FirstOrDefault(m => m.Complete == false);
            viewModel.StepsForJob = viewModel.currentStep;
            viewModel.prevStep = StepsForJobList.FirstOrDefault(m => m.Complete == true && m.Consecutivo == (viewModel.currentStep.Consecutivo - 1));
            viewModel.nextStep = StepsForJobList.FirstOrDefault(m => m.Complete == false && m.Consecutivo == (viewModel.currentStep.Consecutivo + 1));
            viewModel.StepInfo = StepsInfoList.FirstOrDefault(m => m.WiringStepID == viewModel.currentStep.WiringStepID);
            viewModel.StopList = wiringRepo.WiringStops.Where(m => m.Reason5ID == 0 && m.Reason1 != 980 && m.Reason1 != 981 && m.Reason1 != 982).ToList();
            viewModel.Reasons1List = wiringRepo.WiringReasons1.Where(m => viewModel.StopList.Any(n => n.Reason1 == m.WiringReason1ID)).ToList();
            
            List<WiringStep> StepsInStageList = StepsInfoList.Where(m => m.Stage == viewModel.StepInfo.Stage).ToList();

            viewModel.StepsForJobList = StepsForJobList;
            viewModel.StepList = StepsInfoList;
            viewModel.TotalStepsPerStage = StepsInStageList.Count();
            viewModel.CurrentStepInStage = StepsForJobList.Where(m => StepsInStageList.Any(s => s.WiringStepID == m.WiringStepID))
                                                          .Where(m => m.Complete == true).Count() + 1;
            viewModel.StopNC = wiringRepo.WiringStops.Where(m => m.WiringID == wiring.WiringID && m.Reason1 != 980 && m.Reason1 != 981)
                                                     .Where(m => m.Reason2 == 0 && m.Critical == false).Any();
            SaveTimeStep(wiring.WiringID);
            return View("StepsOnProgress", viewModel);
        }

        public IActionResult StepsOnProgress(WiringViewModel viewModel, string movement)
        {
            VerifyWiringJobStatus(viewModel.Wiring.WiringID);

            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == viewModel.Wiring.WiringID);
            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);

            int stepsLeft = wiringRepo.WiringStepsForJobs.Where(m => m.WiringID == viewModel.Wiring.WiringID && m.Obsolete == false && m.Complete == false).Count();

            if (movement == "previus")
                PreviusStep(viewModel.prevStep.WiringStepForJobID, viewModel.currentStep.WiringStepForJobID);

            if (movement == "next")
            {
                if(stepsLeft == 1 && viewModel.StopNC == true)
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Error,  tiene un paro pendiente por terminar";
                    return RedirectToAction("ContinueStep", new { WiringID = wiring.WiringID });
                }

                NextStep(viewModel.currentStep.WiringStepForJobID);
            }

            stepsLeft = wiringRepo.WiringStepsForJobs.Where(m => m.WiringID == viewModel.Wiring.WiringID && m.Obsolete == false && m.Complete == false).Count();

            if (stepsLeft > 0)
                return RedirectToAction("ContinueStep", new { WiringID = wiring.WiringID });

            wiring.CompletedDate = DateTime.Now;
            wiringRepo.SaveWiring(wiring);

            statusPO.Status = "Waiting for PXP";
            jobRepo.SaveStatusPO(statusPO);

            TempData["message"] = $"El WiringJob con el PO #{po.PONumb} se ha completado con exito!";
            TempData["alert"] = $"alert-success";
            return RedirectToAction("List");

        }

        public IActionResult JobCompletion(int WiringID)
        {
            bool isProdAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            bool isAdmin = GetCurrentUserRole("Admin").Result;

            if (isAdmin == false && isProdAdmin == false)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, no tiene permisos para realizar esta accion, contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == WiringID);
            if (wiring == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el WiringJob no existe, contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);
            if (statusPO.Status == "Waiting for PXP")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el wiringJob ya se completo, contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

            WiringStop stop = wiringRepo.WiringStops.FirstOrDefault(m => m.Reason1 != 980 & m.Reason1 != 981 && m.Reason2 == 0 && m.WiringID == wiring.WiringID);
            if (stop != null)
                return RedirectToAction("FinishPendingStops", "Stop", new { WiringID = wiring.WiringID });

            WiringViewModel viewModel = new WiringViewModel()
            {
                Wiring = wiring,
                StepsLeft = wiringRepo.WiringStepsForJobs.Where(m => m.WiringID == WiringID && m.Obsolete == false && m.Complete == false).Count(),
            };

            return View(viewModel);

        }

        [HttpPost]
        public IActionResult JobCompletion(WiringViewModel viewModel)
        {
            if(viewModel.ElapsedTimeHours == 0 && viewModel.ElapsedTimeMinutes == 0)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, minutos y horas en cero";
                return View(viewModel);
            }

            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == viewModel.Wiring.WiringID);
            if (wiring == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el WiringJob no existe, contacte al Admin";
                return RedirectToAction("AdminDashboard");
            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);
            if (statusPO.Status != "WR: Adding features" && statusPO.Status != "Wiring on progress" && statusPO.Status != "WR: Reassignment"
                  && statusPO.Status != "WR: Stopped" && statusPO.Status != "WR: Shift End")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el wiringJob ya se completo, contacte al Admin";
                return RedirectToAction("AdminDashboard");
            }

            List<WiringStop> otherStops = wiringRepo.WiringStops.Where(p => wiring.WiringID == p.WiringID && (p.Reason1 == 980 || p.Reason1 == 981))
                                                     .Where(p => p.Reason2 == 0 && p.Reason3 == 0).ToList();

            if (otherStops.Count > 0)
            {
                foreach (WiringStop otherStop in otherStops)
                {
                    TimeSpan auxTime = (DateTime.Now - otherStop.StartDate);
                    otherStop.Elapsed += auxTime;
                    otherStop.StopDate = DateTime.Now;
                    otherStop.StartDate = DateTime.Now;
                    otherStop.Reason2 = 982;
                    otherStop.Reason3 = 982;
                    otherStop.Reason4 = 982;
                    otherStop.Reason5ID = 982;
                    otherStop.Description = "Job was send to complete";
                    wiringRepo.SaveWiringStop(otherStop);

                }
            }

            List<WiringStepForJob> IncompleteStepsForJob = wiringRepo.WiringStepsForJobs.Where(m => m.WiringID == viewModel.Wiring.WiringID && m.Obsolete == false && m.Complete == false)
                                                                                        .OrderBy(m => m.Consecutivo).ToList();
            List<WiringStep> IncompleteStepsForJobInfo = wiringRepo.WiringSteps.Where(m => IncompleteStepsForJob.Any(s => s.WiringStepID == m.WiringStepID)).ToList();
            
            double ExpectecTimeSUM = 0;
            double ElapseHoursFromView = viewModel.ElapsedTimeHours + (viewModel.ElapsedTimeMinutes * 0.01666666666666666666666666666667);

            foreach (WiringStep step in IncompleteStepsForJobInfo)
            {
                double StepExpectTime = ToHours(step.ExpectedTime);
                ExpectecTimeSUM += StepExpectTime;
            }

            foreach (WiringStepForJob step in IncompleteStepsForJob)
            {
                if (IncompleteStepsForJobInfo.First(m => m.WiringStepID == step.WiringStepID).ExpectedTime.Minute != 0)
                {
                    double ExpectedTimeForStep = ToHours(IncompleteStepsForJobInfo.First(m => m.WiringStepID == step.WiringStepID).ExpectedTime);
                    double TimePercentage = ExpectedTimeForStep / ExpectecTimeSUM;
                    if (TimePercentage == 0) TimePercentage = 1;
                    double TotalTime = ElapseHoursFromView * TimePercentage;

                    step.Elapsed = ToDateTime(TotalTime);
                    step.Complete = true;
                    wiringRepo.SaveWiringStepForJob(step);
                }

            }

            wiring.CompletedDate = DateTime.Now;
            wiringRepo.SaveWiring(wiring);

            statusPO.Status = "Waiting for PXP";
            jobRepo.SaveStatusPO(statusPO);

            TempData["message"] = $"You have completed the WiringJob PO# {po.PONumb}";
            return RedirectToAction("AdminDashboard");

        }

        public IActionResult CurrentStepsList(WiringViewModel viewModel, int targetStepID)
        {
            ///Current step
            WiringStepForJob currentStep = wiringRepo.WiringStepsForJobs.FirstOrDefault(m => m.WiringStepForJobID == viewModel.currentStep.WiringStepForJobID && m.Obsolete == false && m.Complete == false);

            currentStep.Elapsed = GetElpasedAsDateTime(currentStep.Elapsed, currentStep.Start, DateTime.Now);
            currentStep.Start = DateTime.Now;
            currentStep.Stop = DateTime.Now;
            wiringRepo.SaveWiringStepForJob(currentStep);

            //Target step
            WiringStepForJob targetStep = wiringRepo.WiringStepsForJobs.FirstOrDefault(m => m.WiringStepForJobID == targetStepID && m.Obsolete == false && m.Complete == true);
            
            if(targetStep != null)
            {
                targetStep.Complete = false;
                targetStep.Start = DateTime.Now;
                targetStep.Stop = DateTime.Now;
                wiringRepo.SaveWiringStepForJob(currentStep);
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el paso qu eligio aun no esta completado";
            }
            

            return RedirectToAction("ContinueStep", new { WiringID = targetStep.WiringID });
        }


        [HttpPost]
        public IActionResult Delete(int WiringId)
        {
            bool isProductionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            bool isAdmin = GetCurrentUserRole("Admin").Result;

            if (isAdmin == false && isProductionAdmin == false)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, no tiene permisos para realizar esta accion, contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == WiringId);
            if (wiring == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el WiringJob no existe, contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);

            Wiring wiringDeleted = wiringRepo.DeleteWiring(WiringId);
            if (wiringDeleted != null)
            {
                TempData["message"] = $"Wirinjob with #PO {po.PONumb} was deleted";

                if (statusPO.Status == "Wiring on progress" || statusPO.Status == "WR: Stopped" ||
                    statusPO.Status == "WR: Reassignment" || statusPO.Status == "WR: Shift End" ||
                    statusPO.Status == "Wiring for pxp" || statusPO.Status == "WR: Adding features")
                {
                    statusPO.Status = "Production";
                    jobRepo.SaveStatusPO(statusPO);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        //Linked to StopController
        [HttpPost]
        public IActionResult Reassignment(WiringViewModel viewModel)
        {
            VerifyAdminPermissions(viewModel.Wiring.WiringID);

            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == viewModel.Wiring.WiringID);
            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);
            string StationName = testRepo.Stations.FirstOrDefault(m => m.StationID == viewModel.NewStationID).Label;

            if (wiring.WiringID == viewModel.NewWirerID && wiring.StationID == viewModel.NewStationID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"No puedes reasiganr el trabajo debido a que el Tecnico y la estacion son iguales";
                return RedirectToAction("AdminDashboard");
            }

            CloseShifEndStops(wiring, statusPO);

            List<WiringStop> CurrentStops = wiringRepo.WiringStops.Where(p => wiring.WiringID == p.WiringID && p.Reason2 == 0 && p.Reason3 == 0)
                                                                  .Where(p => p.Reason1 != 980 && p.Reason1 != 981).ToList();
            if (CurrentStops.Count > 0)
            {
                foreach (WiringStop CurrentStop in CurrentStops)
                {

                    WiringStop CopyStop = new WiringStop();

                    //Firts copy the stop
                    CopyStop.Reason1 = CurrentStop.Reason1;
                    CopyStop.Reason2 = CurrentStop.Reason2;
                    CopyStop.Reason3 = CurrentStop.Reason3;
                    CopyStop.Reason4 = CurrentStop.Reason4;
                    CopyStop.Reason5ID = CurrentStop.Reason5ID;
                    CopyStop.Critical = CurrentStop.Critical;
                    CopyStop.Description = CurrentStop.Description;
                    CopyStop.WiringID = CurrentStop.WiringID;

                    CopyStop.StartDate = DateTime.Now;
                    CopyStop.StopDate = DateTime.Now;
                    CopyStop.Elapsed = new DateTime(1, 1, 1, 0, 0, 0);
                    CopyStop.AuxStationID = viewModel.NewStationID;
                    CopyStop.AuxWirerID = viewModel.NewWirerID;
                    wiringRepo.SaveWiringStop(CopyStop);

                    CurrentStop.Elapsed = GetElpasedAsDateTime(CurrentStop.Elapsed, CurrentStop.StartDate, DateTime.Now);
                    CurrentStop.StopDate = DateTime.Now;
                    CurrentStop.StartDate = DateTime.Now;
                    CurrentStop.Reason2 = 980;
                    CurrentStop.Reason3 = 980;
                    CurrentStop.Reason4 = 980;
                    CurrentStop.Reason5ID = 980;
                    CurrentStop.Description = "Job was reassigned";
                    wiringRepo.SaveWiringStop(CurrentStop);
                }

            }

            WiringStop ReassignmentStop = wiringRepo.WiringStops.LastOrDefault(p => p.WiringID == wiring.WiringID && p.Reason1 == 980 && p.Reason2 == 0 && p.Reason3 == 0);
            WiringStop NewtStop = new WiringStop();

            if (ReassignmentStop == null)
            {
                NewtStop = new WiringStop
                {
                    WiringID = wiring.WiringID,
                    Reason1 = 980,
                    Reason2 = 0,
                    Reason3 = 0,
                    Reason4 = 0,
                    Reason5ID = 0,
                    Description = "Job was reassigned",
                    Critical = true,
                    StartDate = DateTime.Now,
                    StopDate = DateTime.Now,
                    Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                    AuxStationID = viewModel.NewStationID,
                    AuxWirerID = viewModel.NewWirerID,
                };
            }
            else
            {
                NewtStop = ReassignmentStop;
                NewtStop.AuxStationID = viewModel.NewStationID;
                NewtStop.AuxWirerID = viewModel.NewWirerID;
            }
            wiringRepo.SaveWiringStop(NewtStop);

            wiring.WirerID = viewModel.NewWirerID;
            wiring.StationID = viewModel.NewStationID;
            wiringRepo.SaveWiring(wiring);

            statusPO.Status = "WR: Reassignment";
            jobRepo.SaveStatusPO(statusPO);

            SaveTimeStep(wiring.WiringID);

            TempData["message"] = $"You have reassinged the wierer for the wiringjob PO# {po.PONumb} to T{viewModel.NewWirerID} and the station to {StationName}";
            return RedirectToAction("AdminDashboard");
        }

        public IActionResult ReturnFromComplete(int WiringId)
        {
            VerifyAdminPermissions(WiringId);

            Wiring wiring= wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == WiringId);
            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);
            bool isNotCompleted = statusPO.Status == "WR: Adding features" || statusPO.Status == "Wiring on progress";

            if (isNotCompleted)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El Wiringjob se encuentra activo, intente de nuevo o contacte al Admin";

                return RedirectToAction("AdminDashboard");
            }

            WiringStepForJob stepsForJob = wiringRepo.WiringStepsForJobs.OrderBy(s => s.Consecutivo).Where(p => p.WiringID == WiringId && p.Obsolete == false).Last();
            stepsForJob.Complete = false;
            wiringRepo.SaveWiringStepForJob(stepsForJob);

            if (AnyWiringJobOnProgress(wiring.WirerID))
            {
                statusPO.Status = "WR: Stopped";
                jobRepo.SaveStatusPO(statusPO);
                WiringStop NewtStop = new WiringStop
                {
                    WiringID = wiring.WiringID,
                    Reason1 = 982,
                    Reason2 = 982,
                    Reason3 = 982,
                    Reason4 = 982,
                    Reason5ID = 0,
                    Description = "The admin was returned the job to working on it",
                    Critical = true,
                    StartDate = DateTime.Now,
                    StopDate = DateTime.Now,
                    Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                    AuxStationID = wiring.StationID,
                    AuxWirerID = wiring.WirerID,
                };
                wiringRepo.SaveWiringStop(NewtStop);
                TempData["message"] = $"You have returned the WiringJob PO# {po.PONumb} to stopped";
                return RedirectToAction("AdminDashboard");
            }
            statusPO.Status = "Wiring on progress";
            jobRepo.SaveStatusPO(statusPO);
            TempData["message"] = $"You have returned the TestJob PO# {po.PONumb} to Working on it";
            return RedirectToAction("AdminDashboard");

        }

        public void ShiftEnd(int wirerID)
        {
            List<Wiring> activeWirings = wiringRepo.Wirings.Where(m => m.StartDate.ToShortDateString() == m.CompletedDate.ToShortDateString())
                                                           .Where(m => m.WirerID == wirerID).ToList();

            List<StatusPO> StatusPoList= jobRepo.StatusPOs.Where(m => activeWirings.Any(n => n.POID == m.POID))
                                                          .Where(m => m.Status == "Wiring on progress" || m.Status == "WR: Stopped" || m.Status == "WR: Reassignment").ToList();
            activeWirings = activeWirings.Where(m => StatusPoList.Any(n => n.POID == m.POID)).ToList();

            if (activeWirings.Count > 0)
            {
                foreach (Wiring wiring in activeWirings)
                {
                    List<WiringStop> stops = new List<WiringStop>();
                    stops = wiringRepo.WiringStops.Where(p => wiring.WiringID == p.WiringID && p.Reason1 != 981 && p.Reason3 == 0 && p.Reason2 == 0).ToList();
                    StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);

                    if (stops.Count > 0)
                    {
                        foreach (WiringStop stop in stops)
                        {
                            TimeSpan auxTime = (DateTime.Now - stop.StartDate);
                            stop.Elapsed += auxTime;
                            stop.StartDate = DateTime.Now;
                            stop.StopDate = DateTime.Now;
                            wiringRepo.SaveWiringStop(stop);
                        }
                    }

                    try
                    {
                        SaveTimeStep(wiring.WiringID);
                    }
                    catch { }

                    WiringStop NewtStop = new WiringStop
                    {
                        WiringID = wiring.WiringID,
                        Reason1 = 981,
                        Reason2 = 0,
                        Reason3 = 0,
                        Reason4 = 0,
                        Reason5ID = 0,
                        Description = "Automatic Shift End",
                        Critical = true,
                        StartDate = DateTime.Now,
                        StopDate = DateTime.Now,
                        Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                        AuxStationID = wiring.StationID,
                        AuxWirerID = wiring.WirerID,
                    };
                    wiringRepo.SaveWiringStop(NewtStop);

                    statusPO.Status = "WR: Shift End";
                    jobRepo.SaveStatusPO(statusPO);
                }
            }

        }

        [AllowAnonymous]
        public void AutomaticShiftEnd()
        {
            List<Wiring> activeWirings = wiringRepo.Wirings.Where(m => m.StartDate.ToShortDateString() == m.CompletedDate.ToShortDateString())
                                                           .ToList();

            List<StatusPO> StatusPoList = jobRepo.StatusPOs.Where(m => activeWirings.Any(n => n.POID == m.POID))
                                                          .Where(m => m.Status == "Wiring on progress" || m.Status == "WR: Stopped" || m.Status == "WR: Reassignment").ToList();
            activeWirings = activeWirings.Where(m => StatusPoList.Any(n => n.POID == m.POID)).ToList();

            if (activeWirings.Count > 0)
            {
                foreach (Wiring wiring in activeWirings)
                {
                    List<WiringStop> stops = new List<WiringStop>();
                    stops = wiringRepo.WiringStops.Where(p => wiring.WiringID == p.WiringID && p.Reason1 != 981 && p.Reason3 == 0 && p.Reason2 == 0).ToList();
                    StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);

                    if (stops.Count > 0)
                    {
                        foreach (WiringStop stop in stops)
                        {
                            TimeSpan auxTime = (DateTime.Now - stop.StartDate);
                            stop.Elapsed += auxTime;
                            stop.StartDate = DateTime.Now;
                            stop.StopDate = DateTime.Now;
                            wiringRepo.SaveWiringStop(stop);
                        }
                    }

                    try
                    {
                        SaveTimeStep(wiring.WiringID);
                    }
                    catch { }

                    WiringStop NewtStop = new WiringStop
                    {
                        WiringID = wiring.WiringID,
                        Reason1 = 981,
                        Reason2 = 0,
                        Reason3 = 0,
                        Reason4 = 0,
                        Reason5ID = 0,
                        Description = "Automatic Shift End",
                        Critical = true,
                        StartDate = DateTime.Now,
                        StopDate = DateTime.Now,
                        Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                        AuxStationID = wiring.StationID,
                        AuxWirerID = wiring.WirerID,
                    };
                    wiringRepo.SaveWiringStop(NewtStop);

                    statusPO.Status = "WR: Shift End";
                    jobRepo.SaveStatusPO(statusPO);
                }
            }

        }



        ///Aditional functions
        public void PreviusStep(int previousStepID, int currentStepID)
        {
            //For Current step
            WiringStepForJob currentStep = wiringRepo.WiringStepsForJobs.FirstOrDefault(m => m.WiringStepForJobID == currentStepID &&  m.Obsolete == false && m.Complete == false);

            if (currentStep.Start != DateTime.Now)
            {
                currentStep.Elapsed = GetElpasedAsDateTime(currentStep.Elapsed, currentStep.Start, DateTime.Now);
            }

            currentStep.Start = DateTime.Now;
            currentStep.Stop = DateTime.Now;
            wiringRepo.SaveWiringStepForJob(currentStep);

            //For Previus Step
            //For target step (before actual step)
            WiringStepForJob previusStep = wiringRepo.WiringStepsForJobs.FirstOrDefault(m => m.WiringStepForJobID == previousStepID && m.Obsolete == false && m.Complete == true);

            previusStep.Complete = false;
            previusStep.Stop = DateTime.Now;
            previusStep.Start = DateTime.Now;
            wiringRepo.SaveWiringStepForJob(previusStep);
        }

        public void NextStep(int currentStepID)
        {
            //For Current step
            WiringStepForJob currentStep = wiringRepo.WiringStepsForJobs.FirstOrDefault(m => m.WiringStepForJobID == currentStepID && m.Obsolete == false && m.Complete == false);
            currentStep.Elapsed = GetElpasedAsDateTime(currentStep.Elapsed, currentStep.Start, DateTime.Now);
            currentStep.Start = DateTime.Now;
            currentStep.Stop = DateTime.Now;
            currentStep.Complete = true;
            wiringRepo.SaveWiringStepForJob(currentStep);

            //For Next step
            WiringStepForJob NextStep = wiringRepo.WiringStepsForJobs.FirstOrDefault(m => m.WiringID == currentStep.WiringID && m.Obsolete == false && m.Complete == false);
            if(NextStep != null)
            {
                NextStep.Start = DateTime.Now;
                NextStep.Stop = DateTime.Now;
                wiringRepo.SaveWiringStepForJob(NextStep);
            }
   
        }

        public void SaveTimeStep(int WiringId)
        {
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == WiringId);

            WiringStepForJob currentStep = wiringRepo.WiringStepsForJobs.FirstOrDefault(m => m.WiringID == WiringId && m.Obsolete == false && m.Complete == false);

            //For Current step
            currentStep.Elapsed = GetElpasedAsDateTime(currentStep.Elapsed, currentStep.Start, DateTime.Now);
            currentStep.Start = DateTime.Now;
            currentStep.Stop = DateTime.Now;
            wiringRepo.SaveWiringStepForJob(currentStep);

        }

        public void RestarTimeStep(int WiringId)
        {
            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == WiringId);

            WiringStepForJob currentStep = wiringRepo.WiringStepsForJobs.FirstOrDefault(m => m.WiringID == WiringId && m.Obsolete == false && m.Complete == false);

            currentStep.Start = DateTime.Now;
            currentStep.Stop = DateTime.Now;
            wiringRepo.SaveWiringStepForJob(currentStep);

        }

        public DateTime GetElpasedAsDateTime(DateTime elapsed, DateTime start, DateTime stop)
        {
            TimeSpan elapsedAfter = stop - start;

            if (elapsed.Hour == 0 && elapsed.Minute == 0 && elapsed.Second == 0)
            {
                int days = 0;
                days = elapsedAfter.Days;

                if (days == 0)
                    days = 1;

                elapsed = new DateTime(1, 1, days, elapsedAfter.Hours, elapsedAfter.Minutes, elapsedAfter.Seconds);
            }
            else
            {
                int newsecond = 0, newhour = 0, newMinute = 0, days = 0;

                newsecond = elapsed.Second + elapsedAfter.Seconds;
                newMinute = elapsed.Minute + elapsedAfter.Minutes;
                newhour = elapsed.Hour + elapsedAfter.Hours;
                days = elapsed.Day + elapsedAfter.Days;
                if (newsecond >= 60)
                {
                    newsecond -= 60;
                    newMinute++;
                }
                newMinute += elapsedAfter.Minutes;
                if (newMinute >= 60)
                {
                    newMinute -= 60;
                    newhour++;
                }
                if (newhour >= 24)
                {
                    newhour -= 24;
                    days++;
                }

                if (days == 0)
                        days = 1;

                elapsed = new DateTime(1, 1, days, newhour, newMinute, newsecond);
            }

            return elapsed;
        }

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

        public bool AnyWiringJobOnProgress(int wirerID)
        {
            bool AnyWiringActive = false;

            List<Wiring> activeWirings = wiringRepo.Wirings.Where(m => m.StartDate.ToShortDateString() == m.CompletedDate.ToShortDateString())
                                                           .Where(m => m.WirerID == wirerID).ToList();

            AnyWiringActive = jobRepo.StatusPOs.Any(m => activeWirings.Any(n => n.POID == m.POID) && m.Status == "Wiring on progress");

            return AnyWiringActive;
        }

        public double ToHours(DateTime date)
        {
            double totalTime = 0;
            totalTime += date.Hour;
            totalTime += (date.Minute * 0.01666666666666666666666666666667);
            totalTime += (date.Second * 0.0002777777777777777777777777777778);
            return totalTime;
        }

        public DateTime ToDateTime(double TotalHours)
        {
            DateTime Date = new DateTime(1, 1, 1, 0, 0, 0);
            double AuxTotalHours = Math.Truncate(TotalHours);
            double AuxTotalMinutes = TotalHours - AuxTotalHours;
            int AuxDays = 0;
            while (AuxTotalHours >= 24)
            {
                AuxTotalHours -= 24;
                AuxDays++;
            };
            int Hours = (int)AuxTotalHours;
            int Minutes = (int)(Math.Round(AuxTotalMinutes * 60));
            if (Minutes == 60)
            {
                Hours++;
                Minutes = 0;
            }

            int Days = 1 + AuxDays;


            return new DateTime(1, 1, Days, Hours, Minutes, 0);
        }

        public IActionResult VerifyAdminPermissions(int WiringID)
        {
            bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
            bool isAdmin = GetCurrentUserRole("Admin").Result;

            if (isAdmin == false && isTechAdmin == false)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, no tiene permisos para realizar esta accion, contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == WiringID);
            if (wiring != null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el WiringJob no existe, contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);
            if (statusPO.Status == "Waiting for PXP")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el wiringJob ya se completo, contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

            return Ok();
        }

        public IActionResult VerifyWiringJobStatus(int WiringID)
        {
            bool Admin = GetCurrentUserRole("Admin").Result;
            bool productionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            int wirerID = GetCurrentUser().Result.EngID;

            Wiring wiring = wiringRepo.Wirings.FirstOrDefault(m => m.WiringID == WiringID);
            if (wiring == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El WiringJob no existe, intente de nuevo o contacte al Admin";
                return RedirectToAction("List");
            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiring.POID);
            PO po = jobRepo.POs.FirstOrDefault(m => m.POID == wiring.POID);

            if (wiring.WirerID != wirerID && !productionAdmin && !Admin)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El WiringJob fue reasignado, intente de nuevo o contacte al Admin";
                return RedirectToAction("List");
            }
            else if (statusPO.Status == "Waiting for PXP")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el WiringJob con PO #{po.PONumb} ya esta terminado";

                return RedirectToAction("List");
            }
            else if (statusPO.Status == "WR: Shift End" || statusPO.Status == "WR: Stopped" || statusPO.Status == "WR: Reassignment")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el WiringJob con PO #{po.PONumb} tiene un paro";

                return RedirectToAction("List");
            }

            return Ok();
        }

        public void CloseShifEndStops(Wiring wiring, StatusPO statusPO)
        {
            WiringStop ShiftEndStop = wiringRepo.WiringStops.LastOrDefault(p => p.WiringID == wiring.WiringID && p.Reason1 == 981 && p.Reason2 == 0 && p.Reason3 == 0);

            if (ShiftEndStop == null)
                return;

            try
            {
                TimeSpan auxTime = (DateTime.Now - ShiftEndStop.StartDate);
                ShiftEndStop.Elapsed += auxTime;
                ShiftEndStop.StopDate = DateTime.Now;
                ShiftEndStop.Reason2 = 981;
                ShiftEndStop.Reason3 = 981;
                ShiftEndStop.Reason4 = 981;
                ShiftEndStop.Reason5ID = 981;
                wiringRepo.SaveWiringStop(ShiftEndStop);

                List<WiringStop> stops = wiringRepo.WiringStops.Where(p => p.WiringID == wiring.WiringID && p.Reason1 != 980 && p.Reason1 != 981 && p.Reason2 == 0).ToList();
                if (stops.Count > 0)
                {
                    foreach (WiringStop stop in stops)
                    {
                        stop.StartDate = DateTime.Now;
                        stop.StopDate = DateTime.Now;
                        wiringRepo.SaveWiringStop(stop);
                    }

                }

                if (stops.Any(m => m.Critical == true))
                {
                    statusPO.Status = "WR: Stopped";
                }
                else
                {
                    statusPO.Status = "Wiring on pogress";
                }

                jobRepo.SaveStatusPO(statusPO);
                return;
            }
            catch
            {
                return;
            }
        }
    }

}
