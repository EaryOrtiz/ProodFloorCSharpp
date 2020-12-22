using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Util;
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

        public IActionResult Index(ReportsViewModel viewModel)
        {
            #region TestStats
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
            #endregion

            #region DailyReports
            viewModel.dailyReports = GetDailyReports(DateTime.Now.AddDays(-1));
            #endregion

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> TestDummy(DateTime startDate , DateTime endDate)
        {
            string webRootPath = _env.WebRootPath;
            string fileName = @"Stops_"+startDate.ToShortDateString()+"_"+startDate.ToShortDateString()+"+.xlsx";
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

        [HttpPost]
        public async Task<IActionResult> StopsReport(DateTime startDate, DateTime endDate)
        {

            //-- Filtering the data with query params 
            List<StopsReport> stopsReport = GetStopsReport(startDate, endDate);


            string webRootPath = _env.WebRootPath;
            string fileName = @"Stops_" + startDate.ToString("yyyy-MM-dd") + "_" + endDate.ToString("yyyy-MM-dd") + ".xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(webRootPath, fileName));
            var memoryStream = new MemoryStream();

            // --- Below code would create excel file with dummy data----  
            using (var fs = new FileStream(Path.Combine(webRootPath, fileName), FileMode.Create, FileAccess.Write))
            {
                stopsReport = stopsReport.OrderBy(m => m.JobNumer).ToList();
                IWorkbook workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Stops_" + startDate.ToString("yyyy-MM-dd") + "_"+ endDate.ToString("yyyy-MM-dd"));
                int i = 0;

                XSSFCellStyle TitleGrey = (XSSFCellStyle)workbook.CreateCellStyle();
                TitleGrey.BorderBottom = BorderStyle.Medium;
                TitleGrey.BorderTop = BorderStyle.Medium;
                TitleGrey.BorderLeft = BorderStyle.Medium;
                TitleGrey.BorderRight = BorderStyle.Medium;
                TitleGrey.FillForegroundColor = HSSFColor.Grey25Percent.Index;
                TitleGrey.FillPattern = FillPattern.SolidForeground;

                XSSFCellStyle Ttable = (XSSFCellStyle)workbook.CreateCellStyle();
                TitleGrey.BorderBottom = BorderStyle.Medium;
                TitleGrey.BorderTop = BorderStyle.Medium;
                TitleGrey.BorderLeft = BorderStyle.Medium;
                TitleGrey.BorderRight = BorderStyle.Medium;

                IRow row = excelSheet.CreateRow(i);
                row.CreateCell(0).SetCellValue("Job #");
                row.CreateCell(1).SetCellValue("PO");
                row.CreateCell(2).SetCellValue("JobType");
                row.CreateCell(3).SetCellValue("Week");
                row.CreateCell(4).SetCellValue("Start");
                row.CreateCell(5).SetCellValue("Stop");
                row.CreateCell(6).SetCellValue("Reason1");
                row.CreateCell(7).SetCellValue("Reason2");
                row.CreateCell(8).SetCellValue("Reason3");
                row.CreateCell(9).SetCellValue("Reason4");
                row.CreateCell(10).SetCellValue("Reason5");
                row.CreateCell(11).SetCellValue("Description");
                row.CreateCell(12).SetCellValue("Critical");
                row.CreateCell(13).SetCellValue("Station");
                row.CreateCell(14).SetCellValue("Time on Stop");
                row.CreateCell(15).SetCellValue("IsCompleted?");
                row.CreateCell(16).SetCellValue("Technician");

                for (int j = 0; j < 17; j++)
                {
                    workbook.GetSheetAt(0).GetRow(i).GetCell(j).CellStyle = TitleGrey;
                    excelSheet.AutoSizeColumn(j);
                }

                i++;

                

                foreach (StopsReport report in stopsReport)
                {
                    row = excelSheet.CreateRow(i);
                    row.CreateCell(0).SetCellValue(report.JobNumer);
                    row.CreateCell(1).SetCellValue(report.PO);
                    row.CreateCell(2).SetCellValue(report.JobTypeName);
                    row.CreateCell(3).SetCellValue(report.WeekNumber);
                    row.CreateCell(4).SetCellValue(report.StartDate.ToString());
                    row.CreateCell(5).SetCellValue(report.StopDate.ToString());
                    row.CreateCell(6).SetCellValue(report.Reason1);
                    row.CreateCell(7).SetCellValue(report.Reason2);
                    row.CreateCell(8).SetCellValue(report.Reason3);
                    row.CreateCell(9).SetCellValue(report.Reason4);
                    row.CreateCell(10).SetCellValue(report.Reason5);
                    row.CreateCell(11).SetCellValue(report.Description);
                    row.CreateCell(12).SetCellValue(report.Critical);
                    row.CreateCell(13).SetCellValue(report.StationName);
                    row.CreateCell(14).SetCellValue(report.Elapsed);
                    row.CreateCell(15).SetCellValue(report.isFinished);
                    row.CreateCell(16).SetCellValue(report.TechFullName);

                    for (int j = 0; j < 17; j++)
                    {
                        workbook.GetSheetAt(0).GetRow(i).GetCell(j).CellStyle = Ttable; 
                        excelSheet.AutoSizeColumn(j);
                    }

                    i++;

                }
                workbook.Write(fs);
            }
            using (var fileStream = new FileStream(Path.Combine(webRootPath, fileName), FileMode.Open))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;
            if (System.IO.File.Exists(Path.Combine(webRootPath, fileName)))
                System.IO.File.Delete(Path.Combine(webRootPath, fileName));

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }


        [HttpPost]
        public async Task<IActionResult> DailyReport(DateTime startDate)
        {
            if (startDate == null) startDate = DateTime.Now.AddDays(-1);

            //-- Filtering the data with query params 
            List<DailyReport> dailyReports = GetDailyReports(startDate);

            string webRootPath = _env.WebRootPath;
            string fileName = @"DailyReport-" + startDate.ToString("yyyy-MM-dd") + ".xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(webRootPath, fileName));
            var memoryStream = new MemoryStream();

            // --- Below code would create excel file with dummy data----  
            using (var fs = new FileStream(Path.Combine(webRootPath, fileName), FileMode.Create, FileAccess.Write))
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("DailyReport-" + startDate.ToString("yyyy-MM-dd"));
                List<TestStats> testStats = new List<TestStats>();
                int i =  0;

                XSSFCellStyle titleGreyStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                titleGreyStyle.BorderBottom = BorderStyle.Medium;
                titleGreyStyle.BorderTop = BorderStyle.Medium;
                titleGreyStyle.BorderLeft = BorderStyle.Medium;
                titleGreyStyle.BorderRight = BorderStyle.Medium;
                titleGreyStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
                titleGreyStyle.FillPattern = FillPattern.SolidForeground;

                XSSFCellStyle tableStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                tableStyle.BorderBottom = BorderStyle.Thin;
                tableStyle.BorderTop = BorderStyle.Thin;
                tableStyle.BorderLeft = BorderStyle.Thin;
                tableStyle.BorderRight = BorderStyle.Thin;

                IRow row = excelSheet.CreateRow(i);
                row.CreateCell(0).SetCellValue("Date: ");
                row.CreateCell(1).SetCellValue(startDate.ToShortDateString());

                for (int j = 0; j < 2; j++)
                {
                    workbook.GetSheetAt(0).GetRow(i).GetCell(j).CellStyle = titleGreyStyle;
                }

                i++;

                row = excelSheet.CreateRow(i);
                row.CreateCell(0).SetCellValue("JobType");
                row.CreateCell(1).SetCellValue("Completed");
                row.CreateCell(2).SetCellValue("TotalEfficiency");


                for (int j = 0; j < 3; j++)
                    {
                        workbook.GetSheetAt(0).GetRow(i).GetCell(j).CellStyle = titleGreyStyle;
                        excelSheet.AutoSizeColumn(j);
                    }

                i++;

                foreach (DailyReport daily in dailyReports)
                {

                    row = excelSheet.CreateRow(i);
                    row.CreateCell(0).SetCellValue(daily.JobTypeName);
                    row.CreateCell(1).SetCellValue(daily.TestJobsCounted);
                    row.CreateCell(2).SetCellValue(daily.TotalEfficiency);

                    for (int j = 0; j < 3; j++)
                    {
                        workbook.GetSheetAt(0).GetRow(i).GetCell(j).CellStyle = tableStyle;
                        excelSheet.AutoSizeColumn(j);
                    }

                    i++;

                }

                workbook.Write(fs);
            }
            using (var fileStream = new FileStream(Path.Combine(webRootPath, fileName), FileMode.Open))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;
            if (System.IO.File.Exists(Path.Combine(webRootPath, fileName)))
                System.IO.File.Delete(Path.Combine(webRootPath, fileName));

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public async Task<IActionResult> EfficiencyReport(DateTime startDate, DateTime endDate)
        {
            //-- Filtering the data with query params 
            List<EfficiencyReport> efficiencyReports = GetEfficiencyReports(startDate, endDate);

            string webRootPath = _env.WebRootPath;
            string fileName = @"Efficiency_" + startDate.ToString("yyyy-MM-dd") + "_" + endDate.ToString("yyyy-MM-dd") + ".xlsx";
            string URL = string.Format("{0}://{1}/{2}", Request.Scheme, Request.Host, fileName);
            FileInfo file = new FileInfo(Path.Combine(webRootPath, fileName));
            var memoryStream = new MemoryStream();

            // --- Below code would create excel file with dummy data----  
            using (var fs = new FileStream(Path.Combine(webRootPath, fileName), FileMode.Create, FileAccess.Write))
            {
                efficiencyReports = efficiencyReports.OrderBy(m => m.TechName).ToList();
                IWorkbook workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Efficiency_" + startDate.ToString("yyyy-MM-dd") + "_" + endDate.ToString("yyyy-MM-dd"));
                int i = 0;

                XSSFCellStyle titleGreyStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                titleGreyStyle.BorderBottom = BorderStyle.Medium;
                titleGreyStyle.BorderTop = BorderStyle.Medium;
                titleGreyStyle.BorderLeft = BorderStyle.Medium;
                titleGreyStyle.BorderRight = BorderStyle.Medium;
                titleGreyStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
                titleGreyStyle.FillPattern = FillPattern.SolidForeground;

                XSSFCellStyle tableStyle = (XSSFCellStyle)workbook.CreateCellStyle();
                tableStyle.BorderBottom = BorderStyle.Thin;
                tableStyle.BorderTop = BorderStyle.Thin;
                tableStyle.BorderLeft = BorderStyle.Thin;
                tableStyle.BorderRight = BorderStyle.Thin;

                IRow row = excelSheet.CreateRow(i);
                row.CreateCell(1).SetCellValue("Dates");

                i++;

                row = excelSheet.CreateRow(i);
                row.CreateCell(0).SetCellValue(startDate.ToString("yyyy-MM-dd"));
                row.CreateCell(2).SetCellValue(endDate.ToString("yyyy-MM-dd"));

                i++;

                row = excelSheet.CreateRow(i);
                i++;

                foreach (EfficiencyReport report in efficiencyReports)
                {
                    row = excelSheet.CreateRow(i);
                    row.CreateCell(0).SetCellValue("Tech: ");
                    row.CreateCell(1).SetCellValue(report.TechName);
                    row.CreateCell(2).SetCellValue("");
                    row.CreateCell(3).SetCellValue("");
                    row.CreateCell(4).SetCellValue("");
                    row.CreateCell(5).SetCellValue("");
                    row.CreateCell(6).SetCellValue("");
                    row.CreateCell(7).SetCellValue("");
                    row.CreateCell(8).SetCellValue("Average Eff: ");
                    row.CreateCell(9).SetCellValue(report.AverageEff);
                    i++;

                    row = excelSheet.CreateRow(i);
                    row.CreateCell(0).SetCellValue("Job No.");
                    row.CreateCell(2).SetCellValue("PO");
                    row.CreateCell(1).SetCellValue("Job Type");
                    row.CreateCell(3).SetCellValue("Percentage");
                    row.CreateCell(4).SetCellValue("Elapsed Time");
                    row.CreateCell(5).SetCellValue("Effiency");
                    row.CreateCell(6).SetCellValue("Station");
                    row.CreateCell(7).SetCellValue("No. Stops");
                    row.CreateCell(8).SetCellValue("Time At Stops");
                    row.CreateCell(9).SetCellValue("Stop(s) Reason(s)");

                    for(int k = i-1; k <= i; k++)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            workbook.GetSheetAt(0).GetRow(k).GetCell(j).CellStyle = titleGreyStyle;

                            excelSheet.AutoSizeColumn(j);
                        }
                    }
                   

                    i++;

                    try
                    {
                        if (report.testJobEfficiencies == null)
                        {
                            row = excelSheet.CreateRow(i);
                            i++;
                            continue;
                        }

                    }
                    catch { }


                    foreach (TestJobEfficiency reportDetails in report.testJobEfficiencies)
                    {
                        row = excelSheet.CreateRow(i);
                        row.CreateCell(0).SetCellValue(reportDetails.JobNumer);
                        row.CreateCell(1).SetCellValue(reportDetails.PO);
                        row.CreateCell(2).SetCellValue(reportDetails.JobTypeName);
                        row.CreateCell(3).SetCellValue(reportDetails.PercentagePerTech);
                        row.CreateCell(4).SetCellValue(reportDetails.ElapsedTimePerTech);
                        row.CreateCell(5).SetCellValue(reportDetails.EfficiencyPerTech);
                        row.CreateCell(6).SetCellValue(reportDetails.StationName);
                        row.CreateCell(7).SetCellValue(reportDetails.StopsCounted);
                        row.CreateCell(8).SetCellValue(reportDetails.TimeAtStops);
                        row.CreateCell(9).SetCellValue(reportDetails.StopsReasons);

                        for (int j = 0; j < 10; j++)
                        {
                            workbook.GetSheetAt(0).GetRow(i).GetCell(j).CellStyle = tableStyle;
                            excelSheet.AutoSizeColumn(j);
                        }

                        i++;

                    }

                    row = excelSheet.CreateRow(i);
                    i++;
                }

                workbook.Write(fs);
            }
            using (var fileStream = new FileStream(Path.Combine(webRootPath, fileName), FileMode.Open))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;

            if (System.IO.File.Exists(Path.Combine(webRootPath, fileName)))
                System.IO.File.Delete(Path.Combine(webRootPath, fileName));

            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        private async Task<bool> GetCurrentUserRole(AppUser user, string role)
        {

            bool isInRole = await userManager.IsInRoleAsync(user, role);

            return isInRole;
        }

        public List<EfficiencyReport> GetEfficiencyReports(DateTime startDate, DateTime endDate)
        {
            if (startDate == null) startDate = DateTime.Now;
            if (endDate == null) endDate = DateTime.Now;

            //-- Filtering the data with query params 
            List<EfficiencyReport> efficiencyReports = new List<EfficiencyReport>();
            List<Station> stations = testingRepo.Stations.ToList();
            List<JobType> jobTypes = itemRepository.JobTypes.Where(m => m.Name != "M3000").ToList();
            IQueryable<Stop> stops = testingRepo.Stops.Where(m => m.Reason1 != 981);
            IQueryable<TestJob> testjobsCompleted = testingRepo.TestJobs.Where(m => m.Status == "Completed" && m.CompletedDate >= startDate)
                                                                        .Where(m =>  m.CompletedDate <= endDate.AddHours(23))
                                                                        .OrderBy(m => m.StartDate);
            List<Step> StepsListInfo = testingRepo.Steps.ToList();
            IQueryable<Job> jobsForTestJobs = jobRepo.Jobs;
            IQueryable<Reason1> reasons1 = testingRepo.Reasons1;
            List<AppUser> users = new List<AppUser>();
            foreach (var user in userManager.Users)
            {
                if (user != null
                && GetCurrentUserRole(user, "Technician").Result)
                {
                    users.Add(user);
                }
            }
            List<TestJob> testJobs = testjobsCompleted
                .Where(m => users.Any(n => n.EngID == m.TechnicianID)).ToList();

            //Filling the EffiencyReport
            foreach (AppUser user in users)
            {
                EfficiencyReport efficiencyReport = new EfficiencyReport();
                efficiencyReport.testJobEfficiencies = new List<TestJobEfficiency>();
                efficiencyReport.TechName = user.FullName;
                double AvgEffSUM = 0;
                int percentageMoreThan50 = 0;

                foreach (TestJob testJob in testJobs)
                {
                    TestJobEfficiency testJobEff = new TestJobEfficiency();
                    testJobEff.StopsReasons = "";
                    double elapsedTimePerTech = 0;
                    double timeAtStops = 0;
                    List<StepsForJob> stepsForJobByUser = testingRepo.StepsForJobs
                        .Where(m => m.AuxTechnicianID == user.EngID)
                        .Where(m => m.TestJobID == testJob.TestJobID)
                        .ToList();

                    int stepsCounted = stepsForJobByUser.Count();

                    if (stepsCounted == 0)
                        continue;

                    double totalTimeOnJob = 0;
                    double effPerStepSUM = 0;
                    List<StepsForJob> totalStepsForJob = testingRepo.StepsForJobs
                        .Where(m => m.TestJobID == testJob.TestJobID)
                        .ToList();

                    foreach (StepsForJob step in totalStepsForJob)
                    {
                        totalTimeOnJob += ToHours(step.Elapsed);
                    }

                    foreach (StepsForJob step in stepsForJobByUser)
                    {
                        elapsedTimePerTech += ToHours(step.Elapsed);

                        double elapsed = ToHours(step.Elapsed);
                        double expected = ToHours(StepsListInfo.First(m => m.StepID == step.StepID).ExpectedTime);

                        double effPerStep = (elapsed / expected) * 100;

                        effPerStepSUM += effPerStep <= 100 ? effPerStep : 100;
                    }

                    Job featuresFromJob = jobsForTestJobs.FirstOrDefault(m => m.JobID == testJob.JobID);
                    List<Stop> stopsForTestjob = stops
                        .Where(m => m.TestJobID == testJob.TestJobID)
                        .Where(m => m.AuxTechnicianID == user.EngID)
                        .ToList();

                    foreach (Stop stop in stopsForTestjob)
                    {
                        string R1 = reasons1.First(m => m.Reason1ID == stop.Reason1).Description;
                        string elapsed = ToClockDateString(stop.Elapsed);

                        testJobEff.StopsReasons += (R1 + " (" +elapsed + "), ");
                        timeAtStops += ToHours(stop.Elapsed);
                    }

                    testJobEff.StationName = stations.FirstOrDefault(m => m.StationID == testJob.StationID).Label;
                    testJobEff.JobTypeName = jobTypes.FirstOrDefault(m => m.JobTypeID == featuresFromJob.JobTypeID).Name;
                    testJobEff.JobNumer = featuresFromJob.JobNum.Remove(0, 5);
                    testJobEff.PO = testJob.SinglePO;
                    testJobEff.StopsCounted = stopsForTestjob.Count();
                    testJobEff.PercentagePerTech = (elapsedTimePerTech / totalTimeOnJob) * 100;
                    testJobEff.EfficiencyPerTech = (effPerStepSUM / stepsCounted);

                    //conversiones de horas a timpeo de reloj
                    DateTime auxDate = ToDateTime(elapsedTimePerTech);
                    testJobEff.ElapsedTimePerTech = ToClockDateString(auxDate);

                    auxDate = ToDateTime(timeAtStops);
                    testJobEff.TimeAtStops = ToClockDateString(auxDate);

                    efficiencyReport.testJobEfficiencies.Add(testJobEff);

                    if (testJobEff.PercentagePerTech > 50)
                    {
                        AvgEffSUM += testJobEff.EfficiencyPerTech;
                        percentageMoreThan50++;
                    }
                }


                efficiencyReport.AverageEff = percentageMoreThan50 > 0 ? (AvgEffSUM / percentageMoreThan50) : percentageMoreThan50;
                efficiencyReports.Add(efficiencyReport);
            }

            return efficiencyReports;
        }

        public List<DailyReport> GetDailyReports(DateTime startDate)
        {
            if (startDate == null || startDate.Day == DateTime.Now.Day) startDate = DateTime.Now.AddDays(-1);

            //-- Filtering the data with query params 
            List<DailyReport> dailyReports = new List<DailyReport>();
            List<Step> StepsListInfo = testingRepo.Steps.ToList();
            List<Station> Stations = testingRepo.Stations.ToList();
            List<JobType> jobTypes = itemRepository.JobTypes.Where(m => m.Name != "M3000").ToList();
            IQueryable<AppUser> users = userManager.Users;
            IQueryable<TestJob> testjobsCompleted = testingRepo.TestJobs.Where(m => m.Status == "Completed" && m.CompletedDate.ToShortDateString() == startDate.ToShortDateString());
            IQueryable<Job> jobsForTestJobs = jobRepo.Jobs.Where(m => testjobsCompleted.Any(n => n.JobID == m.JobID));


            //Filling daily and textsts list by jobtype
            foreach (JobType jobtype in jobTypes)
            {
                string efficiencyColor = "grey";
                int testJobsCounted = 0;
                int totalEfficiency = 0;
                DailyReport daily = new DailyReport();
                daily.TestStats = new List<TestStats>();
                IQueryable<Job> jobs = jobsForTestJobs.Where(m => m.JobTypeID == jobtype.JobTypeID);
                IQueryable<TestJob> testjobs = testjobsCompleted.Where(m => jobs.Any(n => n.JobID == m.JobID));

                foreach (TestJob testjob in testjobs)
                {
                    Job FeaturesFromJob = jobs.FirstOrDefault(m => m.JobID == testjob.JobID);
                    List<StepsForJob> stepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testjob.TestJobID)
                                                                         .Where(m => m.Complete == true)
                                                                         .OrderBy(n => n.Consecutivo).ToList();

                    string jobNum = FeaturesFromJob.JobNum.Remove(0, 5);
                    string techName = users.FirstOrDefault(m => m.EngID == testjob.TechnicianID).FullName;
                    string stationName = Stations.FirstOrDefault(m => m.StationID == testjob.StationID).Label;
                    double expectedTimeSUM = 0;
                    double realTimeSUM = 0;
                    double efficiency = 0;

                    foreach (StepsForJob step in stepsForJob)
                    {
                        expectedTimeSUM += ToHours(StepsListInfo.FirstOrDefault(m => m.StepID == step.StepID).ExpectedTime);

                        realTimeSUM += ToHours(step.Elapsed);
                    }

                    efficiency = Math.Round((expectedTimeSUM / realTimeSUM) * 100);
                    if (efficiency > 99) efficiency = 99;
                    totalEfficiency = (int)(totalEfficiency + efficiency);
                    testJobsCounted++;


                    TestStats testStat = new TestStats
                    {
                        JobNumer = jobNum,
                        TechName = techName,
                        Station = stationName,
                        Efficiency = efficiency
                    };

                    daily.TestStats.Add(testStat);
                }

                if (testJobsCounted > 0) totalEfficiency = totalEfficiency / testJobsCounted;

                if (totalEfficiency > 99) totalEfficiency = 99;
                if (totalEfficiency >= 80) efficiencyColor = "green";
                else if (totalEfficiency >= 60) efficiencyColor = "yellow";
                else if (totalEfficiency > 0) efficiencyColor = "red";

                daily.JobTypeName = jobtype.Name;
                daily.TestJobsCounted = testJobsCounted;
                daily.TotalEfficiency = totalEfficiency;
                daily.EfficiencyColor = efficiencyColor;
                daily.TodayDate = startDate.ToShortDateString();

                dailyReports.Add(daily);
            }

            return dailyReports;
        }

        public List<StopsReport> GetStopsReport(DateTime startDate, DateTime endDate)
        {
            if (startDate == null) startDate = DateTime.Now;
            if (endDate == null) endDate = DateTime.Now;

            //-- Filtering the data with query params 
            List<StopsReport> stopsReport = new List<StopsReport>();
            List<Station> stations = testingRepo.Stations.ToList();
            List<JobType> jobTypes = itemRepository.JobTypes.Where(m => m.Name != "M3000").ToList();
            IQueryable<Stop> stops = testingRepo.Stops.Where(m => m.StopDate >= startDate && m.StopDate <= endDate.AddHours(23))
                                                      .Where(m => m.Reason1 != 981)
                                                      .OrderBy(m => m.StartDate);

            IQueryable<TestJob> testjobs = testingRepo.TestJobs.Where(m => stops.Any(n => n.TestJobID == m.TestJobID));
            IQueryable<Job> jobsForTestJobs = jobRepo.Jobs.Where(m => testjobs.Any(n => n.JobID == m.JobID));
            IQueryable<AppUser> users = userManager.Users;
            IQueryable<Reason1> reasons1 = testingRepo.Reasons1;
            IQueryable<Reason2> reasons2 = testingRepo.Reasons2;
            IQueryable<Reason3> reasons3 = testingRepo.Reasons3;
            IQueryable<Reason4> reasons4 = testingRepo.Reasons4;
            IQueryable<Reason5> reasons5 = testingRepo.Reasons5;

            //Filling the StopsRpeort List

            foreach (Stop stop in stops)
            {

                string technician = users.FirstOrDefault(m => m.EngID == stop.AuxTechnicianID).FullName;
                Station station = stations.FirstOrDefault(m => m.StationID == stop.AuxStationID);
                TestJob testJob = testjobs.FirstOrDefault(m => m.TestJobID == stop.TestJobID);
                Job featuresFromJob = jobsForTestJobs.FirstOrDefault(m => m.JobID == testJob.JobID);
                JobType jobType = jobTypes.FirstOrDefault(m => m.JobTypeID == featuresFromJob.JobTypeID);
                string reason1 = reasons1.FirstOrDefault(m => m.Reason1ID == stop.Reason1).Description;
                string reason2 = stop.Reason2 == 0 ? "N/A" : reasons2.FirstOrDefault(m => m.Reason2ID == stop.Reason2).Description;
                string reason3 = stop.Reason3 == 0 ? "N/A" : reasons3.FirstOrDefault(m => m.Reason3ID == stop.Reason3).Description;
                string reason4 = stop.Reason4 == 0 ? "N/A" : reasons4.FirstOrDefault(m => m.Reason4ID == stop.Reason4).Description;
                string reason5 = stop.Reason5ID == 0 ? "N/A" : reasons5.FirstOrDefault(m => m.Reason5ID == stop.Reason5ID).Description;
                bool isStopFinished = stop.Reason5ID == 0 ? false : true;
                string elapsed = (stop.Elapsed.Day - 1).ToString() + ":" + stop.Elapsed.Hour.ToString() + ":" + stop.Elapsed.Minute.ToString() + ":" + stop.Elapsed.Second.ToString();

                StopsReport report = new StopsReport
                {
                    JobNumer = featuresFromJob.JobNum.Remove(0, 5),
                    PO = testJob.SinglePO,
                    JobTypeName = jobType.Name,
                    WeekNumber = stop.GetWeekOfYear,
                    StartDate = stop.StartDate.ToString(),
                    StopDate = stop.StopDate.ToString(),
                    Reason1 = reason1,
                    Reason2 = reason2,
                    Reason3 = reason3,
                    Reason4 = reason4,
                    Reason5 = reason5,
                    Description = stop.Description,
                    Critical = stop.Critical,
                    StationName = station.Label,
                    Elapsed = elapsed,
                    isFinished = isStopFinished,
                    TechFullName = technician
                };

                stopsReport.Add(report);
            }

            return stopsReport;
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

        public string ToClockDateString(DateTime time)
        {
            string elapsed = (time.Day - 1).ToString() + ":" + time.Hour.ToString() + ":" + time.Minute.ToString() + ":" + time.Second.ToString();

            return elapsed;
        }
    }
}
