using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.Wiring;

namespace ProdFloor.Controllers
{
    public class WiringPXPController : Controller
    {
        private IJobRepository jobRepo;
        private IWiringRepository wiringRepo;
        private IItemRepository itemRepo;
        private UserManager<AppUser> userManager;
        private ITestingRepository testingRepo;
        private IHostingEnvironment _env;
        public int PageSize = 7;
        string appDataFolder => _env.WebRootPath.ToString() + @"\AppData\";

        public WiringPXPController(IWiringRepository repo,
            IJobRepository repo2,
            IItemRepository repo3,
            ITestingRepository repo4,
            UserManager<AppUser> userMgr,
            IHostingEnvironment env)
        {
            jobRepo = repo2;
            wiringRepo = repo;
            itemRepo = repo3;
            testingRepo = repo4;
            userManager = userMgr;
            _env = env;
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

        public IActionResult PXPProductionDashboard(string filtrado, string Sort = "default", int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            bool productionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            bool admin = GetCurrentUserRole("Admin").Result;
            if (filtrado != null) Sort = filtrado;
            if (productionAdmin || admin)
            {

                List<JobType> JobTyPeList = itemRepo.JobTypes.ToList();

                List<PO> AllPOsList = new List<PO>();


                List<StatusPO> MyStatusPOList = jobRepo.StatusPOs
                    .Where(s => s.Status == "PXP on progress")
                    .ToList();


                List<PO> OnProgressPOsList = jobRepo.POs.Where(m => MyStatusPOList.Any(n => n.POID == m.POID))
                                                                       .ToList();

                List<Job> MyjobsList = jobRepo.Jobs.Where(m => OnProgressPOsList.Any(n => n.JobID == m.JobID))
                                                    .OrderBy(n => n.LatestFinishDate).ToList();


                List<WiringPXP> MyWiringPXPList = wiringRepo.WiringPXPs.Where(m => MyStatusPOList.Any(n => n.POID == m.POID))
                                                                       .ToList();

                AllPOsList.AddRange(OnProgressPOsList);

                List<StatusPO> OnCrossStatusPOList = jobRepo.StatusPOs
                        .Where(j => j.Status == "Waiting for test")
                        .ToList();

                List<PO> TestQueAllPOsList = jobRepo.POs.Where(m => OnCrossStatusPOList.Any(n => n.POID == m.POID))
                                                                       .ToList();

                List<Job> OnCrossJobsList = jobRepo.Jobs.Where(m => TestQueAllPOsList.Any(n => n.JobID == m.JobID))
                                                        .OrderBy(n => n.LatestFinishDate).ToList();

                List<WiringPXP> OnCrossWiringPXPList = wiringRepo.WiringPXPs.Where(m => OnCrossStatusPOList.Any(n => n.POID == m.POID))
                                                                       .ToList();
                AllPOsList.AddRange(TestQueAllPOsList);

                List<Job> AllJobsList = MyjobsList;
                AllJobsList.AddRange(OnCrossJobsList);

                DashboardIndexViewModel dashboard = new DashboardIndexViewModel()
                {
                    MyWiringPXPs = MyWiringPXPList.Skip((MyJobsPage - 1) * 6).Take(6),
                    MyWiringPXPsPagingInfo = new PagingInfo
                    {
                        CurrentPage = MyJobsPage,
                        ItemsPerPage = 6,
                        TotalItems = MyWiringPXPList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },
                    JobTypes = JobTyPeList,
                    POs = AllPOsList,
                    PendingToCrossWiringPXPs = wiringRepo.WiringPXPs.ToList(),
                    MyJobs = AllJobsList,
                    OnCrossWiringPXPS = OnCrossWiringPXPList.Skip((OnCrossJobPage - 1) * 6).Take(6),
                    OnCrossWiringPXPsPagingInfo = new PagingInfo
                    {
                        CurrentPage = OnCrossJobPage,
                        ItemsPerPage = 6,
                        TotalItems = OnCrossWiringPXPList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"
                    },

                    PendingToCrossJobs = new List<Job>(),
                    PendingToCrossJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = PendingToCrossJobPage,
                        ItemsPerPage = 6,
                        TotalItems = 0,
                        sort = Sort != "default" ? Sort : "deafult"
                    },
                    StationList = testingRepo.Stations.ToList(),
                    JobTypesList = itemRepo.JobTypes.ToList(),
                };

                return View(dashboard);
            }

            return NotFound();
        }

