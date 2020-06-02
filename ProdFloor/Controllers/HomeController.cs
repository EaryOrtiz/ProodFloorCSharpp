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

namespace ProdFloor.Controllers
{

    [Authorize(Roles = "Admin,TechAdmin,Engineer,Technician,EngAdmin,CrossApprover")]
    public class HomeController : Controller
    {
        private IJobRepository repository;
        private ITestingRepository testingRepo;
        private IItemRepository itemRepo;
        public int PageSize = 3;
        private UserManager<AppUser> userManager;

        public HomeController(IJobRepository repo, IItemRepository item, ITestingRepository testRepo, UserManager<AppUser> userMrg)
        {
            repository = repo;
            testingRepo = testRepo;
            itemRepo = item;
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

            if (filtrado != null) Sort = filtrado;
            if (engineer)
            {
                List<JobType> JobTyPeList = itemRepo.JobTypes.ToList();
                List<PO> POsList = repository.POs.ToList();

                List<Job> MyjobsList = repository.Jobs.Include(s => s._JobAdditional)
                    .Where(j => j.EngID == currentUser.EngID)
                    .Where(j => j.Status == "Incomplete" || j.Status == "Not Reviewed" || j.Status == "Working on it" || j.Status == "Cross Approval Complete").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> OnCrossJobsList = repository.Jobs.Include(s => s._JobAdditional)
                        .Where(j => j.CrossAppEngID == currentUser.EngID)
                        .Where(j => j.Status == "On Cross Approval").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> PendingToCrossJobList = repository.Jobs.Include(s => s._JobAdditional)
                        .Where(j => j.EngID != currentUser.EngID)
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
                    .Where(j => j.Status == "Stopped" || j.Status == "Working on it" || j.Status == "Incomplete" || j.Status == "Reassignment")
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

            return NotFound();
        }

        public ActionResult EngineerAdminDashBoard(string filtrado, string Sort = "default", int ActiveJobPage = 1, int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            bool engineer = GetCurrentUserRole("EngAdmin").Result;
            if (filtrado != null) Sort = filtrado;
            if (engineer)
            {

                List<JobType> JobTyPeList = itemRepo.JobTypes.ToList();

                List<PO> POsList = repository.POs.ToList();

                List<Job> MyjobsList = repository.Jobs
                    .Where(j => j.Status == "Cross Approval Complete")
                    .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> OnCrossJobsList = repository.Jobs
                        .Where(j => j.Status == "On Cross Approval").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> PendingToCrossJobList = repository.Jobs
                        .Where(j => j.Status == "Cross Approval Pending").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

                List<Job> ActiveJobList = repository.Jobs
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
            List<Job> jobList = repository.Jobs.Where(m => m.JobNum.Contains(jobNumber)).ToList();
            List<Job> OnCrossJobsList = repository.Jobs
                       .Where(j => j.Status == "On Cross Approval").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();
            

            List<Job> PendingToCrossJobList = repository.Jobs
                    .Where(j => j.Status == "Cross Approval Pending").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

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

        public ActionResult MorningDashBoard(string filtrado, string Sort = "default", int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            bool engineer = GetCurrentUserRole("Engineer").Result;
            if (filtrado != null) Sort = filtrado;
            if (engineer)
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
                    MyJobAdditionals = MyJobAdditionalList.Skip((MyJobsPage - 1) * 12).Take(12).ToList(),
                    MyJobs = MyjobsList.Skip((MyJobsPage - 1) * 12).Take(12),
                    MyJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = MyJobsPage,
                        ItemsPerPage = 12,
                        TotalItems = MyJobAdditionalList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },
                    JobTypes = JobTyPeList,
                    POs = POsList,
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
                return View(NotFound());
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
            if (jobToCross != null)
            {
                jobToCross.CrossAppEngID = GetCurrentUser().Result.EngID;
                jobToCross.Status = "On Cross Approval";
                repository.SaveJob(jobToCross);
                TempData["message"] = $"You've taken Job #{jobToCross.JobNum}.";
            }
            else
            {
                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"There was an error with your request";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ChangeStatus(DashboardIndexViewModel viewModel)
        {
            Job job = repository.Jobs.FirstOrDefault(m => m.JobID == viewModel.JobID);
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
            int CurrentEngID = job.EngID;
            if ((job.EngID != viewModel.CurrentEngID) && (job.CrossAppEngID != viewModel.CurrentCrosAppEngID))
            {
                if (viewModel.CurrentEngID != viewModel.CurrentCrosAppEngID)
                {
                    job.EngID = viewModel.CurrentEngID;
                    job.CrossAppEngID = viewModel.CurrentCrosAppEngID;
                    repository.SaveJob(job);

                    TempData["message"] = $"You have reassinged the the Engineer for the Job #{job.JobNum} to E{job.EngID} and the CrossApprover E{job.EngID}";
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

                    TempData["message"] = $"You have reassinged the CrossApprover for the Job #{job.JobNum} to E{job.EngID}";
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
        public IActionResult MorningReport(DashboardIndexViewModel viewModel)
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

        public IActionResult JobChartsDashBoard(string ChartName)
        {
            #region JobTypePieChartByJobType

            int M2000Count = repository.Jobs.Where(j => j.JobTypeID == 2 && j.EngID > 0 && j.EngID < 100
                                                   && (j.Status == "Working on it" || j.Status == "Cross Approval Pending"
                                                   || j.Status == "On Cross Approval" || j.Status == "Cross Approval Complete")).Count();

            int ElementHydroListCount = repository.Jobs.Where(j => j.JobTypeID == 1 && j.EngID > 0 && j.EngID < 100
                                                   && (j.Status == "Working on it" || j.Status == "Cross Approval Pending"
                                                   || j.Status == "On Cross Approval" || j.Status == "Cross Approval Complete")).Count();

            int ElementTractionCount = repository.Jobs.Where(j => j.JobTypeID == 5 && j.EngID > 0 && j.EngID < 100
                                                   && (j.Status == "Working on it" || j.Status == "Cross Approval Pending"
                                                   || j.Status == "On Cross Approval" || j.Status == "Cross Approval Complete")).Count();


            List<double?> DataPieChart = new List<double?> { M2000Count, ElementHydroListCount, ElementTractionCount };
            List<string> LabelsPiechart = new List<string> { "M2000", "Element Hydro", "Element Traction" };

            var JobTypePieChart = new Chart { Type = Enums.ChartType.Pie };

            var data = new Data { Labels = LabelsPiechart };

            var dataset = new PieDataset
            {
                Label = "My dataset",
                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                    ChartColor.FromHexString("#FFCE56")
                },
                HoverBackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                    ChartColor.FromHexString("#FFCE56")
                },
                Data = DataPieChart
            };

            data.Datasets = new List<Dataset> { dataset };

            JobTypePieChart.Data = data;
            ViewData["JobTypePieChart"] = JobTypePieChart;


            #endregion

            #region PriorityPieChartByJobType

            List<Job> AllJobs = repository.Jobs.Where(j => (j.JobTypeID == 2 || j.JobTypeID == 1 || j.JobTypeID == 5) && j.EngID > 0 && j.EngID < 100
                                                   && (j.Status == "Working on it" || j.Status == "Cross Approval Pending"
                                                   || j.Status == "On Cross Approval" || j.Status == "Cross Approval Complete")).ToList();

            List<JobAdditional> MyJobAdditionalList = repository.JobAdditionals.Where(m => AllJobs.Any(n => n.JobID == m.JobID)).ToList();

            int NormalPriority = MyJobAdditionalList.Where(m => m.Priority == 0).Count();
            int HighPriority = MyJobAdditionalList.Where(m => m.Priority == 1).Count();
            int ShortLeadPriority = MyJobAdditionalList.Where(m => m.Priority == 2).Count();


            List<double?> DataPieChartPriority = new List<double?> { NormalPriority, HighPriority, ShortLeadPriority };
            List<string> PriorityLabelsPiechart = new List<string> { "Normal", "High", "Short Lead" };

            var PieChartPriority = new Chart { Type = Enums.ChartType.Pie };

            var dataPriority = new Data { Labels = PriorityLabelsPiechart };

            var datasetPriority = new PieDataset
            {
                Label = "My dataset",
                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#6ADE4E"),
                    ChartColor.FromHexString("#FCF927"),
                    ChartColor.FromHexString("#F54242")
                },
                HoverBackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#6ADE4E"),
                    ChartColor.FromHexString("#FCF927"),
                    ChartColor.FromHexString("#F54242")
                },
                Data = DataPieChartPriority
            };

            dataPriority.Datasets = new List<Dataset> { datasetPriority };

            PieChartPriority.Data = dataPriority;
            ViewData["PieChartPriority"] = PieChartPriority;
            #endregion

            #region JobTypePieChartM200ByStatus

            int WorkinOnItCountM2000 = repository.Jobs.Where(j => j.JobTypeID == 2 && j.EngID > 0 && j.EngID < 100 && j.Status == "Working on it").Count();
            int CrossPendingListCountM2000 = repository.Jobs.Where(j => j.JobTypeID == 2 && j.EngID > 0 && j.EngID < 100 && j.Status == "Cross Approval Pending").Count();
            int OnCrossCountM2000 = repository.Jobs.Where(j => j.JobTypeID == 2 && j.EngID > 0 && j.EngID < 100 && j.Status == "On Cross Approval").Count();
            int CrossCompleteCountM2000 = repository.Jobs.Where(j => j.JobTypeID == 2 && j.EngID > 0 && j.EngID < 100 && j.Status == "Cross Approval Complete").Count();

            List<double?> DataPieChartM2000 = new List<double?> { WorkinOnItCountM2000, CrossPendingListCountM2000, OnCrossCountM2000, CrossCompleteCountM2000 };
            List<string> LabelsPiechartM2000 = new List<string> { "Working on it", "Cross Approval Pending", "On Cross Approval" , "Cross Approval Complete" };

            var JobTypePieChartM2000 = new Chart { Type = Enums.ChartType.Pie };

            var dataM2000 = new Data { Labels = LabelsPiechartM2000 };

            var datasetM2000 = new PieDataset
            {
                Label = "My dataset",
                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                    ChartColor.FromHexString("#FFCE56"),
                    ChartColor.FromHexString("#7cf233")
                },
                HoverBackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                    ChartColor.FromHexString("#FFCE56"),
                    ChartColor.FromHexString("#7cf233")
                },
                Data = DataPieChartM2000
            };

            dataM2000.Datasets = new List<Dataset> { datasetM2000 };

            JobTypePieChartM2000.Data = dataM2000;
            ViewData["JobTypePieChartM2000"] = JobTypePieChartM2000;


            #endregion

            #region PriorityPieChartByM2000

            List<Job> JobsM2000 = repository.Jobs.Where(j => j.JobTypeID == 2 && j.EngID > 0 && j.EngID < 100
                                                   && (j.Status == "Working on it" || j.Status == "Cross Approval Pending"
                                                   || j.Status == "On Cross Approval" || j.Status == "Cross Approval Complete")).ToList();

            List<JobAdditional> MyJobAdditionalListM2000 = repository.JobAdditionals.Where(m => JobsM2000.Any(n => n.JobID == m.JobID)).ToList();

            int NormalPriorityM2000 = MyJobAdditionalListM2000.Where(m => m.Priority == 0).Count();
            int HighPriorityM2000 = MyJobAdditionalListM2000.Where(m => m.Priority == 1).Count();
            int ShortLeadPriorityM2000 = MyJobAdditionalListM2000.Where(m => m.Priority == 2).Count();


            List<double?> DataPieChartPriorityM2000 = new List<double?> { NormalPriorityM2000, HighPriorityM2000, ShortLeadPriorityM2000 };
            List<string> PriorityLabelsPiechartM2000 = new List<string> { "Normal", "High", "Short Lead" };

            var PieChartPriorityM2000 = new Chart { Type = Enums.ChartType.Pie };

            var dataPriorityM2000 = new Data { Labels = PriorityLabelsPiechartM2000 };

            var datasetPriorityM2000 = new PieDataset
            {
                Label = "My dataset",
                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#6ADE4E"),
                    ChartColor.FromHexString("#FCF927"),
                    ChartColor.FromHexString("#F54242")
                },
                HoverBackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#6ADE4E"),
                    ChartColor.FromHexString("#FCF927"),
                    ChartColor.FromHexString("#F54242")
                },
                Data = DataPieChartPriorityM2000
            };

