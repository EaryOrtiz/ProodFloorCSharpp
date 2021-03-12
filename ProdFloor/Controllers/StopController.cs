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
    [Authorize(Roles = "Admin,TechAdmin,Technician,Enginner")]
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

        public IActionResult List(TestJobSearchViewModel searchViewModel, int page = 1)
        {
            if (searchViewModel.CleanFields) return RedirectToAction("List");
            if (searchViewModel.Stop == null) searchViewModel.Stop = new Stop();

            IQueryable<Stop> StopSearchList = testingRepo.Stops.Where(m => !string.IsNullOrEmpty(m.Description));
            if (searchViewModel.WithReassignment == false) StopSearchList = StopSearchList.Where(m => m.Reason1 != 980);
            if (searchViewModel.WithShiftEnd == false) StopSearchList = StopSearchList.Where(m => m.Reason1 != 981);
            if (searchViewModel.WithReturnedFromComplete == false) StopSearchList = StopSearchList.Where(m => m.Reason1 != 982);

            #region StopsInfo
            if (searchViewModel.Stop.Reason1 > 0)
            {
                StopSearchList = StopSearchList.Where(m => m.Reason1 == searchViewModel.Stop.Reason1);
                if (searchViewModel.Stop.Reason2 > 0)
                {
                    StopSearchList = StopSearchList.Where(m => m.Reason2 == searchViewModel.Stop.Reason2);
                    if (searchViewModel.Stop.Reason3 > 0)
                    {
                        StopSearchList = StopSearchList.Where(m => m.Reason3 == searchViewModel.Stop.Reason3);
                        if (searchViewModel.Stop.Reason4 > 0)
                        {
                            StopSearchList = StopSearchList.Where(m => m.Reason4 == searchViewModel.Stop.Reason2);
                            if (searchViewModel.Stop.Reason5ID > 0)
                            {
                                StopSearchList = StopSearchList.Where(m => m.Reason5ID == searchViewModel.Stop.Reason5ID);
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(searchViewModel.Stop.Description)) StopSearchList = StopSearchList.Where(m => m.Description.Contains(searchViewModel.Stop.Description));
            if (!string.IsNullOrEmpty(searchViewModel.Critical)) StopSearchList = StopSearchList.Where(m => searchViewModel.Critical == "Si" ? m.Critical == true : m.Critical == false);
            #endregion


            searchViewModel.StopList = StopSearchList.OrderBy(p => p.StopID).Skip((page - 1) * 10).Take(10).ToList();
            searchViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 10,
                TotalItems = StopSearchList.Count()
            };
            searchViewModel.TestJobsSearchList = testingRepo.TestJobs.ToList();
            searchViewModel.JobsSearchList = jobRepo.Jobs.ToList();
            searchViewModel.Reasons1List = testingRepo.Reasons1.ToList();
            searchViewModel.Reasons2List = testingRepo.Reasons2.ToList();
            searchViewModel.Reasons3List = testingRepo.Reasons3.ToList();
            searchViewModel.Reasons4List = testingRepo.Reasons4.ToList();
            searchViewModel.Reasons5List = testingRepo.Reasons5.ToList();
            return View(searchViewModel);
        }

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
            if (Stop.Reason1 != 0)
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

                /**Esto es para el actual step*/
                var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJob.TestJobID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
                StepsForJob actualStepForAUX = AllStepsForJob.FirstOrDefault(m => m.Complete == false);
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
                /**Esto es para el actual step*/

                testingRepo.SaveStop(NewtStop);
                Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.StopID == testingRepo.Stops.Max(x => x.StopID));
                string Reason1Name = testingRepo.Reasons1.FirstOrDefault(m => m.Reason1ID == CurrentStop.Reason1).Description;
                testJob.Status = "Stopped";
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
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, seleccione una razonn valida";
                return View("NewStop", Stop);
            }

        }

        public IActionResult WaitingForRestar(int ID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);

            if (testJob != null)
            {
                AppUser currentUser = GetCurrentUser().Result;
                bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool isAdmin = GetCurrentUserRole("Admin").Result;
                bool isSameEngineer = currentUser.EngID == testJob.TechnicianID;
                bool isNotCompleted = testJob.Status != "Completed";

                if (isNotCompleted && (isSameEngineer || isAdmin || isTechAdmin))
                {
                    TestJob OnGoingtestJob = testingRepo.TestJobs.FirstOrDefault(m => m.TechnicianID == testJob.TechnicianID && m.Status == "Working on it");
                    Stop ReturnedFromCompleteStop = testingRepo.Stops.LastOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason1 == 982 && p.Reason2 == 0);
                    if (OnGoingtestJob != null)
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Error, Tiene un trabajo activo, intente de nuevo o contacte al admin";
                        return RedirectToAction("Index", "Home");
                    }
                    else if (ReturnedFromCompleteStop != null)
                    {
                        ReturnedFromCompleteStop.StopDate = DateTime.Now;
                        ReturnedFromCompleteStop.Elapsed += GetElapsed(ReturnedFromCompleteStop.StartDate, ReturnedFromCompleteStop.StopDate);
                        testingRepo.SaveStop(ReturnedFromCompleteStop);

                        testJob.Status = "Working on it";
                        testingRepo.SaveTestJob(testJob);
                        return ContinueStep(testJob.TestJobID);
                    }
                    Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason2 == 0 && p.Critical == true);
                    string Reason1Name = testingRepo.Reasons1.FirstOrDefault(m => m.Reason1ID == CurrentStop.Reason1).Description;
                    Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
                    return View("WaitingForRestar", new TestJobViewModel { Job = job, Stop = CurrentStop, Reason1Name = Reason1Name });
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

        public IActionResult RestartReassignment(int ID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);

            if (testJob != null)
            {
                AppUser currentUser = GetCurrentUser().Result;
                bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool isAdmin = GetCurrentUserRole("Admin").Result;
                bool isSameEngineer = currentUser.EngID == testJob.TechnicianID;
                bool isNotCompleted = testJob.Status != "Completed";

                if (isNotCompleted && (isSameEngineer || isAdmin || isTechAdmin))
                {
                    List<Stop> Stops = new List<Stop>();
                    TestJob OnGoingtestJob = testingRepo.TestJobs.FirstOrDefault(m => m.TechnicianID == testJob.TechnicianID && m.Status == "Working on it");
                    Stop ReassignmentStop = testingRepo.Stops.LastOrDefault(p => p.TestJobID == testJob.TestJobID && p.Reason1 == 980 && p.Reason2 == 0 && p.Reason3 == 0);

                    if (OnGoingtestJob != null)
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Error, Tiene un trabajo activo, intente de nuevo o contacte al admin";
                        return RedirectToAction("Index", "Home");
                    }

                    ReassignmentStop.StopDate = DateTime.Now;
                    ReassignmentStop.Elapsed += GetElapsed(ReassignmentStop.StartDate, ReassignmentStop.StopDate);
                    ReassignmentStop.Reason2 = 980;
                    ReassignmentStop.Reason3 = 980;
                    ReassignmentStop.Reason4 = 980;
                    ReassignmentStop.Reason5ID = 980;
                    testingRepo.SaveStop(ReassignmentStop);

                    Stops = testingRepo.Stops.Where(p => testJob.TestJobID == p.TestJobID && p.StopID != 982 && p.StopID != 981 && p.Reason2 == 0).ToList();

                    if (Stops.Count > 0)
                    {
                        foreach(Stop stop in Stops)
                        {
                            stop.StartDate = DateTime.Now;
                            stop.StopDate = DateTime.Now;
                            testingRepo.SaveStop(stop);
                        }

                        if(Stops.Any(m => m.Critical == true))
                        {
                            testJob.Status = "Stopped";
                            testingRepo.SaveTestJob(testJob);
                            return WaitingForRestar(testJob.TestJobID);
                        }

                    }


                    var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJob.JobID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
                    StepsForJob CurrentStep = AllStepsForJob.FirstOrDefault(m => m.Complete == false);
                    CurrentStep.Start = DateTime.Now;
                    CurrentStep.Stop = DateTime.Now;
                    testingRepo.SaveStepsForJob(CurrentStep);

                    testJob.Status = "Working on it";
                    testingRepo.SaveTestJob(testJob);
                    return ContinueStep(testJob.TestJobID);


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

        public IActionResult RestarTestJob(int ID, bool Critical = true)
        {
            Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.StopID == ID);
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStop.TestJobID);

            AppUser currentUser = GetCurrentUser().Result;
            bool isSameEngineer = currentUser.EngID == testJob.TechnicianID;
            bool isNotCompleted = testJob.Status != "Completed";
            bool isNotOnReassigment = testJob.Status != "Reassignment";
            bool isNotOnShiftEnd = testJob.Status != "Shift End";
            bool isOnWorkingOnIt = testJob.Status == "Working on it";

            if (isNotOnShiftEnd && isNotOnReassigment && isNotCompleted && isSameEngineer)
            {
                if (Critical == true)
                {
                    CurrentStop.Critical = true;
                    testingRepo.SaveStop(CurrentStop);
                    Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
                    return View(new TestJobViewModel { Job = job, Stop = CurrentStop, TestJob = testJob });
                }
                else
                {
                    CurrentStop.Critical = false;
                    testingRepo.SaveStop(CurrentStop);
                    testJob.Status = "Working on it";
                    testingRepo.SaveTestJob(testJob);
                    return ContinueStep(testJob.TestJobID);
                }
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                if (isNotCompleted == false) TempData["message"] = $"Error, El Testjob ya ha sido completado, intente de nuevo o contacte al Admin";
                else if (!isNotOnShiftEnd) TempData["message"] = $"Error, El Testjob esta en shift end, pilse el boton de continuar";
                else TempData["message"] = $"Error, El Testjob a sido reasignado, intente de nuevo o contacte al Admin";

                return RedirectToAction("Index", "Home");
            }

            

        }

        [HttpPost]
        public IActionResult RestarTestJob(TestJobViewModel viewModel)
        {

            Stop UpdatedStop = testingRepo.Stops.FirstOrDefault(m => m.StopID == viewModel.Stop.StopID);
            UpdatedStop.StopDate = DateTime.Now;
            UpdatedStop.Elapsed += GetElapsed(UpdatedStop.StartDate, UpdatedStop.StopDate);
            UpdatedStop.Reason1 = viewModel.Stop.Reason1;
            UpdatedStop.Reason2 = viewModel.Stop.Reason2;
            UpdatedStop.Reason3 = viewModel.Stop.Reason3;
            UpdatedStop.Reason4 = viewModel.Stop.Reason4;
            UpdatedStop.Reason5ID = viewModel.Stop.Reason5ID;
            UpdatedStop.Description = viewModel.Stop.Description;

            testingRepo.SaveStop(UpdatedStop);
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == UpdatedStop.TestJobID);

            var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJob.TestJobID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
            StepsForJob CurrentStep = AllStepsForJob.FirstOrDefault(m => m.Complete == false);
            CurrentStep.Start = DateTime.Now;
            CurrentStep.Stop = DateTime.Now;
            testingRepo.SaveStepsForJob(CurrentStep);


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


        public IActionResult FinishPendingStops(int TestJobID)
        {

            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == TestJobID);
            List<Stop> stops = testingRepo.Stops.Where(p => testJob.TestJobID == p.TestJobID && p.Reason1 != 980 && p.Reason1 != 981 && p.Reason2 == 0).ToList();

            if (testJob != null && stops.Count > 0)
            {

                bool isTechAdmin = GetCurrentUserRole("TechAdmin").Result;
                bool isAdmin = GetCurrentUserRole("Admin").Result;
                bool isNotCompleted = testJob.Status != "Completed";

                if (isNotCompleted && (isAdmin || isTechAdmin))
                {
                    Stop CurrentStop = testingRepo.Stops.Where(m => m.Reason1 != 980 & m.Reason1 != 981 && m.Reason2 == 0)
                                                        .FirstOrDefault(p => p.TestJobID == testJob.TestJobID);

                    Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
                    return View(new TestJobViewModel { Job = job, Stop = CurrentStop, TestJob = testJob });

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
                if (stops.Count == 0)
                    return RedirectToAction("JobCompletion", "TestJob", new { TestJobID = testJob.TestJobID });
                else
                    TempData["message"] = $"Error, El Testjob no existe o a sido eliminado, intente de nuevo o contacte al Admin";

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult FinishPendingStops(TestJobViewModel testJobView)
        {
            Stop UpdatedStop = testingRepo.Stops.FirstOrDefault(m => m.StopID == testJobView.Stop.StopID);
            UpdatedStop.StopDate = DateTime.Now;
            UpdatedStop.Elapsed += GetElapsed(UpdatedStop.StartDate, UpdatedStop.StopDate);
            UpdatedStop.Reason1 = testJobView.Stop.Reason1;
            UpdatedStop.Reason2 = testJobView.Stop.Reason2;
            UpdatedStop.Reason3 = testJobView.Stop.Reason3;
            UpdatedStop.Reason4 = testJobView.Stop.Reason4;
            UpdatedStop.Reason5ID = testJobView.Stop.Reason5ID;
            UpdatedStop.Description = testJobView.Stop.Description;

            testingRepo.SaveStop(UpdatedStop);
            TempData["message"] = $"{testJobView.Stop.Description} has been saved..";

            Stop stop = testingRepo.Stops.FirstOrDefault(m => m.Reason1 != 980 & m.Reason1 != 981 && m.Reason2 == 0 && m.TestJobID == testJobView.Stop.TestJobID);

            if (stop != null)
            {
                return RedirectToAction("FinishPendingStops", "Stop", new { TestJobID = testJobView.TestJob.TestJobID }); ;
            }

            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == UpdatedStop.TestJobID);

            var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJob.TestJobID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
            StepsForJob CurrentStep = AllStepsForJob.FirstOrDefault(m => m.Complete == false);
            CurrentStep.Start = DateTime.Now;
            CurrentStep.Stop = DateTime.Now;
            testingRepo.SaveStepsForJob(CurrentStep);

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

            List<Stop> stops = testingRepo.Stops.Where(m => m.Reason5ID != 0).ToList();

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
                    xw.WriteElementString("Description", stop.Description != null ? stop.Description : "Nulo");
                    xw.WriteElementString("AuxTechnicianID", stop.AuxTechnicianID.ToString());
                    xw.WriteElementString("AuxStationID", stop.AuxStationID.ToString());
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
            doc.Load(@"C:\ProdFloorNew90\wwwroot\AppData\Stops.xml");

            var XMLobs = doc.DocumentElement.SelectSingleNode("//Stops");

            var XMLStop = XMLobs.SelectNodes(".//Stop");

            if (XMLobs != null && context.Reasons5.Any() && !context.Stops.Any())
            {
                foreach (XmlElement stop in XMLStop)
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
                    var AuxTechnicianID = stop.SelectSingleNode(".//AuxTechnicianID").InnerText;
                    var AuxStationID = stop.SelectSingleNode(".//AuxStationID").InnerText;
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
                        AuxStationID = Int32.Parse(AuxTechnicianID),
                        AuxTechnicianID = Int32.Parse(AuxStationID),

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

        public ViewResult ContinueStep(int ID)
        {
            var AllStepsForJob = testingRepo.StepsForJobs.Where(m => m.TestJobID == ID && m.Obsolete == false).OrderBy(m => m.Consecutivo).ToList();
            var AllStepsForJobInfo = testingRepo.Steps.Where(m => AllStepsForJob.Any(s => s.StepID == m.StepID)).ToList();

            List<Stop> StopsFromTestJob = testingRepo.Stops.Where(m => m.TestJobID == ID && m.Critical == false)
                                                           .Where(m => m.StopID != 980 & m.StopID != 981 && m.Reason2 == 0).ToList();

            bool StopNC = false;
            if (StopsFromTestJob.Count > 0 && StopsFromTestJob[0] != null) StopNC = true;


            List<Reason1> reason1s = testingRepo.Reasons1.ToList();

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

        public TimeSpan GetElapsed(DateTime startDate, DateTime endDate)
        {
            double endDateHours = ToHours(endDate);
            double startDateHours = ToHours(startDate);
            double extraHours = 0;

            if(endDate.Day > startDate.Day)
            {
                int extraDays = endDate.Day - startDate.Day;
                extraHours = extraDays * 24;
            }

            double elapsedTime = endDateHours - startDateHours + extraHours;

            DateTime totalDateTime  = ToDateTime(elapsedTime);
            int auxDay = 0;
            if(totalDateTime.Day > 1)
            {
               auxDay = totalDateTime.Day - 1;
            }

            return new TimeSpan(auxDay, totalDateTime.Hour, totalDateTime.Minute, totalDateTime.Second);
        }

        public async Task<IActionResult> StopSearchList(TestJobSearchViewModel searchViewModel, int page = 1, int totalitemsfromlastsearch = 0)
        {
            if (searchViewModel.CleanFields) return RedirectToAction("StopSearchList");
            if (searchViewModel.Stop == null) searchViewModel.Stop = new Stop();

            IQueryable<Stop> StopSearchList = testingRepo.Stops.Where(m => m.Reason1 != 981 && m.Reason1 != 980 & !string.IsNullOrEmpty(m.Description));

            #region StopsInfo
            if (searchViewModel.Stop.Reason1 > 0)
            {
                StopSearchList = StopSearchList.Where(m => m.Reason1 == searchViewModel.Stop.Reason1);
                if (searchViewModel.Stop.Reason2 > 0)
                {
                    StopSearchList = StopSearchList.Where(m => m.Reason2 == searchViewModel.Stop.Reason2);
                    if (searchViewModel.Stop.Reason3 > 0)
                    {
                        StopSearchList = StopSearchList.Where(m => m.Reason3 == searchViewModel.Stop.Reason3);
                        if (searchViewModel.Stop.Reason4 > 0)
                        {
                            StopSearchList = StopSearchList.Where(m => m.Reason4 == searchViewModel.Stop.Reason2);
                            if (searchViewModel.Stop.Reason5ID > 0)
                            {
                                StopSearchList = StopSearchList.Where(m => m.Reason5ID == searchViewModel.Stop.Reason5ID);
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(searchViewModel.Stop.Description)) StopSearchList = StopSearchList.Where(m => m.Description.Contains(searchViewModel.Stop.Description));
            if (!string.IsNullOrEmpty(searchViewModel.Critical)) StopSearchList = StopSearchList.Where(m => searchViewModel.Critical == "Si" ? m.Critical == true : m.Critical == false);
            #endregion

            int TotalItemsSearch = StopSearchList.Count();
            if (page == 1)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
            }
            else if (TotalItemsSearch != totalitemsfromlastsearch)
            {
                totalitemsfromlastsearch = TotalItemsSearch;
                page = 1;
            }
            searchViewModel.StopList = StopSearchList.OrderBy(p => p.StopID).Skip((page - 1) * 10).Take(10).ToList();
            searchViewModel.PagingInfo = new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = 10,
                TotalItemsFromLastSearch = totalitemsfromlastsearch,
                TotalItems = StopSearchList.Count()
            };

            return View(searchViewModel);
        }
    }
}