        public IActionResult PXPDashboard(int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;

            List<StatusPO> MyStatusPOList = jobRepo.StatusPOs
                 .Where(s => s.Status == "PXP on progress")
                 .ToList();


            List<PO> OnProgressPOsList = jobRepo.POs.Where(m => MyStatusPOList.Any(n => n.POID == m.POID))
                                                                   .ToList();

            List<Job> MyjobsList = jobRepo.Jobs.Where(m => OnProgressPOsList.Any(n => n.JobID == m.JobID))
                                                .OrderBy(n => n.LatestFinishDate).ToList();


            List<WiringPXP> MyWiringPXPList = wiringRepo.WiringPXPs.Where(m => MyStatusPOList.Any(n => n.POID == m.POID))
                                                                   .ToList();

            var CurrentWiringPXPs = MyWiringPXPList
                   .Where(j => j.WirerPXPID == currentUser.EngID)
                   .OrderBy(p => p.WirerPXPID).ToList();

            List<Job> DummyOnCrossJobsList = new List<Job>();

            List<Job> DummyPendingToCrossJobList = new List<Job>();

            return View(new DashboardIndexViewModel
            {
                MyWiringPXPs = CurrentWiringPXPs.Skip((MyJobsPage - 1) * 5).Take(5),
                MyJobsPagingInfo = new PagingInfo
                {
                    CurrentPage = MyJobsPage,
                    ItemsPerPage = 5,
                    TotalItems = CurrentWiringPXPs.Count(),
                },


                MyJobs = MyjobsList,
                JobTypesList = itemRepo.JobTypes.ToList(),
                OnCrossJobs = DummyOnCrossJobsList,
                StationList = testingRepo.Stations.ToList(),
                OnCrossJobsPagingInfo = new PagingInfo
                {
                    CurrentPage = OnCrossJobPage,
                    ItemsPerPage = PageSize,
                    TotalItems = DummyOnCrossJobsList.Count(),
                    sort = "deafult"
                },

                PendingToCrossJobs = DummyPendingToCrossJobList.Skip((PendingToCrossJobPage - 1) * PageSize).Take(PageSize),
                PendingToCrossJobsPagingInfo = new PagingInfo
                {
                    CurrentPage = PendingToCrossJobPage,
                    ItemsPerPage = PageSize,
                    TotalItems = DummyPendingToCrossJobList.Count(),
                    sort = "deafult"
                },
            });
        }

        public IActionResult NewWiringPXP(int PONumb)
        {
            WiringPXPViewModel viewModel = new WiringPXPViewModel();
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

            WiringPXP wiringPXP = wiringRepo.WiringPXPs.FirstOrDefault(m => m.SinglePO == PONumb);
            if (wiringPXP == null)
            {
                viewModel.wiringPXP = new WiringPXP();
                viewModel.wiringPXP.SinglePO = PONumb;
                viewModel.pXPErrorList = new List<PXPError>();
            }
            else if (currentUser.EngID != wiringPXP.WirerPXPID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP a sido reasigando";

                return RedirectToAction("PXPDashboard");
            }
            else if (job.Status == "Waiting for test")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP con PO #{wiringPXP.SinglePO} ya esta terminado";

                return RedirectToAction("PXPDashboard");
            }
            else
            {
                viewModel.wiringPXP = wiringPXP;
                viewModel.pXPErrorList = wiringRepo.PXPErrors.Where(m => m.WiringPXPID == wiringPXP.WiringPXPID)
                                                             .ToList();
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult SaveWiringPXP(WiringPXPViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;


            WiringPXP wiring = new WiringPXP()
            {
                POID = viewModel.PO.POID,
                StationID = viewModel.wiringPXP.StationID,
                WirerPXPID = currentUser.EngID,
                SinglePO = viewModel.wiringPXP.SinglePO,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now,
            };

            wiringRepo.SaveWiringPXP(wiring);

            WiringPXP wiringPXP = wiringRepo.WiringPXPs.FirstOrDefault(m => m.SinglePO == viewModel.wiringPXP.SinglePO);


            WirersPXPInvolved involved = wiringRepo.WirersPXPInvolveds
                                                  .Where(m => m.WiringPXPID == wiringPXP.WiringPXPID)
                                                  .FirstOrDefault(m => m.WirerPXPID == currentUser.EngID);
            if (involved == null)
            {
                WirersPXPInvolved wirersInvolved = new WirersPXPInvolved();
                wirersInvolved.WiringPXPID = wiringPXP.WiringPXPID;
                wirersInvolved.WirerPXPID = currentUser.EngID;
                wiringRepo.SaveWirersPXPInvolved(wirersInvolved);
            }


            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiringPXP.POID);

            statusPO.Status = "PXP on progress";
            jobRepo.SaveStatusPO(statusPO);


            return RedirectToAction("NewWiringPXP", "WiringPXP", new { PONumb = wiringPXP.SinglePO });
        }

