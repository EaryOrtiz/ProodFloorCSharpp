using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            UserManager<AppUser> userMgr,
            IHostingEnvironment env)
        {
            jobRepo = repo2;
            wiringRepo = repo;
            itemRepo = repo3;
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


        public IActionResult PXPDashboard(DashboardIndexViewModel viewModel, int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1, int pendingJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;

            var CurrentWiringPXPs = wiringRepo.wiringPXPs
                    .Where(j => j.WirerID == currentUser.EngID)
                    .Where(j => j.Status != "Completed")
                    .OrderBy(p => p.WirerID).ToList();


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


                MyJobs = jobRepo.Jobs.Where(m => CurrentWiringPXPs.Any(s => s.JobID == m.JobID)),
                JobTypesList = itemRepo.JobTypes.ToList(),
                OnCrossJobs = new List<Job>(),
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
                    sort =  "deafult"
                },
            });
        }

        public IActionResult NewWiringPXP(int PONumb)
        {
            WiringPXPViewModel viewModel = new WiringPXPViewModel();
            DashboardIndexViewModel viewHomeModel = new DashboardIndexViewModel();
            viewHomeModel.POJobSearch = PONumb;

            PO po = jobRepo.POs.FirstOrDefault(m => m.PONumb == PONumb);
            if (po == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el PO no existe";

                return RedirectToAction("SearchByPO","Home", viewHomeModel);
            }

            Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == po.JobID);
            if (po == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, el Job no existe";

                return RedirectToAction("SearchByPO", "Home", viewHomeModel);
            }

            viewModel.Job = job;
            viewModel.PO = po;
            viewModel.JobTypeName = itemRepo.JobTypes.FirstOrDefault(m => m.JobTypeID == job.JobTypeID).Name;

            WiringPXP wiringPXP = wiringRepo.wiringPXPs.FirstOrDefault(m => m.JobID == job.JobID);
            if (wiringPXP == null)
            {
                viewModel.wiringPXP = new WiringPXP();
                viewModel.wiringPXP.SinglePO = PONumb;
                viewModel.pXPErrorList = new List<PXPError>();
            }
            else
            {
                viewModel.wiringPXP = wiringPXP;
                viewModel.pXPErrorList = wiringRepo.pXPErrors.Where(m => m.WiringPXPID == wiringPXP.WiringPXPID)
                                                             .ToList();
            }

            return View(viewModel);
        }

        public IActionResult EndWiringPXP(WiringPXPViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            WiringPXP wiringPXP = wiringRepo.wiringPXPs.FirstOrDefault(m => m.WiringPXPID == viewModel.wiringPXP.WiringPXPID);

            if (wiringPXP == null)
            {
                WiringPXP wiring = new WiringPXP()
                {
                    JobID = viewModel.Job.JobID,
                    StationID = viewModel.wiringPXP.StationID,
                    WirerID = currentUser.EngID,
                    SinglePO = viewModel.wiringPXP.SinglePO,
                    Status = "Complete",
                };

                wiringRepo.SaveWiringPXP(wiringPXP);

                wiringPXP = wiringRepo.wiringPXPs.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);
            }
            else
            {
                wiringPXP.Status = "Complete";
                wiringRepo.SaveWiringPXP(wiringPXP);
            }

            TempData["message"] = $"PXP del PO #" +wiringPXP.SinglePO + " finalizado";
            return RedirectToAction("PXPDashboard");
        }

        public IActionResult NewPXPError(WiringPXPViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            WiringPXP wiringPXP = wiringRepo.wiringPXPs.FirstOrDefault(m => m.WiringPXPID == viewModel.wiringPXP.WiringPXPID);

            if (wiringPXP == null)
            {
                WiringPXP wiring = new WiringPXP()
                {
                    JobID = viewModel.Job.JobID,
                    StationID = viewModel.wiringPXP.StationID,
                    WirerID = currentUser.EngID,
                    SinglePO = viewModel.wiringPXP.SinglePO,
                    Status = "Working on it",
                };

                wiringRepo.SaveWiringPXP(wiringPXP);

                wiringPXP = wiringRepo.wiringPXPs.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);
            }

            viewModel.wiringPXP = wiringPXP;
            viewModel.pXPError = new PXPError();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddPXPError(WiringPXPViewModel viewModel)
        {
            WiringPXP wiringPXP = wiringRepo.wiringPXPs.FirstOrDefault(m => m.WiringPXPID == viewModel.wiringPXP.WiringPXPID);

            viewModel.pXPError.WiringPXPID = wiringPXP.WiringPXPID;
            wiringRepo.SavePXPError(viewModel.pXPError);

            TempData["message"] = $"Nuevo error añadido con exito.";
            return RedirectToAction("NewWiringPXP", wiringPXP.SinglePO);
        }


        public IActionResult EditPXPError(int ID)
        {
            PXPError pXPError = wiringRepo.pXPErrors.FirstOrDefault(m => m.WiringPXPID == ID);
            WiringPXP wiringPXP = wiringRepo.wiringPXPs.FirstOrDefault(m => m.WiringPXPID == pXPError.WiringPXPID);

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
            return RedirectToAction("NewWiringPXP", viewModel.wiringPXP.SinglePO);
        }

        [HttpPost]
        public IActionResult DeletePXPError(int ID)
        {
            PXPError deletedError = wiringRepo.DeletePXPError(ID);
            WiringPXP wiringPXP = wiringRepo.wiringPXPs.FirstOrDefault(m => m.WiringPXPID == deletedError.WiringPXPID);
            PXPReason reasonPXP = wiringRepo.pXPReasons.FirstOrDefault(m => m.PXPReasonID == deletedError.PXPReasonID);

            if (deletedError != null)
            {
                TempData["message"] = $"{reasonPXP.Description} was deleted";
            }

            return RedirectToAction("NewWiringPXP", wiringPXP.SinglePO);
        }

        


    }
}
