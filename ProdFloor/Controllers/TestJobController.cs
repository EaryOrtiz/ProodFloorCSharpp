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
    [Authorize(Roles = "Admin,Technician")]
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

        public ViewResult SearchTestJob()
        {
            TestJobViewModel testJobView = new TestJobViewModel
            {
                TestJobList = testingRepo.TestJobs
               .OrderBy(p => p.TechnicianID)
               .Skip((1 - 1) * 10)
               .Take(10).ToList(),
                JobList = jobRepo.Jobs.ToList(),
                StationsList = testingRepo.Stations.ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = 1,
                    ItemsPerPage = 10,
                    TotalItems = testingRepo.TestJobs.Count()
                }
            };
            return View(testJobView);
        }

        [HttpPost]
        public IActionResult SearchTestJob(TestJobViewModel viewModel, int page = 1)
        {
            List<TestJob> testJobsList = new List<TestJob>();
            List<Job> jobList = jobRepo.Jobs.Where(m => m.JobNum == viewModel.Job.JobNum).ToList();
            foreach (Job job in jobList)
            {
                TestJob TestjobAux = testingRepo.TestJobs.FirstOrDefault(m => m.JobID == job.JobID);
                if (TestjobAux != null) testJobsList.Add(TestjobAux);
            }

            TestJobViewModel testJobView = new TestJobViewModel
            {
                TestJobList = testJobsList
               .OrderBy(p => p.TechnicianID)
               .Skip((1 - 1) * 10)
               .Take(10).ToList(),
                JobList = jobRepo.Jobs.ToList(),
                StationsList = testingRepo.Stations.ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = 1,
                    ItemsPerPage = 10,
                    TotalItems = testJobsList.Count()
                }
            };

            if (testJobsList.Count > 0 && testJobsList[0] != null) return View(testJobView);
            TempData["message"] = $"Does not exist any job with the JobNum #{viewModel.Job.JobNum}, please try again.";
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
                    try
                    {

                        var onePO = POSearch.First(m => m.PONumb == viewModel.POJobSearch);
                        if (onePO != null)
                        {
                            var _jobSearch = jobSearch.First(m => m.JobID == onePO.JobID);
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


                                TestJobViewModel testJobView = new TestJobViewModel
                                {
                                    TestJob = currentTestJob,
                                    Job = _jobSearch
                                };

                                return View("NewTestFeatures", testJobView);

                            }
                        }

                    }
                    catch (Exception e)
                    {
                        return View("NewDummyJob", new TestJobViewModel
                        {
                            Job = new Job(),
                            JobExtension = new JobExtension(),
                            HydroSpecific = new HydroSpecific(),
                            GenericFeatures = new GenericFeatures(),
                            Indicator = new Indicator(),
                            HoistWayData = new HoistWayData(),
                            SpecialFeature = new SpecialFeatures(),
                            PO = new PO { PONumb = viewModel.POJobSearch }
                        });
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
                return View("NewTestJob",testJobSearchAux);

            }

            return View("NewTestJob", testJobSearchAux);
        }

        [HttpPost]
        public IActionResult NewDummyJob(TestJobViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            PO poUniqueAUx = new PO();
            try
            {
                poUniqueAUx = jobRepo.POs.FirstOrDefault(m => m.PONumb == viewModel.PO.PONumb);
            }
            catch (Exception)
            {

            }
            if (poUniqueAUx == null && viewModel.PO.PONumb != 0)
            {
                //Save the dummyJob
                Job Job = viewModel.Job;
                Job.Contractor = "Fake"; Job.Cust = "Fake"; Job.FireCodeID = 1; Job.LatestFinishDate = new DateTime(1, 1, 1);
                Job.EngID = currentUser.EngID; Job.Status = "Pending"; Job.CrossAppEngID = 1;
                if (viewModel.Canada == true) Job.CityID = 10;
                else Job.CityID = 40;
                if (viewModel.Ontario == true) Job.CityID = 11;
                else Job.CityID = 40;
                jobRepo.SaveJob(Job);
                Job currentJob = jobRepo.Jobs.FirstOrDefault(p => p.JobID == jobRepo.Jobs.Max(x => x.JobID));

                int jobtypeID = jobRepo.Jobs.First(m => m.JobID == currentJob.JobID).JobTypeID;
                switch (JobTypeName(jobtypeID))
                {
                    case "M2000":

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

                var currentTestJob = testingRepo.TestJobs.FirstOrDefault(p => p.TestJobID == testingRepo.TestJobs.Max(x => x.TestJobID));

                TestJobViewModel testJobView = new TestJobViewModel
                {
                    TestJob = currentTestJob,
                    Job = currentJob
                };

                return View("NewTestFeatures", testJobView);
            }
            else
            {
                TempData["message"] = $"El Job PO #ya existe por favor introduzca uno nuevo, o faltan campos por rellenar!";
                TempData["alert"] = $"alert-danger";
                return View("NewDummyJob", viewModel);
            }
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
        public ViewResult NewTestFeatures(TestJobViewModel testJobView)
        {
            TestJob testJobToUpdate = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestJob.TestJobID);
            TestJob StationAuxTestJob = testingRepo.TestJobs.FirstOrDefault(m => m.StationID == testJobView.TestJob.StationID && m.Status == "Working on it");
            if (StationAuxTestJob != null)
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
                        List<StepsForJob> NewStepsForJob = StepsForJobList(testJobView);

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
                        foreach (StepsForJob step in StepsForJobList(testJobView))
                        {
                            if (step != null) testingRepo.SaveStepsForJob(step);
                        }
                    }


                    //Despues de terminar de hacer la lista de steps para job se manda el primero a la siguiente vista
                    var stepsForAUX = testingRepo.StepsForJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestFeature.TestJobID && m.Consecutivo == 1); stepsForAUX.Start = DateTime.Now;
                    testingRepo.SaveStepsForJob(stepsForAUX);

                    var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJobView.TestFeature.TestJobID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
                    var stepsFor = AllStepsForJob.FirstOrDefault(m => (m.TestJobID == testJobView.TestFeature.TestJobID && m.Consecutivo == 1 && m.Complete == false) || (m.Complete == false));

                    var AllStepsForJobInfo = testingRepo.Steps.Where(m => AllStepsForJob.Any(s => s.StepID == m.StepID)).ToList();
                    var stepInfo = AllStepsForJobInfo.FirstOrDefault(m => m.StepID == stepsFor.StepID);

                    var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestJob.TestJobID);
                    var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);

                    List<Stop> StopsFromTestJob = testingRepo.Stops.Where(m => m.TestJobID == testJobView.TestJob.TestJobID && m.Critical == false).ToList(); bool StopNC = false;
                    if (StopsFromTestJob.Count > 0 && StopsFromTestJob[0] != null) StopNC = true;

                    var auxtStepsPerStageInfo = AllStepsForJobInfo.Where(m => m.Stage == stepInfo.Stage).ToList();
                    int StepsPerStage = auxtStepsPerStageInfo.Count();
                    int auxtStepsPerStage = AllStepsForJob.Where(m => auxtStepsPerStageInfo.Any(s => s.StepID == m.StepID)).Where(m => m.Complete == true).Count() + 1;


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
        public IActionResult StepsForJob(TestJobViewModel viewModel, int next)
        {
            var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == viewModel.TestJob.TestJobID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
            var AllStepsForJobInfo = testingRepo.Steps.Where(m => AllStepsForJob.Any(s => s.StepID == m.StepID)).ToList();
            List<Stop> StopsFromTestJob = testingRepo.Stops.Where(m => m.TestJobID == viewModel.TestJob.TestJobID && m.Critical == false).ToList(); bool StopNC = false;
            List <Reason1> reason1s = testingRepo.Reasons1.ToList();
            if (StopsFromTestJob.Count > 0 && StopsFromTestJob[0] != null) StopNC = true;
            
            if (next == 0)
            {
                return View("StepsForJob", viewModel);

            }//When all steps are completed
            else if (next > testingRepo.StepsForJobs.Where(m => m.TestJobID == viewModel.TestJob.TestJobID).Distinct().Count())
            {

                if (StopsFromTestJob.Count > 0 && StopsFromTestJob[0] != null)
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Error, tiene una parada pendiente por terminar, terminelo e intente de nuevo o contacte al Admin";
                    return (ContinueStep(viewModel.TestJob.TestJobID));
                }
                else
                {
                    var currentStepForJob = AllStepsForJob.FirstOrDefault(m => m.Consecutivo == viewModel.StepsForJob.Consecutivo); currentStepForJob.Stop = DateTime.Now;
                    currentStepForJob.Complete = true;
                    TimeSpan elapsed = currentStepForJob.Stop - currentStepForJob.Start;
                    currentStepForJob.Elapsed += elapsed;
                    testingRepo.SaveStepsForJob(currentStepForJob);
                    var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == viewModel.TestJob.TestJobID); testjobinfo.Status = "Completed";
                    testingRepo.SaveTestJob(testjobinfo);

                    TempData["message"] = $"El Test Job {testjobinfo.TestJobID} se ha completado con exito!";
                    TempData["alert"] = $"alert-success";
                    return RedirectToAction("Index", "Home", 1);
                }
                    
            }
            else if (next == 777)
            {
                ///Va a mandar a la pagina de Stops
                return View("StepsForJob", viewModel);
            }//For Next Step
            else if (viewModel.StepsForJob.Consecutivo == (next - 1))
            {
                var currentStepForJob = AllStepsForJob.FirstOrDefault(m => m.Consecutivo == viewModel.StepsForJob.Consecutivo); currentStepForJob.Stop = DateTime.Now;
                currentStepForJob.Complete = true;
                TimeSpan elapsed = currentStepForJob.Stop - currentStepForJob.Start;
                currentStepForJob.Elapsed += elapsed;
                testingRepo.SaveStepsForJob(currentStepForJob);

                //NextStep
                var stepsForAUX = AllStepsForJob.FirstOrDefault(m => m.Complete == false); stepsForAUX.Start = DateTime.Now;
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
            else if (viewModel.StepsForJob.Consecutivo == (next + 1))
            {
                //Previus step
                var previusStepForAUX = AllStepsForJob.OrderByDescending(m => m.Consecutivo).FirstOrDefault(m => m.Complete == true);
                previusStepForAUX.Start = DateTime.Now; previusStepForAUX.Complete = false;
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

            var AllStepsForJobInfo = testingRepo.Steps.Where(m => StepsForJobList.Any(s => s.StepID == m.StepID)).ToList();

            var currentStepForJob = StepsForJobList.FirstOrDefault(m => m.Consecutivo == viewModel.StepsForJob.Consecutivo); currentStepForJob.Stop = DateTime.Now;
            currentStepForJob.Complete = false; TimeSpan elapsed = currentStepForJob.Stop - currentStepForJob.Start; currentStepForJob.Elapsed += elapsed;
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
            int auxtStepsPerStage = StepsForJobList.Where(m => auxtStepsPerStageInfo.Any(s => s.StepID == m.StepID)).Where(m => m.Complete == true).Count() + 1;

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

        public ViewResult ContinueStep(int ID)
        {
            var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == ID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
            List<Reason1> reason1s = testingRepo.Reasons1.ToList();
            var AllStepsForJobInfo = testingRepo.Steps.Where(m => AllStepsForJob.Any(s => s.StepID == m.StepID)).ToList();

            List<Stop> StopsFromTestJob = testingRepo.Stops.Where(m => m.TestJobID == ID && m.Critical == false).ToList(); bool StopNC = false;
            if (StopsFromTestJob.Count > 0 && StopsFromTestJob[0] != null) StopNC = true;

            StepsForJob CurrentStep = AllStepsForJob.FirstOrDefault(m => m.Complete == false); CurrentStep.Start = DateTime.Now;
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


        public List<StepsForJob> StepsForJobList(TestJobViewModel testJobView)
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
                    Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.StopID == testingRepo.Stops.Max(x => x.StopID) && p.Critical == true && p.Reason2 == 0);
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
            StepsForJob stepsForJob = testingRepo.StepsForJobs.FirstOrDefault(p => p.TestJobID == testJob.TestJobID && p.StepsForJobID == testingRepo.StepsForJobs.Max(x => x.StepsForJobID));
            testJob.Status = "Working on it";
            stepsForJob.Complete = false;
            testingRepo.SaveTestJob(testJob);
            testingRepo.SaveStepsForJob(stepsForJob);
            TempData["message"] = $"You have retuned the TestJob PO# {testJob.SinglePO} to Working on it";
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
                    else
                    {

                        Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.StopID == testingRepo.Stops.Max(x => x.StopID) && p.Critical == true && p.Reason1 != 980);
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
                    Stop ShiftEndStop = testingRepo.Stops.FirstOrDefault(p => p.TestJobID == testJob.TestJobID && p.StopID == testingRepo.Stops.Max(x => x.StopID) && p.Reason1 == 981);
                    Stop ReassignmentStop = testingRepo.Stops.FirstOrDefault(p => p.TestJobID == testJob.TestJobID && p.StopID == testingRepo.Stops.Max(x => x.StopID) && p.Reason1 == 980);
                    Stop PreviusStop = testingRepo.Stops.FirstOrDefault(p => p.TestJobID == testJob.TestJobID && p.StopID == testingRepo.Stops.Max(x => x.StopID) && p.Reason2 == 0 && p.Critical == true);
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
    }
}