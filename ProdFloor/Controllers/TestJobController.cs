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
        public int PageSize = 10;

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

        [HttpPost]
        public IActionResult SearchJob(TestJobViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            var jobSearch = jobRepo.Jobs.AsQueryable();
            var POSearch = jobRepo.POs.AsQueryable();
            TestJobViewModel testJobSearchAux = new TestJobViewModel { };

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

                            TestJob testJob = new TestJob { JobID = _jobSearch.JobID, TechnicianID = currentUser.EngID, SinglePO = viewModel.POJobSearch, Status = "Working on it" };
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

            return View(testJobSearchAux);
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
                else currentExtension.DoorOperatorID = 1;
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
                SpecialFeatures featureFake = viewModel.SpecialFeature; featureFake.JobID = currentJob.JobID; featureFake.Description = null;
                jobRepo.SaveSpecialFeatures(featureFake);

                PO POFake = viewModel.PO; POFake.JobID = currentJob.JobID;
                jobRepo.SavePO(POFake);

                //Create the new TestJob
                TestJob testJob = new TestJob { JobID = currentJob.JobID, TechnicianID = currentUser.EngID, SinglePO = POFake.PONumb, Status = "Working on it" };
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
            List<StepsForJob> stepsForJobToDelete = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJobView.TestJob.TestJobID).ToList();
            if (stepsForJobToDelete.Count > 0)
            {
                foreach (StepsForJob step in stepsForJobToDelete)
                {
                    testingRepo.DeleteStepsForJob(step.StepsForJobID);
                }
            }
            //Checa que la lista de features no este vacia o nula
            if (testJobView.TestFeature != null)
            {
                //guarda la lista de features
                TestJob testJobModify = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestJob.TestJobID);
                testJobModify.Station = testJobView.TestJob.Station; testJobModify.JobLabel = testJobView.TestJob.JobLabel;
                testingRepo.SaveTestJob(testJobModify);
                testingRepo.SaveTestFeature(testJobView.TestFeature);

                //Rellena las listas que se llenaran para la comparacion
                List<TriggeringFeature> TriggersWithNameNull = testingRepo.TriggeringFeatures.Where(m => m.Name == null).ToList();
                List<TriggeringFeature> TriggersWithOutNameNull = testingRepo.TriggeringFeatures.Where(m => m.Name != null).ToList();
                var FeaturesFromTestJob = testingRepo.TestFeatures.First(m => m.TestJobID == testJobView.TestFeature.TestJobID);
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
                                            if(FeaturesFromJob._GenericFeatures.Monitoring != null)
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
                                        Consecutivo = consecutivo
                                    };
                                    testingRepo.SaveStepsForJob(stepForJob);
                                    consecutivo++;
                                }
                            }
                        }
                        else
                        {
                            return View(NotFound());
                        }
                    }
                }
                //Despues de terminar de hacer la lista de steps para job se manda el primero a la siguiente vista
                var stepsForAUX = testingRepo.StepsForJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestFeature.TestJobID && m.Consecutivo == 1); stepsForAUX.Start = DateTime.Now;
                testingRepo.SaveStepsForJob(stepsForAUX);
                var stepsFor = testingRepo.StepsForJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestFeature.TestJobID && m.Consecutivo == 1);
                var stepInfo = testingRepo.Steps.FirstOrDefault(m => m.StepID == stepsFor.StepID);
                var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == testJobView.TestJob.TestJobID);
                var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);
                var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJobView.TestFeature.TestJobID).ToList();
                var AllStepsForJobInfo = testingRepo.Steps.Where(m => AllStepsForJob.Any(s => s.StepID == m.StepID)).ToList();
                return View("StepsForJob", new TestJobViewModel { StepsForJob = stepsFor, Step = stepInfo, Job = job, TestJob = testjobinfo, StepList = AllStepsForJobInfo, StepsForJobList = AllStepsForJob });
            }

            return View(NotFound());
        }

        [HttpPost]
        public IActionResult StepsForJob(TestJobViewModel viewModel, int next)
        {
            List<StepsForJob> StepsForJobList = testingRepo.StepsForJobs.FromSql("select * from dbo.StepsForJobs where dbo.StepsForJobs.StepsForJobID " +
                "IN( select  Max(dbo.StepsForJobs.StepsForJobID ) from dbo.StepsForJobs where dbo.StepsForJobs.TestJobID = {0} group by dbo.StepsForJobs.Consecutivo)", viewModel.TestJob.TestJobID).ToList();
            var AllStepsForJobInfo = testingRepo.Steps.Where(m => StepsForJobList.Any(s => s.StepID == m.StepID)).ToList();
            if (next == 0)
            {
                return View("StepsForJob", viewModel);

            }//When all steps are completed
            else if (next > StepsForJobList.Distinct().Count())
            {
                var currentStepForJob = StepsForJobList.FirstOrDefault(m => m.Consecutivo == viewModel.StepsForJob.Consecutivo); currentStepForJob.Stop = DateTime.Now;
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
            else if (next == 777)
            {
                ///Va a mandar a la pagina de Stops
                return View("StepsForJob", viewModel);
            }//For Next Step
            else if (viewModel.StepsForJob.Consecutivo == (next - 1))
            {
                var currentStepForJob = StepsForJobList.FirstOrDefault(m => m.Consecutivo == viewModel.StepsForJob.Consecutivo); currentStepForJob.Stop = DateTime.Now;
                currentStepForJob.Complete = true;
                TimeSpan elapsed = currentStepForJob.Stop - currentStepForJob.Start;
                currentStepForJob.Elapsed += elapsed;
                testingRepo.SaveStepsForJob(currentStepForJob);

                //NextStep
                var stepsForAUX = StepsForJobList.FirstOrDefault(m => m.Complete == false); stepsForAUX.Start = DateTime.Now;
                testingRepo.SaveStepsForJob(stepsForAUX);
                var nextStepFor = StepsForJobList.FirstOrDefault(m => m.StepsForJobID == stepsForAUX.StepsForJobID);
                var stepInfo = testingRepo.Steps.FirstOrDefault(m => m.StepID == nextStepFor.StepID);
                var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == viewModel.TestJob.TestJobID);
                var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);
                return View("StepsForJob", new TestJobViewModel { StepsForJob = nextStepFor, Step = stepInfo, Job = job, TestJob = testjobinfo, StepList = AllStepsForJobInfo, StepsForJobList = StepsForJobList });

            }//For Previous Step
            else if (viewModel.StepsForJob.Consecutivo == (next + 1))
            {
                //Previus step
                var previusStepForAUX = StepsForJobList.OrderByDescending(m => m.Consecutivo).FirstOrDefault(m => m.Complete == true);
                previusStepForAUX.Start = DateTime.Now; previusStepForAUX.Complete = false;
                testingRepo.SaveStepsForJob(previusStepForAUX);
                var previusStepFor = StepsForJobList.FirstOrDefault(m => m.StepsForJobID == previusStepForAUX.StepsForJobID);
                var stepInfo = testingRepo.Steps.FirstOrDefault(m => m.StepID == previusStepFor.StepID);
                var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == viewModel.TestJob.TestJobID);
                var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);
                return View("StepsForJob", new TestJobViewModel { StepsForJob = previusStepFor, Step = stepInfo, Job = job, TestJob = testjobinfo, StepList = AllStepsForJobInfo, StepsForJobList = StepsForJobList });
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
            return View("StepsForJob", new TestJobViewModel { StepsForJob = nextStepFor, Step = stepInfo, Job = job, TestJob = testjobinfo, StepList = AllStepsForJobInfo, StepsForJobList = StepsForJobList });
        }

        public IActionResult ContinueStep(int ID)
        {
            List<StepsForJob> StepsForJobList = testingRepo.StepsForJobs.FromSql("select * from dbo.StepsForJobs where dbo.StepsForJobs.StepsForJobID " +
               "IN( select  Max(dbo.StepsForJobs.StepsForJobID ) from dbo.StepsForJobs where dbo.StepsForJobs.TestJobID = {0} group by dbo.StepsForJobs.Consecutivo)", ID).ToList();
            var AllStepsForJobInfo = testingRepo.Steps.Where(m => StepsForJobList.Any(s => s.StepID == m.StepID)).ToList();
            StepsForJob CurrentStep = StepsForJobList.FirstOrDefault(m => m.Complete == false); CurrentStep.Start = DateTime.Now;
            testingRepo.SaveStepsForJob(CurrentStep);
            var stepInfo = testingRepo.Steps.FirstOrDefault(m => m.StepID == CurrentStep.StepID);
            var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStep.TestJobID);
            var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);
            return View("StepsForJob", new TestJobViewModel { StepsForJob = CurrentStep, Step = stepInfo, Job = job, TestJob = testjobinfo, StepList = AllStepsForJobInfo, StepsForJobList = StepsForJobList });
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
    }
}