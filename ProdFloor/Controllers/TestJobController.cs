using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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


        public IActionResult SearchTestJob(string Clean, int jobNumber, string jobnumb = "0", int MyJobsPage = 1, int PendingToCrossJobPage = 1, int OnCrossJobPage = 1)
        {
            if (jobnumb != "0") jobNumber = Int32.Parse(jobnumb);
            if (jobNumber != 0) jobnumb = jobNumber.ToString();

            List<TestJob> testJobsInCompleted = new List<TestJob>();
            List<TestJob> testJobsCompleted = new List<TestJob>();
            List<TestJob> testJobsList = new List<TestJob>();
            List<Job> jobList = jobRepo.Jobs.Where(m => m.JobNum == jobNumber).ToList();


            foreach (Job job in jobList)
            {
                TestJob TestjobAux = testingRepo.TestJobs.FirstOrDefault(m => m.JobID == job.JobID);
                if (TestjobAux != null) testJobsList.Add(TestjobAux);
            }

            if (testJobsList != null && testJobsList.Count > 0)
            {
                testJobsInCompleted = testJobsList
                 .Where(m => m.Status != "Completed").ToList();

                testJobsCompleted = testJobsList
                    .Where(m => m.Status == "Completed").ToList();
            }

            if (testJobsInCompleted == null || jobnumb == "0") testJobsInCompleted = testingRepo.TestJobs.Where(m => m.Status != "Completed").ToList();

            if (testJobsCompleted == null || jobnumb == "0") testJobsCompleted = testingRepo.TestJobs.Where(m => m.Status == "Completed").ToList();

            if (Clean == "true") RedirectToAction("SearchTestJob", "TestJob");

            var requestQuery = Request.Query;

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

                JobList = jobRepo.Jobs.ToList(),
                StationsList = testingRepo.Stations.ToList(),
                StepList = testingRepo.Steps.ToList(),
                StepsForJobList = testingRepo.StepsForJobs.ToList(),
                StopList = testingRepo.Stops.Where(m => m.StopID != 980 & m.StopID != 981 && m.Reason2 == 0).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = OnCrossJobPage,
                    JobNumb = jobnumb,
                    ItemsPerPage = 5,
                    TotalItems = testJobsList.Count()
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
            if (jobNumber == 0) return View(testJobView);
            if (testJobsList.Count > 0 && testJobsList[0] != null) return View(testJobView);
            TempData["message"] = $"Does not exist any job with the JobNum #{jobNumber}, please try again.";
            TempData["alert"] = $"alert-danger";
            return View(testJobView);
        }

        [HttpPost]
        public IActionResult SearchJob(TestJobViewModel viewModel)
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
                        if (_jobSearch != null && _jobSearch.Status != "Incomplete")
                        {

                            TestJob testJob = new TestJob
                            {
                                JobID = _jobSearch.JobID,
                                TechnicianID = currentUser.EngID,
                                SinglePO = viewModel.POJobSearch,
                                Status = "Working on it",
                                StartDate = DateTime.Now,
                                CompletedDate = DateTime.Now,
                                StationID = 0
                            };
                            testingRepo.SaveTestJob(testJob);

                            var currentTestJob = testingRepo.TestJobs
                                .FirstOrDefault(p => p.TestJobID == testingRepo.TestJobs.Max(x => x.TestJobID));


                            TestJobViewModel NewtestJobView = new TestJobViewModel();
                            NewtestJobView.TestJob = currentTestJob;
                            NewtestJobView.Job = _jobSearch;
                            NewtestJobView.JobExtension = jobRepo.JobsExtensions.FirstOrDefault(m => m.JobID == _jobSearch.JobID);
                            NewtestJobView.HydroSpecific = jobRepo.HydroSpecifics.FirstOrDefault(m => m.JobID == _jobSearch.JobID);
                            NewtestJobView.GenericFeatures = jobRepo.GenericFeaturesList.FirstOrDefault(m => m.JobID == _jobSearch.JobID);
                            NewtestJobView.Indicator = jobRepo.Indicators.FirstOrDefault(m => m.JobID == _jobSearch.JobID);
                            NewtestJobView.HoistWayData = jobRepo.HoistWayDatas.FirstOrDefault(m => m.JobID == _jobSearch.JobID);
                            NewtestJobView.POJobSearch = testJob.SinglePO;
                            NewtestJobView.PO = jobRepo.POs.FirstOrDefault(m => m.JobID == _jobSearch.JobID);
                            NewtestJobView.isNotDummy = true;
                            NewtestJobView.TestFeature = new TestFeature();
                            NewtestJobView.TestFeature.TestJobID = testJob.TestJobID;
                            NewtestJobView.CurrentTab = "NewFeatures";


                            return View("NextForm", NewtestJobView);

                        }
                    }
                    else
                    {
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

            return View("NewTestJob", testJobSearchAux);
        }

        [HttpPost]
        public IActionResult NextForm(TestJobViewModel nextViewModel)
        {

            if (nextViewModel.TestFeature != null)
            {
                testingRepo.SaveTestFeature(nextViewModel.TestFeature);
                if (nextViewModel.isNotDummy == false) SaveDummyJob(nextViewModel);
                TempData["message"] = $"everything was saved";

                return NewTestFeatures(nextViewModel);
            }
            else
            {

                if (nextViewModel.isNotDummy == false)
                {
                    nextViewModel = SaveDummyJob(nextViewModel);
                    TempData["message"] = $"Job was saved";
                }


                nextViewModel.Job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);
                nextViewModel.PO = jobRepo.POs.FirstOrDefault(m => m.JobID == nextViewModel.Job.JobID);

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

                        nextViewModel.HydroSpecific.BatteryBrand = element.HAPS == true ?  "HAPS" : "";
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

                

                nextViewModel.TestFeature = new TestFeature();
                nextViewModel.TestFeature.TestJobID = nextViewModel.TestJob.TestJobID;
                nextViewModel.CurrentTab = "NewFeatures";
                return View(nextViewModel);
            }

        }

        public ViewResult EditTestJob(int ID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);
            Job CurrentJob = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
            TestFeature testFeature = testingRepo.TestFeatures.FirstOrDefault(m => m.TestJobID == testJob.TestJobID);
            TestJobViewModel nextViewModel = new TestJobViewModel();

            nextViewModel.PO = jobRepo.POs.FirstOrDefault(m => m.JobID == CurrentJob.JobID);
            nextViewModel.TestJob = testJob;
            nextViewModel.Job = CurrentJob;
            nextViewModel.TestFeature = testFeature;
            nextViewModel.isNotDummy = CurrentJob.Contractor == "Fake" ? false : true;
            nextViewModel.CurrentTab = "DummyJob";

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
                    return View(nextViewModel);
        }

        [HttpPost]
        public ViewResult EditTestJob(TestJobViewModel viewModel)
        {
            testingRepo.SaveTestFeature(viewModel.TestFeature);
            UpdateTestFeatures(viewModel);
            if (viewModel.isNotDummy == false) SaveDummyJob(viewModel);

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

            TempData["message"] = $"everything was saved";
            return View(nextViewModel);
        }


        public void UpdateTestFeatures(TestJobViewModel testJobView)
        {
            TestJob testJobToUpdate = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestJob.TestJobID);
            TestJob StationAuxTestJob = testingRepo.TestJobs.FirstOrDefault(m => m.StationID == testJobView.TestJob.StationID && m.Status == "Working on it");

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
            }

        }

        public TestJobViewModel SaveDummyJob(TestJobViewModel viewModel)
        {
            Job currentJob = new Job();
            TestJob currentTestJob = new TestJob();
            PO poUniqueAUx = poUniqueAUx = jobRepo.POs.FirstOrDefault(m => m.PONumb == viewModel.PO.PONumb);
            bool isNew = true;
            if (poUniqueAUx != null) isNew = false;

            AppUser currentUser = GetCurrentUser().Result;
            if (isNew == true)
            {
                Job Job = viewModel.Job;
                Job.Contractor = "Fake"; Job.Cust = "Fake"; Job.FireCodeID = 1; Job.LatestFinishDate = new DateTime(1, 1, 1);
                Job.EngID = currentUser.EngID; Job.Status = "Pending"; Job.CrossAppEngID = 1;
                if (viewModel.Canada == true) Job.CityID = 10;
                else Job.CityID = 40;
                if (viewModel.Ontario == true) Job.CityID = 11;
                else Job.CityID = 40;
                jobRepo.SaveJob(Job);
                currentJob = jobRepo.Jobs.FirstOrDefault(p => p.JobID == jobRepo.Jobs.Max(x => x.JobID));
            }
            else currentJob = jobRepo.Jobs.FirstOrDefault(m => m.JobID == poUniqueAUx.JobID);
           
            

            int jobtypeID = jobRepo.Jobs.First(m => m.JobID == currentJob.JobID).JobTypeID;
            switch (JobTypeName(jobtypeID))
            {
                case "M2000":
                case "M4000":

                    //Save the dummy Job Extension
                    JobExtension currentExtension = viewModel.JobExtension; currentExtension.JobID = currentJob.JobID; currentExtension.InputFrecuency = 60; currentExtension.InputPhase = 3; currentExtension.DoorGate = "Fake";
                    currentExtension.InputVoltage = 1; currentExtension.NumOfStops = 2; currentExtension.SHCRisers = 1; currentExtension.DoorHoist = "Fake"; currentExtension.JobTypeAdd = "Fake";
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

                    //Save the dummy Job HoistWayData
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
                    if (viewModel.Manual == true) element.DoorOperatorID = 2;
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
                    if (viewModel.Manual == true) element2.DoorOperatorID = 2;
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

            if (isNew == true)
            {
                //Create the new TestJob
                TestJob testJob = new TestJob
                {
                    JobID = currentJob.JobID,
                    TechnicianID = currentUser.EngID,
                    SinglePO = POFake.PONumb,
                    Status = "Working on it",
                    StartDate = DateTime.Now,
                    CompletedDate = DateTime.Now,
                    StationID = 0
                };
                testingRepo.SaveTestJob(testJob);

                currentTestJob = testingRepo.TestJobs.FirstOrDefault(p => p.TestJobID == testingRepo.TestJobs.Max(x => x.TestJobID));
            }
            else
            {
                currentTestJob = testingRepo.TestJobs.FirstOrDefault(p => p.JobID == currentJob.JobID);
            }

            

            TestJobViewModel testJobView = new TestJobViewModel
            {
                TestJob = currentTestJob,
                Job = currentJob
            };

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
        public IActionResult NewTestFeatures(TestJobViewModel testJobView)
        {
            bool techAdmin = GetCurrentUserRole("TechAdmin").Result;
            int TechnicianID = GetCurrentUser().Result.EngID;

            TestJob testJobToUpdate = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestJob.TestJobID);
            TestJob StationAuxTestJob = testingRepo.TestJobs.FirstOrDefault(m => m.StationID == testJobView.TestJob.StationID && m.Status == "Working on it");

            if (testJobToUpdate.TechnicianID != TechnicianID && techAdmin == false) return RedirectToAction("Index", "Home");
            if (StationAuxTestJob != null && techAdmin == false && TechnicianID != testJobToUpdate.TechnicianID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, la estacion esta ocupada, seleccione otra e intente de nuevo o contacte al Admin";
                return View("NewTestFeatures", testJobView);
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

        [HttpPost]
        public IActionResult StepsForJob(TestJobViewModel viewModel, string movement)
        {
            var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == viewModel.TestJob.TestJobID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
            var AllStepsForJobInfo = testingRepo.Steps.Where(m => AllStepsForJob.Any(s => s.StepID == m.StepID)).ToList();
            List<Stop> StopsFromTestJob = testingRepo.Stops.Where(m => m.TestJobID == viewModel.TestJob.TestJobID && m.Critical == false).ToList(); bool StopNC = false;
            List<Reason1> reason1s = testingRepo.Reasons1.ToList();
            if (StopsFromTestJob.Count > 0 && StopsFromTestJob[0] != null) StopNC = true;


            //When all steps are completed
            if (AllStepsForJob.Where(m => m.Complete == false).Count() == 1 && movement == "next")
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

            var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == ID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
            List<Reason1> reason1s = testingRepo.Reasons1.ToList();
            var AllStepsForJobInfo = testingRepo.Steps.Where(m => AllStepsForJob.Any(s => s.StepID == m.StepID)).ToList();

            List<Stop> StopsFromTestJob = testingRepo.Stops.Where(m => m.TestJobID == ID && m.Critical == false).ToList(); bool StopNC = false;
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


        [HttpPost]
        public IActionResult Delete(int ID)
        {
            TestJob deletedItem = testingRepo.DeleteTestJob(ID);

            if (deletedItem != null)
            {
                TempData["message"] = $"{deletedItem.TestJobID} was deleted";
            }
            return RedirectToAction("List");
        }

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

        public IActionResult Reassignment(TestJobViewModel testJobView)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestJob.TestJobID);
            string StationName = testingRepo.Stations.FirstOrDefault(m => m.StationID == testJobView.NewStationID).Label;

            if (testJob.TechnicianID == testJobView.NewTechnicianID && testJob.StationID == testJobView.NewStationID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You cannot reassing the CrossApprover and the station because are the same";
                return RedirectToAction("SearchTestJob");
            }
            else if (testJob.TestJobID != testJobView.NewTechnicianID || testJob.StationID != testJobView.NewStationID)
            {
                if (testJob.Status == "Stopped")
                {
                    Stop CurrentStop = testingRepo.Stops.LastOrDefault(p => p.Critical == true && p.Reason2 == 0);
                    Stop CopyStop = CurrentStop;

                    TimeSpan auxTime = (DateTime.Now - CurrentStop.StartDate);
                    CurrentStop.Elapsed += auxTime;
                    CurrentStop.StopDate = DateTime.Now;
                    CurrentStop.Description = "Job was reassigned";
                    testingRepo.SaveStop(CurrentStop);


                    CopyStop.StopID = 0;
                    CopyStop.StartDate = DateTime.Now;
                    CopyStop.StopDate = DateTime.Now;
                    CopyStop.Elapsed = new DateTime(1, 1, 1, 0, 0, 0);
                    CopyStop.AuxStationID = testJobView.NewStationID;
                    CopyStop.AuxTechnicianID = testJobView.NewTechnicianID;
                    testingRepo.SaveStop(CopyStop);
                }
                testJob.TechnicianID = testJobView.NewTechnicianID;
                testJob.StationID = testJobView.NewStationID;
                testJob.Status = "Reassignment";
                testingRepo.SaveTestJob(testJob);
                Stop NewtStop = new Stop
                {
                    TestJobID = testJob.TestJobID,
                    Reason1 = 980,
                    Reason2 = 980,
                    Reason3 = 980,
                    Reason4 = 980,
                    Reason5ID = 980,
                    Description = "Job was reassigned",
                    Critical = true,
                    StartDate = DateTime.Now,
                    StopDate = DateTime.Now,
                    Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                    AuxStationID = testJob.StationID,
                    AuxTechnicianID = testJob.TechnicianID,
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

        public IActionResult ReturnFromComplete(TestJobViewModel testJobView)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestJob.TestJobID);
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

        public void ShiftEnd(int TechnicianID)
        {
            List<TestJob> testJobList = testingRepo.TestJobs.Where(m => m.TechnicianID == TechnicianID && (m.Status != "Completed" && m.Status != "Incomplete")).ToList();
            if (testJobList.Count > 0)
            {
                foreach (TestJob testjob in testJobList)
                {
                    if (testjob.Status != "Stopped")
                    {
                        testjob.Status = "Shift End";
                        testingRepo.SaveTestJob(testjob);

                        Stop NewtStop = new Stop
                        {
                            TestJobID = testjob.TestJobID,
                            Reason1 = 981,
                            Reason2 = 981,
                            Reason3 = 981,
                            Reason4 = 981,
                            Reason5ID = 981,
                            Description = "Automatic Shift End",
                            Critical = true,
                            StartDate = DateTime.Now,
                            StopDate = DateTime.Now,
                            Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                            AuxStationID = testjob.StationID,
                            AuxTechnicianID = testjob.TechnicianID,
                        };
                        testingRepo.SaveStop(NewtStop);

                    }
                    else if (testjob.Status == "Stopped")
                    {

                        Stop CurrentStop = testingRepo.Stops.LastOrDefault(p => p.Critical == true && p.Reason1 != 980);
                        TimeSpan auxTime = (DateTime.Now - CurrentStop.StartDate);
                        CurrentStop.Elapsed += auxTime;
                        CurrentStop.StopDate = DateTime.Now;
                        testingRepo.SaveStop(CurrentStop);

                        testjob.Status = "Shift End";
                        testingRepo.SaveTestJob(testjob);

                        Stop NewtStop = new Stop
                        {
                            TestJobID = testjob.TestJobID,
                            Reason1 = 981,
                            Reason2 = 981,
                            Reason3 = 981,
                            Reason4 = 981,
                            Reason5ID = 981,
                            Description = "Automatic Shift End",
                            Critical = true,
                            StartDate = DateTime.Now,
                            StopDate = DateTime.Now,
                            Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                            AuxStationID = testjob.StationID,
                            AuxTechnicianID = testjob.TechnicianID,
                        };
                        testingRepo.SaveStop(NewtStop);
                    }
                }
            }

        }

        public void AutomaticShiftEnd()
        {
            List<TestJob> testJobs = testingRepo.TestJobs.Where(m => m.Status == "Working on it" || m.Status == "Stopped").ToList();
            if (testJobs.Count > 0)
            {
                foreach (TestJob testjob in testJobs)
                {
                    if (testjob.Status != "Stopped")
                    {
                        testjob.Status = "Shift End";
                        testingRepo.SaveTestJob(testjob);

                        Stop NewtStop = new Stop
                        {
                            TestJobID = testjob.TestJobID,
                            Reason1 = 981,
                            Reason2 = 981,
                            Reason3 = 981,
                            Reason4 = 981,
                            Reason5ID = 981,
                            Description = "Automatic Shift End",
                            Critical = true,
                            StartDate = DateTime.Now,
                            StopDate = DateTime.Now,
                            Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                            AuxStationID = testjob.StationID,
                            AuxTechnicianID = testjob.TechnicianID,
                        };
                        testingRepo.SaveStop(NewtStop);

                    }
                    else
                    {

                        Stop CurrentStop = testingRepo.Stops.LastOrDefault(p => p.Critical == true && p.Reason1 != 980);
                        TimeSpan auxTime = (DateTime.Now - CurrentStop.StartDate);
                        CurrentStop.Elapsed += auxTime;
                        CurrentStop.StopDate = DateTime.Now;
                        testingRepo.SaveStop(CurrentStop);

                        testjob.Status = "Shift End";
                        testingRepo.SaveTestJob(testjob);

                        Stop NewtStop = new Stop
                        {
                            TestJobID = testjob.TestJobID,
                            Reason1 = 981,
                            Reason2 = 981,
                            Reason3 = 981,
                            Reason4 = 981,
                            Reason5ID = 981,
                            Description = "Automatic Shift End",
                            Critical = true,
                            StartDate = DateTime.Now,
                            StopDate = DateTime.Now,
                            Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                            AuxStationID = testjob.StationID,
                            AuxTechnicianID = testjob.TechnicianID,
                        };
                        testingRepo.SaveStop(NewtStop);
                    }
                }
            }

        }

        public void RestartShiftEnd(int TechnicianID)
        {
            List<TestJob> testJobList = testingRepo.TestJobs.Where(m => m.TechnicianID == TechnicianID && m.Status == "Shift End").ToList();
            if (testJobList.Count > 0)
            {
                foreach (TestJob testJob in testJobList)
                {
                    Stop ShiftEndStop = testingRepo.Stops.LastOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason1 == 981);

                    Stop ReassignmentStop = testingRepo.Stops.LastOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason1 == 980);
                    Stop PreviusStop = testingRepo.Stops.FirstOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason2 == 0 && p.Critical == true);

                    if (ReassignmentStop != null)
                    {
                        TimeSpan auxTime = (DateTime.Now - ShiftEndStop.StartDate);
                        ShiftEndStop.Elapsed += auxTime;
                        ShiftEndStop.StopDate = DateTime.Now;
                        testingRepo.SaveStop(ShiftEndStop);

                        testJob.Status = "Reassignment";
                        testingRepo.SaveTestJob(testJob);
                    }
                    else if (PreviusStop != null)
                    {
                        TimeSpan auxTime = (DateTime.Now - ShiftEndStop.StartDate);
                        ShiftEndStop.Elapsed += auxTime;
                        ShiftEndStop.StopDate = DateTime.Now;
                        testingRepo.SaveStop(ShiftEndStop);

                        testJob.Status = "Stopped";
                        testingRepo.SaveTestJob(testJob);
                    }
                    else
                    {
                        TimeSpan auxTime = (DateTime.Now - ShiftEndStop.StartDate);
                        ShiftEndStop.Elapsed += auxTime;
                        ShiftEndStop.StopDate = DateTime.Now;
                        testingRepo.SaveStop(ShiftEndStop);

                        testJob.Status = "Working on it";
                        testingRepo.SaveTestJob(testJob);
                    }
                }
            }

        }

        public ViewResult JobCompletion(int TestJobID)
        {

            JobCompletionViewModel jobCompletion = new JobCompletionViewModel()
            {
                TestJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == TestJobID),
                StepsForJobList = testingRepo.StepsForJobs.Where(m => m.TestJobID == TestJobID && m.Obsolete == false && m.Complete == false).OrderBy(m => m.Consecutivo).ToList(),
            };
            return View(jobCompletion);

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

            testJob.CompletedDate = jobCompletion.FinishDate;
            testJob.Status = "Completed";
            testingRepo.SaveTestJob(testJob);
            TempData["message"] = $"You have completed the TestJob PO# {testJob.SinglePO} to Working on it";
            return RedirectToAction("SearchTestJob");

        }

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
                foreach (int number in names)
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
            totalTime += (date.Minute * .01);
            return totalTime;
        }

        public DateTime ToDateTime(double TotalHours)
        {
            DateTime Date = new DateTime(1, 1, 1, 0, 0, 0);
            double AuxTotalHours = Math.Truncate(TotalHours);
            double AuxTotalMinutes = TotalHours - AuxTotalHours;
            int AuxDays = 0;
            while (AuxTotalHours > 24)
            {
                AuxTotalHours -= 24;
                AuxDays++;
            };
            int Hours = (int)AuxTotalHours;
            int Minutes = (int)(Math.Round(AuxTotalMinutes * 60));
            int Days = 0;
            if (AuxDays > 1) Days = AuxDays - 1;
            else Days = 1;

            return new DateTime(1, 1, Days, Hours, Minutes, 0);
        }


        public async Task<IActionResult> TestJobSearchList(TestJobSearchViewModel searchViewModel, int page = 1)
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
            if (searchViewModel.JobNum >= 2015000000 && searchViewModel.JobNum <= 2021000000)
            {
                jobSearchRepo = jobSearchRepo.Where(s => s.JobNum == searchViewModel.JobNum); anyFeatureFromJob = true;
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

            searchViewModel.TestJobsSearchList = testJobSearchList.OrderBy(p => p.TechnicianID).Skip((page - 1) * 10).Take(10).ToList();
            searchViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 10,
                TotalItems = testJobSearchList.Count()
            };

            return View(searchViewModel);
        }
    }
}