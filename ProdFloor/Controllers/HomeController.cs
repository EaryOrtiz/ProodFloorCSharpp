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

    [Authorize(Roles = "Admin,TechAdmin,Engineer,Technician,EngAdmin,CrossApprover,Manager,Kitting, ProductionAdmin, WirerPXP")]
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

        public ActionResult Index(string filtrado, string Sort = "default", int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1, int pendingJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            bool engineer = GetCurrentUserRole("Engineer").Result;
            bool tech = GetCurrentUserRole("Technician").Result;
            bool admin = GetCurrentUserRole("Admin").Result;
            bool techAdmin = GetCurrentUserRole("TechAdmin").Result;
            bool manager = GetCurrentUserRole("Manager").Result;
            bool kitting = GetCurrentUserRole("Kitting").Result;
            bool ProdctionAdmin = GetCurrentUserRole("ProductionAdmin").Result;
            bool wirer = GetCurrentUserRole("Wirer").Result;
            bool wirerPXP = GetCurrentUserRole("WirerPXP").Result;

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

            if (admin) return RedirectToAction("SuperUserDashBoard");

            if (techAdmin) return RedirectToAction("SearchTestJob", "TestJob");

            if (manager) return RedirectToAction("ManagerDashboard", "Report");

            if (kitting) return RedirectToAction("NewPrintable", "PlanningReport");

            if (ProdctionAdmin) return RedirectToAction("AdminDashboard", "Wiring");

            if (wirer || wirerPXP) return RedirectToAction("List", "Wiring");

            

            return NotFound();
        }

        public ActionResult EngineerAdminDashBoard(string filtrado, string Sort = "default", int ActiveJobPage = 1, int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            bool engAdmin = GetCurrentUserRole("EngAdmin").Result;
            bool engineer = GetCurrentUserRole("Engineer").Result;
            bool admin = GetCurrentUserRole("Admin").Result;
            if (filtrado != null) Sort = filtrado;
            if (engineer || admin || engAdmin)
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
                    StatusPOCount = repository.StatusPOs.Count(),
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
                StatusPOCount = repository.StatusPOs.Count()

            };

            if (string.IsNullOrEmpty(jobNumber)) return View(dashboard);
            if (MyjobsList.Count > 0 && MyjobsList[0] != null) return View(dashboard);
            TempData["message"] = $"Does not exist any job with the JobNum #{jobNumber} or with the status 'On Cross Approval' or 'Cross Approval Pending', please try again.";
            TempData["alert"] = $"alert-danger";
            return View(dashboard);
        }

        public ActionResult MorningDashBoard(string filtrado, bool isEngAdmin = false, string Sort = "default", int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            bool engineer = GetCurrentUserRole("Engineer").Result;
            if (filtrado != null) Sort = filtrado;

            if (isEngAdmin)
            {
                List<JobType> JobTyPeList = itemRepo.JobTypes.ToList();
                List<PO> POsList = repository.POs.ToList();

                List<Job> MyjobsList = repository.Jobs
                    .Where(s => s.Status == "Incomplete")
                    .Where(s => s.Status == "Cross Approval Complete")
                    .Where(m => m.Status == "On Cross Approval" && m.Status == "Cross Approval Pending" && m.Status == "Working on it")
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
                    if (UpdateStatus.Status == "Working on it")
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

                        try
                        {
                            List<PO> pOs = repository.POs.Where(m => m.JobID == UpdateStatus.JobID).ToList();
                            foreach (PO po in pOs)
                            {
                                StatusPO statusPO = repository.StatusPOs.FirstOrDefault(m => m.POID == po.POID);
                                statusPO.Status = "Production";

                                repository.SaveStatusPO(statusPO);
                            }
                        }
                        catch (Exception e)
                        {

                        }
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

                    try
                    {
                        List<PO> pOs = repository.POs.Where(m => m.JobID == UpdateStatus.JobID).ToList();
                        foreach (PO po in pOs)
                        {
                            StatusPO statusPO = repository.StatusPOs.FirstOrDefault(m => m.POID == po.POID);
                            statusPO.Status = "Completed";

                            repository.SaveStatusPO(statusPO);
                        }
                    }
                    catch (Exception e)
                    {

                    }

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
        public IActionResult MorningReport(bool isAdmin, DashboardIndexViewModel viewModel)
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

                AppUser CurrentEng = GetCurrentUser().Result;
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

        public string JobTypeName(string material)
        {
            string JobType = "";

            if (material.Contains("2000"))
                JobType = "M2000";
            else if (material.Contains("4000"))
                JobType = "M4000";
            else if (material.Contains("ELEMENT-HYDRO"))
                JobType = "ElmHydro";
            else if (material.Contains("ELEMENT-AC"))
                JobType = "ElmTract";
            else if (material.Contains("M-VIEW-SYSTEM"))
                JobType = "mView";
            else if (material.Contains("CUSTOM-T"))
                JobType = "CustomT";
            else if (material.Contains("RESISTOR"))
                JobType = "Resistor";
            else if (material.Contains("M-GROUP"))
                JobType = "mGroup";
            else if (material.Contains("M-CARTOP"))
                JobType = "mCartop";
            else if (material.Contains("IMONITOR"))
                JobType = "iMonitor";
            else if (material.Contains("CUSTOM-H"))
                JobType = "CustomH";
            else if (material.Contains("IREPORT"))
                JobType = "iReport";
            else JobType = "Generic";


            return JobType;
        }

        public ViewResult SearchByPO(string side = "1")
        {
            return View(new DashboardIndexViewModel {Side = side });
        }

        [HttpPost]
        public IActionResult SearchByPO(DashboardIndexViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            bool engineer = GetCurrentUserRole("Engineer").Result;
            bool tech = GetCurrentUserRole("Technician").Result;
            bool kitting = GetCurrentUserRole("Kitting").Result;
            bool wirerPXP = GetCurrentUserRole("WirerPXP").Result;
            bool wirer = GetCurrentUserRole("Wirer").Result;

            if (!(viewModel.POJobSearch >= 3000000 && viewModel.POJobSearch <= 4900000))
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, PO fuera de rango";

                return RedirectToAction("SearchByPO", viewModel);
            }

            PO onePO = repository.POs.FirstOrDefault(m => m.PONumb == viewModel.POJobSearch);
            if (onePO != null)
            {
                Job job = repository.Jobs.FirstOrDefault(m => m.JobID == onePO.JobID);
                if (job.Status == "Incomplete")
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Error,Job aun en ingenieria o duplicado, intente de nuevo o contacte al Admin";
                    return RedirectToAction("SearchByPO", viewModel);
                }

                viewModel.PO = onePO;

                StatusPO statusPO = repository.StatusPOs
                                           .FirstOrDefault(s => s.POID == onePO.POID);

                if (statusPO == null)
                {
                    statusPO = new StatusPO();
                    statusPO.POID = onePO.POID;

                    if (job.Status.Contains("Cross Approval") || job.Status == "Incomplete"
                        || job.Status == "Working on it")
                    {
                        statusPO.Status = "Engineering";
                    }
                    else if (job.Status == "Waiting for test")
                    {
                        statusPO.Status = "Waiting for test";
                    }
                    else if (job.Status == "PXP on progress")
                    {
                        statusPO.Status = "PXP on progress";
                    }
                    else if (job.Status == "Completed")
                    {
                        statusPO.Status = "Completed";
                    }
                    else
                    {
                        statusPO.Status = "Production";
                    }

                    repository.SaveStatusPO(statusPO);

                    statusPO = repository.StatusPOs
                                        .FirstOrDefault(s => s.POID == onePO.POID);
                }

                if (tech)
                {
                    TestJob CurrentTestJob = testingRepo.TestJobs.FirstOrDefault(m => m.TechnicianID == currentUser.EngID && m.Status == "Working on it");
                    if (CurrentTestJob != null)
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Error, tiene un testjob activo, intente de nuevo o contacte al Admin";
                        return RedirectToAction("SearchByPO", viewModel);
                    }

                    TestJob TestJobWithSamePO = testingRepo.TestJobs.FirstOrDefault(m => m.SinglePO == onePO.PONumb);
                    if (CurrentTestJob != null)
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Error, Ya existe un TestJob con ese PO, intente de nuevo o contacte al Admin";
                        return RedirectToAction("SearchByPO", viewModel);
                    }


                    return RedirectToAction("NewTestJob", "TestJob", onePO.PONumb);
                }
                else if (wirerPXP && viewModel.Side == "1")
                {
                   
                    WiringPXP WiringPXPWithSamePO = wiringRepo.WiringPXPs.FirstOrDefault(m => m.SinglePO == onePO.POID);
                    if (WiringPXPWithSamePO != null)
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Error, Ya existe un PXP con ese PO, intente de nuevo o contacte al Admin";
                        return RedirectToAction("SearchByPO", viewModel);
                    }

                    if (statusPO.Status != "Production")
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Error, El Job aun no esta en produccion o ha sido completado, intente de nuevo o contacte al Admin";
                        return RedirectToAction("SearchByPO", viewModel);
                    }


                    return RedirectToAction("NewWiringPXP", "WiringPXP", new { PONumb = viewModel.POJobSearch });

                }
                else if (wirer && viewModel.Side == "2")
                {

                    Wiring WiringWithSamePO = wiringRepo.Wirings.FirstOrDefault(m => m.POID == onePO.POID);
                    if (WiringWithSamePO != null)
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Error, Ya existe un PXP con ese PO, intente de nuevo o contacte al Admin";
                        return RedirectToAction("SearchByPO", viewModel);
                    }

                    if (statusPO.Status != "Production")
                    {
                        TempData["alert"] = $"alert-danger";
                        TempData["message"] = $"Error, El Job aun no esta en produccion o ha sido completado, intente de nuevo o contacte al Admin";
                        return RedirectToAction("SearchByPO", viewModel);
                    }


                    return RedirectToAction("NewWiringJob", "Wiring", new { PONumb = viewModel.POJobSearch });

                }
                else
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Error, usted no tiene acceso aqui";
                    return RedirectToAction("SearchByPO", viewModel);
                }


            }

            PlanningReportRow reportRow = itemRepo.PlanningReportRows.FirstOrDefault(m => m.PO == viewModel.POJobSearch);
            if (reportRow == null)
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"Error, El PO no existe";
                return RedirectToAction("SearchByPO", viewModel);
            }



            viewModel.POJobSearch = CreateDummyByPlanning(reportRow);
            if (tech)
                return RedirectToAction("NewTestJob", "TestJob", viewModel.POJobSearch);
            else if (wirerPXP)
                return RedirectToAction("NewWiringPXP", "WiringPXP", new { PONumb = viewModel.POJobSearch });
            else if (wirer)
            {
                JobType jobType = itemRepo.JobTypes.FirstOrDefault(m => m.Name == JobTypeName(reportRow.Material));

                if (jobType.Name != "M2000" && jobType.Name != "ElmHydro" &&
                    jobType.Name != "M4000" && jobType.Name != "ElmTract")
                {
                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"Error, El PO corresponde aun " + reportRow.Material.ToLower();
                    return RedirectToAction("SearchByPO", viewModel);
                }

                return RedirectToAction("NewWiringJob", "Wiring", new { PONumb = viewModel.POJobSearch });
            }
                


            TempData["alert"] = $"alert-danger";
            TempData["message"] = $"Algo salio mal xD";
            return RedirectToAction("SearchByPO", viewModel);
        }


        public int CreateDummyByPlanning(PlanningReportRow reportRow)
        {
            DashboardIndexViewModel viewModel = new DashboardIndexViewModel();

            Job currentJob = new Job();
            AppUser currentUser = GetCurrentUser().Result;
            JobType jobType = itemRepo.JobTypes.FirstOrDefault(m => m.Name == JobTypeName(reportRow.Material));

            Job Job = new Job();
            Job.Contractor = "Fake";
            Job.Cust = "Fake";
            Job.FireCodeID = 1;
            Job.LatestFinishDate = new DateTime(1, 1, 1);
            Job.EngID = Int32.Parse(reportRow.MRP.Remove(0, 1));
            Job.Status = "Pending";
            Job.CrossAppEngID = 0;
            Job.CityID = 1;
            Job.JobNum = reportRow.JobNumber;
            Job.JobTypeID = jobType.JobTypeID;
            Job.Name = reportRow.JobName;
            Job.ShipDate = DateTime.Parse(reportRow.ShippingDate);
            repository.SaveJob(Job);
            currentJob = repository.Jobs.FirstOrDefault(p => p.JobID == repository.Jobs.Max(x => x.JobID));


            switch (jobType.Name)
            {
                case "M2000":
                case "M4000":

                    //Save the dummy Job Extension
                    JobExtension currentExtension = new JobExtension(); currentExtension.JobID = currentJob.JobID; currentExtension.InputFrecuency = 60; currentExtension.InputPhase = 3; currentExtension.DoorGate = "Fake";
                    currentExtension.InputVoltage = 1; currentExtension.NumOfStops = 2; currentExtension.SHCRisers = 1; currentExtension.DoorHoist = "Fake"; currentExtension.JobTypeAdd = "Fake";
                    currentExtension.JobTypeMain = "Simplex";
                    currentExtension.SCOP = false;
                    currentExtension.SHC = false;
                    currentExtension.DoorOperatorID = 1;
                    repository.SaveJobExtension(currentExtension);

                    //Save the dummy Job HydroSpecific
                    HydroSpecific currenHydroSpecific = new HydroSpecific(); currenHydroSpecific.JobID = currentJob.JobID; currenHydroSpecific.FLA = 1; currenHydroSpecific.HP = 1;
                    currenHydroSpecific.SPH = 1; currenHydroSpecific.Starter = "Fake"; currenHydroSpecific.ValveCoils = 1; currenHydroSpecific.ValveBrand = "Fake";
                    currenHydroSpecific.MotorsNum = 1;
                    repository.SaveHydroSpecific(currenHydroSpecific);

                    //Save the dummy job Indicators
                    Indicator currentIndicator = new Indicator(); currentIndicator.CarCallsVoltage = "Fake"; currentIndicator.CarCallsVoltageType = "Fake"; currentIndicator.CarCallsType = "Fake";
                    currentIndicator.HallCallsVoltage = "Fake"; currentIndicator.HallCallsVoltageType = "Fake"; currentIndicator.HallCallsType = "Fake"; currentIndicator.IndicatorsVoltageType = "Fake";
                    currentIndicator.IndicatorsVoltage = 1; currentIndicator.JobID = currentJob.JobID;
                    repository.SaveIndicator(currentIndicator);

                    //Save the dummy Job HoistWayData
                    HoistWayData currentHoistWayData = new HoistWayData(); currentHoistWayData.JobID = currentJob.JobID; currentHoistWayData.Capacity = 1; currentHoistWayData.DownSpeed = 1;
                    currentHoistWayData.TotalTravel = 1; currentHoistWayData.UpSpeed = 1; currentHoistWayData.HoistWaysNumber = 1; currentHoistWayData.MachineRooms = 1;
                    currentHoistWayData.LandingSystemID = 1;
                    repository.SaveHoistWayData(currentHoistWayData);

                    //Save the dummy Job Generic Features
                    GenericFeatures currentGenericFeatures = new GenericFeatures(); currentGenericFeatures.JobID = currentJob.JobID;
                    currentGenericFeatures.Monitoring = "Fake";
                    repository.SaveGenericFeatures(currentGenericFeatures);

                    break;
                case "ElmHydro":
                    Element element = new Element
                    {
                        JobID = currentJob.JobID,
                        LandingSystemID = 5,
                        DoorGate = "Fake",
                        HAPS = false,
                        INA = "fake",
                        Capacity = 1,
                        Frequency = 1,
                        LoadWeigher = "fake",
                        Phase = 1,
                        Speed = 1,
                        Voltage = 1,
                        DoorBrand = "fake",
                    };
                    element.DoorOperatorID = 1;

                    repository.SaveElement(element);
                    ElementHydro elementHydro = new ElementHydro
                    {
                        JobID = currentJob.JobID,
                        FLA = 20,
                        HP = 20,
                        SPH = 14,
                        Starter = "fake",
                        ValveBrand = "fake",
                    };
                    repository.SaveElementHydro(elementHydro);
                    break;
                case "ElmTract":

                    Element element2 = new Element
                    {
                        JobID = currentJob.JobID,
                        LandingSystemID = 9,
                        DoorGate = "Fake",
                        HAPS = false,
                        INA = "fake",
                        Capacity = 1,
                        Frequency = 1,
                        LoadWeigher = "fake",
                        Phase = 1,
                        Speed = 1,
                        Voltage = 1,
                        DoorBrand = "fake",
                    };
                    element2.DoorOperatorID = 1;

                    repository.SaveElement(element2);
                    ElementTraction elementTraction = new ElementTraction
                    {
                        JobID = currentJob.JobID,
                        Contact = "fake",
                        Current = 10,
                        FLA = 10,
                        HoldVoltage = 10,
                        HP = 10,
                        MachineLocation = "fake",
                        MotorBrand = "fake",
                        PickVoltage = 10,
                        Resistance = 10,
                        VVVF = "fake"

                    };
                    repository.SaveElementTraction(elementTraction);
                    break;
                default: break;
            }

            if(jobType.Name == "M2000" || jobType.Name == "ElmHydro" ||
                jobType.Name == "M4000" || jobType.Name == "ElmTract")
            {
                SpecialFeatures featureFake = new SpecialFeatures(); featureFake.JobID = currentJob.JobID; featureFake.Description = null;
                repository.SaveSpecialFeatures(featureFake);
            }

            

            PO POFake = new PO();
            POFake.JobID = currentJob.JobID;
            POFake.PONumb = reportRow.PO;
            repository.SavePO(POFake);

            POFake = repository.POs.FirstOrDefault(m => m.JobID == Job.JobID);

            StatusPO statusPO = new StatusPO();
            statusPO.POID = POFake.POID;
            statusPO.Status = "Production";

            repository.SaveStatusPO(statusPO);

            return POFake.PONumb;
        }



    }
}

