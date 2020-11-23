using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using ProdFloor.Models;

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

        public IActionResult Index()
        {
            return View();
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
    }
}