            dataPriorityM2000.Datasets = new List<Dataset> { datasetPriorityM2000 };

            PieChartPriorityM2000.Data = dataPriorityM2000;
            ViewData["PieChartPriorityM2000"] = PieChartPriorityM2000;
            #endregion

            #region JobTypePieChartHydroByStatus

            int WorkinOnItCountHydro = repository.Jobs.Where(j => j.JobTypeID == 1 && j.EngID > 0 && j.EngID < 100 && j.Status == "Working on it").Count();
            int CrossCompleteCountHydro = repository.Jobs.Where(j => j.JobTypeID == 1 && j.EngID > 0 && j.EngID < 100 && j.Status == "Cross Approval Complete").Count();

            List<double?> DataPieChartHydro = new List<double?> { WorkinOnItCountHydro,  CrossCompleteCountHydro };
            List<string> LabelsPiechartHydro = new List<string> { "Working on it", "Cross Approval Complete" };

            var JobTypePieChartHydro = new Chart { Type = Enums.ChartType.Pie };

            var dataHydro = new Data { Labels = LabelsPiechartHydro };

            var datasetHydro = new PieDataset
            {
                Label = "My dataset",
                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                },
                HoverBackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                },
                Data = DataPieChartHydro
            };

            dataHydro.Datasets = new List<Dataset> { datasetHydro };

            JobTypePieChartHydro.Data = dataHydro;
            ViewData["JobTypePieChartHydro"] = JobTypePieChartHydro;


            #endregion

            #region PriorityPieChartByHydro

            List<Job> JobsHydro = repository.Jobs.Where(j => j.JobTypeID == 1 && j.EngID > 0 && j.EngID < 100
                                                   && (j.Status == "Working on it" || j.Status == "Cross Approval Pending"
                                                   || j.Status == "On Cross Approval" || j.Status == "Cross Approval Complete")).ToList();

            List<JobAdditional> MyJobAdditionalListHydro = repository.JobAdditionals.Where(m => JobsHydro.Any(n => n.JobID == m.JobID)).ToList();

            int NormalPriorityHydro = MyJobAdditionalListHydro.Where(m => m.Priority == 0).Count();
            int HighPriorityHydro = MyJobAdditionalListHydro.Where(m => m.Priority == 1).Count();
            int ShortLeadPriorityHydro = MyJobAdditionalListHydro.Where(m => m.Priority == 2).Count();


            List<double?> DataPieChartPriorityHydro = new List<double?> { NormalPriorityHydro, HighPriorityHydro, ShortLeadPriorityHydro };
            List<string> PriorityLabelsPiechartHydro = new List<string> { "Normal", "High", "Short Lead" };

            var PieChartPriorityHydro = new Chart { Type = Enums.ChartType.Pie };

            var dataPriorityHydro = new Data { Labels = PriorityLabelsPiechartHydro };

            var datasetPriorityHydro = new PieDataset
            {
                Label = "My dataset",
                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#6ADE4E"),
                    ChartColor.FromHexString("#FCF927"),
                    ChartColor.FromHexString("#F54242")
                },
                HoverBackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#6ADE4E"),
                    ChartColor.FromHexString("#FCF927"),
                    ChartColor.FromHexString("#F54242")
                },
                Data = DataPieChartPriorityHydro
            };

            dataPriorityHydro.Datasets = new List<Dataset> { datasetPriorityHydro };

            PieChartPriorityHydro.Data = dataPriorityHydro;
            ViewData["PieChartPriorityHydro"] = PieChartPriorityHydro;
            #endregion

            #region JobTypePieChartTractionByStatus

            int WorkinOnItCountTraction = repository.Jobs.Where(j => j.JobTypeID == 5 && j.EngID > 0 && j.EngID < 100 && j.Status == "Working on it").Count();
            int CrossCompleteCountTraction = repository.Jobs.Where(j => j.JobTypeID == 5 && j.EngID > 0 && j.EngID < 100 && j.Status == "Cross Approval Complete").Count();

            List<double?> DataPieChartTraction = new List<double?> { WorkinOnItCountTraction, CrossCompleteCountTraction };
            List<string> LabelsPiechartTraction = new List<string> { "Working on it", "Cross Approval Complete" };

            var JobTypePieChartTraction = new Chart { Type = Enums.ChartType.Pie };

            var dataTraction = new Data { Labels = LabelsPiechartTraction };

            var datasetTraction = new PieDataset
            {
                Label = "My dataset",
                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                },
                HoverBackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                },
                Data = DataPieChartTraction
            };

            dataTraction.Datasets = new List<Dataset> { datasetTraction };

            JobTypePieChartTraction.Data = dataTraction;
            ViewData["JobTypePieChartTraction"] = JobTypePieChartTraction;


            #endregion

            #region PriorityPieChartByTraction

            List<Job> JobsTraction = repository.Jobs.Where(j => j.JobTypeID == 5 && j.EngID > 0 && j.EngID < 100
                                                   && (j.Status == "Working on it" || j.Status == "Cross Approval Pending"
                                                   || j.Status == "On Cross Approval" || j.Status == "Cross Approval Complete")).ToList();

            List<JobAdditional> MyJobAdditionalListTraction = repository.JobAdditionals.Where(m => JobsTraction.Any(n => n.JobID == m.JobID)).ToList();

            int NormalPriorityTraction = MyJobAdditionalListTraction.Where(m => m.Priority == 0).Count();
            int HighPriorityTraction = MyJobAdditionalListTraction.Where(m => m.Priority == 1).Count();
            int ShortLeadPriorityTraction = MyJobAdditionalListTraction.Where(m => m.Priority == 2).Count();


            List<double?> DataPieChartPriorityTraction = new List<double?> { NormalPriorityTraction, HighPriorityTraction, ShortLeadPriorityTraction };
            List<string> PriorityLabelsPiechartTraction = new List<string> { "Normal", "High", "Short Lead" };

            var PieChartPriorityTraction = new Chart { Type = Enums.ChartType.Pie };

            var dataPriorityTraction = new Data { Labels = PriorityLabelsPiechartTraction };

            var datasetPriorityTraction = new PieDataset
            {
                Label = "My dataset",
                BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#6ADE4E"),
                    ChartColor.FromHexString("#FCF927"),
                    ChartColor.FromHexString("#F54242")
                },
                HoverBackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#6ADE4E"),
                    ChartColor.FromHexString("#FCF927"),
                    ChartColor.FromHexString("#F54242")
                },
                Data = DataPieChartPriorityTraction
            };

            dataPriorityTraction.Datasets = new List<Dataset> { datasetPriorityTraction };

            PieChartPriorityTraction.Data = dataPriorityTraction;
            ViewData["PieChartPriorityTraction"] = PieChartPriorityTraction;
            #endregion

            #region ChartsByUser
            List<AppUser> users = userManager.Users.Where(m => m.EngID >= 1 && m.EngID <= 99 && !m.UserName.Contains("Tester")).ToList();
            EngineerChartsViewModel dashboard = new EngineerChartsViewModel()
            {
                users = users,
                ChartName = ChartName
            };

            foreach (AppUser user in users)
            {
                string username = user.FullName.Replace(" ", "");

                //M2000NyStatus
                int WorkinOnItCountM2000User = repository.Jobs.Where(j => j.JobTypeID == 2 && j.EngID == user.EngID && j.Status == "Working on it").Count();
                int CrossPendingListCountM2000User = repository.Jobs.Where(j => j.JobTypeID == 2 && j.EngID == user.EngID && j.Status == "Cross Approval Pending").Count();
                int OnCrossCountM2000User = repository.Jobs.Where(j => j.JobTypeID == 2 && j.EngID == user.EngID && j.Status == "On Cross Approval").Count();
                int CrossCompleteCountM2000User = repository.Jobs.Where(j => j.JobTypeID == 2 && j.EngID == user.EngID && j.Status == "Cross Approval Complete").Count();

                List<double?> DataPieChartM2000User = new List<double?> { WorkinOnItCountM2000User, CrossPendingListCountM2000User, OnCrossCountM2000User, CrossCompleteCountM2000User };
                List<string> LabelsPiechartM2000User = new List<string> { "Working on it", "Cross Approval Pending", "On Cross Approval", "Cross Approval Complete" };

                var JobTypePieChartM2000User = new Chart { Type = Enums.ChartType.Pie };

                var dataM2000User = new Data { Labels = LabelsPiechartM2000User };

                var datasetM2000User = new PieDataset
                {
                    Label = "My dataset",
                    BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                    ChartColor.FromHexString("#FFCE56"),
                    ChartColor.FromHexString("#7cf233")
                },
                    HoverBackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                    ChartColor.FromHexString("#FFCE56"),
                    ChartColor.FromHexString("#7cf233")
                },
                    Data = DataPieChartM2000User
                };

                dataM2000User.Datasets = new List<Dataset> { datasetM2000User };

                JobTypePieChartM2000User.Data = dataM2000User;
                ViewData["M2000byEnginnerPieChart" + username] = JobTypePieChartM2000User;



                //PriorityBy User
                List<Job> JobsM2000User = repository.Jobs.Where(j => j.JobTypeID == 2 && j.EngID == user.EngID
                                                   && (j.Status == "Working on it" || j.Status == "Cross Approval Pending"
                                                   || j.Status == "On Cross Approval" || j.Status == "Cross Approval Complete")).ToList();

                List<JobAdditional> MyJobAdditionalListUser = repository.JobAdditionals.Where(m => JobsM2000User.Any(n => n.JobID == m.JobID)).ToList();

                int NormalPriorityUser = MyJobAdditionalListUser.Where(m => m.Priority == 0).Count();
                int HighPriorityUser = MyJobAdditionalListUser.Where(m => m.Priority == 1).Count();
                int ShortLeadPriorityUser = MyJobAdditionalListUser.Where(m => m.Priority == 2).Count();


                List<double?> DataPieChartPriorityUser = new List<double?> { NormalPriorityUser, HighPriorityUser, ShortLeadPriorityUser };
                List<string> PriorityLabelsPiechartUser = new List<string> { "Normal", "High", "Short Lead" };

                var PieChartPriorityUser = new Chart { Type = Enums.ChartType.Pie };

                var dataPriorityUser = new Data { Labels = PriorityLabelsPiechartUser };

                var datasetPriorityUser = new PieDataset
                {
                    Label = "My dataset",
                    BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#6ADE4E"),
                    ChartColor.FromHexString("#FCF927"),
                    ChartColor.FromHexString("#F54242")
                },
                    HoverBackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#6ADE4E"),
                    ChartColor.FromHexString("#FCF927"),
                    ChartColor.FromHexString("#F54242")
                },
                    Data = DataPieChartPriorityUser
                };

                dataPriorityUser.Datasets = new List<Dataset> { datasetPriorityUser };

                PieChartPriorityUser.Data = dataPriorityUser;
                ViewData["PieChartPriority" + username] = PieChartPriorityUser;

                //Morning Report
                List<Job> JobsM2000ByEnginner = repository.Jobs.Where(j => j.JobTypeID == 2 && j.EngID == user.EngID
                                                   && (j.Status == "Working on it" || j.Status == "Cross Approval Pending"
                                                   || j.Status == "On Cross Approval" || j.Status == "Cross Approval Complete")).ToList();

                List<JobAdditional> MyJobAdditionalListByEnginner = repository.JobAdditionals.Where(m => JobsM2000ByEnginner.Any(n => n.JobID == m.JobID)).ToList();

                int NotReviewedMorning = MyJobAdditionalList.Where(m => m.Status == "Not Reviewed").Count();
                int WorkingOnItMorning = MyJobAdditionalList.Where(m => m.Status == "Working on it").Count();
                int MissingDatalMorning = MyJobAdditionalList.Where(m => m.Status == "Missing Data").Count();
                int OnSalesMorning = MyJobAdditionalList.Where(m => m.Status == "On Sales").Count();
                int CrossApprovalMorning = MyJobAdditionalList.Where(m => m.Status == "Cross Approval").Count();
                int ReleasedMorning = MyJobAdditionalList.Where(m => m.Status == "Released").Count();

                List<double?> DataPieChartMorning = new List<double?> { NotReviewedMorning, WorkingOnItMorning, MissingDatalMorning, OnSalesMorning, CrossApprovalMorning, ReleasedMorning };
                List<string> MorningLabelsPiechart = new List<string> { "Not Reviewed", "Working on it", "Missing Data", "On Sales", "Cross Approval", "Released" };

                var PieChartMorning = new Chart { Type = Enums.ChartType.Pie };

                var dataMorning = new Data { Labels = MorningLabelsPiechart };

                var datasetMorning = new PieDataset
                {
                    Label = "My dataset",
                    BackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#6ADE4E"),
                    ChartColor.FromHexString("#FCF927"),
                    ChartColor.FromHexString("#D441F8"),
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                    ChartColor.FromHexString("#5C50D1"),
                },
                    HoverBackgroundColor = new List<ChartColor>
                {
                    ChartColor.FromHexString("#6ADE4E"),
                    ChartColor.FromHexString("#FCF927"),
                    ChartColor.FromHexString("#D441F8"),
                    ChartColor.FromHexString("#FF6384"),
                    ChartColor.FromHexString("#36A2EB"),
                    ChartColor.FromHexString("#5C50D1"),
                },
                    Data = DataPieChartMorning
                };

                dataMorning.Datasets = new List<Dataset> { datasetMorning };

                PieChartMorning.Data = dataMorning;
                ViewData["PieChartMorning" + username] = PieChartMorning;
            }
            #endregion


            return View("AdminDashBoard", dashboard);
        }

        public ActionResult EngineerListDashBoard(string ChartName, string Sort = "default", int WorkingOnItM2000Page = 1, int PendingM2000Page = 1, int OnCrossM2000Page = 1, int CompleteM2000Page = 1,
                                                                                            int WorkingOnItHydroPage = 1, int CompleteHydroPage = 1, int WorkingOnItTractionPage = 1, int CompleteTractionPage = 1)
        {
            int PageSize = 12;
            AppUser currentUser = GetCurrentUser().Result;
            bool engineer = GetCurrentUserRole("EngAdmin").Result;
            if (ChartName != null) Sort = ChartName;


            List<JobType> JobTyPeList = itemRepo.JobTypes.ToList();
             List<JobAdditional> MyJobAdditionalList = repository.JobAdditionals.ToList();
             List<PO> POsList = repository.POs.ToList();


             //M2000
             List<Job> JobsWorkingOnItM2000 = repository.Jobs.Where(j => j.Status == "Working on it" && j.JobTypeID == 2  && j.EngID > 0 && j.EngID < 100)
                                                                .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

             List<Job> JobsCrossPendingM2000 = repository.Jobs.Where(j => j.Status == "Cross Approval Pending" && j.JobTypeID == 2 && j.EngID > 0 && j.EngID < 100)
                                                                .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

            List<Job> JobsOnCrossM2000 = repository.Jobs.Where(j => j.Status == "On Cross Approval" && j.JobTypeID == 2 && j.EngID > 0 && j.EngID < 100)
                                                                .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

            List<Job> JobsCrossCompleteM2000 = repository.Jobs.Where(j => j.Status == "Cross Approval Complete" && j.JobTypeID == 2 && j.EngID > 0 && j.EngID < 100)
                                                                .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();


            //Hydro
            List<Job> JobsWorkingOnItHydro = repository.Jobs.Where(j => j.Status == "Working on it" &&  j.JobTypeID == 1  && j.EngID > 0 && j.EngID < 100)
                                                                .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

            List<Job> JobsCrossCompleteHydro = repository.Jobs.Where(j => j.Status == "Cross Approval Complete" && j.JobTypeID == 1 && j.EngID > 0 && j.EngID < 100)
                                                                .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();


            //Traction
            List<Job> JobsWorkingOnItTraction = repository.Jobs.Where(j => j.Status == "Working on it" &&  j.JobTypeID == 5 && j.EngID > 0 && j.EngID < 100)
                                                                .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

            List<Job> JobsCrossCompleteTraction = repository.Jobs.Where(j => j.Status == "Cross Approval Complete" && j.JobTypeID == 5 && j.EngID > 0 && j.EngID < 100)
                                                                .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();




            EngineerChartsViewModel dashboard = new EngineerChartsViewModel()
                {
                    JobTypes = JobTyPeList,
                    POs = POsList,
                    MyJobAdditionals = MyJobAdditionalList,
                    ChartName = ChartName,

                    JobsWorkingOnItM2000 = JobsWorkingOnItM2000.Skip((WorkingOnItM2000Page - 1) * PageSize).Take(PageSize),
                    PagingInfoWorkingOnItM2000 = new PagingInfo
                    {
                        CurrentPage = WorkingOnItM2000Page,
                        ItemsPerPage = PageSize,
                        TotalItems = JobsWorkingOnItM2000.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },
                    JobsCrossPendingM2000 = JobsCrossPendingM2000.Skip((PendingM2000Page - 1) * PageSize).Take(PageSize),
                    PagingInfoCrossPendingM2000 = new PagingInfo
                    {
                        CurrentPage = PendingM2000Page,
                        ItemsPerPage = PageSize,
                        TotalItems = JobsCrossPendingM2000.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },

                    JobsOnCrossM2000 = JobsOnCrossM2000.Skip((OnCrossM2000Page - 1) * PageSize).Take(PageSize),
                    PagingInfoOnCrossM2000 = new PagingInfo
                    {
                        CurrentPage = OnCrossM2000Page,
                        ItemsPerPage = PageSize,
                        TotalItems = JobsOnCrossM2000.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },

                    JobsCrossCompleteM2000 = JobsCrossCompleteM2000.Skip((CompleteM2000Page - 1) * PageSize).Take(PageSize),
                    PagingInfoCrossCompleteM2000 = new PagingInfo
                    {
                        CurrentPage = CompleteM2000Page,
                        ItemsPerPage = PageSize,
                        TotalItems = JobsCrossCompleteM2000.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },

                    JobsWorkingOnItHydro = JobsWorkingOnItHydro.Skip((WorkingOnItHydroPage - 1) * PageSize).Take(PageSize),
                    PagingInfoWorkingOnItHydro = new PagingInfo
                    {
                        CurrentPage = WorkingOnItHydroPage,
                        ItemsPerPage = PageSize,
                        TotalItems = JobsWorkingOnItHydro.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },

                    JobsCrossCompleteHydro = JobsCrossCompleteHydro.Skip((CompleteHydroPage - 1) * PageSize).Take(PageSize),
                    PagingInfoCrossCompleteHydro = new PagingInfo
                    {
                        CurrentPage = CompleteHydroPage,
                        ItemsPerPage = PageSize,
                        TotalItems = JobsCrossCompleteHydro.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },


                    JobsWorkingOnItTraction = JobsWorkingOnItTraction.Skip((WorkingOnItTractionPage - 1) * PageSize).Take(PageSize),
                    PagingInfoWorkingOnItTraction = new PagingInfo
                    {
                        CurrentPage = WorkingOnItTractionPage,
                        ItemsPerPage = PageSize,
                        TotalItems = JobsWorkingOnItTraction.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },

                    JobsCrossCompleteTraction = JobsCrossCompleteTraction.Skip((CompleteTractionPage - 1) * PageSize).Take(PageSize),
                    PagingInfoCrossCompleteTraction = new PagingInfo
                    {
                        CurrentPage = CompleteTractionPage,
                        ItemsPerPage = PageSize,
                        TotalItems = JobsCrossCompleteTraction.Count(),
                        sort = Sort != "default" ? Sort : "deafult"

                    },


                };

                return View("EngineerListsChart", dashboard);
        }


        public string JobTypeName(int ID)
        {
            return itemRepo.JobTypes.FirstOrDefault(m => m.JobTypeID == ID).Name;
        }

    }

}