        public IActionResult NewPXPError(int ID)
        {
            AppUser currentUser = GetCurrentUser().Result;
            WiringPXP wiringPXP = wiringRepo.WiringPXPs.FirstOrDefault(m => m.WiringPXPID == ID);

            if (wiringPXP == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP no existe";

                return RedirectToAction("PXPDashboard");
            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiringPXP.POID);

            if (currentUser.EngID != wiringPXP.WirerPXPID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP a sido reasigando";

                return RedirectToAction("PXPDashboard");
            }
            else if (statusPO.Status == "Waiting for test")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP con PO #{wiringPXP.SinglePO} ya esta terminado";

                return RedirectToAction("PXPDashboard");
            }

            WiringPXPViewModel viewModel = new WiringPXPViewModel();

            viewModel.wiringPXP = wiringPXP;
            viewModel.pXPError = new PXPError();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddPXPError(WiringPXPViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            WiringPXP wiringPXP = wiringRepo.WiringPXPs.FirstOrDefault(m => m.WiringPXPID == viewModel.wiringPXP.WiringPXPID);

            if (wiringPXP == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP no existe";

                return RedirectToAction("PXPDashboard");
            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiringPXP.POID);

            if (currentUser.EngID != wiringPXP.WirerPXPID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP a sido reasigando";

                return RedirectToAction("PXPDashboard");
            }
            else if (statusPO.Status == "Waiting for test")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP con PO #{wiringPXP.SinglePO} ya esta terminado";

                return RedirectToAction("PXPDashboard");
            }

            viewModel.pXPError.WiringPXPID = wiringPXP.WiringPXPID;
            wiringRepo.SavePXPError(viewModel.pXPError);

            WirersPXPInvolved involved = wiringRepo.WirersPXPInvolveds
                                                 .Where(m => m.WiringPXPID == wiringPXP.WiringPXPID)
                                                 .FirstOrDefault(m => m.WirerPXPID == currentUser.EngID);
            if (involved == null)
            {
                WirersPXPInvolved wirersInvolved = new WirersPXPInvolved();
                wirersInvolved.WiringPXPID = wiringPXP.WiringPXPID;
                wirersInvolved.WirerPXPID = currentUser.EngID;
                wiringRepo.SaveWirersPXPInvolved(wirersInvolved);
            }

            TempData["message"] = $"Nuevo error añadido con exito.";

            return RedirectToAction("NewWiringPXP", "WiringPXP", new { PONumb = wiringPXP.SinglePO });
        }


        public IActionResult EditPXPError(int ID)
        {
            PXPError pXPError = wiringRepo.PXPErrors.FirstOrDefault(m => m.WiringPXPID == ID);
            WiringPXP wiringPXP = wiringRepo.WiringPXPs.FirstOrDefault(m => m.WiringPXPID == pXPError.WiringPXPID);

            WiringPXPViewModel viewModel = new WiringPXPViewModel()
            {
                pXPError = pXPError,
                wiringPXP = wiringPXP,
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditPXPError(WiringPXPViewModel viewModel)
        {
            wiringRepo.SavePXPError(viewModel.pXPError);

            TempData["message"] = $"PXPError actualizado con exito.";

            return RedirectToAction("NewWiringPXP", "WiringPXP", new { PONumb = viewModel.wiringPXP.SinglePO });
        }

        [HttpPost]
        public IActionResult DeletePXPError(int ID)
        {
            AppUser currentUser = GetCurrentUser().Result;
            
            
            PXPError pXPError = wiringRepo.PXPErrors.FirstOrDefault(m => m.PXPErrorID == ID);
            WiringPXP wiringPXP = wiringRepo.WiringPXPs.FirstOrDefault(m => m.WiringPXPID == pXPError.WiringPXPID);

            if (wiringPXP == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP no existe";

                return RedirectToAction("PXPDashboard");
            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiringPXP.POID);
            if (currentUser.EngID != wiringPXP.WirerPXPID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP a sido reasigando";

                return RedirectToAction("PXPDashboard");
            }
            else if (statusPO.Status == "Waiting for test")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP con PO #{wiringPXP.SinglePO} ya esta terminado";

                return RedirectToAction("PXPDashboard");
            }

            PXPError deletedError = wiringRepo.DeletePXPError(ID);
            PXPReason reasonPXP = wiringRepo.PXPReasons.FirstOrDefault(m => m.PXPReasonID == deletedError.PXPReasonID);
            if (deletedError != null)
            {
                TempData["message"] = $"{reasonPXP.Description} was deleted";
            }

            return RedirectToAction("NewWiringPXP", "WiringPXP", new { PONumb = wiringPXP.SinglePO });
        }

        [HttpPost]
        public IActionResult DeleteWiringPXP(int ID)
        {
            WiringPXP wiringPXP = wiringRepo.WiringPXPs.FirstOrDefault(m => m.WiringPXPID == ID);
            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiringPXP.POID);

            if (statusPO.Status == "Waiting for test")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP con PO #{wiringPXP.SinglePO} ya esta terminado";

                 return RedirectToAction("PXPProductionDashboard");
            }

