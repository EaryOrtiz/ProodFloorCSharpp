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
using System.Collections.Generic;
using ChartJSCore.Models;
using System;
using ChartJSCore.Helpers;
using ProdFloor.Models.ViewModels.TestJob;
using ProdFloor.Models.ViewModels.Wiring;

namespace ProdFloor.Controllers
{

    [Authorize(Roles = "Admin,TechAdmin,Engineer,Technician,EngAdmin,CrossApprover,Manager,Kitting")]
    public class HomeController : Controller
    {
        private IJobRepository repository;
        private ITestingRepository testingRepo;
        private IItemRepository itemRepo;
        private IWiringRepository wiringRepo;
        public int PageSize = 3;
        private UserManager<AppUser> userManager;

        public HomeController(IJobRepository repo,
            IItemRepository item,
            ITestingRepository testRepo,
            IWiringRepository wirerRepo,
            UserManager<AppUser> userMrg)
        {
            repository = repo;
            testingRepo = testRepo;
            itemRepo = item;
            wiringRepo = wirerRepo;
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

        public ActionResult Index(string filtrado, string Sort = "default", int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1,int pendingJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            bool engineer = GetCurrentUserRole("Engineer").Result;
            bool tech = GetCurrentUserRole("Technician").Result;
            bool admin = GetCurrentUserRole("Admin").Result;
            bool techAdmin = GetCurrentUserRole("TechAdmin").Result;
            bool manager = GetCurrentUserRole("Manager").Result;
            bool kitting = GetCurrentUserRole("Kitting").Result;
            bool WirerAdmin = GetCurrentUserRole("WirerAdmin").Result;
            bool wirer = GetCurrentUserRole("Wirer").Result;

            if (filtrado != null) Sort = filtrado;
            if (engineer)
            {
                List<JobType> JobTyPeList = itemRepo.JobTypes.ToList();
                List<PO> POsList = repository.POs.ToList();

                List<Job> MyjobsList = repository.Jobs.Include(s => s._JobAdditional)
                    .Where(j => j.EngID == currentUser.EngID)
                    .Where(s => s.Status != "Pending")
                    .Where(j => j.Status == "Incomplete" || j.Status == "Not Reviewed" || j.Status == "Working on it" || j.Status == "Cross Approval Complete").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> OnCrossJobsList = repository.Jobs.Include(s => s._JobAdditional)
                        .Where(j => j.CrossAppEngID == currentUser.EngID)
                        .Where(s => s.Status != "Pending")
                        .Where(j => j.Status == "On Cross Approval").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> PendingToCrossJobList = repository.Jobs.Include(s => s._JobAdditional)
                        .Where(j => j.EngID != currentUser.EngID)
                        .Where(s => s.Status != "Pending")
                        .Where(j => j.Status == "Cross Approval Pending").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<JobAdditional> MyJobAdditionalList = repository.JobAdditionals.Where(m => MyjobsList.Any(n => n.JobID == m.JobID)).ToList();
                List<JobAdditional> OnCrossJobAdditionalList = repository.JobAdditionals.Where(m => OnCrossJobsList.Any(n => n.JobID == m.JobID)).ToList();
                List<JobAdditional> PendingJobAdditionalList = repository.JobAdditionals.Where(m => PendingToCrossJobList.Any(n => n.JobID == m.JobID)).ToList();

                switch (Sort)
                {
                    case "1JobNumAsc": MyjobsList = MyjobsList.OrderBy(m => m.JobNum).ToList(); break;
                    case "2JobNumAsc": OnCrossJobsList = OnCrossJobsList.OrderBy(m => m.JobNum).ToList(); break;
                    case "3JobNumAsc": PendingToCrossJobList = PendingToCrossJobList.OrderBy(m => m.JobNum).ToList(); break;

                    case "1JobNumDesc": MyjobsList = MyjobsList.OrderByDescending(m => m.JobNum).ToList(); break;
                    case "2JobNumDesc": OnCrossJobsList = OnCrossJobsList.OrderByDescending(m => m.JobNum).ToList(); break;
                    case "3JobNumDesc": PendingToCrossJobList = PendingToCrossJobList.OrderByDescending(m => m.JobNum).ToList(); break;

                    case "1NameAsc": MyjobsList = MyjobsList.OrderBy(m => m.Name).ToList(); break;
                    case "2NameAsc": OnCrossJobsList = OnCrossJobsList.OrderBy(m => m.Name).ToList(); break;
                    case "3NameAsc": PendingToCrossJobList = PendingToCrossJobList.OrderBy(m => m.Name).ToList(); break;

                    case "1NameDesc": MyjobsList = MyjobsList.OrderByDescending(m => m.Name).ToList(); break;
                    case "2NameDesc": OnCrossJobsList = OnCrossJobsList.OrderByDescending(m => m.Name).ToList(); break;
                    case "3NameDesc": PendingToCrossJobList = PendingToCrossJobList.OrderByDescending(m => m.Name).ToList(); break;

                    case "1DateAsc": MyjobsList = MyjobsList.OrderBy(m => m.LatestFinishDate).ToList(); break;
                    case "2DateAsc": OnCrossJobsList = OnCrossJobsList.OrderBy(m => m.LatestFinishDate).ToList(); break;
                    case "3DateAsc": PendingToCrossJobList = PendingToCrossJobList.OrderBy(m => m.LatestFinishDate).ToList(); break;

                    case "1DateDesc": MyjobsList = MyjobsList.OrderByDescending(m => m.LatestFinishDate).ToList(); break;
                    case "2DateDesc": OnCrossJobsList = OnCrossJobsList.OrderByDescending(m => m.LatestFinishDate).ToList(); break;
                    case "3DateDesc": PendingToCrossJobList = PendingToCrossJobList.OrderByDescending(m => m.LatestFinishDate).ToList(); break;
                    default: break;
                }

                DashboardIndexViewModel dashboard = new DashboardIndexViewModel()
                {
                    MyJobs = MyjobsList.Skip((MyJobsPage - 1) * PageSize).Take(PageSize),
                    MyJobAdditionals = MyJobAdditionalList,
                    OnCrossJobAdditionals = OnCrossJobAdditionalList,
                    PendingJobAdditionals = PendingJobAdditionalList,
                    MyJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = MyJobsPage,
                        ItemsPerPage = PageSize,
                        TotalItems = MyjobsList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },
                    JobTypes = JobTyPeList,
                    POs = POsList,
                    OnCrossJobs = OnCrossJobsList.Skip((OnCrossJobPage - 1) * PageSize).Take(PageSize),
                    OnCrossJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = OnCrossJobPage,
                        ItemsPerPage = PageSize,
                        TotalItems = OnCrossJobsList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"
                    },

                    PendingToCrossJobs = PendingToCrossJobList.Skip((PendingToCrossJobPage - 1) * PageSize).Take(PageSize),
                    PendingToCrossJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = PendingToCrossJobPage,
                        ItemsPerPage = PageSize,
                        TotalItems = PendingToCrossJobList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"
                    },
                };
                return View("EngineerDashBoard", dashboard);
            }

