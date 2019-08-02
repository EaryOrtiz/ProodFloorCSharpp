using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.TestJob;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,Technician,Enginner")]
    public class StopController : Controller
    {
        private IJobRepository jobRepo;
        private ITestingRepository testingRepo;
        private UserManager<AppUser> userManager;
        public int PageSize = 10;

        public StopController(ITestingRepository repo, IJobRepository repo2, UserManager<AppUser> userMgr)
        {
            jobRepo = repo2;
            testingRepo = repo;
            userManager = userMgr;
        }

        public ViewResult List(int page = 1)
            => View(new TestJobViewModel
            {
                StopList = testingRepo.Stops
                .OrderBy(p => p.StopID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                JobList = jobRepo.Jobs.ToList(),
                TestJobList = testingRepo.TestJobs.ToList(),
                Reasons1List = testingRepo.Reasons1.ToList(),
                Reasons2List = testingRepo.Reasons2.ToList(),
                Reasons3List = testingRepo.Reasons3.ToList(),
                Reasons4List = testingRepo.Reasons4.ToList(),
                Reasons5List = testingRepo.Reasons5.ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = testingRepo.Stops.Count()
                }
            });

        private async Task<AppUser> GetCurrentUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            return user;
        }

        public ViewResult NewStop(int ID)
        {
            return View(new Stop { TestJobID = ID });
        }

        [HttpPost]
        public ViewResult NewStop(Stop Stop)
        {
            bool admin = GetCurrentUserRole("Admin").Result;
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == Stop.TestJobID);
            Stop NewtStop = new Stop
            {
                TestJobID = Stop.TestJobID,
                Reason1 = Stop.Reason1,
                Reason2 = 0,
                Reason3 = 0,
                Reason4 = 0,
                Reason5ID = 0,
                Description = null,
                Critical = true,
                StartDate = DateTime.Now,
                StopDate = DateTime.Now,
                Elapsed = new DateTime(1, 1, 1, 0, 0, 0),
                AuxStationID = testJob.StationID,
                AuxTechnicianID = testJob.TechnicianID,
            };

            testingRepo.SaveStop(NewtStop);
            Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.StopID == testingRepo.Stops.Max(x => x.StopID));
            string Reason1Name = testingRepo.Reasons1.FirstOrDefault(m => m.Reason1ID == CurrentStop.Reason1).Description;
            testJob.Status = "Stoped";
            testingRepo.SaveTestJob(testJob);
            Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
            if (admin)
            {
                Stop CurrentStop2 = testingRepo.Stops.FirstOrDefault(p => p.StopID == CurrentStop.StopID);
                TestJob testJob2 = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStop.TestJobID);
                Job job2 = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
                return View("Edit", new TestJobViewModel { Job = job2, Stop = CurrentStop2, TestJob = testJob2 });
            }
            return View("WaitingForRestar", new TestJobViewModel { Job = job, Stop = CurrentStop, TestJob = testJob, Reason1Name = Reason1Name });
        }

        public ViewResult WaitingForRestar(int ID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);
            Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason2 == 0 && p.Critical == true);
            string Reason1Name = testingRepo.Reasons1.FirstOrDefault(m => m.Reason1ID == CurrentStop.Reason1).Description;
            Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
            return View("WaitingForRestar", new TestJobViewModel { Job = job, Stop = CurrentStop, Reason1Name = Reason1Name });
        }

        public IActionResult RestartReassignment(int ID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);
            Stop ReassignmentStop = testingRepo.Stops.FirstOrDefault(p => p.TestJobID == testJob.TestJobID && p.StopID == testingRepo.Stops.Max(x => x.StopID) && p.Reason1 == 980);
            Stop PreviusStop = testingRepo.Stops.FirstOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason2 == 0 && p.Critical == true);
            if (PreviusStop != null)
            {
                TimeSpan auxTime = (DateTime.Now - ReassignmentStop.StartDate);
                ReassignmentStop.Elapsed += auxTime;
                ReassignmentStop.StopDate = DateTime.Now;
                testingRepo.SaveStop(ReassignmentStop);


                PreviusStop.StartDate = DateTime.Now;
                PreviusStop.StopDate = DateTime.Now;
                testingRepo.SaveStop(PreviusStop);

                testJob.Status = "Stopped";
                testingRepo.SaveTestJob(testJob);
                return WaitingForRestar(testJob.TestJobID);
            }
            else
            {
                TimeSpan auxTime = (DateTime.Now - ReassignmentStop.StartDate);
                ReassignmentStop.Elapsed += auxTime;
                ReassignmentStop.StopDate = DateTime.Now;
                testingRepo.SaveStop(ReassignmentStop);

                testJob.Status = "Working on it";
                testingRepo.SaveTestJob(testJob);
                return ContinueStep(testJob.TestJobID);
            }
        }

        public ViewResult RestarTestJob(int ID, bool Critical = true)
        {
            Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.StopID == ID);
            if (Critical == true)
            {
                CurrentStop.Critical = true;
                testingRepo.SaveStop(CurrentStop);
                TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStop.TestJobID);
                Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
                return View(new TestJobViewModel { Job = job, Stop = CurrentStop, TestJob = testJob });
            }
            else
            {
                CurrentStop.Critical = false;
                testingRepo.SaveStop(CurrentStop);
                TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStop.TestJobID);
                testJob.Status = "Working on it";
                testingRepo.SaveTestJob(testJob);
                return ContinueStep(testJob.TestJobID);
            }

        }

        [HttpPost]
        public IActionResult RestarTestJob(TestJobViewModel viewModel)
        {
            TimeSpan auxTime = (DateTime.Now - viewModel.Stop.StartDate);
            viewModel.Stop.Elapsed += auxTime;
            Stop UpdatedStop = testingRepo.Stops.FirstOrDefault(m => m.StopID == viewModel.Stop.StopID);
            UpdatedStop.Reason1 = viewModel.Stop.Reason1;
            UpdatedStop.Reason2 = viewModel.Stop.Reason2;
            UpdatedStop.Reason3 = viewModel.Stop.Reason3;
            UpdatedStop.Reason4 = viewModel.Stop.Reason4;
            UpdatedStop.Reason5ID = viewModel.Stop.Reason5ID;
            UpdatedStop.Description = viewModel.Stop.Description;
            UpdatedStop.StopDate = DateTime.Now;
            testingRepo.SaveStop(UpdatedStop);
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == UpdatedStop.TestJobID);
            testJob.Status = "Working on it";
            testingRepo.SaveTestJob(testJob);

            return ContinueStep(testJob.TestJobID);
        }

        public ViewResult Edit(int ID)
        {
            Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.StopID == ID);
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStop.TestJobID);
            Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
            return View(new TestJobViewModel { Job = job, Stop = CurrentStop, TestJob = testJob });
        }

        [HttpPost]
        public IActionResult Edit(Stop stop)
        {
            if (ModelState.IsValid)
            {
                TimeSpan auxTime = (DateTime.Now - stop.StartDate);
                stop.Elapsed += auxTime;
                testingRepo.SaveStop(stop);
                TempData["message"] = $"{stop.Description} has been saved..";
                return RedirectToAction("SearchTestJob", "TestJob");
            }
            else
            {
                // there is something wrong with the data values
                return View(stop);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            Stop deletedStop = testingRepo.DeleteStop(ID);

            if (deletedStop != null)
            {
                TempData["message"] = $"{deletedStop.StopID} was deleted";
            }
            return RedirectToAction("SearchTestJob", "TestJob");
        }


        public ViewResult FinishPendingStops(int TestJobID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == TestJobID);
            Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.TestJobID == testJob.TestJobID);

            Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
            return View(new TestJobViewModel { Job = job, Stop = CurrentStop, TestJob = testJob });
        }

        [HttpPost]
        public IActionResult FinishPendingStops(TestJobViewModel testJobView)
        {
            TimeSpan auxTime = (DateTime.Now - testJobView.Stop.StartDate);
            testJobView.Stop.Elapsed += auxTime;
            testingRepo.SaveStop(testJobView.Stop);
            TempData["message"] = $"{testJobView.Stop.Description} has been saved..";

            Stop stop = testingRepo.Stops.FirstOrDefault(m => m.StopID != 980 & m.StopID != 981 && m.Reason2 == 0 && m.TestJobID == testJobView.Stop.TestJobID);
            if (stop != null)
            {
                return FinishPendingStops(testJobView.TestJob.TestJobID);
            }

            return RedirectToAction("JobCompletion", "TestJob", new { TestJobID = testJobView.TestJob.TestJobID });
            
        }

        private async Task<bool> GetCurrentUserRole(string role)
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            bool isInRole = await userManager.IsInRoleAsync(user, role);

            return isInRole;
        }


        [HttpPost]
        public FileStreamResult ExportToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            List<Stop> stops = testingRepo.Stops.ToList();

            using (XmlWriter xw = XmlWriter.Create(ms, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("Stops");

                foreach (Stop stop in stops)
                {
                    xw.WriteStartElement("Stop");

                    xw.WriteElementString("StopID", stop.StopID.ToString());
                    xw.WriteElementString("TestJobID", stop.TestJobID.ToString());
                    xw.WriteElementString("Reason1", stop.Reason1.ToString());
                    xw.WriteElementString("Reason2", stop.Reason2.ToString());
                    xw.WriteElementString("Reason3", stop.Reason3.ToString());
                    xw.WriteElementString("Reason4", stop.Reason4.ToString());
                    xw.WriteElementString("Reason5ID", stop.Reason5ID.ToString());
                    xw.WriteElementString("StartDate", stop.StartDate.ToString());
                    xw.WriteElementString("StopDate", stop.StopDate.ToString());
                    xw.WriteElementString("Elapsed", stop.Elapsed.ToString());
                    xw.WriteElementString("Description", stop.Description.ToString());
                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
            ms.Position = 0;
            return File(ms, "text/xml", "Stops.xml");
        }

        public static void ImportXML(IServiceProvider services)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();
            XmlDocument doc = new XmlDocument();
            doc.Load(@"C:\Users\eary.ortiz\Documents\GitHub\ProodFloorCSharpp\ProdFloor\wwwroot\AppData\Stops.xml");

            var XMLobs = doc.DocumentElement.SelectSingleNode("//Stops");

            var XMLStop = XMLobs.SelectNodes(".//Stop");

            if (XMLobs != null && context.Reasons5.Any() && !context.Stops.Any())
            {
                foreach(XmlElement stop in XMLStop)
                {
                    var StopID = stop.SelectSingleNode(".//StopID").InnerText;
                    var TestJobID = stop.SelectSingleNode(".//TestJobID").InnerText;
                    var Reason1 = stop.SelectSingleNode(".//Reason1").InnerText;
                    var Reason2 = stop.SelectSingleNode(".//Reason2").InnerText;
                    var Reason3 = stop.SelectSingleNode(".//Reason3").InnerText;
                    var Reason4 = stop.SelectSingleNode(".//Reason4").InnerText;
                    var Reason5ID = stop.SelectSingleNode(".//Reason5ID").InnerText;
                    var StartDate = stop.SelectSingleNode(".//StartDate").InnerText;
                    var StopDate = stop.SelectSingleNode(".//StopDate").InnerText;
                    var Elapsed = stop.SelectSingleNode(".//Elapsed").InnerText;
                    var Description = stop.SelectSingleNode(".//Description").InnerText;
                    context.Stops.Add(new Stop
                    {
                        StopID = Int32.Parse(StopID),
                        TestJobID = Int32.Parse(TestJobID),
                        Reason1 = Int32.Parse(Reason1),
                        Reason2 = Int32.Parse(Reason2),
                        Reason3 = Int32.Parse(Reason3),
                        Reason4 = Int32.Parse(Reason4),
                        Reason5ID = Int32.Parse(Reason5ID),
                        StartDate = DateTime.Parse(StartDate),
                        StopDate = DateTime.Parse(StopDate),
                        Elapsed = DateTime.Parse(Elapsed),
                        Description = Description,

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Stops ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Stops OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
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

        public ViewResult ContinueStep(int ID)
        {
            var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == ID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
            var AllStepsForJobInfo = testingRepo.Steps.Where(m => AllStepsForJob.Any(s => s.StepID == m.StepID)).ToList();

            List<Stop> StopsFromTestJob = testingRepo.Stops.Where(m => m.TestJobID == ID && m.Critical == false).ToList(); bool StopNC = false;
            if (StopsFromTestJob.Count > 0 && StopsFromTestJob[0] != null) StopNC = true;
            List<Reason1> reason1s = testingRepo.Reasons1.ToList();

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
    }
}