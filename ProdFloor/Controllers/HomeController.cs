using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using ProdFloor.Models.ViewModels.Home;

namespace ProdFloor.Controllers
{
    [Authorize(Roles ="Admin,Engineer")]
    public class HomeController : Controller
    {
        private IJobRepository repository;
        public int PageSize = 2;
        private UserManager<AppUser> userManager;

        public HomeController(IJobRepository repo, UserManager<AppUser> userMrg)
        {
            repository = repo;
            userManager = userMrg;
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

        public ActionResult Index(int pendingJobPage = 1, int productionJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            bool engineer = GetCurrentUserRole("Engineer").Result;
            
            if(engineer)
            {
                return View("EngineerDashBoard", new DashboardIndexViewModel
                {
                    PendingJobs = repository.Jobs
                    .Where(j => j.EngID == currentUser.EngID)
                    .Where(j => j.Status == "Incomplete" || j.Status == "Not Reviewed" || j.Status == "Working on it")
                  .OrderBy(p => p.JobID)
                  .Skip((pendingJobPage - 1) * PageSize)
                  .Take(PageSize),
                    PendingJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = pendingJobPage,
                        ItemsPerPage = PageSize,
                        TotalItems = repository.Jobs
                        .Where(j => j.EngID == currentUser.EngID)
                        .Where(j => j.Status == "Incomplete" || j.Status == "Not Reviewed" || j.Status == "Working on it")
                        .Count()
                    },
                    ProductionJobs = repository.Jobs
                        .Where(j => j.EngID == currentUser.EngID)
                        .Where(j => j.Status == "Cross Approval Pending" || j.Status == "On Cross Approval" || j.Status == "Cross Approval Complete")
                        .OrderBy(p => p.JobID)
                  .Skip((productionJobPage - 1) * PageSize)
                  .Take(PageSize),
                    ProductionJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = productionJobPage,
                        ItemsPerPage = PageSize,
                        TotalItems = repository.Jobs
                        .Where(j => j.EngID == currentUser.EngID)
                        .Where(j => j.Status == "Cross Approval Pending" || j.Status == "Cross Approval Complete")
                        .Count()
                    }
                });
            }

            return View("AdminDashBoard", new DashboardIndexViewModel
            {
                PendingJobs = repository.Jobs
                  .OrderBy(p => p.JobID)
                  .Skip((pendingJobPage - 1) * PageSize)
                  .Take(PageSize),
                PendingJobsPagingInfo = new PagingInfo
                {
                    CurrentPage = pendingJobPage,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Jobs.Count()
                },
                ProductionJobs = repository.Jobs
                  .OrderBy(p => p.JobID)
                  .Skip((productionJobPage - 1) * PageSize)
                  .Take(PageSize),
                ProductionJobsPagingInfo = new PagingInfo
                {
                    CurrentPage = productionJobPage,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Jobs.Count()
                }
            });
        }

        public ActionResult CrossHub(int pendingJobPage = 1, int productionJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            return View("EngineerDashBoard", new CrossAppHubViewModel
            {
                JobsToCross = repository.Jobs
                    .Where(j => j.Status == "Cross Approval Pending")
                  .OrderBy(p => p.ShipDate)
                  .Skip((pendingJobPage - 1) * PageSize)
                  .Take(PageSize),
                InCrossPagingInfo = new PagingInfo
                {
                    CurrentPage = pendingJobPage,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Jobs
                        .Where(j => j.EngID == currentUser.EngID)
                        .Where(j => j.Status == "Incomplete" || j.Status == "Not Reviewed" || j.Status == "Working on it")
                        .Count()
                },
                JobsInCross = repository.Jobs
                        .Where(j => j.EngID == currentUser.EngID)
                        .Where(j => j.Status == "Cross Approval Pending" || j.Status == "On Cross Approval" || j.Status == "Cross Approval Complete")
                        .OrderBy(p => p.JobID)
                  .Skip((productionJobPage - 1) * PageSize)
                  .Take(PageSize),
                ToCrossPagingInfo = new PagingInfo
                {
                    CurrentPage = productionJobPage,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Jobs
                        .Where(j => j.EngID == currentUser.EngID)
                        .Where(j => j.Status == "Cross Approval Pending" || j.Status == "Cross Approval Complete")
                        .Count()
                }
            });
        }


        public IActionResult ToCross()
        {
            TempData["message"] = $"That was a GET";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ToCross(int ID)
        {
            Job jobToCross = repository.Jobs.FirstOrDefault(j => j.JobID == ID);
            if (jobToCross != null)
            {
                jobToCross.Status = "Cross Approval Pending";
                repository.SaveJob(jobToCross);
                TempData["message"] = $"Job #{jobToCross.JobNum} has being sent to cross approval.";
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult GetCross(int ID)
        {
            Job jobToCross = repository.Jobs.FirstOrDefault(j => j.JobID == ID);
            if (jobToCross != null)
            {
                //Add CrossAppEngID field
                //jobToCross.CrossAppEngID = GetCurrentUser().Result.EngID;
                jobToCross.Status = "On Cross Approval";
                repository.SaveJob(jobToCross);
                TempData["message"] = $"You've taken Job #{jobToCross.JobNum}.";
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request";
            }
            return RedirectToAction("CrossHub");
        }
    }

}