            if (tech)
            {
                var CurrentTestJobs = testingRepo.TestJobs
                    .Where(j => j.TechnicianID == currentUser.EngID)
                    .Where(j => j.Status != "Completed")
                    .OrderBy(p => p.TestJobID).ToList();
                  

                List<Job> DummyOnCrossJobsList = repository.Jobs
                        .Where(j => j.Status == "On Cross Approval").ToList();

                List<Job> DummyPendingToCrossJobList = repository.Jobs
                        .Where(j => j.Status == "On Cross Approval").ToList();

                return View("TechnicianDashBoard", new DashboardIndexViewModel
                {
                    PendingTestJobs = CurrentTestJobs.Skip((MyJobsPage - 1) * 5).Take(5),
                    MyJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = MyJobsPage,
                        ItemsPerPage = 5,
                        TotalItems = CurrentTestJobs.Count(),
                    },


                    PendingJobs = repository.Jobs.Where(m => CurrentTestJobs.Any(s => s.JobID == m.JobID)),
                    JobTypesList = itemRepo.JobTypes.ToList(),
                    OnCrossJobs = DummyOnCrossJobsList.Skip((OnCrossJobPage - 1) * PageSize).Take(PageSize),
                    StationList = testingRepo.Stations.ToList(),
                    OnCrossJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = OnCrossJobPage,
                        ItemsPerPage = PageSize,
                        TotalItems = DummyOnCrossJobsList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"
                    },

                    PendingToCrossJobs = DummyPendingToCrossJobList.Skip((PendingToCrossJobPage - 1) * PageSize).Take(PageSize),
                    PendingToCrossJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = PendingToCrossJobPage,
                        ItemsPerPage = PageSize,
                        TotalItems = DummyPendingToCrossJobList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"
                    },
                });
            }

            if(admin) return RedirectToAction("SuperUserDashBoard");

            if (techAdmin)  return RedirectToAction("SearchTestJob","TestJob");

            if (manager) return RedirectToAction("ManagerDashboard", "Report");

            if (WirerAdmin) return RedirectToAction("ProductionAdminDash", "Wiring");

            if (wirer) return RedirectToAction("ProductionAdminDash", "Wiring");

            return NotFound();
        }

        public ActionResult EngineerAdminDashBoard(string filtrado, string Sort = "default", int ActiveJobPage = 1, int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            bool engAdmin = GetCurrentUserRole("EngAdmin").Result;
            bool engineer = GetCurrentUserRole("Engineer").Result;
            bool admin = GetCurrentUserRole("Admin").Result;
            if (filtrado != null) Sort = filtrado;
            if (engineer || admin || engAdmin )
            {

                List<JobType> JobTyPeList = itemRepo.JobTypes.ToList();

                List<PO> POsList = repository.POs.ToList();

                List<Job> MyjobsList = repository.Jobs
                    .Where(s => s.Status != "Pending")
                    .Where(j => j.Status == "Cross Approval Complete")
                    .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> OnCrossJobsList = repository.Jobs
                        .Where(s => s.Status != "Pending")
                        .Where(j => j.Status == "On Cross Approval").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> PendingToCrossJobList = repository.Jobs
                        .Where(s => s.Status != "Pending")
                        .Where(j => j.Status == "Cross Approval Pending").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> ActiveJobList = repository.Jobs
                        .Where(s => s.Status != "Pending")
                        .Where(j => j.Status == "Working on it").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<JobAdditional> MyJobAdditionalList = repository.JobAdditionals.Where(m => MyjobsList.Any(n => n.JobID == m.JobID)).ToList();
                List<JobAdditional> OnCrossJobAdditionalList = repository.JobAdditionals.Where(m => OnCrossJobsList.Any(n => n.JobID == m.JobID)).ToList();
                List<JobAdditional> PendingJobAdditionalList = repository.JobAdditionals.Where(m => PendingToCrossJobList.Any(n => n.JobID == m.JobID)).ToList();
                List<JobAdditional> ActiveJobAdditionalList = repository.JobAdditionals.Where(m => ActiveJobList.Any(n => n.JobID == m.JobID)).ToList();


                switch (Sort)
                {
                    case "1JobNumAsc": MyjobsList = MyjobsList.OrderBy(m => m.JobNum).ToList(); break;
                    case "2JobNumAsc": OnCrossJobsList = OnCrossJobsList.OrderBy(m => m.JobNum).ToList(); break;
                    case "3JobNumAsc": PendingToCrossJobList = PendingToCrossJobList.OrderBy(m => m.JobNum).ToList(); break;
                    case "4JobNumAsc": ActiveJobList = ActiveJobList.OrderBy(m => m.JobNum).ToList(); break;

                    case "1JobNumDesc": MyjobsList = MyjobsList.OrderByDescending(m => m.JobNum).ToList(); break;
                    case "2JobNumDesc": OnCrossJobsList = OnCrossJobsList.OrderByDescending(m => m.JobNum).ToList(); break;
                    case "3JobNumDesc": PendingToCrossJobList = PendingToCrossJobList.OrderByDescending(m => m.JobNum).ToList(); break;
                    case "4JobNumDesc": ActiveJobList = ActiveJobList.OrderByDescending(m => m.JobNum).ToList(); break;

                    case "1NameAsc": MyjobsList = MyjobsList.OrderBy(m => m.Name).ToList(); break;
                    case "2NameAsc": OnCrossJobsList = OnCrossJobsList.OrderBy(m => m.Name).ToList(); break;
                    case "3NameAsc": PendingToCrossJobList = PendingToCrossJobList.OrderBy(m => m.Name).ToList(); break;
                    case "4NameAsc": ActiveJobList = ActiveJobList.OrderBy(m => m.Name).ToList(); break;

                    case "1NameDesc": MyjobsList = MyjobsList.OrderByDescending(m => m.Name).ToList(); break;
                    case "2NameDesc": OnCrossJobsList = OnCrossJobsList.OrderByDescending(m => m.Name).ToList(); break;
                    case "3NameDesc": PendingToCrossJobList = PendingToCrossJobList.OrderByDescending(m => m.Name).ToList(); break;
                    case "4NameDesc": ActiveJobList = ActiveJobList.OrderByDescending(m => m.Name).ToList(); break;

                    case "1DateAsc": MyjobsList = MyjobsList.OrderBy(m => m.LatestFinishDate).ToList(); break;
                    case "2DateAsc": OnCrossJobsList = OnCrossJobsList.OrderBy(m => m.LatestFinishDate).ToList(); break;
                    case "3DateAsc": PendingToCrossJobList = PendingToCrossJobList.OrderBy(m => m.LatestFinishDate).ToList(); break;
                    case "4DateAsc": ActiveJobList = ActiveJobList.OrderBy(m => m.LatestFinishDate).ToList(); break;

                    case "1DateDesc": MyjobsList = MyjobsList.OrderByDescending(m => m.LatestFinishDate).ToList(); break;
                    case "2DateDesc": OnCrossJobsList = OnCrossJobsList.OrderByDescending(m => m.LatestFinishDate).ToList(); break;
                    case "3DateDesc": PendingToCrossJobList = PendingToCrossJobList.OrderByDescending(m => m.LatestFinishDate).ToList(); break;
                    case "4DateDesc": ActiveJobList = ActiveJobList.OrderByDescending(m => m.LatestFinishDate).ToList(); break;

                    case "4ShippingAsc": ActiveJobList = ActiveJobList.OrderBy(m => m.ShipDate).ToList(); break;
                    case "4ShippingDesc": ActiveJobList = ActiveJobList.OrderByDescending(m => m.ShipDate).ToList(); break;
                    default: break;
                }

                DashboardIndexViewModel dashboard = new DashboardIndexViewModel()
                {
                    MyJobs = MyjobsList.Skip((MyJobsPage - 1) * 6).Take(6),
                    MyJobAdditionals = MyJobAdditionalList,
                    OnCrossJobAdditionals = OnCrossJobAdditionalList,
                    PendingJobAdditionals = PendingJobAdditionalList,
                    ActiveJobAdditionals = ActiveJobAdditionalList,
                    MyJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = MyJobsPage,
                        ItemsPerPage = 6,
                        TotalItems = MyjobsList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },
                    JobTypes = JobTyPeList,
                    POs = POsList,
                    OnCrossJobs = OnCrossJobsList.Skip((OnCrossJobPage - 1) * 6).Take(6),
                    OnCrossJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = OnCrossJobPage,
                        ItemsPerPage = 6,
                        TotalItems = OnCrossJobsList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"
                    },

                    PendingToCrossJobs = PendingToCrossJobList.Skip((PendingToCrossJobPage - 1) * 6).Take(6),
                    PendingToCrossJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = PendingToCrossJobPage,
                        ItemsPerPage = 6,
                        TotalItems = PendingToCrossJobList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"
                    },

                    ActiveJobs = ActiveJobList.Skip((ActiveJobPage - 1) * 6).Take(6),
                    ActiveJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = ActiveJobPage,
                        ItemsPerPage = 6,
                        TotalItems = ActiveJobList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"
                    },
                };

                return View("EngineerAdminDashBoard", dashboard);
            }

            return NotFound();
        }

        public IActionResult SuperUserDashBoard(string Clean, string jobNumber, string jobnumb = "", int MyJobsPage = 1, int PendingToCrossJobPage = 1, int OnCrossJobPage = 1)
        {
            if (!string.IsNullOrEmpty(jobnumb)) jobNumber = jobnumb;
            if (!string.IsNullOrEmpty(jobNumber)) jobnumb = jobNumber;

            List<JobType> JobTyPeList = itemRepo.JobTypes.ToList();
            List<PO> POsList = repository.POs.ToList();
            List<JobAdditional> MyJobAdditionalList = repository.JobAdditionals.ToList();
            List<Job> MyjobsList = new List<Job>();

            List<Job> jobList = repository.Jobs
                .Where(s => s.Status != "Pending")
                .Where(m => m.JobNum.Contains(jobNumber)).ToList();

            List<Job> OnCrossJobsList = repository.Jobs
                .Where(s => s.Status != "Pending")
                .Where(j => j.Status == "On Cross Approval")
                .OrderByDescending(m => m._JobAdditional.Priority)
                 .ThenBy(n => n.LatestFinishDate).ToList();
            

            List<Job> PendingToCrossJobList = repository.Jobs
                .Where(s => s.Status != "Pending")
                .Where(j => j.Status == "Cross Approval Pending")
                .OrderByDescending(m => m._JobAdditional.Priority)
                .ThenBy(n => n.LatestFinishDate).ToList();

            if (jobList != null && jobList.Count > 0)
            {
                MyjobsList = jobList
                    .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();
            }

            if (Clean == "true" || string.IsNullOrEmpty(jobnumb))
            {
                MyjobsList = repository.Jobs.OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();
                jobnumb = "";
            }

            DashboardIndexViewModel dashboard = new DashboardIndexViewModel()
            {
                MyJobs = MyjobsList.Skip((MyJobsPage - 1) * 6).Take(6),
                MyJobAdditionals = MyJobAdditionalList,
                MyJobsPagingInfo = new PagingInfo
                {
                    CurrentPage = MyJobsPage,
                    ItemsPerPage = 6,
                    TotalItems = MyjobsList.Count(),
                    JobNumb = jobnumb,

                },
                POs = POsList,
                OnCrossJobs = OnCrossJobsList.Skip((OnCrossJobPage - 1) * 6).Take(6),
                OnCrossJobsPagingInfo = new PagingInfo
                {
                    CurrentPage = OnCrossJobPage,
                    ItemsPerPage = 6,
                    TotalItems = OnCrossJobsList.Count(),
                },

                PendingToCrossJobs = PendingToCrossJobList.Skip((PendingToCrossJobPage - 1) * 6).Take(6),
                PendingToCrossJobsPagingInfo = new PagingInfo
                {
                    CurrentPage = PendingToCrossJobPage,
                    ItemsPerPage = 6,
                    TotalItems = PendingToCrossJobList.Count(),
                },
                JobTypesList = JobTyPeList,

            };

            if (string.IsNullOrEmpty(jobNumber)) return View(dashboard);
            if (MyjobsList.Count > 0 && MyjobsList[0] != null) return View(dashboard);
            TempData["message"] = $"Does not exist any job with the JobNum #{jobNumber} or with the status 'On Cross Approval' or 'Cross Approval Pending', please try again.";
            TempData["alert"] = $"alert-danger";
            return View(dashboard);
        }

        public ActionResult MorningDashBoard(string filtrado, bool isEngAdmin = false,string Sort = "default", int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            bool engineer = GetCurrentUserRole("Engineer").Result;
            if (filtrado != null) Sort = filtrado;

            if (isEngAdmin)
            {
                List<JobType> JobTyPeList = itemRepo.JobTypes.ToList();
                List<PO> POsList = repository.POs.ToList();

                List<Job> MyjobsList = repository.Jobs
                    .Where(s => s.Status != "Pending")
                    .Where(m => m.Status != "Cross Approval Complete" && m.Status != "Test" && m.Status != "Completed")
                    .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> OnCrossJobsList = repository.Jobs
                    .Where(s => s.Status != "Pending")
                    .Where(j => j.Status == "On Cross Approval")
                    .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> PendingToCrossJobList = repository.Jobs
                    .Where(s => s.Status != "Pending")
                    .Where(j => j.Status == "Cross Approval Pending")
                    .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<JobAdditional> MyJobAdditionalList = repository.JobAdditionals.Where(m => MyjobsList.Any(n => n.JobID == m.JobID)).OrderBy(m => m.ERDate).ToList();
                List<JobAdditional> OnCrossJobAdditionalList = repository.JobAdditionals.Where(m => OnCrossJobsList.Any(n => n.JobID == m.JobID)).ToList();
                List<JobAdditional> PendingJobAdditionalList = repository.JobAdditionals.Where(m => PendingToCrossJobList.Any(n => n.JobID == m.JobID)).ToList();


                DashboardIndexViewModel dashboard = new DashboardIndexViewModel()
                {
                    MyJobAdditionals = MyJobAdditionalList.Skip((MyJobsPage - 1) * 10).Take(10).ToList(),
                    MyJobs = MyjobsList,
                    MyJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = MyJobsPage,
                        ItemsPerPage = 10,
                        TotalItems = MyJobAdditionalList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },
                    JobTypes = JobTyPeList,
                    POs = POsList,
                    CurrentUserID = currentUser.EngID,
                    isEngAdmin = isEngAdmin,
                    OnCrossJobAdditionals = OnCrossJobAdditionalList,
                    PendingJobAdditionals = PendingJobAdditionalList,
                    OnCrossJobs = OnCrossJobsList.Skip((OnCrossJobPage - 1) * PageSize).Take(PageSize),
                    OnCrossJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = OnCrossJobPage,
                        ItemsPerPage = PageSize,
                        TotalItems = OnCrossJobsList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"
                    },

                    PendingToCrossJobs = PendingToCrossJobList.Skip((PendingToCrossJobPage - 1) * PageSize).Take(PageSize),
                    PendingToCrossJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = PendingToCrossJobPage,
                        ItemsPerPage = PageSize,
                        TotalItems = PendingToCrossJobList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"
                    },
                };
                return View("MorningDashBoard", dashboard);
            }
            else
            {
                List<JobType> JobTyPeList = itemRepo.JobTypes.ToList();
                List<PO> POsList = repository.POs.ToList();

                List<Job> MyjobsList = repository.Jobs
                    .Where(j => j.EngID == currentUser.EngID).Where(m => m.Status != "Cross Approval Complete" && m.Status != "Test" && m.Status != "Completed")
                    .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> OnCrossJobsList = repository.Jobs
                        .Where(j => j.CrossAppEngID == currentUser.EngID)
                        .Where(j => j.Status == "On Cross Approval").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> PendingToCrossJobList = repository.Jobs
                        .Where(j => j.EngID != currentUser.EngID)
                        .Where(j => j.Status == "Cross Approval Pending").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<JobAdditional> MyJobAdditionalList = repository.JobAdditionals.Where(m => MyjobsList.Any(n => n.JobID == m.JobID)).ToList();
                List<JobAdditional> OnCrossJobAdditionalList = repository.JobAdditionals.Where(m => OnCrossJobsList.Any(n => n.JobID == m.JobID)).ToList();
                List<JobAdditional> PendingJobAdditionalList = repository.JobAdditionals.Where(m => PendingToCrossJobList.Any(n => n.JobID == m.JobID)).ToList();


                DashboardIndexViewModel dashboard = new DashboardIndexViewModel()
                {
                    MyJobAdditionals = MyJobAdditionalList.Skip((MyJobsPage - 1) * 10).Take(10).ToList(),
                    MyJobs = MyjobsList,
                    MyJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = MyJobsPage,
                        ItemsPerPage = 10,
                        TotalItems = MyJobAdditionalList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },
                    JobTypes = JobTyPeList,
                    POs = POsList,
                    isEngAdmin = isEngAdmin,
                    CurrentUserID = currentUser.EngID,
                    OnCrossJobAdditionals = OnCrossJobAdditionalList,
                    PendingJobAdditionals = PendingJobAdditionalList,
                    OnCrossJobs = OnCrossJobsList.Skip((OnCrossJobPage - 1) * PageSize).Take(PageSize),
                    OnCrossJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = OnCrossJobPage,
                        ItemsPerPage = PageSize,
                        TotalItems = OnCrossJobsList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"
                    },

                    PendingToCrossJobs = PendingToCrossJobList.Skip((PendingToCrossJobPage - 1) * PageSize).Take(PageSize),
                    PendingToCrossJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = PendingToCrossJobPage,
                        ItemsPerPage = PageSize,
                        TotalItems = PendingToCrossJobList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"
                    },
                };
                return View("MorningDashBoard", dashboard);
            }
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

        [HttpPost]
        public IActionResult ToCross(int JobCrossID, DashboardIndexViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            Job UpdateStatus = repository.Jobs.FirstOrDefault(j => j.JobID == JobCrossID);
            JobAdditional jobAdditional = repository.JobAdditionals.FirstOrDefault(m => m.JobID == UpdateStatus.JobID);
            if (UpdateStatus != null)
            {
                if (viewModel.buttonAction == "ToCross" && currentUser.EngID == UpdateStatus.EngID)
                {
                    if(UpdateStatus.Status == "Working on it")
                    {
                        UpdateStatus.Status = "Cross Approval Pending";
                        repository.SaveJob(UpdateStatus);
                        jobAdditional.Status = "Cross Approval";
                        repository.SaveJobAdditional(jobAdditional);

                        TempData["message"] = $"You have released the Job #{UpdateStatus.JobNum} to Cross Approval";
                    }
                    else
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"There was an error with your request{JobCrossID}";
                    }
                    
                }
                else if (viewModel.buttonAction == "CrossApproved" && currentUser.EngID == UpdateStatus.CrossAppEngID)
                {
                    if (UpdateStatus.Status == "On Cross Approval")
                    {
                        UpdateStatus.Status = "Cross Approval Complete";
                        repository.SaveJob(UpdateStatus);
                        jobAdditional.Status = "Released";
                        repository.SaveJobAdditional(jobAdditional);

                        TempData["message"] = $"You have approved the Job #{UpdateStatus.JobNum}";
                    }
                    else
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"There was an error with your request{JobCrossID}";
                    }
                }
                else if (viewModel.buttonAction == "ToProduction" && currentUser.EngID == UpdateStatus.EngID)
                {
                    if (UpdateStatus.Status == "Cross Approval Complete")
                    {
                        UpdateStatus.Status = "Test";
                        repository.SaveJob(UpdateStatus);
                        TempData["message"] = $"You have sent to production the Job #{UpdateStatus.JobNum}";
                    }
                    else
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"There was an error with your request{JobCrossID}";
                    }
                }
                else if (viewModel.buttonAction == "Completed" && currentUser.EngID == UpdateStatus.EngID)
                {
                    UpdateStatus.Status = "Completed";
                    repository.SaveJob(UpdateStatus);

                    TempData["message"] = $"You have sent to Completed the Job #{UpdateStatus.JobNum}";
                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"There was an error with your request{JobCrossID}";
                }
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request{JobCrossID}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult GetCross(int JobCrossID)
        {
            Job jobToCross = repository.Jobs.FirstOrDefault(j => j.JobID == JobCrossID);
            if (jobToCross != null && jobToCross.Status == "Cross Approval Pending")
            {
                jobToCross.CrossAppEngID = GetCurrentUser().Result.EngID;
                jobToCross.Status = "On Cross Approval";
                repository.SaveJob(jobToCross);
                TempData["message"] = $"You've taken Job #{jobToCross.JobNum}.";
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Job status has changed or another engineer has taken it";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ChangeStatus(DashboardIndexViewModel viewModel)
        {
            Job job = repository.Jobs.FirstOrDefault(m => m.JobID == viewModel.JobID); AppUser CurrentEng = GetCurrentUser().Result;
            bool EngAdmin = GetCurrentUserRole("EngAdmin").Result;
            bool Admin = GetCurrentUserRole("Admin").Result;

            if (!EngAdmin && !Admin && CurrentEng.EngID != job.EngID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You cant only changes your jobs";
                return RedirectToAction(nameof(EngineerAdminDashBoard));
            }

            if (job.Status != viewModel.CurrentStatus)
            {

                job.Status = viewModel.CurrentStatus;
                if (viewModel.CurrentStatus == "Cross Approval Pending" || viewModel.CurrentStatus == "Working on it") job.CrossAppEngID = 0;
                repository.SaveJob(job);

                TempData["message"] = $"You have change the status of  the Job #{job.JobNum} to {viewModel.CurrentStatus}";
                return RedirectToAction("EngineerAdminDashBoard");
            }

            TempData["alert"] = $"alert-danger";
            TempData["message"] = $"There was an error with your request, the status is the same";
            return RedirectToAction("EngineerAdminDashBoard");
        }

        [HttpPost]
        public IActionResult ChangeStatusAdmin(DashboardIndexViewModel viewModel)
        {
            Job job = repository.Jobs.FirstOrDefault(m => m.JobID == viewModel.JobID);

            AppUser CurrentEng = GetCurrentUser().Result;
            bool EngAdmin = GetCurrentUserRole("EngAdmin").Result;
            bool Admin = GetCurrentUserRole("Admin").Result;

            if (!EngAdmin && !Admin && CurrentEng.EngID != job.EngID)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You cant only changes your jobs";
                return RedirectToAction("SuperUserDashBoard");
            }

            if (job.Status != viewModel.CurrentStatus)
            {

                job.Status = viewModel.CurrentStatus;
                if (viewModel.CurrentStatus == "Cross Approval Pending" || viewModel.CurrentStatus == "Working on it") job.CrossAppEngID = 0;
                repository.SaveJob(job);

                TempData["message"] = $"You have change the status of  the Job #{job.JobNum} to {viewModel.CurrentStatus}";
                return RedirectToAction("SuperUserDashBoard");
            }

            TempData["alert"] = $"alert-danger";
            TempData["message"] = $"There was an error with your request, the status is the same";
            return RedirectToAction("SuperUserDashBoard");
        }

        public IActionResult JobReassignment(DashboardIndexViewModel viewModel)
        {
            Job job = repository.Jobs.FirstOrDefault(m => m.JobID == viewModel.JobID);

            AppUser CurrentEng = GetCurrentUser().Result;
            bool EngAdmin = GetCurrentUserRole("EngAdmin").Result;
            bool Admin = GetCurrentUserRole("Admin").Result;

            if (!EngAdmin && !Admin)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You cant reassign";
                return RedirectToAction(nameof(EngineerAdminDashBoard));
            }

            int CurrentEngID = job.EngID;
            if ((job.EngID != viewModel.CurrentEngID) && (job.CrossAppEngID != viewModel.CurrentCrosAppEngID))
            {
                if (viewModel.CurrentEngID != viewModel.CurrentCrosAppEngID)
                {
                    job.EngID = viewModel.CurrentEngID;
                    job.CrossAppEngID = viewModel.CurrentCrosAppEngID;
                    repository.SaveJob(job);

                    TempData["message"] = $"You have reassigned the the Engineer for the Job #{job.JobNum} to E{job.EngID} and the CrossApprover E{job.EngID}";
                    return RedirectToAction("EngineerAdminDashBoard");
                }

                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You cannot reassing the CrossApprover because is the same Engineer who owns the Job";
                return RedirectToAction("EngineerAdminDashBoard");

            }
            else if (job.EngID != viewModel.CurrentEngID)
            {
                job.EngID = viewModel.CurrentEngID;
                repository.SaveJob(job);

                TempData["message"] = $"You have reassinged the Engineer for the Job #{job.JobNum} to E{job.EngID}";
                return RedirectToAction("EngineerAdminDashBoard");

            }
            else if (job.CrossAppEngID != viewModel.CurrentCrosAppEngID)
            {
                if (CurrentEngID != viewModel.CurrentCrosAppEngID)
                {
                    job.CrossAppEngID = viewModel.CurrentCrosAppEngID;
                    repository.SaveJob(job);

                    TempData["message"] = $"You have reassinged the CrossApprover for the Job #{job.JobNum} to E{job.CrossAppEngID}";
                    return RedirectToAction("EngineerAdminDashBoard");
                }

                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You cannot reassing the CrossApprover because is the same Engineer who owns the Job";
                return RedirectToAction("EngineerAdminDashBoard");
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"You cannot reassing the CrossApprover because is the same Engineer who owns the Job";
                return RedirectToAction("EngineerAdminDashBoard");
            }

        }

        [HttpPost]
        public IActionResult MorningReport(bool isAdmin,DashboardIndexViewModel viewModel)
        {
            if (viewModel.MyJobAdditionals != null && viewModel.MyJobAdditionals.Count > 0)
            {
                foreach (JobAdditional jobinfo in viewModel.MyJobAdditionals)
                {
                    JobAdditional CurrentJobAdd = repository.JobAdditionals.FirstOrDefault(m => m.JobAdditionalID == jobinfo.JobAdditionalID);
                    CurrentJobAdd.Status = jobinfo.Status == null ? "" : jobinfo.Status;
                    CurrentJobAdd.Action = jobinfo.Action == null ? "" : jobinfo.Action;
                    CurrentJobAdd.ERDate = jobinfo.ERDate;

                    repository.SaveJobAdditional(CurrentJobAdd);
                }
                TempData["message"] = $"The morning report was updated successfully";
                return Redirect("MorningDashBoard");
            }
            TempData["alert"] = $"alert-danger";
            TempData["message"] = $"There was an error with your request";
            return View("MorningDashBoard");
        }

        [HttpPost]
        public IActionResult ChangePriority(int btnPriority, int btnJobID)
        {
            if (btnPriority >= 0 && btnPriority < 4)
            {
                Job job = repository.Jobs.FirstOrDefault(m => m.JobID == btnJobID);

                AppUser CurrentEng = GetCurrentUser().Result;
                bool EngAdmin = GetCurrentUserRole("EngAdmin").Result;
                bool Admin = GetCurrentUserRole("Admin").Result;

                if (!EngAdmin && !Admin && CurrentEng.EngID != job.EngID)
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"You cant only changes your jobs";
                    return RedirectToAction(nameof(Index));
                }

                JobAdditional jobAdditional = repository.JobAdditionals.FirstOrDefault(m => m.JobID == btnJobID);
                jobAdditional.Priority = btnPriority;
                repository.SaveJobAdditional(jobAdditional);
                TempData["message"] = $"You have changed the priority for the job #{job.JobNum} successfully";
                return RedirectToAction(nameof(Index));
            }

            TempData["alert"] = $"alert-danger";
            TempData["message"] = $"There was an error with your request";
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        public IActionResult ChangePriorityAdmin(int btnPriority, int btnJobID)
        {
            if (btnPriority >= 0 && btnPriority < 4)
            {
                Job job = repository.Jobs.FirstOrDefault(m => m.JobID == btnJobID);

                AppUser CurrentEng  = GetCurrentUser().Result;
                bool EngAdmin = GetCurrentUserRole("EngAdmin").Result;
                bool Admin = GetCurrentUserRole("Admin").Result;

                if (!EngAdmin && !Admin && CurrentEng.EngID != job.EngID)
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"You cant only changes your jobs";
                    return RedirectToAction(nameof(EngineerAdminDashBoard));
                }


                JobAdditional jobAdditional = repository.JobAdditionals.FirstOrDefault(m => m.JobID == btnJobID);
                jobAdditional.Priority = btnPriority;
                repository.SaveJobAdditional(jobAdditional);
                TempData["message"] = $"You have changed the priority for the job #{job.JobNum} successfully";
                return RedirectToAction(nameof(EngineerAdminDashBoard));
            }

            TempData["alert"] = $"alert-danger";
            TempData["message"] = $"There was an error with your request";
            return RedirectToAction(nameof(EngineerAdminDashBoard));

        }

        public string JobTypeName(int ID)
        {
            return itemRepo.JobTypes.FirstOrDefault(m => m.JobTypeID == ID).Name;
        }

        public IActionResult SearchByPO(DashboardIndexViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            bool engineer = GetCurrentUserRole("Engineer").Result;
            bool tech = GetCurrentUserRole("Technician").Result;
            bool kitting = GetCurrentUserRole("Kitting").Result;
            bool wirerPXP = GetCurrentUserRole("WirerPXP").Result;

            if (!(viewModel.POJobSearch >= 3000000 && viewModel.POJobSearch <= 4900000))
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, PO fuera de rango";

                return RedirectToAction("SearchByPO", viewModel);
            }

            PO onePO = repository.POs.FirstOrDefault(m => m.PONumb == viewModel.POJobSearch);
            if(onePO != null)
            {
                Job job = repository.Jobs.FirstOrDefault(m => m.JobID == onePO.JobID);
                if(job.Status == "Incomplete")
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Error,Job aun en ingenieria o duplicado, intente de nuevo o contacte al Admin";
                    return RedirectToAction("SearchByPO", viewModel);
                }

                viewModel.PO = onePO;
                viewModel.Job = job;
                viewModel.JobTypeName = itemRepo.JobTypes.FirstOrDefault(m => m.JobTypeID == job.JobTypeID).Name;

                viewModel.JobExtension = repository.JobsExtensions.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);
                viewModel.HydroSpecific = repository.HydroSpecifics.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);
                viewModel.GenericFeatures = repository.GenericFeaturesList.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);
                viewModel.Indicator = repository.Indicators.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);
                viewModel.HoistWayData = repository.HoistWayDatas.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);

                viewModel.Element = repository.Elements.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);
                viewModel.ElementTraction = repository.ElementTractions.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);
                viewModel.ElementHydro = repository.ElementHydros.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);

                if (tech)
                {
                    TestJob CurrentTestJob = testingRepo.TestJobs.FirstOrDefault(m => m.TechnicianID == currentUser.EngID && m.Status == "Working on it");
                    if (CurrentTestJob == null)
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Error, Ya existe un TestJob con ese PO, intente de nuevo o contacte al Admin";
                        return RedirectToAction("SearchByPO", viewModel);
                    }

                    TestJobViewModel newJobViewModel = new TestJobViewModel();

                    newJobViewModel.Job = (viewModel.Job ?? new Job());
                    newJobViewModel.JobExtension = (viewModel.JobExtension ?? new JobExtension());
                    newJobViewModel.HydroSpecific = (viewModel.HydroSpecific ?? new HydroSpecific());
                    newJobViewModel.GenericFeatures = (viewModel.GenericFeatures ?? new GenericFeatures());
                    newJobViewModel.Indicator = (viewModel.Indicator ?? new Indicator());
                    newJobViewModel.HoistWayData = (viewModel.HoistWayData ?? new HoistWayData());

                    newJobViewModel.Element = (viewModel.Element ?? new Element());
                    newJobViewModel.ElementTraction = (viewModel.ElementTraction ?? new ElementTraction());
                    newJobViewModel.ElementHydro = (viewModel.ElementHydro ?? new ElementHydro());

                    return RedirectToAction("NewTestJob", "TestJob", newJobViewModel);
                }else if(wirerPXP)
                {

                    WiringPXP CurrentWiringPXP = wiringRepo.wirerPXPs.FirstOrDefault(m => m.WirerID == currentUser.EngID && m.Status == "Working on it");
                    if (CurrentWiringPXP == null)
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Error, Ya existe un PXP con ese PO, intente de nuevo o contacte al Admin";
                        return RedirectToAction("SearchByPO", viewModel);
                    }
                    
                    WiringPXPViewModel newJobViewModel = new WiringPXPViewModel();
                     
                    newJobViewModel.Job = (viewModel.Job ?? new Job());
                    newJobViewModel.JobExtension = (viewModel.JobExtension ?? new JobExtension());
                    newJobViewModel.HydroSpecific = (viewModel.HydroSpecific ?? new HydroSpecific());
                    newJobViewModel.GenericFeatures = (viewModel.GenericFeatures ?? new GenericFeatures());
                    newJobViewModel.Indicator = (viewModel.Indicator ?? new Indicator());
                    newJobViewModel.HoistWayData = (viewModel.HoistWayData ?? new HoistWayData());

                    newJobViewModel.Element = (viewModel.Element ?? new Element());
                    newJobViewModel.ElementTraction = (viewModel.ElementTraction ?? new ElementTraction());
                    newJobViewModel.ElementHydro = (viewModel.ElementHydro ?? new ElementHydro());

                    return RedirectToAction("NewWiringPXP", "WiringPX", newJobViewModel);

                }


            }

            return View();
        }

        public DashboardIndexViewModel GetJobDependencies(PO po, Job job)
        {
            DashboardIndexViewModel viewModel = new DashboardIndexViewModel();
            
            viewModel.JobExtension = new JobExtension();
            viewModel.HydroSpecific = new HydroSpecific();
            viewModel.GenericFeatures = new GenericFeatures();
            viewModel.Indicator = new Indicator();
            viewModel.HoistWayData = new HoistWayData();
            viewModel.Element = new Element();
            viewModel.ElementHydro = new ElementHydro();
            viewModel.ElementTraction = new ElementTraction();


            switch (viewModel.JobTypeName)
            {
                case "M2000":
                case "M4000":
                    

                    break;
                case "ElmHydro":
                    Element element = repository.Elements.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);
                    ElementHydro elementHydro = repository.ElementHydros.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);

                    break;
                case "ElmTract":
                    Element element2 = jobRepo.Elements.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);
                    ElementTraction elementTract = repository.ElementTractions.FirstOrDefault(m => m.JobID == viewModel.Job.JobID);
                    break;

            }

            return viewModel;
        }

    }

}
