using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.TestJob;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,TechAdmin,Technician")]
    public class TestJobController : Controller
    {
        private IJobRepository jobRepo;
        private ITestingRepository testingRepo;
        private IItemRepository itemRepository;
        private UserManager<AppUser> userManager;
        public int PageSize = 7;

        public TestJobController(ITestingRepository repo, IJobRepository repo2, IItemRepository repo3, UserManager<AppUser> userMgr)
        {
            jobRepo = repo2;
            testingRepo = repo;
            userManager = userMgr;
            itemRepository = repo3;
        }

        public ViewResult List(int page = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;

            IQueryable<TestJob> testJobList = testingRepo.TestJobs
                .Where(m => m.TechnicianID == currentUser.EngID);


            TestJobViewModel testJobView = new TestJobViewModel
            {
                TestJobList = testJobList.OrderBy(p => p.TechnicianID)
                                .Skip((page - 1) * PageSize)
                                .Take(PageSize).ToList(),
                JobList = jobRepo.Jobs.Where(m => testJobList.Any(s => s.JobID == m.JobID)).ToList(),
                JobTypeList = itemRepository.JobTypes.ToList(),
                StationsList = testingRepo.Stations.ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = testJobList.Count()
                }
            };
            return View(testJobView);
        }

        public ViewResult NewTestJob()
        {
            return View(new TestJobViewModel());
        }


        public IActionResult SearchTestJob(string Clean, string jobNumber, string jobnumb = "", int MyJobsPage = 1, int PendingToCrossJobPage = 1, int OnCrossJobPage = 1)
        {
            bool admin = GetCurrentUserRole("Admin").Result;
            if (!string.IsNullOrEmpty(jobnumb)) jobNumber = jobnumb;
            if (!string.IsNullOrEmpty(jobNumber)) jobnumb = jobNumber;

            if (!string.IsNullOrEmpty(Clean))
            {
                RedirectToAction("SearchTestJob", "TestJob");
                jobnumb = "";
            }

            List<TestJob> testJobsInCompleted = new List<TestJob>();
            List<TestJob> testJobsWorkingOnIt = new List<TestJob>();
            List<TestJob> testJobsCompleted = new List<TestJob>();
            List<TestJob> testJobsList = new List<TestJob>();
            List<Job> jobList = jobRepo.Jobs.Where(m => m.JobNum.Contains(jobnumb)).ToList();


            foreach (Job job in jobList)
            {
                TestJob TestjobAux = testingRepo.TestJobs.FirstOrDefault(m => m.JobID == job.JobID);
                if (TestjobAux != null) testJobsList.Add(TestjobAux);
            }

            if (testJobsList != null && testJobsList.Count > 0)
            {
                testJobsInCompleted = testJobsList
                 .Where(m => m.Status != "Completed" && m.Status != "Deleted" && m.Status != "Working on it").ToList();

                testJobsWorkingOnIt = testJobsList
                    .Where(m => m.Status == "Working on it").OrderBy(s => s.StationID).ToList();

                testJobsCompleted = testJobsList
                    .Where(m => m.Status == "Completed"
                    && (m.CompletedDate.AddDays(-2) < (DateTime.Now))).OrderBy(s => s.CompletedDate).ToList();
            }

            if (!admin)
            {

                if (testJobsInCompleted == null || string.IsNullOrEmpty(jobnumb))
                    testJobsInCompleted = testingRepo.TestJobs.Where(m => m.Status != "Completed" && m.Status != "Deleted" && m.Status != "Working on it").ToList();

                if (testJobsWorkingOnIt == null || string.IsNullOrEmpty(jobnumb)) testJobsWorkingOnIt = testingRepo.TestJobs.Where(m => m.Status == "Working on it").OrderBy(s => s.StationID).ToList();

                if (testJobsCompleted == null || string.IsNullOrEmpty(jobnumb)) testJobsCompleted = testingRepo.TestJobs.Where(m => m.Status == "Completed"
                    && (m.CompletedDate.AddDays(2) > (DateTime.Now))).OrderBy(s => s.CompletedDate).ToList();
            }
            else
            {
                if (testJobsInCompleted == null || string.IsNullOrEmpty(jobnumb)) testJobsInCompleted = testingRepo.TestJobs.Where(m => m.Status != "Completed" && m.Status != "Deleted" && m.Status != "Working on it").ToList();

                if (testJobsCompleted == null || string.IsNullOrEmpty(jobnumb)) testJobsCompleted = testingRepo.TestJobs.Where(m => m.Status == "Working on it").OrderBy(s => s.StationID).ToList();

                if (testJobsCompleted == null || string.IsNullOrEmpty(jobnumb)) testJobsCompleted = testingRepo.TestJobs.Where(m => m.Status == "Completed" || m.Status == "Deleted"
                    && (m.CompletedDate.AddDays(2) > (DateTime.Now))).OrderBy(s => s.CompletedDate).ToList();
            }

            TestJobViewModel testJobView = new TestJobViewModel
            {
                TestJobIncompletedList = testJobsInCompleted
                    .OrderBy(p => p.TechnicianID)
                    .Skip((MyJobsPage - 1) * 5)
                    .Take(5).ToList(),
                TestJobCompletedList = testJobsCompleted
                    .OrderBy(p => p.TechnicianID)
                    .Skip((OnCrossJobPage - 1) * 5)
                    .Take(5).ToList(),
                TestWorkingOnItList = testJobsWorkingOnIt
                    .OrderBy(p => p.TechnicianID)
                    .Skip((PendingToCrossJobPage - 1) * 5)
                    .Take(5).ToList(),

                JobList = jobRepo.Jobs.ToList(),
                JobTypeList = itemRepository.JobTypes.ToList(),
                StationsList = testingRepo.Stations.ToList(),
                StepList = testingRepo.Steps.ToList(),
                StepsForJobList = testingRepo.StepsForJobs.ToList(),
                StopList = testingRepo.Stops.Where(m => m.StopID != 980 & m.StopID != 981 && m.Reason2 == 0).ToList(),
                PagingInfoWorkingOnIt = new PagingInfo
                {
                    CurrentPage = PendingToCrossJobPage,
                    JobNumb = jobnumb,
                    ItemsPerPage = 5,
                    TotalItems = testJobsWorkingOnIt.Count()
                },

                PagingInfoIncompleted = new PagingInfo
                {
                    CurrentPage = MyJobsPage,
                    ItemsPerPage = 5,
                    JobNumb = jobnumb,
                    TotalItems = testJobsInCompleted.Count()
                },
                PagingInfoCompleted = new PagingInfo
                {
                    CurrentPage = OnCrossJobPage,
                    ItemsPerPage = 5,
                    JobNumb = jobnumb,
                    TotalItems = testJobsCompleted.Count()
                },

            };

            if (string.IsNullOrEmpty(jobNumber))
                return View(testJobView);
            if (testJobsList.Count > 0 && testJobsList[0] != null)
                return View(testJobView);

            TempData["message"] = $"Does not exist any job with the JobNum #{jobNumber}, please try again.";
            TempData["alert"] = $"alert-danger";
            return View(testJobView);
        }

        [HttpPost]
        public IActionResult SearchJob(TestJobViewModel viewModel, string JobType)
        {
            AppUser currentUser = GetCurrentUser().Result;
            var jobSearch = jobRepo.Jobs.AsQueryable();
            var POSearch = jobRepo.POs.AsQueryable();
            TestJobViewModel testJobSearchAux = new TestJobViewModel { };

            TestJob CurrentTestJob = testingRepo.TestJobs.FirstOrDefault(m => m.TechnicianID == currentUser.EngID && m.Status == "Working on it");
            if (CurrentTestJob == null)
            {
                if (viewModel.POJobSearch >= 3000000 && viewModel.POJobSearch <= 4900000)
                {

                    PO onePO = POSearch.FirstOrDefault(m => m.PONumb == viewModel.POJobSearch);
                    if (onePO != null)
                    {
                        var _jobSearch = jobSearch.FirstOrDefault(m => m.JobID == onePO.JobID);

                        if (_jobSearch != null && _jobSearch.Status != "Incomplete" && _jobSearch.Contractor != "Fake")
                        {
                            TestJob testJobAu = testingRepo.TestJobs.FirstOrDefault(m => m.SinglePO == onePO.PONumb);
                            if (testJobAu != null)
                            {
                                TempData["alert"] = $"alert-danger";
                                TempData["message"] = $"Error, Ya existe un TestJob con ese PO, intente de nuevo o contacte al Admin";
                                return View("NewTestJob", testJobSearchAux);
                            }

                            TestJob testJob = new TestJob
                            {
                                JobID = _jobSearch.JobID,
                                TechnicianID = currentUser.EngID,
                                SinglePO = viewModel.POJobSearch,
                                Status = "Incomplete",
                                StartDate = DateTime.Now,
                                CompletedDate = DateTime.Now,
                                StationID = 0
                            };
                            testingRepo.SaveTestJob(testJob);

                            var currentTestJob = testingRepo.TestJobs.Last(s => s.TestJobID == testJob.TestJobID);


                            TestJobViewModel NewtestJobView = new TestJobViewModel();
                            NewtestJobView.TestJob = currentTestJob;
                            NewtestJobView.Job = _jobSearch;
                            NewtestJobView.POJobSearch = testJob.SinglePO;
                            NewtestJobView.PO = jobRepo.POs.FirstOrDefault(m => m.JobID == _jobSearch.JobID);
                            NewtestJobView.isNotDummy = true;
                            NewtestJobView.TestFeature = new TestFeature();
                            NewtestJobView.TestFeature.TestJobID = testJob.TestJobID;
                            NewtestJobView.CurrentTab = "NewFeatures";
                            NewtestJobView.Job.JobNumFirstDigits = getJobNumbDivided(_jobSearch.JobNum).firstDigits;
                            NewtestJobView.Job.JobNumLastDigits = getJobNumbDivided(_jobSearch.JobNum).lastDigits;

                            int jobtypeID = jobRepo.Jobs.First(m => m.JobID == _jobSearch.JobID).JobTypeID;
                            switch (JobTypeName(jobtypeID))
                            {
                                case "M2000":
                                case "M4000":
                                    NewtestJobView.JobExtension = jobRepo.JobsExtensions.FirstOrDefault(m => m.JobID == NewtestJobView.Job.JobID);
                                    NewtestJobView.HydroSpecific = jobRepo.HydroSpecifics.FirstOrDefault(m => m.JobID == NewtestJobView.Job.JobID);
                                    NewtestJobView.GenericFeatures = jobRepo.GenericFeaturesList.FirstOrDefault(m => m.JobID == NewtestJobView.Job.JobID);
                                    NewtestJobView.Indicator = jobRepo.Indicators.FirstOrDefault(m => m.JobID == NewtestJobView.Job.JobID);
                                    NewtestJobView.HoistWayData = jobRepo.HoistWayDatas.FirstOrDefault(m => m.JobID == NewtestJobView.Job.JobID);
                                    break;
                                case "ElmHydro":
                                    Element element = jobRepo.Elements.FirstOrDefault(m => m.JobID == NewtestJobView.Job.JobID);
                                    ElementHydro elementHydro = jobRepo.ElementHydros.FirstOrDefault(m => m.JobID == NewtestJobView.Job.JobID);
                                    NewtestJobView.JobExtension = new JobExtension();
                                    NewtestJobView.HydroSpecific = new HydroSpecific();
                                    NewtestJobView.GenericFeatures = new GenericFeatures();
                                    NewtestJobView.Indicator = new Indicator();
                                    NewtestJobView.HoistWayData = new HoistWayData();

                                    NewtestJobView.HoistWayData.LandingSystemID = element.LandingSystemID;
                                    if (element.DoorOperatorID == 7) NewtestJobView.MOD = true;
                                    else NewtestJobView.MOD = false;
                                    if (element.DoorOperatorID == 2) NewtestJobView.Manual = true;
                                    else NewtestJobView.Manual = false;

                                    NewtestJobView.HydroSpecific.BatteryBrand = element.HAPS == true ? "HAPS" : "";
                                    NewtestJobView.JobExtension.JobTypeMain = "Simplex";


                                    break;
                                case "ElmTract":
                                    Element element2 = jobRepo.Elements.FirstOrDefault(m => m.JobID == NewtestJobView.Job.JobID);
                                    ElementTraction elementTract = jobRepo.ElementTractions.FirstOrDefault(m => m.JobID == NewtestJobView.Job.JobID);
                                    NewtestJobView.JobExtension = new JobExtension();
                                    NewtestJobView.HydroSpecific = new HydroSpecific();
                                    NewtestJobView.GenericFeatures = new GenericFeatures();
                                    NewtestJobView.Indicator = new Indicator();
                                    NewtestJobView.HoistWayData = new HoistWayData();

                                    NewtestJobView.HoistWayData.LandingSystemID = element2.LandingSystemID;
                                    if (element2.DoorOperatorID == 7) NewtestJobView.MOD = true;
                                    else NewtestJobView.MOD = false;
                                    if (element2.DoorOperatorID == 2) NewtestJobView.Manual = true;
                                    else NewtestJobView.Manual = false;

                                    NewtestJobView.HydroSpecific.BatteryBrand = element2.HAPS == true ? "HAPS" : "";
                                    NewtestJobView.JobExtension.JobTypeMain = "Simplex";
                                    break;

                            }

                            return View("NextForm", NewtestJobView);

                        }
                        else
                        {
                            TempData["alert"] = $"alert-danger";
                            TempData["message"] = $"Error,Job aun en ingenieria o duplicado, intente de nuevo o contacte al Admin";
                            return View("NewTestJob", testJobSearchAux);

                        }
                    }
                    else
                    {
                        int JopTypeID = itemRepository.JobTypes.FirstOrDefault(m => m.Name == JobType).JobTypeID;
                        TestJobViewModel testJobView = new TestJobViewModel()
                        {
                            Job = new Job(),
                            JobExtension = new JobExtension(),
                            HydroSpecific = new HydroSpecific(),
                            GenericFeatures = new GenericFeatures(),
                            Indicator = new Indicator(),
                            HoistWayData = new HoistWayData(),
                            SpecialFeature = new SpecialFeatures(),
                            PO = new PO { PONumb = viewModel.POJobSearch },
                            TestFeature = new TestFeature()
                        };

                        testJobView.Job.JobTypeID = JopTypeID;
                        testJobView.Job.ShipDate = DateTime.Now;
                        testJobView.isNotDummy = false;
                        testJobView.CurrentTab = "DummyJob";

                        return View("NextForm", testJobView);
                    }

                }
                else
                {
                    return View(testJobSearchAux);
                }
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, tiene un job pendiente por terminar, terminelo e intente de nuevo o contacte al Admin";
                return View("NewTestJob", testJobSearchAux);

            }
        }

        [HttpPost]
        public IActionResult NextForm(TestJobViewModel nextViewModel)
        {
            nextViewModel.Job.JobNum = getJobNumb(nextViewModel.Job.JobNumFirstDigits, nextViewModel.Job.JobNumLastDigits);

            if (nextViewModel.TestFeature != null)
            {
                TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == nextViewModel.TestJob.TestJobID);


                if (testJob != null)
                {
                    Job CurrentJob = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
                    JobExtension jobExtension = jobRepo.JobsExtensions.FirstOrDefault(m => m.JobID == testJob.JobID);

                    try
                    {
                        if (jobExtension != null)
                            nextViewModel.JobExtension = jobExtension;
                    }
                    catch { }


                    TestFeature testFeature = testingRepo.TestFeatures.FirstOrDefault(m => m.TestJobID == testJob.TestJobID);

                    TestJob StationAuxTestJob = testingRepo.TestJobs.FirstOrDefault(m => m.StationID == nextViewModel.TestJob.StationID && m.Status == "Working on it");
                    if (StationAuxTestJob != null || nextViewModel.TestJob.StationID == 0)
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Error en la estacion, seleccione otra e intente de nuevo o contacte al Admin";
                        nextViewModel.TestJob.StationID = 0;
                        return View("NextForm", nextViewModel);
                    }

                    AppUser currentUser = GetCurrentUser().Result;
                    bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                    bool isAdmin = GetCurrentUserRole("Admin").Result;
                    bool isSameEngineer = currentUser.EngID == testJob.TechnicianID;
                    bool isNotCompleted = testJob.Status != "Completed";

                    if (isNotCompleted && (isSameEngineer || isAdmin || isTechAdmin))
                    {
                        testingRepo.SaveTestFeature(nextViewModel.TestFeature);
                        if (nextViewModel.isNotDummy == false) UpdateDummyJob(nextViewModel);
                        testJob.Status = "Working on it";
                        testJob.StationID = nextViewModel.TestJob.StationID;
                        testJob.JobLabel = nextViewModel.TestJob.JobLabel;
                        testingRepo.SaveTestJob(testJob);
                        nextViewModel.TestJob = testJob;
                        nextViewModel.JobTypeName = itemRepository.JobTypes.FirstOrDefault(m => m.JobTypeID == nextViewModel.Job.JobTypeID).Name;
                        TempData["message"] = $"everything was saved";

                        return NewTestFeatures(nextViewModel);
                    }
                    else
                    {
                        TempData["alert"] = $"alert-danger";
                        if (isNotCompleted == false) TempData["message"] = $"Error, El Testjob ya ha sido completado, intente de nuevo o contacte al Admin";
                        else TempData["message"] = $"Error, El Testjob a sido reasignado, intente de nuevo o contacte al Admin";

                        return RedirectToAction("Index", "Home");
                    }

                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {

                if (nextViewModel.isNotDummy == false)
                {
                    nextViewModel = NewDummyJob(nextViewModel);
                    TempData["message"] = $"Job was saved";
                }


                nextViewModel.Job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                nextViewModel.PO = jobRepo.POs.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                if (nextViewModel.Job.CityID == 10) nextViewModel.Canada = true;
                if (nextViewModel.Job.CityID == 11) nextViewModel.Ontario = true;

                int jobtypeID = jobRepo.Jobs.First(m => m.JobID == nextViewModel.Job.JobID).JobTypeID;
                switch (JobTypeName(jobtypeID))
                {
                    case "M2000":
                    case "M4000":
                        nextViewModel.JobExtension = jobRepo.JobsExtensions.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                        nextViewModel.HydroSpecific = jobRepo.HydroSpecifics.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                        nextViewModel.GenericFeatures = jobRepo.GenericFeaturesList.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                        nextViewModel.Indicator = jobRepo.Indicators.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                        nextViewModel.HoistWayData = jobRepo.HoistWayDatas.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                        break;
                    case "ElmHydro":
                        Element element = jobRepo.Elements.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                        ElementHydro elementHydro = jobRepo.ElementHydros.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                        nextViewModel.JobExtension = new JobExtension();
                        nextViewModel.HydroSpecific = new HydroSpecific();
                        nextViewModel.GenericFeatures = new GenericFeatures();
                        nextViewModel.Indicator = new Indicator();
                        nextViewModel.HoistWayData = new HoistWayData();

                        nextViewModel.HoistWayData.LandingSystemID = element.LandingSystemID;
                        if (element.DoorOperatorID == 7) nextViewModel.MOD = true;
                        else nextViewModel.MOD = false;
                        if (element.DoorOperatorID == 2) nextViewModel.Manual = true;
                        else nextViewModel.Manual = false;

                        nextViewModel.HydroSpecific.BatteryBrand = element.HAPS == true ? "HAPS" : "";
                        nextViewModel.JobExtension.JobTypeMain = "Simplex";


                        break;
                    case "ElmTract":
                        Element element2 = jobRepo.Elements.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                        ElementTraction elementTract = jobRepo.ElementTractions.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                        nextViewModel.JobExtension = new JobExtension();
                        nextViewModel.HydroSpecific = new HydroSpecific();
                        nextViewModel.GenericFeatures = new GenericFeatures();
                        nextViewModel.Indicator = new Indicator();
                        nextViewModel.HoistWayData = new HoistWayData();

                        nextViewModel.HoistWayData.LandingSystemID = element2.LandingSystemID;
                        if (element2.DoorOperatorID == 7) nextViewModel.MOD = true;
                        else nextViewModel.Manual = false;

                        nextViewModel.HydroSpecific.BatteryBrand = element2.HAPS == true ? "HAPS" : "";
                        nextViewModel.JobExtension.JobTypeMain = "Simplex";

                        break;
                }


                nextViewModel.JobTypeName = itemRepository.JobTypes.FirstOrDefault(m => m.JobTypeID == nextViewModel.Job.JobTypeID).Name;
                nextViewModel.TestFeature = new TestFeature();
                nextViewModel.TestFeature.TestJobID = nextViewModel.TestJob.TestJobID;
                nextViewModel.CurrentTab = "NewFeatures";
                return View(nextViewModel);
            }

        }

        [HttpPost]
        public IActionResult NewTestFeatures(TestJobViewModel testJobView)
        {
            bool techAdmin = GetCurrentUserRole("TechAdmin").Result;
            int TechnicianID = GetCurrentUser().Result.EngID;

            TestJob testJobToUpdate = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestJob.TestJobID);

            if (testJobToUpdate.TechnicianID != TechnicianID && techAdmin == false) return RedirectToAction("Index", "Home");
            if (testJobView.TestJob.StationID == 0 && techAdmin == false && TechnicianID == testJobToUpdate.TechnicianID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, en la estacion, seleccione otra e intente de nuevo o contacte al Admin";
                return View("NextForm", testJobView);
            }
            else
            {
                testJobToUpdate.StationID = testJobView.TestJob.StationID;
                testJobToUpdate.JobLabel = testJobView.TestJob.JobLabel;
                testingRepo.SaveTestJob(testJobToUpdate);
                //Checa que la lista de features no este vacia o nula
                if (testJobView.TestFeature != null)
                {
                    List<StepsForJob> OldStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJobView.TestJob.TestJobID).ToList();
                    if (OldStepsForJob.Count > 0)
                    {
                        List<StepsForJob> NewStepsForJob = MakeStepsForJobList(testJobView);

                        foreach (StepsForJob OldStep in OldStepsForJob)
                        {
                            if (!(NewStepsForJob.Any(s => s.StepID == OldStep.StepID)))
                            {
                                OldStep.Obsolete = true;
                                testingRepo.SaveStepsForJob(OldStep);
                            }
                        }

                        foreach (StepsForJob Newstep in NewStepsForJob)
                        {
                            if (!(OldStepsForJob.Any(s => s.StepID == Newstep.StepID)))
                            {
                                testingRepo.SaveStepsForJob(Newstep);
                            }
                        }

                    }
                    else
                    {
                        foreach (StepsForJob step in MakeStepsForJobList(testJobView))
                        {
                            if (step != null) testingRepo.SaveStepsForJob(step);
                        }
                    }
                    //Nuevos consecutivos
                    var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJobView.TestFeature.TestJobID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();

                    int PreviusStepNumber = 1;
                    foreach (StepsForJob step in AllStepsForJob)
                    {
                        step.Consecutivo = PreviusStepNumber;
                        testingRepo.SaveStepsForJob(step);
                        PreviusStepNumber++;
                    }


                    //Despues de terminar de hacer la lista de steps para job se manda el primero a la siguiente vista
                    var stepsForAUX = testingRepo.StepsForJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestFeature.TestJobID && m.Consecutivo == 1); stepsForAUX.Start = DateTime.Now;
                    testingRepo.SaveStepsForJob(stepsForAUX);


                    var stepsFor = AllStepsForJob.FirstOrDefault(m => (m.TestJobID == testJobView.TestFeature.TestJobID && m.Consecutivo == 1 && m.Complete == false) || (m.Complete == false));

                    var AllStepsForJobInfo = testingRepo.Steps.Where(m => AllStepsForJob.Any(s => s.StepID == m.StepID)).ToList();
                    var stepInfo = AllStepsForJobInfo.FirstOrDefault(m => m.StepID == stepsFor.StepID);

                    var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestJob.TestJobID);
                    testjobinfo.StartDate = DateTime.Now;
                    testingRepo.SaveTestJob(testjobinfo);

                    var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);

                    List<Stop> StopsFromTestJob = testingRepo.Stops.Where(m => m.TestJobID == testJobView.TestJob.TestJobID && m.Critical == false).ToList(); bool StopNC = false;
                    if (StopsFromTestJob.Count > 0 && StopsFromTestJob[0] != null) StopNC = true;

                    var auxtStepsPerStageInfo = AllStepsForJobInfo.Where(m => m.Stage == stepInfo.Stage).ToList();
                    int StepsPerStage = auxtStepsPerStageInfo.Count();
                    int auxtStepsPerStage = AllStepsForJob.Where(m => auxtStepsPerStageInfo.Any(s => s.StepID == m.StepID)).Where(m => m.Complete == true).Count() + 1;


                    if (techAdmin) return RedirectToAction("Index", "Home");

                    return View("StepsForJob", new TestJobViewModel
                    {
                        StepsForJob = stepsFor,
                        Step = stepInfo,
                        Job = job,
                        TestJob = testjobinfo,
                        StepList = AllStepsForJobInfo,
                        StepsForJobList = AllStepsForJob,
                        CurrentStep = auxtStepsPerStage,
                        TotalStepsPerStage = StepsPerStage,
                        StopNC = StopNC,
                        StopList = StopsFromTestJob
                    });
                }
            }



            return View(NotFound());
        }

        public IActionResult ContinueForm(int ID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);

            if (testJob != null)
            {
                Job CurrentJob = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
                TestFeature testFeature = testingRepo.TestFeatures.FirstOrDefault(m => m.TestJobID == testJob.TestJobID);
                TestJobViewModel nextViewModel = new TestJobViewModel();

                AppUser currentUser = GetCurrentUser().Result;
                bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool isAdmin = GetCurrentUserRole("Admin").Result;
                bool isSameEngineer = currentUser.EngID == testJob.TechnicianID;
                bool isNotCompleted = testJob.Status != "Completed";

                if (isNotCompleted && (isSameEngineer || isAdmin || isTechAdmin))
                {
                    int jobtypeID = jobRepo.Jobs.First(m => m.JobID == CurrentJob.JobID).JobTypeID;
                    nextViewModel.PO = jobRepo.POs.FirstOrDefault(m => m.JobID == CurrentJob.JobID);
                    nextViewModel.TestJob = testJob;
                    nextViewModel.Job = CurrentJob;

                    nextViewModel.TestFeature = testFeature != null ? testFeature : new TestFeature() { TestJobID = testJob.TestJobID };
                    nextViewModel.isNotDummy = CurrentJob.Contractor == "Fake" ? false : true;
                    nextViewModel.CurrentTab = "NewFeatures";
                    nextViewModel.JobTypeName = JobTypeName(jobtypeID);

                    nextViewModel.Job.JobNumFirstDigits = getJobNumbDivided(nextViewModel.Job.JobNum).firstDigits;
                    nextViewModel.Job.JobNumLastDigits = getJobNumbDivided(nextViewModel.Job.JobNum).lastDigits;



                    switch (JobTypeName(jobtypeID))
                    {
                        case "M2000":
                        case "M4000":
                            nextViewModel.JobExtension = jobRepo.JobsExtensions.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            nextViewModel.HydroSpecific = jobRepo.HydroSpecifics.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            nextViewModel.GenericFeatures = jobRepo.GenericFeaturesList.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            nextViewModel.Indicator = jobRepo.Indicators.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            nextViewModel.HoistWayData = jobRepo.HoistWayDatas.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            break;
                        case "ElmHydro":
                            Element element = jobRepo.Elements.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            ElementHydro elementHydro = jobRepo.ElementHydros.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            nextViewModel.JobExtension = new JobExtension();
                            nextViewModel.HydroSpecific = new HydroSpecific();
                            nextViewModel.GenericFeatures = new GenericFeatures();
                            nextViewModel.Indicator = new Indicator();
                            nextViewModel.HoistWayData = new HoistWayData();

                            nextViewModel.HoistWayData.LandingSystemID = element.LandingSystemID;
                            if (element.DoorOperatorID == 7) nextViewModel.MOD = true;
                            else nextViewModel.MOD = false;
                            if (element.DoorOperatorID == 2) nextViewModel.Manual = true;
                            else nextViewModel.Manual = false;

                            nextViewModel.HydroSpecific.BatteryBrand = element.HAPS == true ? "HAPS" : "";
                            nextViewModel.JobExtension.JobTypeMain = "Simplex";


                            break;
                        case "ElmTract":
                            Element element2 = jobRepo.Elements.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            ElementTraction elementTract = jobRepo.ElementTractions.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            nextViewModel.JobExtension = new JobExtension();
                            nextViewModel.HydroSpecific = new HydroSpecific();
                            nextViewModel.GenericFeatures = new GenericFeatures();
                            nextViewModel.Indicator = new Indicator();
                            nextViewModel.HoistWayData = new HoistWayData();

                            nextViewModel.HoistWayData.LandingSystemID = element2.LandingSystemID;
                            if (element2.DoorOperatorID == 7) nextViewModel.MOD = true;
                            else nextViewModel.MOD = false;
                            if (element2.DoorOperatorID == 2) nextViewModel.Manual = true;
                            else nextViewModel.Manual = false;

                            nextViewModel.HydroSpecific.BatteryBrand = element2.HAPS == true ? "HAPS" : "";
                            nextViewModel.JobExtension.JobTypeMain = "Simplex";
                            break;

                    }

                    return View("NextForm", nextViewModel);
                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    if (isNotCompleted == false) TempData["message"] = $"Error, El Testjob ya ha sido completado, intente de nuevo o contacte al Admin";
                    else TempData["message"] = $"Error, El Testjob a sido reasignado, intente de nuevo o contacte al Admin";

                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult EditTestJob(int ID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);

            if (testJob != null)
            {
                Job CurrentJob = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
                TestFeature testFeature = testingRepo.TestFeatures.FirstOrDefault(m => m.TestJobID == testJob.TestJobID);
                TestJobViewModel nextViewModel = new TestJobViewModel();

                AppUser currentUser = GetCurrentUser().Result;
                bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool isAdmin = GetCurrentUserRole("Admin").Result;
                bool isSameEngineer = currentUser.EngID == testJob.TechnicianID;
                bool isNotCompleted = testJob.Status != "Completed";
                bool isIncomplete = testJob.Status == "Incomplete";

                if (isIncomplete && (isAdmin || isTechAdmin))
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Error, El Testjob no esta completo";

                    return RedirectToAction("Index", "Home");
                }

                if (!isNotCompleted && !(isAdmin || isTechAdmin))
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Error, El Testjob ya ha sido completado, intente de nuevo o contacte al Admin";
                    return RedirectToAction("Index", "Home");
                }

                if (isSameEngineer || isAdmin || isTechAdmin)
                {
                    nextViewModel.PO = jobRepo.POs.FirstOrDefault(m => m.JobID == CurrentJob.JobID);
                    nextViewModel.TestJob = testJob;
                    nextViewModel.Job = CurrentJob;
                    nextViewModel.TestFeature = testFeature;
                    nextViewModel.isNotDummy = CurrentJob.Contractor == "Fake" ? false : true;
                    nextViewModel.CurrentTab = "DummyJob";

                    nextViewModel.Job.JobNumFirstDigits = getJobNumbDivided(nextViewModel.Job.JobNum).firstDigits;
                    nextViewModel.Job.JobNumLastDigits = getJobNumbDivided(nextViewModel.Job.JobNum).lastDigits;

                    if (CurrentJob.CityID == 10) nextViewModel.Canada = true;
                    if (CurrentJob.CityID == 11) nextViewModel.Ontario = true;

                    int jobtypeID = jobRepo.Jobs.First(m => m.JobID == CurrentJob.JobID).JobTypeID;
                    switch (JobTypeName(jobtypeID))
                    {
                        case "M2000":
                        case "M4000":
                            nextViewModel.JobExtension = jobRepo.JobsExtensions.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            nextViewModel.HydroSpecific = jobRepo.HydroSpecifics.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            nextViewModel.GenericFeatures = jobRepo.GenericFeaturesList.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            nextViewModel.Indicator = jobRepo.Indicators.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            nextViewModel.HoistWayData = jobRepo.HoistWayDatas.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            break;
                        case "ElmHydro":
                            Element element = jobRepo.Elements.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            ElementHydro elementHydro = jobRepo.ElementHydros.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            nextViewModel.JobExtension = new JobExtension();
                            nextViewModel.HydroSpecific = new HydroSpecific();
                            nextViewModel.GenericFeatures = new GenericFeatures();
                            nextViewModel.Indicator = new Indicator();
                            nextViewModel.HoistWayData = new HoistWayData();

                            nextViewModel.HoistWayData.LandingSystemID = element.LandingSystemID;
                            if (element.DoorOperatorID == 7) nextViewModel.MOD = true;
                            else nextViewModel.MOD = false;
                            if (element.DoorOperatorID == 2) nextViewModel.Manual = true;
                            else nextViewModel.Manual = false;

                            nextViewModel.HydroSpecific.BatteryBrand = element.HAPS == true ? "HAPS" : "";
                            nextViewModel.JobExtension.JobTypeMain = "Simplex";


                            break;
                        case "ElmTract":
                            Element element2 = jobRepo.Elements.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            ElementTraction elementTract = jobRepo.ElementTractions.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                            nextViewModel.JobExtension = new JobExtension();
                            nextViewModel.HydroSpecific = new HydroSpecific();
                            nextViewModel.GenericFeatures = new GenericFeatures();
                            nextViewModel.Indicator = new Indicator();
                            nextViewModel.HoistWayData = new HoistWayData();

                            nextViewModel.HoistWayData.LandingSystemID = element2.LandingSystemID;
                            if (element2.DoorOperatorID == 7) nextViewModel.MOD = true;
                            else nextViewModel.MOD = false;
                            if (element2.DoorOperatorID == 2) nextViewModel.Manual = true;
                            else nextViewModel.Manual = false;

                            nextViewModel.HydroSpecific.BatteryBrand = element2.HAPS == true ? "HAPS" : "";
                            nextViewModel.JobExtension.JobTypeMain = "Simplex";
                            break;

                    }
                    nextViewModel.JobTypeName = itemRepository.JobTypes.FirstOrDefault(m => m.JobTypeID == CurrentJob.JobTypeID).Name;


                    if (!isNotCompleted && (isAdmin || isTechAdmin))
                        nextViewModel.isNotDummy = true;

                    return View(nextViewModel);
                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Error, El Testjob ya ha sido reasignado, intente de nuevo o contacte al Admin";


                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }



        }

        [HttpPost]
        public IActionResult EditTestJob(TestJobViewModel viewModel)
        {
            viewModel.Job.JobNum = getJobNumb(viewModel.Job.JobNumFirstDigits, viewModel.Job.JobNumLastDigits);

            bool techAdmin = GetCurrentUserRole("TechAdmin").Result;
            int TechnicianID = GetCurrentUser().Result.EngID;

            TestJob testJobToUpdate = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == viewModel.TestJob.TestJobID);
            TestJob StationAuxTestJob = testingRepo.TestJobs.FirstOrDefault(m => m.StationID == viewModel.TestJob.StationID && m.Status == "Working on it");

            if (testJobToUpdate.TechnicianID != TechnicianID && techAdmin == false) return RedirectToAction("Index", "Home");
            if (StationAuxTestJob != null || (techAdmin == false && TechnicianID != testJobToUpdate.TechnicianID))
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error la estacion esta ocupada, seleccione otra e intente de nuevo o contacte al Admin";
                return EditTestJob(testJobToUpdate.TestJobID);
            }

            testingRepo.SaveTestFeature(viewModel.TestFeature);
            UpdateTestFeatures(viewModel);
            if (viewModel.isNotDummy == false) UpdateDummyJob(viewModel);

            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == viewModel.TestJob.TestJobID);
            Job CurrentJob = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
            TestFeature testFeature = testingRepo.TestFeatures.FirstOrDefault(m => m.TestJobID == testJob.TestJobID);
            TestJobViewModel nextViewModel = new TestJobViewModel();

            nextViewModel.PO = jobRepo.POs.FirstOrDefault(m => m.JobID == CurrentJob.JobID);
            nextViewModel.TestJob = testJob;
            nextViewModel.Job = CurrentJob;
            nextViewModel.TestFeature = testFeature;
            nextViewModel.isNotDummy = CurrentJob.Contractor == "Fake" ? false : true;
            nextViewModel.CurrentTab = "NewFeatures";

            int jobtypeID = jobRepo.Jobs.First(m => m.JobID == CurrentJob.JobID).JobTypeID;
            switch (JobTypeName(jobtypeID))
            {
                case "M2000":
                case "M4000":
                    nextViewModel.JobExtension = jobRepo.JobsExtensions.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                    nextViewModel.HydroSpecific = jobRepo.HydroSpecifics.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                    nextViewModel.GenericFeatures = jobRepo.GenericFeaturesList.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                    nextViewModel.Indicator = jobRepo.Indicators.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                    nextViewModel.HoistWayData = jobRepo.HoistWayDatas.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                    break;
                case "ElmHydro":
                    Element element = jobRepo.Elements.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                    ElementHydro elementHydro = jobRepo.ElementHydros.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                    nextViewModel.JobExtension = new JobExtension();
                    nextViewModel.HydroSpecific = new HydroSpecific();
                    nextViewModel.GenericFeatures = new GenericFeatures();
                    nextViewModel.Indicator = new Indicator();
                    nextViewModel.HoistWayData = new HoistWayData();

                    nextViewModel.HoistWayData.LandingSystemID = element.LandingSystemID;
                    if (element.DoorOperatorID == 7) nextViewModel.MOD = true;
                    else nextViewModel.MOD = false;
                    if (element.DoorOperatorID == 2) nextViewModel.Manual = true;
                    else nextViewModel.Manual = false;

                    nextViewModel.HydroSpecific.BatteryBrand = element.HAPS == true ? "HAPS" : "";
                    nextViewModel.JobExtension.JobTypeMain = "Simplex";


                    break;
                case "ElmTract":
                    Element element2 = jobRepo.Elements.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                    ElementTraction elementTract = jobRepo.ElementTractions.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                    nextViewModel.JobExtension = new JobExtension();
                    nextViewModel.HydroSpecific = new HydroSpecific();
                    nextViewModel.GenericFeatures = new GenericFeatures();
                    nextViewModel.Indicator = new Indicator();
                    nextViewModel.HoistWayData = new HoistWayData();

                    nextViewModel.HoistWayData.LandingSystemID = element2.LandingSystemID;
                    if (element2.DoorOperatorID == 7) nextViewModel.MOD = true;
                    else nextViewModel.MOD = false;
                    if (element2.DoorOperatorID == 2) nextViewModel.Manual = true;
                    else nextViewModel.Manual = false;

                    nextViewModel.HydroSpecific.BatteryBrand = element2.HAPS == true ? "HAPS" : "";
                    nextViewModel.JobExtension.JobTypeMain = "Simplex";
                    break;

            }
            nextViewModel.JobTypeName = itemRepository.JobTypes.FirstOrDefault(m => m.JobTypeID == CurrentJob.JobTypeID).Name;
            TempData["message"] = $"everything was saved";
            return View(nextViewModel);
        }

        public void UpdateTestFeatures(TestJobViewModel testJobView)
        {
            TestJob testJobToUpdate = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestFeature.TestJobID);
            TestJob StationAuxTestJob = testingRepo.TestJobs.FirstOrDefault(m => m.StationID == testJobView.TestJob.StationID && m.Status == "Working on it");

            testJobToUpdate.JobLabel = testJobView.TestJob.JobLabel;
            if (testJobView.TestJob.StationID != 0 || testJobToUpdate.StationID == 0) testJobToUpdate.StationID = testJobView.TestJob.StationID;
            testingRepo.SaveTestJob(testJobToUpdate);
            //Checa que la lista de features no este vacia o nula
            if (testJobView.TestFeature != null)
            {
                List<StepsForJob> OldStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJobView.TestJob.TestJobID).ToList();
                if (OldStepsForJob.Count > 0)
                {
                    List<StepsForJob> NewStepsForJob = MakeStepsForJobList(testJobView);

                    foreach (StepsForJob OldStep in OldStepsForJob)
                    {
                        if (!(NewStepsForJob.Any(s => s.StepID == OldStep.StepID)))
                        {
                            OldStep.Obsolete = true;
                            testingRepo.SaveStepsForJob(OldStep);
                        }
                    }

                    foreach (StepsForJob Newstep in NewStepsForJob)
                    {
                        if (!(OldStepsForJob.Any(s => s.StepID == Newstep.StepID)))
                        {
                            testingRepo.SaveStepsForJob(Newstep);
                        }
                    }

                }
                else
                {
                    foreach (StepsForJob step in MakeStepsForJobList(testJobView))
                    {
                        if (step != null) testingRepo.SaveStepsForJob(step);
                    }
                }
                //Nuevos consecutivos
                var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJobView.TestFeature.TestJobID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();

                int PreviusStepNumber = 1;
                foreach (StepsForJob step in AllStepsForJob)
                {
                    step.Consecutivo = PreviusStepNumber;
                    testingRepo.SaveStepsForJob(step);
                    PreviusStepNumber++;
                }
            }

        }

        public TestJobViewModel UpdateDummyJob(TestJobViewModel viewModel)
        {
            Job currentJob = new Job();
            TestJob currentTestJob = new TestJob();
            PO poUniqueAUx = poUniqueAUx = jobRepo.POs.FirstOrDefault(m => m.PONumb == viewModel.PO.PONumb);

            AppUser currentUser = GetCurrentUser().Result;

            currentJob = jobRepo.Jobs.FirstOrDefault(m => m.JobID == poUniqueAUx.JobID);
            currentJob.Name = viewModel.Job.Name;
            currentJob.JobNum = viewModel.Job.JobNum;
            currentJob.ShipDate = viewModel.Job.ShipDate;
            if (viewModel.Canada == true) currentJob.CityID = 10;
            else if (viewModel.Ontario == true) currentJob.CityID = 11;
            else currentJob.CityID = 40;
            jobRepo.SaveJob(currentJob);
            currentJob = jobRepo.Jobs.FirstOrDefault(m => m.JobID == poUniqueAUx.JobID);

            int jobtypeID = jobRepo.Jobs.First(m => m.JobID == currentJob.JobID).JobTypeID;
            switch (JobTypeName(jobtypeID))
            {
                case "M2000":
                case "M4000":

                    //Save the dummy Job Extension
                    JobExtension currentExtension = jobRepo.JobsExtensions.FirstOrDefault(m => m.JobID == currentJob.JobID);
                    if (viewModel.MOD == true) currentExtension.DoorOperatorID = 7;
                    else currentExtension.DoorOperatorID = 1;
                    if (viewModel.Manual == true) currentExtension.DoorOperatorID = 2;
                    else currentExtension.DoorOperatorID = 1;
                    jobRepo.SaveJobExtension(currentExtension);

                    //Save the dummy Job HydroSpecific
                    HydroSpecific currenHydroSpecific = jobRepo.HydroSpecifics.FirstOrDefault(m => m.JobID == currentJob.JobID);
                    if (viewModel.TwosStarters == true) currenHydroSpecific.MotorsNum = 3;
                    else currenHydroSpecific.MotorsNum = 1;
                    jobRepo.SaveHydroSpecific(currenHydroSpecific);

                    //Save the dummy Job Generic Features
                    GenericFeatures currentGenericFeatures = jobRepo.GenericFeaturesList.FirstOrDefault(m => m.JobID == currentJob.JobID);
                    if (viewModel.IMonitor == true) currentGenericFeatures.Monitoring = "IMonitor Interface";
                    else if (viewModel.MView == true) currentGenericFeatures.Monitoring = "MView Interface";
                    else currentGenericFeatures.Monitoring = "Fake";
                    jobRepo.SaveGenericFeatures(currentGenericFeatures);

                    break;
                case "ElmTract":
                case "ElmHydro":
                    Element element = jobRepo.Elements.FirstOrDefault(m => m.JobID == currentJob.JobID);
                    element.LandingSystemID = viewModel.HoistWayData.LandingSystemID;
                    element.HAPS = viewModel.HydroSpecific.BatteryBrand == "HAPS" ? true : false;
                    if (viewModel.MOD == true) element.DoorOperatorID = 7;
                    else element.DoorOperatorID = 1;
                    jobRepo.SaveElement(element);
                    break;
            }

            currentTestJob = testingRepo.TestJobs.FirstOrDefault(p => p.JobID == currentJob.JobID);

            TestJobViewModel testJobView = new TestJobViewModel
            {
                TestJob = currentTestJob,
                Job = currentJob
            };

            return testJobView;
        }


        public TestJobViewModel NewDummyJob(TestJobViewModel viewModel)
        {
            Job currentJob = new Job();
            TestJob currentTestJob = new TestJob();
            PO poUniqueAUx = poUniqueAUx = jobRepo.POs.FirstOrDefault(m => m.PONumb == viewModel.PO.PONumb);

            AppUser currentUser = GetCurrentUser().Result;
            Job Job = viewModel.Job;
            Job.Contractor = "Fake"; Job.Cust = "Fake"; Job.FireCodeID = 1; Job.LatestFinishDate = new DateTime(1, 1, 1);
            Job.EngID = currentUser.EngID; Job.Status = "Pending"; Job.CrossAppEngID = 1;
            if (viewModel.Canada == true) Job.CityID = 10;
            else if (viewModel.Ontario == true) Job.CityID = 11;
            else Job.CityID = 40;
            jobRepo.SaveJob(Job);
            currentJob = jobRepo.Jobs.FirstOrDefault(p => p.JobID == jobRepo.Jobs.Max(x => x.JobID));


            int jobtypeID = jobRepo.Jobs.First(m => m.JobID == currentJob.JobID).JobTypeID;
            switch (JobTypeName(jobtypeID))
            {
                case "M2000":
                case "M4000":

                    //Save the dummy Job Extension
                    JobExtension currentExtension = viewModel.JobExtension; currentExtension.JobID = currentJob.JobID; currentExtension.InputFrecuency = 60; currentExtension.InputPhase = 3; currentExtension.DoorGate = "Fake";
                    currentExtension.InputVoltage = 1; currentExtension.NumOfStops = 2; currentExtension.SHCRisers = 1; currentExtension.DoorHoist = "Fake"; currentExtension.JobTypeAdd = "Fake";
                    currentExtension.SCOP = viewModel.JobExtension.SCOP;
                    currentExtension.SHC = viewModel.JobExtension.SHC;
                    if (viewModel.MOD == true) currentExtension.DoorOperatorID = 7;
                    else currentExtension.DoorOperatorID = 1;
                    if (viewModel.Manual == true) currentExtension.DoorOperatorID = 2;
                    else currentExtension.DoorOperatorID = 1;
                    jobRepo.SaveJobExtension(currentExtension);

                    //Save the dummy Job HydroSpecific
                    HydroSpecific currenHydroSpecific = viewModel.HydroSpecific; currenHydroSpecific.JobID = currentJob.JobID; currenHydroSpecific.FLA = 1; currenHydroSpecific.HP = 1;
                    currenHydroSpecific.SPH = 1; currenHydroSpecific.Starter = "Fake"; currenHydroSpecific.ValveCoils = 1; currenHydroSpecific.ValveBrand = "Fake";
                    if (viewModel.TwosStarters == true) currenHydroSpecific.MotorsNum = 3;
                    else currenHydroSpecific.MotorsNum = 1;
                    jobRepo.SaveHydroSpecific(currenHydroSpecific);

                    //Save the dummy job Indicators
                    Indicator currentIndicator = viewModel.Indicator; currentIndicator.CarCallsVoltage = "Fake"; currentIndicator.CarCallsVoltageType = "Fake"; currentIndicator.CarCallsType = "Fake";
                    currentIndicator.HallCallsVoltage = "Fake"; currentIndicator.HallCallsVoltageType = "Fake"; currentIndicator.HallCallsType = "Fake"; currentIndicator.IndicatorsVoltageType = "Fake";
                    currentIndicator.IndicatorsVoltage = 1; currentIndicator.JobID = currentJob.JobID;
                    jobRepo.SaveIndicator(currentIndicator);

                    //Save the dummy Job HoistWayData
                    HoistWayData currentHoistWayData = viewModel.HoistWayData; currentHoistWayData.JobID = currentJob.JobID; currentHoistWayData.Capacity = 1; currentHoistWayData.DownSpeed = 1;
                    currentHoistWayData.TotalTravel = 1; currentHoistWayData.UpSpeed = 1; currentHoistWayData.HoistWaysNumber = 1; currentHoistWayData.MachineRooms = 1;
                    jobRepo.SaveHoistWayData(currentHoistWayData);

                    //Save the dummy Job Generic Features
                    GenericFeatures currentGenericFeatures = viewModel.GenericFeatures; currentGenericFeatures.JobID = currentJob.JobID;
                    if (viewModel.IMonitor == true) currentGenericFeatures.Monitoring = "IMonitor Interface";
                    else currentGenericFeatures.Monitoring = "Fake";
                    jobRepo.SaveGenericFeatures(currentGenericFeatures);
                    if (viewModel.MView == true) currentGenericFeatures.Monitoring = "MView Interface";
                    else currentGenericFeatures.Monitoring = "Fake";
                    jobRepo.SaveGenericFeatures(currentGenericFeatures);

                    break;
                case "ElmHydro":
                    Element element = new Element
                    {
                        JobID = currentJob.JobID,
                        LandingSystemID = viewModel.HoistWayData.LandingSystemID,
                        DoorGate = "Fake",
                        HAPS = viewModel.HydroSpecific.BatteryBrand == "HAPS" ? true : false,
                        INA = "fake",
                        Capacity = 1,
                        Frequency = 1,
                        LoadWeigher = "fake",
                        Phase = 1,
                        Speed = 1,
                        Voltage = 1,
                        DoorBrand = "fake",
                    };
                    if (viewModel.MOD == true) element.DoorOperatorID = 7;
                    else element.DoorOperatorID = 1;

                    jobRepo.SaveElement(element);
                    ElementHydro elementHydro = new ElementHydro
                    {
                        JobID = currentJob.JobID,
                        FLA = 20,
                        HP = 20,
                        SPH = 14,
                        Starter = "fake",
                        ValveBrand = "fake",
                    };
                    jobRepo.SaveElementHydro(elementHydro);
                    break;
                case "ElmTract":

                    Element element2 = new Element
                    {
                        JobID = currentJob.JobID,
                        LandingSystemID = viewModel.HoistWayData.LandingSystemID,
                        DoorGate = "Fake",
                        HAPS = viewModel.HydroSpecific.BatteryBrand == "HAPS" ? true : false,
                        INA = "fake",
                        Capacity = 1,
                        Frequency = 1,
                        LoadWeigher = "fake",
                        Phase = 1,
                        Speed = 1,
                        Voltage = 1,
                        DoorBrand = "fake",
                    };
                    if (viewModel.MOD == true) element2.DoorOperatorID = 7;
                    else element2.DoorOperatorID = 1;

                    jobRepo.SaveElement(element2);
                    ElementTraction elementTraction = new ElementTraction
                    {
                        JobID = currentJob.JobID,
                        Contact = "fake",
                        Current = 10,
                        FLA = 10,
                        HoldVoltage = 10,
                        HP = 10,
                        MachineLocation = "fake",
                        MotorBrand = "fake",
                        PickVoltage = 10,
                        Resistance = 10,
                        VVVF = "fake"

                    };
                    jobRepo.SaveElementTraction(elementTraction);
                    break;
            }

            SpecialFeatures featureFake = viewModel.SpecialFeature; featureFake.JobID = currentJob.JobID; featureFake.Description = null;
            jobRepo.SaveSpecialFeatures(featureFake);

            PO POFake = viewModel.PO; POFake.JobID = currentJob.JobID;
            jobRepo.SavePO(POFake);

            //Create the new TestJob
            TestJob testJob = new TestJob
            {
                JobID = currentJob.JobID,
                TechnicianID = currentUser.EngID,
                SinglePO = POFake.PONumb,
                Status = "Incomplete",
                StartDate = DateTime.Now,
                CompletedDate = DateTime.Now,
                StationID = 0
            };
            testingRepo.SaveTestJob(testJob);

            currentTestJob = testingRepo.TestJobs.FirstOrDefault(p => p.TestJobID == testingRepo.TestJobs.Max(x => x.TestJobID));


            TestJobViewModel testJobView = new TestJobViewModel
            {
                TestJob = currentTestJob,
                Job = currentJob
            };
            testJobView.Job.JobNumFirstDigits = viewModel.Job.JobNumFirstDigits;
            testJobView.Job.JobNumLastDigits = viewModel.Job.JobNumLastDigits;

            return testJobView;
        }

        public ViewResult EditTestFeatures(int ID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);
            Job CurrentJob = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
            TestFeature testFeature = testingRepo.TestFeatures.FirstOrDefault(m => m.TestJobID == testJob.TestJobID);
            TestJobViewModel viewModel = new TestJobViewModel
            {
                TestJob = testJob,
                TestFeature = testFeature,
                Job = CurrentJob
            };

            return View("NewTestFeatures", viewModel);
        }



        [HttpPost]
        public IActionResult StepsForJob(TestJobViewModel viewModel, string movement)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == viewModel.TestJob.TestJobID);

            if (testJob != null)
            {
                AppUser currentUser = GetCurrentUser().Result;
                bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool isAdmin = GetCurrentUserRole("Admin").Result;
                bool isSameEngineer = currentUser.EngID == testJob.TechnicianID;
                bool isNotCompleted = testJob.Status != "Completed";
                bool isNotOnReassigment = testJob.Status != "Reassignment";

                if (isNotCompleted && isNotOnReassigment && (isSameEngineer || isAdmin || isTechAdmin))
                {

                    var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == viewModel.TestJob.TestJobID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
                    var AllStepsForJobInfo = testingRepo.Steps.Where(m => AllStepsForJob.Any(s => s.StepID == m.StepID)).ToList();

                    List<Stop> StopsFromTestJob = testingRepo.Stops.Where(m => m.TestJobID == viewModel.TestJob.TestJobID && m.Critical == false)
                                                                    .Where(m => m.StopID != 980 & m.StopID != 981 && m.Reason2 == 0).ToList();
                    bool StopNC = false;
                    List<Reason1> reason1s = testingRepo.Reasons1.ToList();
                    if (StopsFromTestJob.Count > 0 && StopsFromTestJob[0] != null) StopNC = true;


                    //When all steps are completed
                    if (AllStepsForJob.Where(m => m.Complete == false).Count() == 0 || (AllStepsForJob.Where(m => m.Complete == false).Count() == 1 && movement == "next"))
                    {

                        if (StopsFromTestJob.Count > 0 && StopsFromTestJob[0] != null)
                        {
                            TempData["alert"] = $"alert-danger";
                            TempData["message"] = $"Error, tiene una parada pendiente por terminar, terminelo e intente de nuevo o contacte al Admin";
                            return (ContinueStep(viewModel.TestJob.TestJobID));
                        }
                        else
                        {
                            var currentStepForJob = AllStepsForJob.FirstOrDefault(m => m.StepsForJobID == viewModel.StepsForJob.StepsForJobID);
                            currentStepForJob.Stop = DateTime.Now;
                            currentStepForJob.Complete = true;
                            TimeSpan elapsed = currentStepForJob.Stop - currentStepForJob.Start;
                            currentStepForJob.Elapsed += elapsed;
                            testingRepo.SaveStepsForJob(currentStepForJob);


                            var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == viewModel.TestJob.TestJobID); testjobinfo.Status = "Completed";
                            testjobinfo.CompletedDate = DateTime.Now;
                            testingRepo.SaveTestJob(testjobinfo);

                            TempData["message"] = $"El Test Job {testjobinfo.TestJobID} se ha completado con exito!";
                            TempData["alert"] = $"alert-success";
                            return RedirectToAction("Index", "Home", 1);
                        }

                    }//For Next Step
                    else if (movement == "next")
                    {
                        var currentStepForJob = AllStepsForJob.FirstOrDefault(m => m.StepsForJobID == viewModel.StepsForJob.StepsForJobID);
                        currentStepForJob.Stop = DateTime.Now;
                        currentStepForJob.Complete = true;
                        TimeSpan elapsed = currentStepForJob.Stop - currentStepForJob.Start;
                        if (currentStepForJob.Elapsed.Hour == 0 && currentStepForJob.Elapsed.Minute == 0 && currentStepForJob.Elapsed.Second == 0)
                        {

                            currentStepForJob.Elapsed = new DateTime(1, 1, 1, elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
                        }
                        else
                        {
                            int newsecond = 0, newhour = 0, newMinute = 0;

                            newsecond = currentStepForJob.Elapsed.Second + elapsed.Seconds;
                            newMinute = currentStepForJob.Elapsed.Minute + elapsed.Minutes;
                            newhour = currentStepForJob.Elapsed.Hour + elapsed.Hours;
                            if (newsecond >= 60)
                            {
                                newsecond -= 60;
                                newMinute++;
                            }
                            newMinute += elapsed.Minutes;
                            if (newMinute >= 60)
                            {
                                newMinute -= 60;
                                newhour++;
                            }


                            currentStepForJob.Elapsed = new DateTime(1, 1, 1, newhour, newMinute, newsecond);
                        }

                        testingRepo.SaveStepsForJob(currentStepForJob);

                        //NextStep
                        var stepsForAUX = AllStepsForJob.OrderBy(m => m.Consecutivo).FirstOrDefault(m => m.Complete == false);
                        stepsForAUX.Start = DateTime.Now;
                        testingRepo.SaveStepsForJob(stepsForAUX);
                        var nextStepFor = AllStepsForJob.FirstOrDefault(m => m.StepsForJobID == stepsForAUX.StepsForJobID);
                        var stepInfo = testingRepo.Steps.FirstOrDefault(m => m.StepID == nextStepFor.StepID);
                        var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == viewModel.TestJob.TestJobID);
                        var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);

                        var auxtStepsPerStageInfo = AllStepsForJobInfo.Where(m => m.Stage == stepInfo.Stage).ToList();
                        int StepsPerStage = auxtStepsPerStageInfo.Count();
                        int auxtStepsPerStage = AllStepsForJob.Where(m => auxtStepsPerStageInfo.Any(s => s.StepID == m.StepID)).Where(m => m.Complete == true).Count() + 1;

                        return View("StepsForJob", new TestJobViewModel
                        {
                            StepsForJob = nextStepFor,
                            Step = stepInfo,
                            Job = job,
                            TestJob = testjobinfo,
                            StepList = AllStepsForJobInfo,
                            StepsForJobList = AllStepsForJob,
                            CurrentStep = auxtStepsPerStage,
                            TotalStepsPerStage = StepsPerStage,
                            StopNC = StopNC,
                            StopList = StopsFromTestJob,
                            Reasons1List = reason1s,
                        });

                    }//For Previous Step
                    else if (movement == "previus")
                    {

                        var previusStepForAUX = AllStepsForJob.OrderByDescending(m => m.Consecutivo).FirstOrDefault(m => m.Complete == true);
                        var actualStepForAUX = AllStepsForJob.FirstOrDefault(m => m.StepsForJobID == viewModel.StepsForJob.StepsForJobID);

                        //For actual Step
                        actualStepForAUX.Stop = DateTime.Now;
                        TimeSpan elapsed = actualStepForAUX.Stop - actualStepForAUX.Start;
                        if (actualStepForAUX.Elapsed.Hour == 0 && actualStepForAUX.Elapsed.Minute == 0 && actualStepForAUX.Elapsed.Second == 0)
                        {

                            actualStepForAUX.Elapsed = new DateTime(1, 1, 1, elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
                        }
                        else
                        {
                            int newsecond = 0, newhour = 0, newMinute = 0;

                            newsecond = actualStepForAUX.Elapsed.Second + elapsed.Seconds;
                            newMinute = actualStepForAUX.Elapsed.Minute + elapsed.Minutes;
                            newhour = actualStepForAUX.Elapsed.Hour + elapsed.Hours;
                            if (newsecond >= 60)
                            {
                                newsecond -= 60;
                                newMinute++;
                            }
                            newMinute += elapsed.Minutes;
                            if (newMinute >= 60)
                            {
                                newMinute -= 60;
                                newhour++;
                            }


                            actualStepForAUX.Elapsed = new DateTime(1, 1, 1, newhour, newMinute, newsecond);
                        }
                        testingRepo.SaveStepsForJob(actualStepForAUX);

                        //For target step (before actual step)

                        previusStepForAUX.Complete = false;
                        previusStepForAUX.Stop = DateTime.Now;

                        previusStepForAUX.Start = DateTime.Now;
                        testingRepo.SaveStepsForJob(previusStepForAUX);


                        var previusStepFor = AllStepsForJob.FirstOrDefault(m => m.StepsForJobID == previusStepForAUX.StepsForJobID);
                        var stepInfo = testingRepo.Steps.FirstOrDefault(m => m.StepID == previusStepFor.StepID);
                        var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == viewModel.TestJob.TestJobID);
                        var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);

                        var auxtStepsPerStageInfo = AllStepsForJobInfo.Where(m => m.Stage == stepInfo.Stage).ToList();
                        int StepsPerStage = auxtStepsPerStageInfo.Count();
                        int auxtStepsPerStage = AllStepsForJob.Where(m => auxtStepsPerStageInfo.Any(s => s.StepID == m.StepID)).Where(m => m.Complete == true).Count() + 1;

                        return View("StepsForJob", new TestJobViewModel
                        {
                            StepsForJob = previusStepFor,
                            Step = stepInfo,
                            Job = job,
                            TestJob = testjobinfo,
                            StepList = AllStepsForJobInfo,
                            StepsForJobList = AllStepsForJob,
                            CurrentStep = auxtStepsPerStage,
                            TotalStepsPerStage = StepsPerStage,
                            StopNC = StopNC,
                            StopList = StopsFromTestJob,
                            Reasons1List = reason1s,
                        });
                    }

                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    if (isNotCompleted == false) TempData["message"] = $"Error, El Testjob ya ha sido completado, intente de nuevo o contacte al Admin";
                    else TempData["message"] = $"Error, El Testjob a sido reasignado, intente de nuevo o contacte al Admin";

                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }


            return View(NotFound());
        }

        public ViewResult StepsForJobList(TestJobViewModel viewModel, int next)
        {
            List<StepsForJob> StepsForJobList = testingRepo.StepsForJobs.FromSql("select * from dbo.StepsForJobs where dbo.StepsForJobs.StepsForJobID " +
                "IN( select  Max(dbo.StepsForJobs.StepsForJobID ) from dbo.StepsForJobs where dbo.StepsForJobs.TestJobID = {0} group by dbo.StepsForJobs.Consecutivo)", viewModel.TestJob.TestJobID).ToList();

            StepsForJobList = StepsForJobList.Where(m => m.Obsolete == false).ToList();

            var AllStepsForJobInfo = testingRepo.Steps.Where(m => StepsForJobList.Any(s => s.StepID == m.StepID)).ToList();

            var currentStepForJob = StepsForJobList.FirstOrDefault(m => m.Consecutivo == viewModel.StepsForJob.Consecutivo);
            currentStepForJob.Stop = DateTime.Now;
            currentStepForJob.Complete = false;
            TimeSpan elapsed = currentStepForJob.Stop - currentStepForJob.Start;
            currentStepForJob.Elapsed = new DateTime(1, 1, 1, elapsed.Hours, elapsed.Minutes, elapsed.Seconds);
            testingRepo.SaveStepsForJob(currentStepForJob);

            //NextStep
            var stepsForAUX = StepsForJobList.FirstOrDefault(m => m.Consecutivo == next && m.Complete == true); stepsForAUX.Start = DateTime.Now; stepsForAUX.Complete = false;
            testingRepo.SaveStepsForJob(stepsForAUX);
            var nextStepFor = StepsForJobList.FirstOrDefault(m => m.StepsForJobID == stepsForAUX.StepsForJobID);
            var stepInfo = testingRepo.Steps.FirstOrDefault(m => m.StepID == nextStepFor.StepID);
            var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == viewModel.TestJob.TestJobID);
            var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);

            var auxtStepsPerStageInfo = AllStepsForJobInfo.Where(m => m.Stage == stepInfo.Stage).ToList();
            int StepsPerStage = auxtStepsPerStageInfo.Count();
            int auxtStepsPerStage = auxtStepsPerStageInfo.IndexOf(stepInfo) + 1;

            return View("StepsForJob", new TestJobViewModel
            {
                StepsForJob = nextStepFor,
                Step = stepInfo,
                Job = job,
                TestJob = testjobinfo,
                StepList = AllStepsForJobInfo,
                StepsForJobList = StepsForJobList,
                CurrentStep = auxtStepsPerStage,
                TotalStepsPerStage = StepsPerStage
            });
        }

        public IActionResult ContinueStep(int ID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);

            if (testJob != null)
            {
                Job CurrentJob = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
                TestFeature testFeature = testingRepo.TestFeatures.FirstOrDefault(m => m.TestJobID == testJob.TestJobID);
                TestJobViewModel nextViewModel = new TestJobViewModel();

                AppUser currentUser = GetCurrentUser().Result;
                bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool isAdmin = GetCurrentUserRole("Admin").Result;
                bool isSameEngineer = currentUser.EngID == testJob.TechnicianID;
                bool isNotCompleted = testJob.Status != "Completed";

                if (isNotCompleted && (isSameEngineer || isAdmin || isTechAdmin))
                {

                    var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == ID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
                    List<Reason1> reason1s = testingRepo.Reasons1.ToList();
                    var AllStepsForJobInfo = testingRepo.Steps.Where(m => AllStepsForJob.Any(s => s.StepID == m.StepID)).ToList();

                    List<Stop> StopsFromTestJob = testingRepo.Stops.Where(m => m.TestJobID == ID && m.Critical == false)
                                                                   .Where(m => m.StopID != 980 & m.StopID != 981 && m.Reason2 == 0).ToList();
                    bool StopNC = false;
                    if (StopsFromTestJob.Count > 0 && StopsFromTestJob[0] != null) StopNC = true;

                    StepsForJob CurrentStep = AllStepsForJob.FirstOrDefault(m => m.Complete == false);
                    testingRepo.SaveStepsForJob(CurrentStep);
                    var stepInfo = testingRepo.Steps.FirstOrDefault(m => m.StepID == CurrentStep.StepID);
                    var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStep.TestJobID);
                    var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);

                    var auxtStepsPerStageInfo = AllStepsForJobInfo.Where(m => m.Stage == stepInfo.Stage).ToList();
                    int StepsPerStage = auxtStepsPerStageInfo.Count();
                    int auxtStepsPerStage = AllStepsForJob.Where(m => auxtStepsPerStageInfo.Any(s => s.StepID == m.StepID)).Where(m => m.Complete == true).Count() + 1;

                    return View("StepsForJob", new TestJobViewModel
                    {
                        StepsForJob = CurrentStep,
                        Step = stepInfo,
                        Job = job,
                        TestJob = testjobinfo,
                        StepList = AllStepsForJobInfo,
                        StepsForJobList = AllStepsForJob,
                        CurrentStep = auxtStepsPerStage,
                        TotalStepsPerStage = StepsPerStage,
                        StopNC = StopNC,
                        StopList = StopsFromTestJob,
                        Reasons1List = reason1s,
                    });
                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    if (isNotCompleted == false) TempData["message"] = $"Error, El Testjob ya ha sido completado, intente de nuevo o contacte al Admin";
                    else TempData["message"] = $"Error, El Testjob a sido reasignado, intente de nuevo o contacte al Admin";

                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

        }


        [HttpPost]
        public IActionResult Delete(int ID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);

            if (testJob != null)
            {

                bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool isAdmin = GetCurrentUserRole("Admin").Result;
                bool isNotCompleted = testJob.Status != "Completed";

                if (isNotCompleted && (isAdmin || isTechAdmin))
                {
                    Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);

                    if (job.Contractor == "Fake")
                    {
                        Job deletDummy = jobRepo.DeleteEngJob(job.JobID);

                        if (deletDummy != null)
                        {
                            TempData["message"] = $"Job with #JobNum {deletDummy.JobNum} and all its depenencies were deleted";
                        }
                    }
                    else
                    {
                        TestJob deletedItem = testingRepo.DeleteTestJob(ID);

                        if (deletedItem != null)
                        {
                            TempData["message"] = $"Testjob with #PO {deletedItem.SinglePO} was deleted";
                        }
                    }



                    if (isAdmin) return RedirectToAction("SearchTestJob", "TestJob");
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    if (isNotCompleted == false) TempData["message"] = $"Error, El Testjob ya ha sido completado, intente de nuevo o contacte al Admin";
                    else TempData["message"] = $"Error, El Testjob a sido reasignado, intente de nuevo o contacte al Admin";

                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult FakeDelete(int ID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);

            if (testJob != null)
            {
                bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool isAdmin = GetCurrentUserRole("Admin").Result;
                bool isCompleted = testJob.Status == "Completed";

                if (isCompleted && (isAdmin || isTechAdmin))
                {

                    TestJob deletedItem = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);

                    deletedItem.Status = "Deleted";
                    testingRepo.SaveTestJob(deletedItem);
                    TempData["message"] = $"You have deleted the TestJob with PO# {deletedItem.SinglePO}";

                    if (isAdmin) return RedirectToAction("SearchTestJob", "TestJob");
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Error, El Testjob se encuentra activo, intente de nuevo o contacte al Admin";

                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult Restore(int ID)
        {
            TestJob restoredItem = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);

            if (restoredItem != null)
            {
                bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool isAdmin = GetCurrentUserRole("Admin").Result;
                bool isCompleted = restoredItem.Status == "Completed";

                if (isCompleted && (isAdmin || isTechAdmin))
                {

                    restoredItem.Status = "Completed";
                    testingRepo.SaveTestJob(restoredItem);
                    TempData["message"] = $"You have restored the TestJob with PO# {restoredItem.SinglePO}";

                    return RedirectToAction("SearchTestJob", "TestJob");

                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Error, El Testjob se encuentra activo, intente de nuevo o contacte al Admin";

                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }
        }

        public ViewResult AllStepsForJob(int ID, int page = 1)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);
            Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
            TestJobViewModel testJobView = new TestJobViewModel
            {
                StepsForJobList = testingRepo.StepsForJobs
                 .Where(m => m.TestJobID == ID)
                 .OrderBy(p => p.Consecutivo)
                 .Skip((page - 1) * PageSize)
                 .Take(PageSize).ToList(),
                Job = job,
                TestJob = testJob,
                StepList = testingRepo.Steps.Where(m => m.JobTypeID == job.JobTypeID).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = testingRepo.StepsForJobs
                     .Where(m => m.TestJobID == ID).ToList().Count()
                }
            };

            return View(testJobView);
        }

        public List<StepsForJob> MakeStepsForJobList(TestJobViewModel testJobView)
        {
            List<StepsForJob> steps = new List<StepsForJob>();
            //guarda la lista de features
            TestJob testJobModify = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestJob.TestJobID);
            //testJobModify.Station = testJobView.TestJob.Station; testJobModify.JobLabel = testJobView.TestJob.JobLabel;
            testingRepo.SaveTestJob(testJobModify);
            testingRepo.SaveTestFeature(testJobView.TestFeature);

            //Rellena las listas que se llenaran para la comparacion
            List<TriggeringFeature> TriggersWithNameNull = testingRepo.TriggeringFeatures.Where(m => m.Name == null).ToList();
            List<TriggeringFeature> TriggersWithOutNameNull = testingRepo.TriggeringFeatures.Where(m => m.Name != null).ToList();
            var FeaturesFromTestJob = testingRepo.TestFeatures.First(m => m.TestJobID == testJobView.TestFeature.TestJobID);


            int jobtypeID = jobRepo.Jobs.First(m => m.JobID == testJobView.TestJob.JobID).JobTypeID;
            switch (JobTypeName(jobtypeID))
            {
                case "M2000":
                case "M4000":

                    Job FeaturesFromJob = jobRepo.Jobs.Include(m => m._jobExtension).Include(m => m._HydroSpecific).Include(m => m._HoistWayData).Include(m => m._GenericFeatures)
                .First(m => m.JobID == testJobView.TestJob.JobID);
                    List<Step> Steps = testingRepo.Steps.OrderBy(m => m.Order).Where(m => m.JobTypeID == FeaturesFromJob.JobTypeID).ToList();
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
                                    TestJobID = FeaturesFromTestJob.TestJobID,
                                    Start = DateTime.Now,
                                    Stop = DateTime.Now,
                                    Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                                    Consecutivo = consecutivo,
                                    AuxStationID = testJobModify.StationID,
                                    AuxTechnicianID = testJobModify.TechnicianID
                                };

                                steps.Add(stepForJob);
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
                                        LandingSystem landing = itemRepository.LandingSystems.FirstOrDefault(m => m.LandingSystemID == FeaturesFromJob._HoistWayData.LandingSystemID);
                                        City UniqueCity = itemRepository.Cities.FirstOrDefault(m => m.CityID == FeaturesFromJob.CityID);
                                        State StateFromCity = itemRepository.States.FirstOrDefault(m => m.StateID == UniqueCity.StateID);
                                        switch (trigger.Name)
                                        {
                                            case "Overlay": if (trigger.IsSelected == FeaturesFromTestJob.Overlay) { countAux++; } break;
                                            case "Group": if (trigger.IsSelected == FeaturesFromTestJob.Group) { countAux++; } break;
                                            case "PC de Cliente": if (trigger.IsSelected == FeaturesFromTestJob.PC) { countAux++; } break;
                                            case "Brake Coil Voltage > 10": if (trigger.IsSelected == FeaturesFromTestJob.BrakeCoilVoltageMoreThan10) { countAux++; } break;
                                            case "EMBrake Module": if (trigger.IsSelected == FeaturesFromTestJob.EMBrake) { countAux++; } break;
                                            case "EMCO Board": if (trigger.IsSelected == FeaturesFromTestJob.EMCO) { countAux++; } break;
                                            case "R6 Regen Unit": if (trigger.IsSelected == FeaturesFromTestJob.R6) { countAux++; } break;
                                            case "Local": if (trigger.IsSelected == FeaturesFromTestJob.Local) { countAux++; } break;
                                            case "Short Floor": if (trigger.IsSelected == FeaturesFromTestJob.ShortFloor) { countAux++; } break;
                                            case "Custom": if (trigger.IsSelected == FeaturesFromTestJob.Custom) { countAux++; } break;
                                            case "MRL": if (trigger.IsSelected == FeaturesFromTestJob.MRL) { countAux++; } break;
                                            case "CTL2": if (trigger.IsSelected == FeaturesFromTestJob.CTL2) { countAux++; } break;
                                            case "Tarjeta CPI Incluida": if (trigger.IsSelected == FeaturesFromTestJob.TrajetaCPI) { countAux++; } break;
                                            case "Door Control en Cartop": if (trigger.IsSelected == FeaturesFromTestJob.Cartop) { countAux++; } break;
                                            case "Canada":
                                                if (trigger.IsSelected == true && StateFromCity.CountryID == 2) countAux++;
                                                else if (trigger.IsSelected == false && StateFromCity.CountryID != 2) countAux++;
                                                break;
                                            case "Ontario":
                                                if (trigger.IsSelected == true && FeaturesFromJob.CityID == 11) countAux++;
                                                else if (trigger.IsSelected == false && FeaturesFromJob.CityID != 11) countAux++;
                                                break;
                                            case "Manual Doors":
                                                if (trigger.IsSelected == true && FeaturesFromJob._jobExtension.DoorOperatorID == 2) countAux++;
                                                else if (trigger.IsSelected == false && FeaturesFromJob._jobExtension.DoorOperatorID != 2) countAux++;
                                                break;
                                            case "Duplex":
                                                if (trigger.IsSelected == true && FeaturesFromJob._jobExtension.JobTypeMain == "Duplex") countAux++;
                                                else if (trigger.IsSelected == false && FeaturesFromJob._jobExtension.JobTypeMain != "Duplex") countAux++;
                                                break;
                                            case "Serial Halls Calls": if (trigger.IsSelected == FeaturesFromJob._jobExtension.SHC) { countAux++; } break;
                                            case "Serial COP": if (trigger.IsSelected == FeaturesFromJob._jobExtension.SCOP) { countAux++; } break;
                                            case "Edge-LS":
                                                if (trigger.IsSelected == true && landing.Name == "LS-EDGE") countAux++;
                                                else if (trigger.IsSelected == false && landing.Name != "LS-EDGE") countAux++;
                                                break;
                                            case "Rail-LS":
                                                if (trigger.IsSelected == true && landing.Name == "LS-Rail") countAux++;
                                                else if (trigger.IsSelected == false && landing.Name != "LS-Rail") countAux++;
                                                break;
                                            case "mView":
                                                if (FeaturesFromJob._GenericFeatures.Monitoring != null)
                                                {
                                                    if (trigger.IsSelected == true && (FeaturesFromJob._GenericFeatures.Monitoring.Contains("MView"))) countAux++;
                                                    else if (trigger.IsSelected == false && !(FeaturesFromJob._GenericFeatures.Monitoring.Contains("MView"))) countAux++;
                                                    break;
                                                }
                                                break;
                                            case "iMonitor":
                                                if (FeaturesFromJob._GenericFeatures.Monitoring != null)
                                                {
                                                    if (trigger.IsSelected == true && (FeaturesFromJob._GenericFeatures.Monitoring.Contains("IMonitor"))) countAux++;
                                                    else if (trigger.IsSelected == false && !(FeaturesFromJob._GenericFeatures.Monitoring.Contains("IMonitor"))) countAux++;
                                                    break;
                                                }
                                                break;
                                            case "HAPS Battery":
                                                if (FeaturesFromJob._HydroSpecific.Battery == true)
                                                {
                                                    if (trigger.IsSelected == true && FeaturesFromJob._HydroSpecific.BatteryBrand == "HAPS") countAux++;
                                                    else if (trigger.IsSelected == false && FeaturesFromJob._HydroSpecific.BatteryBrand != "HAPS") countAux++;
                                                    break;
                                                }
                                                else break;
                                            case "2+ Starters":
                                                if (trigger.IsSelected == true && FeaturesFromJob._HydroSpecific.MotorsNum >= 2) countAux++;
                                                else if (trigger.IsSelected == false && FeaturesFromJob._HydroSpecific.MotorsNum < 2) countAux++;
                                                break;
                                            case "MOD Door Operator":
                                                if (trigger.IsSelected == true && (FeaturesFromJob._jobExtension.DoorOperatorID == 7 || FeaturesFromJob._jobExtension.DoorOperatorID == 8)) countAux++;
                                                else if (trigger.IsSelected == false && (FeaturesFromJob._jobExtension.DoorOperatorID != 7 || FeaturesFromJob._jobExtension.DoorOperatorID != 8)) countAux++;
                                                break;
                                            default: break;
                                        }
                                    }
                                    //Si se vuelve valido agrega el step a la lista de steps for job
                                    if (count == countAux)
                                    {
                                        StepsForJob stepForJob = new StepsForJob
                                        {
                                            StepID = step.StepID,
                                            TestJobID = FeaturesFromTestJob.TestJobID,
                                            Start = DateTime.Now,
                                            Stop = DateTime.Now,
                                            Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                                            Consecutivo = consecutivo,
                                            AuxStationID = testJobModify.StationID,
                                            AuxTechnicianID = testJobModify.TechnicianID
                                        };
                                        steps.Add(stepForJob);
                                        consecutivo++;
                                    }
                                }
                            }
                        }
                    }

                    break;
                case "ElmHydro":
                case "ElmTract":

                    Job FeaturesFromJob2 = jobRepo.Jobs.First(m => m.JobID == testJobView.TestJob.JobID);
                    Element element = jobRepo.Elements.FirstOrDefault(j => j.JobID == FeaturesFromJob2.JobID);
                    ElementHydro elementHydro = jobRepo.ElementHydros.FirstOrDefault(j => j.JobID == FeaturesFromJob2.JobID);
                    List<Step> Steps2 = testingRepo.Steps.OrderBy(m => m.Order).Where(m => m.JobTypeID == FeaturesFromJob2.JobTypeID).ToList();
                    //Checa si la lista de steps no esta vacia
                    if (Steps2.Count > 0)
                    {
                        //inicia el contador del consecutivo
                        int consecutivo = 1;

                        //Checa cada step de la lista
                        foreach (Step step in Steps2)
                        {
                            //Obtiene el primer trigger del step actual step
                            TriggeringFeature TriggerInStep = step._TriggeringFeatures.FirstOrDefault();

                            //si su name es nulo significa que es un step por default, debido a esto lo agrega a step for Job
                            if (TriggerInStep.Name == null)
                            {
                                StepsForJob stepForJob = new StepsForJob
                                {
                                    StepID = step.StepID,
                                    TestJobID = FeaturesFromTestJob.TestJobID,
                                    Start = DateTime.Now,
                                    Stop = DateTime.Now,
                                    Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                                    Consecutivo = consecutivo,
                                    AuxStationID = testJobModify.StationID,
                                    AuxTechnicianID = testJobModify.TechnicianID
                                };

                                steps.Add(stepForJob);
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
                                        LandingSystem landing = itemRepository.LandingSystems.FirstOrDefault(m => m.LandingSystemID == element.LandingSystemID);
                                        City UniqueCity = itemRepository.Cities.FirstOrDefault(m => m.CityID == FeaturesFromJob2.CityID);
                                        State StateFromCity = itemRepository.States.FirstOrDefault(m => m.StateID == UniqueCity.StateID);
                                        switch (trigger.Name)
                                        {
                                            case "Overlay": if (trigger.IsSelected == FeaturesFromTestJob.Overlay) { countAux++; } break;
                                            case "Group": if (trigger.IsSelected == FeaturesFromTestJob.Group) { countAux++; } break;
                                            case "PC de Cliente": if (trigger.IsSelected == FeaturesFromTestJob.PC) { countAux++; } break;
                                            case "Brake Coil Voltage > 10": if (trigger.IsSelected == FeaturesFromTestJob.BrakeCoilVoltageMoreThan10) { countAux++; } break;
                                            case "EMBrake Module": if (trigger.IsSelected == FeaturesFromTestJob.EMBrake) { countAux++; } break;
                                            case "EMCO Board": if (trigger.IsSelected == FeaturesFromTestJob.EMCO) { countAux++; } break;
                                            case "R6 Regen Unit": if (trigger.IsSelected == FeaturesFromTestJob.R6) { countAux++; } break;
                                            case "Local": if (trigger.IsSelected == FeaturesFromTestJob.Local) { countAux++; } break;
                                            case "Short Floor": if (trigger.IsSelected == FeaturesFromTestJob.ShortFloor) { countAux++; } break;
                                            case "Custom": if (trigger.IsSelected == FeaturesFromTestJob.Custom) { countAux++; } break;
                                            case "MRL": if (trigger.IsSelected == FeaturesFromTestJob.MRL) { countAux++; } break;
                                            case "CTL2": if (trigger.IsSelected == FeaturesFromTestJob.CTL2) { countAux++; } break;
                                            case "Tarjeta CPI Incluida": if (trigger.IsSelected == FeaturesFromTestJob.TrajetaCPI) { countAux++; } break;
                                            case "Door Control en Cartop": if (trigger.IsSelected == FeaturesFromTestJob.Cartop) { countAux++; } break;
                                            case "Canada":
                                                if (trigger.IsSelected == true && StateFromCity.CountryID == 2) countAux++;
                                                else if (trigger.IsSelected == false && StateFromCity.CountryID != 2) countAux++;
                                                break;
                                            case "Ontario":
                                                if (trigger.IsSelected == true && FeaturesFromJob2.CityID == 11) countAux++;
                                                else if (trigger.IsSelected == false && FeaturesFromJob2.CityID != 11) countAux++;
                                                break;
                                            case "Manual Doors":
                                                if (trigger.IsSelected == true && element.DoorOperatorID == 2) countAux++;
                                                else if (trigger.IsSelected == false && element.DoorOperatorID != 2) countAux++;
                                                break;
                                            case "Edge-LS":
                                                if (trigger.IsSelected == true && landing.Name == "LS-EDGE") countAux++;
                                                else if (trigger.IsSelected == false && landing.Name != "LS-EDGE") countAux++;
                                                break;
                                            case "HAPS Battery": if (trigger.IsSelected == element.HAPS) { countAux++; } break;
                                            case "MOD Door Operator":
                                                if (trigger.IsSelected == true && (element.DoorOperatorID == 7 || element.DoorOperatorID == 8)) countAux++;
                                                else if (trigger.IsSelected == false && (element.DoorOperatorID != 7 || element.DoorOperatorID != 8)) countAux++;
                                                break;
                                            default: break;
                                        }
                                    }
                                    //Si se vuelve valido agrega el step a la lista de steps for job
                                    if (count == countAux)
                                    {
                                        StepsForJob stepForJob = new StepsForJob
                                        {
                                            StepID = step.StepID,
                                            TestJobID = FeaturesFromTestJob.TestJobID,
                                            Start = DateTime.Now,
                                            Stop = DateTime.Now,
                                            Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                                            Consecutivo = consecutivo,
                                            AuxStationID = testJobModify.StationID,
                                            AuxTechnicianID = testJobModify.TechnicianID
                                        };
                                        steps.Add(stepForJob);
                                        consecutivo++;
                                    }
                                }
                            }
                        }
                    }

                    break;
            }

            return steps;
        }



        //==========================================================================

        public ViewResult StopsFromTestJob(int ID, int page = 1)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);
            TestJobViewModel testJobView = new TestJobViewModel
            {
                StopList = testingRepo.Stops
                 .Where(m => m.TestJobID == ID)
                 .OrderBy(p => p.StopID)
                 .Skip((page - 1) * PageSize)
                 .Take(PageSize).ToList(),
                Job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID),
                TestJob = testJob,
                Reasons1List = testingRepo.Reasons1.ToList(),
                Reasons2List = testingRepo.Reasons2.ToList(),
                Reasons3List = testingRepo.Reasons3.ToList(),
                Reasons4List = testingRepo.Reasons4.ToList(),
                Reasons5List = testingRepo.Reasons5.ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = testingRepo.Stops.Where(m => m.TestJobID == ID).Count()
                }
            };

            return View(testJobView);
        }

        public IActionResult Reassignment(TestJobViewModel testJobView)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestJob.TestJobID);

            if (testJob != null)
            {

                bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool isAdmin = GetCurrentUserRole("Admin").Result;
                bool isNotCompleted = testJob.Status != "Completed";

                if (isNotCompleted && (isAdmin || isTechAdmin))
                {
                    string StationName = testingRepo.Stations.FirstOrDefault(m => m.StationID == testJobView.NewStationID).Label;

                    if (testJob.TechnicianID == testJobView.NewTechnicianID && testJob.StationID == testJobView.NewStationID)
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"No puedes reasiganr el trabajo debido a que el Tecnico y la estacion son iguales";
                        return RedirectToAction("SearchTestJob");
                    }
                    else if (testJob.TechnicianID != testJobView.NewTechnicianID || testJob.StationID != testJobView.NewStationID)
                    {
                        List<Stop> CurrentStops = testingRepo.Stops.Where(p => testJob.TestJobID == p.TestJobID && p.Reason2 == 0 && p.Reason3 == 0).ToList();

                        if (CurrentStops.Count > 0)
                        {
                            bool wasOnShiftEnd = CheckShiftEnd(testJob.TestJobID);
                            CurrentStops = testingRepo.Stops.Where(p => testJob.TestJobID == p.TestJobID && p.Reason2 == 0 && p.Reason3 == 0).ToList();

                            foreach (Stop CurrentStop in CurrentStops)
                            {
                                if (CurrentStop.Reason1 != 980 && CurrentStop.Reason1 != 981)
                                {
                                    Stop CopyStop = new Stop();

                                    //Firts copy the stop
                                    CopyStop.Reason1 = CurrentStop.Reason1;
                                    CopyStop.Reason2 = CurrentStop.Reason2;
                                    CopyStop.Reason3 = CurrentStop.Reason3;
                                    CopyStop.Reason4 = CurrentStop.Reason4;
                                    CopyStop.Reason5ID = CurrentStop.Reason5ID;
                                    CopyStop.Critical = CurrentStop.Critical;
                                    CopyStop.Description = CurrentStop.Description;
                                    CopyStop.TestJobID = CurrentStop.TestJobID;

                                    CopyStop.StartDate = DateTime.Now;
                                    CopyStop.StopDate = DateTime.Now;
                                    CopyStop.Elapsed = new DateTime(1, 1, 1, 0, 0, 0);
                                    CopyStop.AuxStationID = testJobView.NewStationID;
                                    CopyStop.AuxTechnicianID = testJobView.NewTechnicianID;
                                    testingRepo.SaveStop(CopyStop);
                                }

                                //then close the older stop
                                if (!wasOnShiftEnd || (CurrentStop.Reason1 == 980 && CurrentStop.Reason2 == 0))
                                {
                                    TimeSpan auxTime = (DateTime.Now - CurrentStop.StartDate);
                                    CurrentStop.Elapsed += auxTime;
                                }
                                CurrentStop.StopDate = DateTime.Now;
                                CurrentStop.StartDate = DateTime.Now;
                                CurrentStop.Reason2 = 980;
                                CurrentStop.Reason3 = 980;
                                CurrentStop.Reason4 = 980;
                                CurrentStop.Reason5ID = 980;
                                CurrentStop.Description = "Job was reassigned";
                                testingRepo.SaveStop(CurrentStop);


                            }

                        }

                        testJob.TechnicianID = testJobView.NewTechnicianID;
                        testJob.StationID = testJobView.NewStationID;
                        testJob.Status = "Reassignment";
                        testingRepo.SaveTestJob(testJob);

                        Stop NewtStop = new Stop
                        {
                            TestJobID = testJob.TestJobID,
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
                            AuxStationID = testJobView.NewStationID,
                            AuxTechnicianID = testJobView.NewTechnicianID,
                        };
                        testingRepo.SaveStop(NewtStop);
                        TempData["message"] = $"You have reassinged the technician for the TestJob PO# {testJob.SinglePO} to T{testJobView.NewTechnicianID} and the station to {StationName}";
                        return RedirectToAction("SearchTestJob");
                    }
                    else
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"You know nothing John Snow";
                        return RedirectToAction("SearchTestJob");
                    }
                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    if (isNotCompleted == false) TempData["message"] = $"Error, El Testjob ya ha sido completado, intente de nuevo o contacte al Admin";
                    else TempData["message"] = $"Error, El Testjob a sido reasignado, intente de nuevo o contacte al Admin";

                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult ReturnFromComplete(int Id)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == Id);

            if (testJob != null)
            {
                bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool isAdmin = GetCurrentUserRole("Admin").Result;
                bool isCompleted = testJob.Status == "Completed";

                if (isCompleted && (isAdmin || isTechAdmin))
                {


                    TestJob OnGoingtestJob = testingRepo.TestJobs.FirstOrDefault(m => m.TechnicianID == testJob.TechnicianID && m.Status == "Working on it");
                    StepsForJob stepsForJob = testingRepo.StepsForJobs.OrderBy(s => s.Consecutivo).Where(p => p.TestJobID == testJob.TestJobID && p.Obsolete == false).Last();
                    stepsForJob.Complete = false;
                    testingRepo.SaveStepsForJob(stepsForJob);
                    if (OnGoingtestJob != null)
                    {
                        testJob.Status = "Stopped";
                        testingRepo.SaveTestJob(testJob);
                        Stop NewtStop = new Stop
                        {
                            TestJobID = testJob.TestJobID,
                            Reason1 = 982,
                            Reason2 = 982,
                            Reason3 = 982,
                            Reason4 = 982,
                            Reason5ID = 982,
                            Description = "The admin was returned the job to working on it",
                            Critical = true,
                            StartDate = DateTime.Now,
                            StopDate = DateTime.Now,
                            Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                            AuxStationID = testJob.StationID,
                            AuxTechnicianID = testJob.TechnicianID,
                        };
                        testingRepo.SaveStop(NewtStop);
                        TempData["message"] = $"You have returned the TestJob PO# {testJob.SinglePO} to stopped";
                        return RedirectToAction("SearchTestJob");
                    }
                    testJob.Status = "Working on it";
                    testingRepo.SaveTestJob(testJob);
                    TempData["message"] = $"You have returned the TestJob PO# {testJob.SinglePO} to Working on it";
                    return RedirectToAction("SearchTestJob");


                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Error, El Testjob se encuentra activo, intente de nuevo o contacte al Admin";

                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }
        }

        public void ShiftEnd(int TechnicianID)
        {
            List<TestJob> testJobList = testingRepo.TestJobs.Where(m => m.TechnicianID == TechnicianID && (m.Status == "Working on it" || m.Status == "Stopped" || m.Status == "Reassignment")).ToList();
            if (testJobList.Count > 0)
            {
                foreach (TestJob testjob in testJobList)
                {
                    List<Stop> stops = new List<Stop>();
                    stops = testingRepo.Stops.Where(p => testjob.TestJobID == p.TestJobID && p.Reason1 != 981 && p.Reason3 == 0 && p.Reason2 == 0).ToList();

                    if (stops.Count > 0)
                    {
                        foreach (Stop stop in stops)
                        {
                            TimeSpan auxTime = (DateTime.Now - stop.StartDate);
                            stop.Elapsed += auxTime;
                            stop.StartDate = DateTime.Now;
                            stop.StopDate = DateTime.Now;
                            testingRepo.SaveStop(stop);
                        }

                    }

                    Stop NewtStop = new Stop
                    {
                        TestJobID = testjob.TestJobID,
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
                        AuxStationID = testjob.StationID,
                        AuxTechnicianID = testjob.TechnicianID,
                    };
                    testingRepo.SaveStop(NewtStop);

                    testjob.Status = "Shift End";
                    testingRepo.SaveTestJob(testjob);
                }
            }

        }

        public void AutomaticShiftEnd()
        {
            List<TestJob> testJobs = testingRepo.TestJobs.Where(m => m.Status == "Working on it" || m.Status == "Stopped" || m.Status == "Reassignment").ToList();
            if (testJobs.Count > 0)
            {
                foreach (TestJob testjob in testJobs)
                {
                    List<Stop> stops = new List<Stop>();
                    stops = testingRepo.Stops.Where(p => testjob.TestJobID == p.TestJobID && p.Reason1 != 981 && p.Reason3 == 0 && p.Reason2 == 0).ToList();

                    if (stops.Count > 0)
                    {
                        foreach (Stop stop in stops)
                        {
                            TimeSpan auxTime = (DateTime.Now - stop.StartDate);
                            stop.Elapsed += auxTime;
                            stop.StartDate = DateTime.Now;
                            stop.StopDate = DateTime.Now;
                            testingRepo.SaveStop(stop);
                        }

                    }

                    Stop NewtStop = new Stop
                    {
                        TestJobID = testjob.TestJobID,
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
                        AuxStationID = testjob.StationID,
                        AuxTechnicianID = testjob.TechnicianID,
                    };
                    testingRepo.SaveStop(NewtStop);

                    testjob.Status = "Shift End";
                    testingRepo.SaveTestJob(testjob);
                }
            }

        }

        public void RestartShiftEnd(int TechnicianID)
        {
            List<TestJob> filteredList = new List<TestJob>();
            IQueryable<TestJob> testJobList = testingRepo.TestJobs.Where(m => m.TechnicianID == TechnicianID);
            IQueryable<Stop> stopsShiftEnd = testingRepo.Stops.Where(m => testJobList.Any(n => n.TestJobID == m.TestJobID) && m.Reason1 == 981 && m.Reason2 == 0 && m.Reason3 == 0);

            filteredList = testJobList.Where(m => stopsShiftEnd.Any(n => n.TestJobID == m.TestJobID)).ToList();

            if (filteredList.Count > 0)
            {
                foreach (TestJob testJob in filteredList)
                {
                    List<Stop> stops = new List<Stop>();

                    Stop ShiftEndStop = testingRepo.Stops.LastOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason1 == 981 && p.Reason2 == 0 && p.Reason3 == 0);

                    Stop ReassignmentStop = testingRepo.Stops.LastOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason1 == 980 && p.Reason2 == 0 && p.Reason3 == 0);

                    stops = testingRepo.Stops.Where(p => testJob.TestJobID == p.TestJobID && p.Reason1 != 980 && p.Reason1 != 981 && p.Reason2 == 0).ToList();

                    TimeSpan auxTime = (DateTime.Now - ShiftEndStop.StartDate);
                    ShiftEndStop.Elapsed += auxTime;
                    ShiftEndStop.StopDate = DateTime.Now;
                    ShiftEndStop.Reason2 = 981;
                    ShiftEndStop.Reason3 = 981;
                    ShiftEndStop.Reason4 = 981;
                    ShiftEndStop.Reason5ID = 981;
                    testingRepo.SaveStop(ShiftEndStop);

                    if (stops.Count > 0)
                    {
                        foreach (Stop stop in stops)
                        {
                            stop.StartDate = DateTime.Now;
                            stop.StopDate = DateTime.Now;
                            testingRepo.SaveStop(stop);
                        }

                    }

                    if (ReassignmentStop != null)
                    {
                        ReassignmentStop.StartDate = DateTime.Now;
                        ReassignmentStop.StopDate = DateTime.Now;
                        testingRepo.SaveStop(ReassignmentStop);
                        testJob.Status = "Reassignment";
                    }
                    else if (stops.Any(m => m.Critical == true))
                    {
                        testJob.Status = "Stopped";
                    }
                    else
                    {
                        testJob.Status = "Working on it";
                    }


                    testingRepo.SaveTestJob(testJob);
                }
            }

        }

        public bool CheckShiftEnd(int testJobID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobID);

            Stop ShiftEndStop = new Stop();
            try
            {
                ShiftEndStop = testingRepo.Stops.LastOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason1 == 981 && p.Reason2 == 0 && p.Reason3 == 0);

                if(ShiftEndStop != null)
                {
                    List<Stop> stops = new List<Stop>();

                    stops = testingRepo.Stops.Where(p => testJob.TestJobID == p.TestJobID && p.Reason1 != 980 && p.Reason1 != 981 && p.Reason2 == 0).ToList();

                    TimeSpan auxTime = (DateTime.Now - ShiftEndStop.StartDate);
                    ShiftEndStop.Elapsed += auxTime;
                    ShiftEndStop.StopDate = DateTime.Now;
                    ShiftEndStop.Reason2 = 981;
                    ShiftEndStop.Reason3 = 981;
                    ShiftEndStop.Reason4 = 981;
                    ShiftEndStop.Reason5ID = 981;
                    testingRepo.SaveStop(ShiftEndStop);

                    if (stops.Count > 0)
                    {
                        foreach (Stop stop in stops)
                        {
                            stop.StartDate = DateTime.Now;
                            stop.StopDate = DateTime.Now;
                            testingRepo.SaveStop(stop);
                        }

                    }

                    if (stops.Any(m => m.Critical == true))
                    {
                        testJob.Status = "Stopped";
                    }
                    else
                    {
                        testJob.Status = "Working on it";
                    }


                    testingRepo.SaveTestJob(testJob);
                    return true;
                }
                else
                {
                    return false;

                }
            }
            catch {
                return false;
            }


        }

        public IActionResult JobCompletion(int TestJobID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == TestJobID);

            if (testJob != null)
            {

                bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool isAdmin = GetCurrentUserRole("Admin").Result;
                bool isNotCompleted = testJob.Status != "Completed";

                if (isNotCompleted && (isAdmin || isTechAdmin))
                {

                    JobCompletionViewModel jobCompletion = new JobCompletionViewModel()
                    {
                        TestJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == TestJobID),
                        StepsForJobList = testingRepo.StepsForJobs.Where(m => m.TestJobID == TestJobID && m.Obsolete == false && m.Complete == false).OrderBy(m => m.Consecutivo).ToList(),
                    };
                    return View(jobCompletion);

                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    if (isNotCompleted == false) TempData["message"] = $"Error, El Testjob ya ha sido completado, intente de nuevo o contacte al Admin";
                    else TempData["message"] = $"Error, El Testjob a sido reasignado, intente de nuevo o contacte al Admin";

                    return RedirectToAction("Index", "Home");
                }

            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";
                return RedirectToAction("Index", "Home");
            }

        }

        [HttpPost]
        public IActionResult JobCompletion(JobCompletionViewModel jobCompletion)
        {

            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == jobCompletion.TestJob.TestJobID);
            List<StepsForJob> IncompleteStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJob.TestJobID && m.Obsolete == false && m.Complete == false).OrderBy(m => m.Consecutivo).ToList();
            List<Step> IncompleteStepsForJobInfo = testingRepo.Steps.Where(m => IncompleteStepsForJob.Any(s => s.StepID == m.StepID)).ToList();
            double ExpectecTimeSUM = 0;
            double ElapseHoursFromView = jobCompletion.ElapsedTimeHours + (jobCompletion.ElapsedTimeHours / 60);

            foreach (Step step in IncompleteStepsForJobInfo)
            {
                double StepExpectTime = ToHours(step.ExpectedTime);
                ExpectecTimeSUM += StepExpectTime;
            }

            foreach (StepsForJob step in IncompleteStepsForJob)
            {
                if (IncompleteStepsForJobInfo.First(m => m.StepID == step.StepID).ExpectedTime.Minute != 0)
                {
                    double ExpectedTimeForStep = ToHours(IncompleteStepsForJobInfo.First(m => m.StepID == step.StepID).ExpectedTime);
                    double TimePercentage = ExpectedTimeForStep / ExpectecTimeSUM;
                    if (TimePercentage == 0) TimePercentage = 1;
                    double TotalTime = ElapseHoursFromView * TimePercentage;

                    step.Elapsed = ToDateTime(TotalTime);
                    step.Complete = true;
                    testingRepo.SaveStepsForJob(step);
                }

            }

            testJob.CompletedDate = DateTime.Now;
            testJob.Status = "Completed";
            testingRepo.SaveTestJob(testJob);
            TempData["message"] = $"You have completed the TestJob PO# {testJob.SinglePO}";
            return RedirectToAction("SearchTestJob");

        }



        //===================================

        private async Task<AppUser> GetCurrentUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            return user;
        }

        private async Task<bool> GetCurrentUserRole(string role)
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            bool isInRole = await userManager.IsInRoleAsync(user, role);

            return isInRole;
        }

        public string JobTypeName(int ID)
        {
            return itemRepository.JobTypes.FirstOrDefault(m => m.JobTypeID == ID).Name;
        }

        public IActionResult SearchJobNum()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var names = jobRepo.Jobs.Where(p => p.JobNum.ToString().Contains(term)).Select(p => p.JobNum).Distinct().ToList();
                List<string> numbers = new List<string>();
                foreach (string number in names)
                {
                    numbers.Add(number.ToString());
                }
                return Ok(numbers);
            }
            catch
            {
                return BadRequest();
            }
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


        public async Task<IActionResult> TestJobSearchList(TestJobSearchViewModel searchViewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (searchViewModel.CleanFields) return RedirectToAction("TestJobSearchList");
            if (searchViewModel.Job == null) searchViewModel.Job = new Job();
            if (searchViewModel.TestJob == null) searchViewModel.TestJob = new TestJob();
            if (searchViewModel.Stop == null) searchViewModel.Stop = new Stop();
            if (searchViewModel.HoistWayData == null) searchViewModel.HoistWayData = new HoistWayData();
            if (searchViewModel.JobExtension == null) searchViewModel.JobExtension = new JobExtension();
            if (searchViewModel.TestJobsSearchList == null) searchViewModel.TestJobsSearchList = new List<TestJob>();
            if (searchViewModel.JobsSearchList == null) searchViewModel.JobsSearchList = new List<Job>();

            IQueryable<TestJob> testJobSearchList = testingRepo.TestJobs.Include(m => m._Stops).Include(m => m._TestFeature);
            IQueryable<Stop> stops = testingRepo.Stops.Where(m => testJobSearchList.Any(s => s.TestJobID == m.TestJobID));
            IQueryable<Job> jobSearchRepo = jobRepo.Jobs.Include(j => j._jobExtension).Include(hy => hy._HydroSpecific).Include(g => g._GenericFeatures)
                .Include(i => i._Indicator).Include(ho => ho._HoistWayData);
            IQueryable<State> statesInCanada = itemRepository.States.Where(m => m.CountryID == 2);
            IQueryable<City> citiesInCanada = itemRepository.Cities.Where(m => statesInCanada.Any(s => s.StateID == m.StateID));
            IQueryable<LandingSystem> landingSystemsEdge = itemRepository.LandingSystems.Where(m => m.Name == "LS-EDGE");
            IQueryable<LandingSystem> landingSystemsRails = itemRepository.LandingSystems.Where(m => m.Name == "LS-Rail");
            searchViewModel.TotalOnDB = testJobSearchList.Count();
            bool anyFeatureFromJob = false;


            #region TestJobInfo
            if (searchViewModel.TestJob.TechnicianID > 100 && searchViewModel.TestJob.TechnicianID < 299) testJobSearchList = testJobSearchList.Where(s => s.TechnicianID == searchViewModel.TestJob.TechnicianID);
            if (searchViewModel.TestJob.SinglePO > 3000000 && searchViewModel.TestJob.SinglePO < 4900000) testJobSearchList = testJobSearchList.Where(s => s.SinglePO == searchViewModel.TestJob.SinglePO);
            if (searchViewModel.TestJob.StationID > 0) testJobSearchList = testJobSearchList.Where(s => s.StationID == searchViewModel.TestJob.StationID);

            if (!string.IsNullOrEmpty(searchViewModel.TestJob.Status)) testJobSearchList = testJobSearchList.Where(s => s.Status.Contains(searchViewModel.TestJob.Status));
            if (!string.IsNullOrEmpty(searchViewModel.TestJob.JobLabel)) testJobSearchList = testJobSearchList.Where(s => s.JobLabel.Contains(searchViewModel.TestJob.JobLabel));
            #endregion


            #region TestFeaturesInfo
            if (!string.IsNullOrEmpty(searchViewModel.Overlay)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.Overlay == "Si" ? s._TestFeature.Overlay == true : s._TestFeature.Overlay == false);
            if (!string.IsNullOrEmpty(searchViewModel.Group)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.Group == "Si" ? s._TestFeature.Group == true : s._TestFeature.Group == false);
            if (!string.IsNullOrEmpty(searchViewModel.PC)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.PC == "Si" ? s._TestFeature.PC == true : s._TestFeature.PC == false);
            if (!string.IsNullOrEmpty(searchViewModel.BrakeCoilVoltageMoreThan10)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.BrakeCoilVoltageMoreThan10 == "Si" ? s._TestFeature.BrakeCoilVoltageMoreThan10 == true : s._TestFeature.BrakeCoilVoltageMoreThan10 == false);
            if (!string.IsNullOrEmpty(searchViewModel.EMBrake)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.EMBrake == "Si" ? s._TestFeature.EMBrake == true : s._TestFeature.EMBrake == false);
            if (!string.IsNullOrEmpty(searchViewModel.EMCO)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.EMCO == "Si" ? s._TestFeature.EMCO == true : s._TestFeature.EMCO == false);
            if (!string.IsNullOrEmpty(searchViewModel.R6)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.R6 == "Si" ? s._TestFeature.R6 == true : s._TestFeature.R6 == false);
            if (!string.IsNullOrEmpty(searchViewModel.Local)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.Local == "Si" ? s._TestFeature.Local == true : s._TestFeature.Local == false);
            if (!string.IsNullOrEmpty(searchViewModel.ShortFloor)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.ShortFloor == "Si" ? s._TestFeature.ShortFloor == true : s._TestFeature.ShortFloor == false);
            if (!string.IsNullOrEmpty(searchViewModel.Custom)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.Custom == "Si" ? s._TestFeature.Custom == true : s._TestFeature.Custom == false);
            if (!string.IsNullOrEmpty(searchViewModel.MRL)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.MRL == "Si" ? s._TestFeature.MRL == true : s._TestFeature.MRL == false);
            if (!string.IsNullOrEmpty(searchViewModel.CTL2)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.CTL2 == "Si" ? s._TestFeature.CTL2 == true : s._TestFeature.CTL2 == false);
            if (!string.IsNullOrEmpty(searchViewModel.TrajetaCPI)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.TrajetaCPI == "Si" ? s._TestFeature.TrajetaCPI == true : s._TestFeature.TrajetaCPI == false);
            if (!string.IsNullOrEmpty(searchViewModel.Cartop)) testJobSearchList = testJobSearchList.Where(s => searchViewModel.Cartop == "Si" ? s._TestFeature.Cartop == true : s._TestFeature.Cartop == false);
            #endregion

            #region JobFromTestJobInfo
            if (!string.IsNullOrEmpty(searchViewModel.JobNum))
            {
                jobSearchRepo = jobSearchRepo.Where(s => s.JobNum.Contains(searchViewModel.JobNum)); anyFeatureFromJob = true;
            }

            if (!string.IsNullOrEmpty(searchViewModel.JobName))
            {
                jobSearchRepo = jobSearchRepo.Where(s => s.Name.Contains(searchViewModel.JobName)); anyFeatureFromJob = true;
            }
            if (searchViewModel.HoistWayData.LandingSystemID > 0)
            {
                jobSearchRepo = jobSearchRepo.Where(s => s._HoistWayData.LandingSystemID == searchViewModel.HoistWayData.LandingSystemID); anyFeatureFromJob = true;
            }

            //special fields for JobInTestjob
            if (!string.IsNullOrEmpty(searchViewModel.Canada))
            {
                if (searchViewModel.Canada == "Si")
                {
                    jobSearchRepo = jobSearchRepo.Where(m => citiesInCanada.Any(s => s.CityID == m.CityID)); anyFeatureFromJob = true;
                }
                else
                {
                    jobSearchRepo = jobSearchRepo.Where(m => citiesInCanada.Any(s => s.CityID != m.CityID)); anyFeatureFromJob = true;
                }

            }
            if (!string.IsNullOrEmpty(searchViewModel.Ontario))
            {
                if (searchViewModel.Ontario == "Si")
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s.CityID == 11); anyFeatureFromJob = true;
                }
                else
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s.CityID != 11); anyFeatureFromJob = true;
                }

            }
            if (!string.IsNullOrEmpty(searchViewModel.MOD))
            {
                if (searchViewModel.MOD == "Si")
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.DoorOperatorID == 7 || s._jobExtension.DoorOperatorID == 8); anyFeatureFromJob = true;
                }
                else
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.DoorOperatorID != 7 && s._jobExtension.DoorOperatorID != 8); anyFeatureFromJob = true;
                }

            }
            if (!string.IsNullOrEmpty(searchViewModel.Manual))
            {
                if (searchViewModel.Manual == "Si")
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.DoorOperatorID == 2); anyFeatureFromJob = true;
                }
                else
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.DoorOperatorID != 2); anyFeatureFromJob = true;
                }
            }
            if (!string.IsNullOrEmpty(searchViewModel.Duplex))
            {
                if (searchViewModel.Duplex == "Si")
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.JobTypeMain == "Duplex"); anyFeatureFromJob = true;
                }
                else
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.JobTypeMain != "Duplex"); anyFeatureFromJob = true;
                }

            }
            if (!string.IsNullOrEmpty(searchViewModel.SHC))
            {
                if (searchViewModel.SHC == "Si")
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.SHC == true); anyFeatureFromJob = true;
                }
                else
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._jobExtension.SHC == false); anyFeatureFromJob = true;
                }

            }
            if (!string.IsNullOrEmpty(searchViewModel.EDGELS))
            {
                if (searchViewModel.EDGELS == "Si")
                {
                    jobSearchRepo = jobSearchRepo.Where(s => landingSystemsEdge.Any(m => m.LandingSystemID == s._HoistWayData.LandingSystemID)); anyFeatureFromJob = true;
                }
                else
                {
                    jobSearchRepo = jobSearchRepo.Where(s => landingSystemsEdge.Any(m => m.LandingSystemID != s._HoistWayData.LandingSystemID)); anyFeatureFromJob = true;
                }

            }
            if (!string.IsNullOrEmpty(searchViewModel.RailLS))
            {
                if (searchViewModel.RailLS == "Si")
                {
                    jobSearchRepo = jobSearchRepo.Where(s => landingSystemsRails.Any(m => m.LandingSystemID == s._HoistWayData.LandingSystemID)); anyFeatureFromJob = true;
                }
                else
                {
                    jobSearchRepo = jobSearchRepo.Where(s => landingSystemsRails.Any(m => m.LandingSystemID != s._HoistWayData.LandingSystemID)); anyFeatureFromJob = true;
                }

            }
            if (!string.IsNullOrEmpty(searchViewModel.MView))
            {
                if (searchViewModel.MView == "Si")
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.Monitoring.Contains("MView")); anyFeatureFromJob = true;
                }
                else
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.Monitoring != "MView"); anyFeatureFromJob = true;
                }

            }
            if (!string.IsNullOrEmpty(searchViewModel.IMonitor))
            {
                if (searchViewModel.IMonitor == "Si")
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.Monitoring.Contains("IMonitor")); anyFeatureFromJob = true;
                }
                else
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._GenericFeatures.Monitoring != "IMonitor"); anyFeatureFromJob = true;
                }

            }
            if (!string.IsNullOrEmpty(searchViewModel.HAPS))
            {
                if (searchViewModel.HAPS == "Si")
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.BatteryBrand == "HAPS"); anyFeatureFromJob = true;
                }
                else
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.BatteryBrand != "HAPS"); anyFeatureFromJob = true;
                }

            }
            if (!string.IsNullOrEmpty(searchViewModel.TwosStarters))
            {
                if (searchViewModel.HAPS == "Si")
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.MotorsNum >= 2); anyFeatureFromJob = true;
                }
                else
                {
                    jobSearchRepo = jobSearchRepo.Where(s => s._HydroSpecific.MotorsNum < 2); anyFeatureFromJob = true;
                }

            }


            #endregion

            if (anyFeatureFromJob) testJobSearchList = testJobSearchList.Where(m => jobSearchRepo.Any(s => s.JobID == m.JobID));


            searchViewModel.jobListAux = jobSearchRepo.Where(m => testJobSearchList.Any(s => s.JobID == m.JobID)).ToList();
            searchViewModel.TestJobsSearchList = testJobSearchList.OrderBy(p => p.TechnicianID).Skip((page - 1) * 10).Take(10).ToList();
            searchViewModel.JobTypeList = itemRepository.JobTypes.ToList();
            searchViewModel.StationsList = testingRepo.Stations.ToList();

            int TotalItemsSearch = testJobSearchList.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            searchViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 10,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = testJobSearchList.Count()
            };

            return View(searchViewModel);
        }

        public String getJobNumb(string firstDigits, int lastDigits)
        {
            string JobNumb = firstDigits + lastDigits.ToString();

            return JobNumb;
        }

        public JobNumber getJobNumbDivided(string JobNumber)
        {
            JobNumber jobNum = new JobNumber();

            jobNum.firstDigits = JobNumber.Remove(5, 5);
            jobNum.lastDigits = int.Parse(JobNumber.Remove(0, 5));

            return jobNum;
        }

        [AllowAnonymous]
        public ViewResult TestStats(TestJobViewModel viewModel, string JobType)
        {
            viewModel.TestStatsList = new List<TestStats>();
            List<TestJob> ActiveTestJobs = testingRepo.TestJobs.Where(m => m.Status != "Completed" && m.Status != "Deleted" && m.Status != "Incomplete").ToList();

            viewModel.StationsM2000List = testingRepo.Stations.Where(m => m.JobTypeID == 2).OrderBy(n => n.Label).ToList();
            viewModel.StationsM2000List.AddRange(testingRepo.Stations.Where(m => m.JobTypeID == 5).OrderBy(n => n.Label).ToList());

            viewModel.StationsM4000List = testingRepo.Stations.Where(m => m.JobTypeID == 4).OrderBy(n => n.Label).ToList();
            viewModel.StationsM4000List.AddRange(testingRepo.Stations.Where(m => m.JobTypeID == 1).OrderBy(n => n.Label).ToList());


            List<AppUser> Users = userManager.Users.Where(m => m.EngID >= 100 && m.EngID <= 299).ToList();
            IQueryable<Job> JobsinTest = jobRepo.Jobs.Where(m => ActiveTestJobs.Any(n => n.JobID == m.JobID));
            List<Station> StationFromTestJobs = testingRepo.Stations.Where(m => ActiveTestJobs.Any(n => n.StationID == m.StationID)).ToList();
            List<Step> StepsListInfo = testingRepo.Steps.ToList();

            List<StepsForJob> stepsForJobCompleted = new List<StepsForJob>();
            List<StepsForJob> stepsForJobNotCompleted = new List<StepsForJob>();
            int counter = 1;

            //Auxiliares


            foreach (TestJob testjob in ActiveTestJobs)
            {
                TestFeature FeaturesFromTestJob = testingRepo.TestFeatures.First(m => m.TestJobID == testjob.TestJobID);
                Job FeaturesFromJob = JobsinTest.Include(m => m._jobExtension).Include(m => m._HydroSpecific).Include(m => m._HoistWayData).Include(m => m._GenericFeatures).FirstOrDefault(m => m.JobID == testjob.JobID);
                City UniqueCity = itemRepository.Cities.FirstOrDefault(m => m.CityID == FeaturesFromJob.CityID);
                State StateFromCity = itemRepository.States.FirstOrDefault(m => m.StateID == UniqueCity.StateID);
                List<StepsForJob> AllSteps = testingRepo.StepsForJobs.Where(m => m.TestJobID == testjob.TestJobID && m.Obsolete == false).OrderBy(n => n.Consecutivo).ToList();

                string JobNum = FeaturesFromJob.JobNum.Remove(0, 5);
                string TechName = Users.FirstOrDefault(m => m.EngID == testjob.TechnicianID).FullName;
                string StationName = StationFromTestJobs.FirstOrDefault(m => m.StationID == testjob.StationID).Label;
                string Stage = "";
                string Status = "";
                string EfficiencyColor = "green";
                string StatusColor = "green";
                string Category = "";
                double Efficiency = 0;
                double ExpectedTimeSUM = 0;
                double RealTimeSUM = 0;
                double TTCAux = 0;
                DateTime TTC = new DateTime();


                //Logic for TTC
                stepsForJobNotCompleted = AllSteps.Where(m => m.TestJobID == testjob.TestJobID && m.Complete == false && m.Obsolete == false).OrderBy(n => n.Consecutivo).ToList();
                StepsForJob LastStepsForJob = stepsForJobNotCompleted.FirstOrDefault(m => m.Complete == false);
                Step LastStepInfo = StepsListInfo.FirstOrDefault(m => m.StepID == LastStepsForJob.StepID);

                foreach (StepsForJob step in stepsForJobNotCompleted)
                {
                    TTCAux += ToHours(StepsListInfo.FirstOrDefault(m => m.StepID == step.StepID).ExpectedTime);
                }

                TTC = ToDateTime(TTCAux);

                stepsForJobCompleted = AllSteps.Where(m => m.TestJobID == testjob.TestJobID && m.Complete == true).OrderBy(n => n.Consecutivo).ToList();

                //a simple query to get the stage
                Stage = LastStepInfo.Stage;

                //if to get efficiency or status(stopped)
                if (testjob.Status == "Working on it")
                {
                    foreach (StepsForJob step in stepsForJobCompleted)
                    {
                        ExpectedTimeSUM += ToHours(StepsListInfo.FirstOrDefault(m => m.StepID == step.StepID).ExpectedTime);

                        RealTimeSUM += ToHours(step.Elapsed);
                    }
                    ExpectedTimeSUM += ToHours(LastStepInfo.ExpectedTime);
                    RealTimeSUM += ToHours(LastStepsForJob.Elapsed);

                    Efficiency = Math.Round((ExpectedTimeSUM / RealTimeSUM) * 100);

                    if (Efficiency > 99) Efficiency = 99;
                    if (Efficiency < 82) EfficiencyColor = "Orange";
                    else if (Efficiency < 69) EfficiencyColor = "#ffc107!important";

                }
                else
                {
                    Efficiency = 99;

                    Stop stop = testingRepo.Stops.Where(m => m.TestJobID == testjob.TestJobID).Last();
                    Reason1 reason = testingRepo.Reasons1.FirstOrDefault(m => m.Reason1ID == stop.Reason1);
                    Status = "Stopped: " + reason.Description;

                    if (stop.Critical) StatusColor = "Red";
                    if (stop.Reason1 == 980 || stop.Reason1 == 981 || stop.Reason1 == 982) StatusColor = "Gray";


                }

                //logic to get the cat(difficulty)
                if (FeaturesFromJob.JobTypeID == 2)
                {
                    if (FeaturesFromJob.CityID == 11) Category = "6";
                    else if (FeaturesFromTestJob.Custom || FeaturesFromTestJob.MRL) Category = "5";
                    else if (FeaturesFromJob._jobExtension.DoorOperatorID == 2 || FeaturesFromTestJob.Overlay) Category = "4";
                    else if (FeaturesFromJob._GenericFeatures.Monitoring.Contains("MView") || FeaturesFromJob._GenericFeatures.Monitoring.Contains("IMonitor") || FeaturesFromTestJob.Local) Category = "3";
                    else if (FeaturesFromJob._HoistWayData.AnyRear || FeaturesFromJob._jobExtension.JobTypeMain == "Duplex" || (FeaturesFromJob._jobExtension.DoorOperatorID == 7 || FeaturesFromJob._jobExtension.DoorOperatorID == 8)
                        || FeaturesFromJob._HydroSpecific.MotorsNum >= 2 || FeaturesFromJob._jobExtension.SHC || FeaturesFromTestJob.EMCO || FeaturesFromTestJob.R6) Category = "2";
                    else Category = "1";

                }
                else if (FeaturesFromJob.JobTypeID == 4)
                {
                    if (FeaturesFromJob.CityID == 11) Category = "6";
                    else if (FeaturesFromTestJob.Custom) Category = "5";
                    else if (FeaturesFromJob._jobExtension.DoorOperatorID == 2 || FeaturesFromTestJob.Overlay) Category = "4";
                    else if (FeaturesFromJob._GenericFeatures.Monitoring.Contains("MView") || FeaturesFromJob._GenericFeatures.Monitoring.Contains("IMonitor") || FeaturesFromTestJob.Local || FeaturesFromTestJob.ShortFloor) Category = "3";
                    else if (FeaturesFromJob._HoistWayData.AnyRear || FeaturesFromJob._jobExtension.JobTypeMain == "Duplex" || (FeaturesFromJob._jobExtension.DoorOperatorID == 7 || FeaturesFromJob._jobExtension.DoorOperatorID == 8)
                        || FeaturesFromJob._HydroSpecific.MotorsNum >= 2) Category = "2";
                    else Category = "1";
                }
                else Category = "Indefinida";

                //JobProgress
                double JobProgress = (stepsForJobCompleted.Count() * 100) / AllSteps.Count();

                //Stage Progress
                List<Step> stepsPerStage = StepsListInfo.Where(m => m.Stage == Stage && AllSteps.Any(n => n.StepID == m.StepID)).ToList();
                int stepsPerJobCompleted = AllSteps.Where(m => stepsPerStage.Any(s => s.StepID == m.StepID)).Where(m => m.Complete == true).Count();

                double StagePogress = (stepsPerJobCompleted * 100) / stepsPerStage.Count();

                if (stepsPerStage.Count == 1 && StagePogress == 0) StagePogress = 50;

                TestStats testStats = new TestStats()
                {
                    JobNumer = JobNum,
                    StationID = testjob.StationID,
                    TechName = TechName,
                    Stage = Stage,
                    Efficiency = Efficiency,
                    StatusColor = StatusColor,
                    Status = Status,
                    Category = Category,
                    Station = StationName,
                    TTC = TTC,
                    JobProgress = JobProgress,
                    StageProgress = StagePogress,
                    EfficiencyColor = EfficiencyColor,

                };

                ViewBag.Jobtype = JobType;
                ViewData["TV"] = "Simontl";
                viewModel.TestStatsList.Add(testStats);
            }

            return View(viewModel);
        }

        [AllowAnonymous]
        public ViewResult TestStatsAux(TestJobViewModel viewModel, string JobType)
        {
            viewModel.TestStatsList = new List<TestStats>();
            List<TestJob> ActiveTestJobs = testingRepo.TestJobs.Where(m => m.Status != "Completed" && m.Status != "Deleted" && m.Status != "Incomplete").ToList();
            IQueryable<Station> stations = testingRepo.Stations.Where(m => m.StationID != 0);
            int JobTypeID = (JobType == "M2000") ? 2 : 4;
            if (JobTypeID == 2)
            {
                viewModel.StationsList = stations.Where(m => m.JobTypeID == JobTypeID).OrderBy(n => n.Label).ToList();
                viewModel.StationsList.AddRange(stations.Where(m => m.JobTypeID == 5).OrderBy(n => n.Label).ToList());
            }
            else
            {
                viewModel.StationsList = stations.Where(m => m.JobTypeID == JobTypeID).OrderBy(n => n.Label).ToList();
                viewModel.StationsList.AddRange(stations.Where(m => m.JobTypeID == 1).OrderBy(n => n.Label).ToList());
            }

            List<AppUser> Users = userManager.Users.Where(m => m.EngID >= 100 && m.EngID <= 299).ToList();
            IQueryable<Job> JobsinTest = jobRepo.Jobs.Where(m => ActiveTestJobs.Any(n => n.JobID == m.JobID));
            List<Station> StationFromTestJobs = testingRepo.Stations.Where(m => ActiveTestJobs.Any(n => n.StationID == m.StationID)).ToList();
            List<Step> StepsListInfo = testingRepo.Steps.ToList();

            List<StepsForJob> stepsForJobCompleted = new List<StepsForJob>();
            List<StepsForJob> stepsForJobNotCompleted = new List<StepsForJob>();
            int counter = 1;

            //Auxiliares


            foreach (TestJob testjob in ActiveTestJobs)
            {
                TestFeature FeaturesFromTestJob = testingRepo.TestFeatures.First(m => m.TestJobID == testjob.TestJobID);
                Job FeaturesFromJob = JobsinTest.Include(m => m._jobExtension).Include(m => m._HydroSpecific).Include(m => m._HoistWayData).Include(m => m._GenericFeatures).FirstOrDefault(m => m.JobID == testjob.JobID);
                City UniqueCity = itemRepository.Cities.FirstOrDefault(m => m.CityID == FeaturesFromJob.CityID);
                State StateFromCity = itemRepository.States.FirstOrDefault(m => m.StateID == UniqueCity.StateID);
                List<StepsForJob> AllSteps = testingRepo.StepsForJobs.Where(m => m.TestJobID == testjob.TestJobID && m.Obsolete == false).OrderBy(n => n.Consecutivo).ToList();

                string JobNum = FeaturesFromJob.JobNum.Remove(0, 5);
                string TechName = Users.FirstOrDefault(m => m.EngID == testjob.TechnicianID).FullName;
                string StationName = StationFromTestJobs.FirstOrDefault(m => m.StationID == testjob.StationID).Label;
                string Stage = "";
                string Status = "";
                string EfficiencyColor = "green";
                string StatusColor = "green";
                string Category = "";
                double Efficiency = 0;
                double ExpectedTimeSUM = 0;
                double RealTimeSUM = 0;
                double TTCAux = 0;
                DateTime TTC = new DateTime();


                //Logic for TTC
                stepsForJobNotCompleted = AllSteps.Where(m => m.TestJobID == testjob.TestJobID && m.Complete == false && m.Obsolete == false).OrderBy(n => n.Consecutivo).ToList();
                StepsForJob LastStepsForJob = stepsForJobNotCompleted.FirstOrDefault(m => m.Complete == false);
                Step LastStepInfo = StepsListInfo.FirstOrDefault(m => m.StepID == LastStepsForJob.StepID);

                foreach (StepsForJob step in stepsForJobNotCompleted)
                {
                    TTCAux += ToHours(StepsListInfo.FirstOrDefault(m => m.StepID == step.StepID).ExpectedTime);
                }

                TTC = ToDateTime(TTCAux);

                stepsForJobCompleted = AllSteps.Where(m => m.TestJobID == testjob.TestJobID && m.Complete == true).OrderBy(n => n.Consecutivo).ToList();

                //a simple query to get the stage
                Stage = LastStepInfo.Stage;

                //if to get efficiency or status(stopped)
                if (testjob.Status == "Working on it")
                {
                    foreach (StepsForJob step in stepsForJobCompleted)
                    {
                        ExpectedTimeSUM += ToHours(StepsListInfo.FirstOrDefault(m => m.StepID == step.StepID).ExpectedTime);

                        RealTimeSUM += ToHours(step.Elapsed);
                    }
                    ExpectedTimeSUM += ToHours(LastStepInfo.ExpectedTime);
                    RealTimeSUM += ToHours(LastStepsForJob.Elapsed);

                    Efficiency = Math.Round((ExpectedTimeSUM / RealTimeSUM) * 100);

                    if (Efficiency > 99) Efficiency = 99;
                    if (Efficiency < 82) EfficiencyColor = "Orange";
                    else if (Efficiency < 69) EfficiencyColor = "#ffc107!important";

                }
                else
                {
                    Efficiency = 99;

                    Stop stop = testingRepo.Stops.Where(m => m.TestJobID == testjob.TestJobID).Last();
                    Reason1 reason = testingRepo.Reasons1.FirstOrDefault(m => m.Reason1ID == stop.Reason1);
                    Status = "Stopped: " + reason.Description;

                    if (stop.Critical) StatusColor = "Red";
                    if (stop.Reason1 == 980 || stop.Reason1 == 981 || stop.Reason1 == 982) StatusColor = "Gray";


                }

                //logic to get the cat(difficulty)
                if (FeaturesFromJob.JobTypeID == 2)
                {
                    if (FeaturesFromJob.CityID == 11) Category = "6";
                    else if (FeaturesFromTestJob.Custom || FeaturesFromTestJob.MRL) Category = "5";
                    else if (FeaturesFromJob._jobExtension.DoorOperatorID == 2 || FeaturesFromTestJob.Overlay) Category = "4";
                    else if (FeaturesFromJob._GenericFeatures.Monitoring.Contains("MView") || FeaturesFromJob._GenericFeatures.Monitoring.Contains("IMonitor") || FeaturesFromTestJob.Local) Category = "3";
                    else if (FeaturesFromJob._HoistWayData.AnyRear || FeaturesFromJob._jobExtension.JobTypeMain == "Duplex" || (FeaturesFromJob._jobExtension.DoorOperatorID == 7 || FeaturesFromJob._jobExtension.DoorOperatorID == 8)
                        || FeaturesFromJob._HydroSpecific.MotorsNum >= 2 || FeaturesFromJob._jobExtension.SHC || FeaturesFromTestJob.EMCO || FeaturesFromTestJob.R6) Category = "2";
                    else Category = "1";

                }
                else if (FeaturesFromJob.JobTypeID == 4)
                {
                    if (FeaturesFromJob.CityID == 11) Category = "6";
                    else if (FeaturesFromTestJob.Custom) Category = "5";
                    else if (FeaturesFromJob._jobExtension.DoorOperatorID == 2 || FeaturesFromTestJob.Overlay) Category = "4";
                    else if (FeaturesFromJob._GenericFeatures.Monitoring.Contains("MView") || FeaturesFromJob._GenericFeatures.Monitoring.Contains("IMonitor") || FeaturesFromTestJob.Local || FeaturesFromTestJob.ShortFloor) Category = "3";
                    else if (FeaturesFromJob._HoistWayData.AnyRear || FeaturesFromJob._jobExtension.JobTypeMain == "Duplex" || (FeaturesFromJob._jobExtension.DoorOperatorID == 7 || FeaturesFromJob._jobExtension.DoorOperatorID == 8)
                        || FeaturesFromJob._HydroSpecific.MotorsNum >= 2) Category = "2";
                    else Category = "1";
                }
                else Category = "Indefinida";

                //JobProgress
                double JobProgress = (stepsForJobCompleted.Count() * 100) / AllSteps.Count();

                //Stage Progress
                List<Step> stepsPerStage = StepsListInfo.Where(m => m.Stage == Stage && AllSteps.Any(n => n.StepID == m.StepID)).ToList();
                int stepsPerJobCompleted = AllSteps.Where(m => stepsPerStage.Any(s => s.StepID == m.StepID)).Where(m => m.Complete == true).Count();

                double StagePogress = (stepsPerJobCompleted * 100) / stepsPerStage.Count();

                if (stepsPerStage.Count == 1 && StagePogress == 0) StagePogress = 50;

                TestStats testStats = new TestStats()
                {
                    JobNumer = JobNum,
                    StationID = testjob.StationID,
                    TechName = TechName,
                    Stage = Stage,
                    Efficiency = Efficiency,
                    StatusColor = StatusColor,
                    Status = Status,
                    Category = Category,
                    Station = StationName,
                    TTC = TTC,
                    JobProgress = JobProgress,
                    StageProgress = StagePogress,
                    EfficiencyColor = EfficiencyColor,

                };

                ViewBag.Jobtype = JobType;
                ViewData["TV"] = "Simontl";
                viewModel.TestStatsList.Add(testStats);
            }

            return View(viewModel);
        }

        [HttpPost]
        public FileStreamResult ExportToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;
            IQueryable<TestJob> testjobsCompleted = testingRepo.TestJobs.Where(m => m.Status == "Completed");

            using (XmlWriter xw = XmlWriter.Create(ms, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("TestJobs-TestFeatures-StepsForJob");

                foreach (TestJob testjob in testjobsCompleted)
                {
                    TestFeature testFeature = testingRepo.TestFeatures
                        .FirstOrDefault(m => m.TestJobID == testjob.TestJobID);

                    IQueryable<StepsForJob> stepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testjob.TestJobID)
                                                                         .Where(m => m.Complete == true)
                                                                         .OrderBy(n => n.Consecutivo);
                    xw.WriteStartElement("TestJob");

                    xw.WriteElementString("TestJobID", testjob.TestJobID.ToString());
                    xw.WriteElementString("JobID", testjob.JobID.ToString());
                    xw.WriteElementString("TechnicianID", testjob.TechnicianID.ToString());
                    xw.WriteElementString("Status", testjob.Status);
                    xw.WriteElementString("SinglePO", testjob.SinglePO.ToString());
                    xw.WriteElementString("JobLabel", testjob.JobLabel);
                    xw.WriteElementString("StartDate", testjob.StartDate.ToString());
                    xw.WriteElementString("CompletedDate", testjob.CompletedDate.ToString());
                    xw.WriteElementString("StationID", testjob.StationID.ToString());



                    xw.WriteStartElement("TestFeature");

                    xw.WriteElementString("TestFeatureID", testFeature.TestFeatureID.ToString());
                    xw.WriteElementString("TestJobID", testFeature.TestJobID.ToString());
                    xw.WriteElementString("Overlay", testFeature.Overlay.ToString());
                    xw.WriteElementString("Group", testFeature.Group.ToString());
                    xw.WriteElementString("PC", testFeature.PC.ToString());
                    xw.WriteElementString("BrakeCoilVoltageMoreThan10", testFeature.BrakeCoilVoltageMoreThan10.ToString());
                    xw.WriteElementString("EMBrake", testFeature.EMBrake.ToString());
                    xw.WriteElementString("EMCO", testFeature.EMCO.ToString());
                    xw.WriteElementString("R6", testFeature.R6.ToString());
                    xw.WriteElementString("Local", testFeature.Local.ToString());
                    xw.WriteElementString("ShortFloor", testFeature.ShortFloor.ToString());
                    xw.WriteElementString("Custom", testFeature.Custom.ToString());
                    xw.WriteElementString("MRL", testFeature.MRL.ToString());
                    xw.WriteElementString("CTL2", testFeature.CTL2.ToString());
                    xw.WriteElementString("TrajetaCPI", testFeature.TrajetaCPI.ToString());
                    xw.WriteElementString("Cartop", testFeature.Cartop.ToString());

                    xw.WriteEndElement();

                    xw.WriteStartElement("StepsForJob");

                    foreach (StepsForJob step in stepsForJob)
                    {
                        xw.WriteStartElement("StepForJob");

                        xw.WriteElementString("StepsForJobID", step.StepsForJobID.ToString());
                        xw.WriteElementString("StepID", step.StepID.ToString());
                        xw.WriteElementString("TestJobID", step.TestJobID.ToString());
                        xw.WriteElementString("Start", step.Start.ToString());
                        xw.WriteElementString("Stop", step.Stop.ToString());
                        xw.WriteElementString("Elapsed", step.Elapsed.ToString());
                        xw.WriteElementString("Complete", step.Complete.ToString());
                        xw.WriteElementString("Consecutivo", step.Consecutivo.ToString());
                        xw.WriteElementString("Obsolete", step.Obsolete.ToString());
                        xw.WriteElementString("AuxTechnicianID", step.AuxTechnicianID.ToString());
                        xw.WriteElementString("AuxStationID", step.AuxStationID.ToString());

                        xw.WriteEndElement();
                    }

                    xw.WriteEndElement();


                    xw.WriteEndElement();
                }
                xw.WriteEndElement();

                xw.WriteEndDocument();
            }

            ms.Position = 0;
            return File(ms, "text/xml", "TestJobs-TestFeatures-StepsForJob.xml");
        }

        public static void ImportXML(IServiceProvider services)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\ProdFloorNew90\wwwroot\AppData\TestJobs-TestFeatures-StepsForJob.xml");


            var XMLMain = doc.DocumentElement.SelectSingleNode("//TestJobs-TestFeatures-StepsForJob");

            var XMLTestJobs = XMLMain.SelectNodes("//TestJob");

            if (XMLTestJobs != null && context.Steps.Any() && context.Jobs.Any()
                && context.Stations.Any())
            {
                foreach (XmlElement testjob in XMLTestJobs)
                {
                    //saving TestJob
                    var TestJobID = testjob.SelectSingleNode(".//TestJobID").InnerText;
                    var JobID = testjob.SelectSingleNode(".//JobID").InnerText;
                    var TechnicianID = testjob.SelectSingleNode(".//TechnicianID").InnerText;
                    var Status = testjob.SelectSingleNode(".//Status").InnerText;
                    var SinglePO = testjob.SelectSingleNode(".//SinglePO").InnerText;
                    var JobLabel = testjob.SelectSingleNode(".//JobLabel").InnerText;
                    var StartDate = testjob.SelectSingleNode(".//StartDate").InnerText;
                    var CompletedDate = testjob.SelectSingleNode(".//CompletedDate").InnerText;
                    var StationID = testjob.SelectSingleNode(".//StationID").InnerText;

                    context.TestJobs.Add(new TestJob
                    {
                        TestJobID = Int32.Parse(TestJobID),
                        JobID = Int32.Parse(JobID),
                        TechnicianID = Int32.Parse(TechnicianID),
                        Status = Status,
                        SinglePO = Int32.Parse(SinglePO),
                        JobLabel = JobLabel,
                        StartDate = DateTime.Parse(StartDate),
                        CompletedDate = DateTime.Parse(CompletedDate),
                        StationID = Int32.Parse(StationID),

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.TestJobs ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.TestJobs OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }

                    //Saving TestFeature
                    var XMLTestFeatures = testjob.SelectSingleNode(".//TestFeature");

                    var TestFeatureID = XMLTestFeatures.SelectSingleNode(".//TestFeatureID").InnerText;
                    var Overlay = XMLTestFeatures.SelectSingleNode(".//Overlay").InnerText;
                    var Group = XMLTestFeatures.SelectSingleNode(".//Group").InnerText;
                    var PC = XMLTestFeatures.SelectSingleNode(".//PC").InnerText;
                    var BrakeCoilVoltageMoreThan10 = XMLTestFeatures.SelectSingleNode(".//BrakeCoilVoltageMoreThan10").InnerText;
                    var EMBrake = XMLTestFeatures.SelectSingleNode(".//EMBrake").InnerText;
                    var EMCO = XMLTestFeatures.SelectSingleNode(".//EMCO").InnerText;
                    var R6 = XMLTestFeatures.SelectSingleNode(".//R6").InnerText;
                    var Local = XMLTestFeatures.SelectSingleNode(".//Local").InnerText;
                    var ShortFloor = XMLTestFeatures.SelectSingleNode(".//ShortFloor").InnerText;
                    var Custom = XMLTestFeatures.SelectSingleNode(".//Custom").InnerText;
                    var MRL = XMLTestFeatures.SelectSingleNode(".//MRL").InnerText;
                    var CTL2 = XMLTestFeatures.SelectSingleNode(".//CTL2").InnerText;
                    var TrajetaCPI = XMLTestFeatures.SelectSingleNode(".//TrajetaCPI").InnerText;
                    var Cartop = XMLTestFeatures.SelectSingleNode(".//Cartop").InnerText;

                    context.TestFeatures.Add(new TestFeature
                    {
                        TestFeatureID = Int32.Parse(TestFeatureID),
                        TestJobID = Int32.Parse(TestJobID),
                        Overlay = Boolean.Parse(Overlay),
                        Group = Boolean.Parse(Group),
                        PC = Boolean.Parse(PC),
                        BrakeCoilVoltageMoreThan10 = Boolean.Parse(BrakeCoilVoltageMoreThan10),
                        EMBrake = Boolean.Parse(EMBrake),
                        EMCO = Boolean.Parse(EMCO),
                        R6 = Boolean.Parse(R6),
                        Local = Boolean.Parse(Local),
                        ShortFloor = Boolean.Parse(ShortFloor),
                        Custom = Boolean.Parse(Custom),
                        MRL = Boolean.Parse(MRL),
                        CTL2 = Boolean.Parse(CTL2),
                        TrajetaCPI = Boolean.Parse(TrajetaCPI),
                        Cartop = Boolean.Parse(Cartop),

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.TestFeatures ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.TestFeatures OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }

                    ///////////Savin all StepsForJob

                    var XMLStepsForJob = testjob.SelectSingleNode(".//StepsForJob");

                    var XMLStepForJob = XMLStepsForJob.SelectNodes(".//StepForJob");

                    foreach (XmlElement stepForJob in XMLStepForJob)
                    {

                        var StepsForJobID = stepForJob.SelectSingleNode(".//StepsForJobID").InnerText;
                        var StepID = stepForJob.SelectSingleNode(".//StepID").InnerText;
                        var Start = stepForJob.SelectSingleNode(".//Start").InnerText;
                        var Stop = stepForJob.SelectSingleNode(".//Stop").InnerText;
                        var Elapsed = stepForJob.SelectSingleNode(".//Elapsed").InnerText;
                        var Complete = stepForJob.SelectSingleNode(".//Complete").InnerText;
                        var Consecutivo = stepForJob.SelectSingleNode(".//Consecutivo").InnerText;
                        var Obsolete = stepForJob.SelectSingleNode(".//Obsolete").InnerText;
                        var AuxTechnicianID = stepForJob.SelectSingleNode(".//AuxTechnicianID").InnerText;
                        var AuxStationID = stepForJob.SelectSingleNode(".//AuxStationID").InnerText;

                        context.StepsForJobs.Add(new StepsForJob
                        {
                            StepsForJobID = Int32.Parse(StepsForJobID),
                            StepID = Int32.Parse(StepID),
                            TestJobID = Int32.Parse(TestJobID),
                            Start = DateTime.Parse(Start),
                            Stop = DateTime.Parse(Stop),
                            Elapsed = DateTime.Parse(Elapsed),
                            Complete = Boolean.Parse(Complete),
                            Consecutivo = Int32.Parse(Consecutivo),
                            Obsolete = Boolean.Parse(Obsolete),
                            AuxTechnicianID = Int32.Parse(AuxTechnicianID),
                            AuxStationID = Int32.Parse(AuxStationID),

                        });
                        context.Database.OpenConnection();
                        try
                        {
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.StepsForJobs ON");
                            context.SaveChanges();
                            context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.StepsForJobs OFF");
                        }
                        finally
                        {
                            context.Database.CloseConnection();
                        }

                    }

                }

            }

        }

        [HttpPost]
        public IActionResult SeedXML()
        {
            ImportXML(HttpContext.RequestServices);
            return RedirectToAction(nameof(List));
        }

    }
}