            WiringPXP deletedPXP = wiringRepo.DeleteWiringPXP(ID);

            if (deletedPXP != null)
            {
                TempData["message"] = $"PXP with PO#{deletedPXP.SinglePO} was deleted";
            }

            return RedirectToAction("PXPProductionDashboard");
        }

        [HttpPost]
        public IActionResult Reassignment(DashboardIndexViewModel viewModel)
        {
            WiringPXP wiringPXP = wiringRepo.WiringPXPs.FirstOrDefault(m => m.WiringPXPID == viewModel.WiringPXP.WiringPXPID);
            
            bool ProductionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            bool Admin = GetCurrentUserRole("Admin").Result;

            if (wiringPXP == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP ah sido completado";

                 return RedirectToAction("PXPProductionDashboard");


            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiringPXP.POID);
            if (statusPO.Status == "Waiting for test")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP con PO #{wiringPXP.SinglePO} ya esta terminado";

                    return RedirectToAction("PXPProductionDashboard");
            }

            if (!ProductionAdmin && !Admin)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You cant reassign";
                return RedirectToAction("PXPDashboard");
            }


            if (wiringPXP.WirerPXPID == viewModel.WiringPXP.WirerPXPID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You cannot reassing the WirerPXP because is the same who owns the PXP";
                return RedirectToAction("PXPProductionDashboard");

            }

            wiringPXP.WirerPXPID = viewModel.WiringPXP.WirerPXPID;
            wiringRepo.SaveWiringPXP(wiringPXP);

            WirersPXPInvolved involved = wiringRepo.WirersPXPInvolveds
                                                    .Where(m => m.WiringPXPID == viewModel.WiringPXP.WiringPXPID)
                                                    .FirstOrDefault(m => m.WirerPXPID == viewModel.WiringPXP.WirerPXPID);
            if (involved == null)
            {
                WirersPXPInvolved wirersInvolved = new WirersPXPInvolved();
                wirersInvolved.WiringPXPID = wiringPXP.WiringPXPID;
                wirersInvolved.WirerPXPID = viewModel.WiringPXP.WirerPXPID;
                wiringRepo.SaveWirersPXPInvolved(wirersInvolved);
            }

            AppUser user = userManager.Users.FirstOrDefault(m => m.EngID == wiringPXP.WirerPXPID);

            TempData["message"] = $"You have reassigned the WirerPXP for the Job with PO #{wiringPXP.SinglePO} to {user.FullName}";
            return RedirectToAction("PXPProductionDashboard");

        }

        public IActionResult EndWiringPXP(int ID)
        {

            AppUser currentUser = GetCurrentUser().Result;
            bool ProdctionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            WiringPXP wiringPXP = wiringRepo.WiringPXPs.FirstOrDefault(m => m.WiringPXPID == ID);

            if (wiringPXP == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP no existe";

                if (ProdctionAdmin)
                    return RedirectToAction("PXPProductionDashboard");

                return RedirectToAction("PXPDashboard");


            }

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiringPXP.POID);
            if (currentUser.EngID != wiringPXP.WirerPXPID && !ProdctionAdmin)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP a sido reasigando";

                return RedirectToAction("PXPDashboard");

            }else if (statusPO.Status == "Waiting for test")
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PXP con PO #{wiringPXP.SinglePO} ya esta terminado";

                if (ProdctionAdmin)
                    return RedirectToAction("PXPProductionDashboard");

                return RedirectToAction("PXPDashboard");
            }


            wiringPXP.EndDate = DateTime.Now;
            wiringRepo.SaveWiringPXP(wiringPXP);

            statusPO.Status = "Waiting for test";
            jobRepo.SaveStatusPO(statusPO);

            TempData["message"] = $"PXP del PO #" + wiringPXP.SinglePO + " finalizado";

            if (ProdctionAdmin)
                return RedirectToAction("PXPProductionDashboard");


            WirersPXPInvolved involved = wiringRepo.WirersPXPInvolveds
                                                   .Where(m => m.WiringPXPID == wiringPXP.WiringPXPID)
                                                   .FirstOrDefault(m => m.WirerPXPID == wiringPXP.WirerPXPID);
            if (involved == null)
            {
                WirersPXPInvolved wirersInvolved = new WirersPXPInvolved();
                wirersInvolved.WiringPXPID = wiringPXP.WiringPXPID;
                wirersInvolved.WirerPXPID = wiringPXP.WirerPXPID;
                wiringRepo.SaveWirersPXPInvolved(wirersInvolved);
            }

            return RedirectToAction("PXPDashboard");
        }

        [HttpPost]
        public IActionResult ReturnFromComplete(int ID)
        {
            AppUser currentUser = GetCurrentUser().Result;
            WiringPXP wiringPXP = wiringRepo.WiringPXPs.FirstOrDefault(m => m.WiringPXPID == ID);

            StatusPO statusPO = jobRepo.StatusPOs.FirstOrDefault(m => m.POID == wiringPXP.POID);
            statusPO.Status = "PXP on progress";
            jobRepo.SaveStatusPO(statusPO);


            TempData["message"] = $"PXP del PO #" + wiringPXP.SinglePO + " finalizado";
            return RedirectToAction("PXPProductionDashboard");
        }

        [HttpPost]
        public FileStreamResult ExportToXML()
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            List<WiringPXP> wiringPXPs = wiringRepo.WiringPXPs.ToList();
            List<PXPError> errors = wiringRepo.PXPErrors.ToList();
            List<WirersPXPInvolved> involveds = wiringRepo.WirersPXPInvolveds.ToList();

            using (XmlWriter xw = XmlWriter.Create(ms, xws))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("PXP");

                xw.WriteStartElement("WiringPXPs");
                foreach (WiringPXP wiringPXP in wiringPXPs)
                {
                    xw.WriteStartElement("WiringPXP");
                    xw.WriteElementString("WiringPXPID", wiringPXP.WiringPXPID.ToString());
                    xw.WriteElementString("POID", wiringPXP.POID.ToString());
                    xw.WriteElementString("StationID", wiringPXP.StationID.ToString());
                    xw.WriteElementString("StartDate", wiringPXP.StartDate.ToString());
                    xw.WriteElementString("EndDate", wiringPXP.EndDate.ToString());
                    xw.WriteElementString("SinglePO", wiringPXP.SinglePO.ToString());
                    xw.WriteElementString("WirerPXPID", wiringPXP.WirerPXPID.ToString());
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();

                xw.WriteStartElement("PXPErrors");

                foreach (PXPError error in errors)
                {
                    xw.WriteStartElement("PXPError");
                    xw.WriteElementString("PXPErrorID", error.PXPErrorID.ToString());
                    xw.WriteElementString("WiringPXPID", error.WiringPXPID.ToString());
                    xw.WriteElementString("PXPReasonID", error.PXPReasonID.ToString());
                    xw.WriteElementString("GuiltyWirerID", error.GuiltyWirerID.ToString());
                    xw.WriteEndElement();

                }
                xw.WriteEndElement();

                xw.WriteStartElement("WirersPXPInvolveds");
                foreach (WirersPXPInvolved involved in involveds)
                {
                    xw.WriteStartElement("WirersPXPInvolved");
                    xw.WriteElementString("WirersPXPInvolvedID", involved.WirersPXPInvolvedID.ToString());
                    xw.WriteElementString("WiringPXPID", involved.WiringPXPID.ToString());
                    xw.WriteElementString("WirerPXPID", involved.WirerPXPID.ToString());
                    xw.WriteEndElement();

                }
                xw.WriteEndElement();


                xw.WriteEndElement();
                xw.WriteEndDocument();
            }

            ms.Position = 0;
            return File(ms, "text/xml", "WiringPXPs-PXPErrors-WirersPXPInvolveds.xml");
        }

        public void ImportXML(IServiceProvider services)
        {
            ApplicationDbContext context = services.GetRequiredService<ApplicationDbContext>();


            XmlDocument doc = new XmlDocument();
            doc.Load(appDataFolder + "WiringPXPs-PXPErrors-WirersPXPInvolveds.xml");

            if (context.JobTypes.Any() && context.Jobs.Any()
                 && context.Stations.Any() && context.PXPReasons.Any())
            {
                var ALLPXP = doc.DocumentElement.SelectSingleNode("//PXP");

                var ALLXMLobs = ALLPXP.SelectSingleNode("//WiringPXPs");
                var XMLobs = ALLXMLobs.SelectNodes("//WiringPXP");

                var ALLerrors = ALLPXP.SelectSingleNode("//PXPErrors");
                var errors = ALLerrors.SelectNodes("//PXPError");

                var ALLinvolveds = ALLPXP.SelectSingleNode("//WirersPXPInvolveds");
                var involveds = ALLinvolveds.SelectNodes("//WirersPXPInvolved");

                foreach (XmlElement XMLob in XMLobs)
                {
                    var wiringPXPID = XMLob.SelectSingleNode(".//WiringPXPID").InnerText;
                    var poid = XMLob.SelectSingleNode(".//POID").InnerText;
                    var stationID = XMLob.SelectSingleNode(".//StationID").InnerText;
                    var startDate = XMLob.SelectSingleNode(".//StartDate").InnerText;
                    var endDate = XMLob.SelectSingleNode(".//EndDate").InnerText;
                    var singlePO = XMLob.SelectSingleNode(".//SinglePO").InnerText;
                    var wirerPXPID = XMLob.SelectSingleNode(".//WirerPXPID").InnerText;

                    context.WiringPXPs.Add(new WiringPXP
                    {
                        WiringPXPID = Int32.Parse(wiringPXPID),
                        POID = Int32.Parse(poid),
                        StationID = Int32.Parse(stationID),
                        StartDate = DateTime.Parse(startDate),
                        EndDate = DateTime.Parse(endDate),
                        SinglePO = Int32.Parse(singlePO),
                        WirerPXPID = Int32.Parse(wirerPXPID)
                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringPXPs ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WiringPXPs OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }

                foreach (XmlElement error in errors)
                {
                    var pXPErrorID = error.SelectSingleNode(".//PXPErrorID").InnerText;
                    var wiringPXPID = error.SelectSingleNode(".//WiringPXPID").InnerText;
                    var pXPReasonID = error.SelectSingleNode(".//PXPReasonID").InnerText;
                    var guiltyWirerID = error.SelectSingleNode(".//GuiltyWirerID").InnerText;

                    context.PXPErrors.Add(new PXPError
                    {
                        PXPErrorID = Int32.Parse(pXPErrorID),
                        WiringPXPID = Int32.Parse(wiringPXPID),
                        PXPReasonID = Int32.Parse(pXPReasonID),
                        GuiltyWirerID = Int32.Parse(guiltyWirerID),

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.PXPErrors ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.PXPErrors OFF");
                    }
                    finally
                    {
                        context.Database.CloseConnection();
                    }
                }

                foreach (XmlElement involved in involveds)
                {
                    var wirersPXPInvolvedID = involved.SelectSingleNode(".//WirersPXPInvolvedID").InnerText;
                    var wiringPXPID = involved.SelectSingleNode(".//WiringPXPID").InnerText;
                    var wirerPXPID = involved.SelectSingleNode(".//WirerPXPID").InnerText;

                    context.WirersPXPInvolveds.Add(new WirersPXPInvolved
                    {
                        WirersPXPInvolvedID = Int32.Parse(wirersPXPInvolvedID),
                        WiringPXPID = Int32.Parse(wiringPXPID),
                        WirerPXPID = Int32.Parse(wirerPXPID),

                    });
                    context.Database.OpenConnection();
                    try
                    {
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WirersPXPInvolveds ON");
                        context.SaveChanges();
                        context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.WirersPXPInvolveds OFF");
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
            return RedirectToAction(nameof(PXPProductionDashboard));
        }

    }
}
