using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels.Report;
using ProdFloor.Models.ViewModels.TestJob;

namespace ProdFloor.Controllers
{
    public class ReportController : Controller
    {
        private IJobRepository jobRepo;
        private ITestingRepository testingRepo;
        private IItemRepository itemRepository;
        private UserManager<AppUser> userManager;
        private IHostingEnvironment _env;
        public int PageSize = 7;

        public ReportController(
            ITestingRepository repo, 
            IJobRepository repo2, 
            IItemRepository repo3, 
            UserManager<AppUser> userMgr,
            IHostingEnvironment env)
        {
            jobRepo = repo2;
            testingRepo = repo;
            userManager = userMgr;
            itemRepository = repo3;
            _env = env;

        }

        public IActionResult Index(TestStatsForReportViewModel viewModel)
        {
            viewModel.TestStatsList = new List<TestStats>();
            List<TestJob> ActiveTestJobs = testingRepo.TestJobs.Where(m => m.Status != "Completed" && m.Status != "Deleted").ToList();

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

                ViewData["TV"] = "Simontl";
                viewModel.TestStatsList.Add(testStats);
            }

            return View(viewModel);
        }

        public async Task<IActionResult> DummyExcel(DateTime? startDate , DateTime? endDate)
        {
            string webRootPath = _env.WebRootPath;
            string fileName = @"Testingdummy.xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(webRootPath, fileName));
            var memoryStream = new MemoryStream();

            //-- Filtering the data with query params
            IQueryable<TestJob> testjobs = testingRepo.TestJobs.Where( m => m.Status == "Completed");
            IQueryable <Station> stations = testingRepo.Stations;
            IQueryable<AppUser> users = userManager.Users;

            if (startDate != null)
                testjobs = testjobs.Where(m => m.CompletedDate >= startDate);

            if (endDate != null)
                testjobs = testjobs.Where(m => m.CompletedDate <= endDate);


            // --- Below code would create excel file with dummy data----  
            using (var fs = new FileStream(Path.Combine(webRootPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Testingdummy");

                IRow row = excelSheet.CreateRow(0);
                row.CreateCell(0).SetCellValue("PO");
                row.CreateCell(1).SetCellValue("Station");
                row.CreateCell(2).SetCellValue("Technican");

                int index = 1;
                foreach (TestJob testjob in testjobs)
                {
                    string station = stations.FirstOrDefault(m => m.StationID == testjob.StationID).Label;
                    string userName = users.FirstOrDefault(m => m.EngID == testjob.TechnicianID).FullName;

                    row = excelSheet.CreateRow(index);
                    row.CreateCell(0).SetCellValue(testjob.SinglePO);
                    row.CreateCell(1).SetCellValue(station);
                    row.CreateCell(2).SetCellValue(userName);

                    index++;
                }


                workbook.Write(fs);
            }
            using (var fileStream = new FileStream(Path.Combine(webRootPath, fileName), FileMode.Open))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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
            if (Minutes == 60)
            {
                Hours++;
                Minutes = 0;
            }
            int Days = 0;
            if (AuxDays > 1) Days = AuxDays - 1;
            else Days = 1;

            return new DateTime(1, 1, Days, Hours, Minutes, 0);
        }
    }
}
