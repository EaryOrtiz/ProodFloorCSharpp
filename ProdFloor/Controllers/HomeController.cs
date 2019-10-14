﻿using Microsoft.AspNetCore.Mvc;
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

namespace ProdFloor.Controllers
{

    [Authorize(Roles = "Admin,TechAdmin,Engineer,Technician")]
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
                    .Where(j => j.Status == "Stopped" || j.Status == "Working on it" || j.Status == "Reassignment")
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

        public ActionResult EngineerAdminDashBoard(string filtrado, string Sort = "default", int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1)
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
                    MyJobs = MyjobsList.Skip((MyJobsPage - 1) * 6).Take(6),
                    MyJobAdditionals = MyJobAdditionalList,
                    OnCrossJobAdditionals = OnCrossJobAdditionalList,
                    PendingJobAdditionals = PendingJobAdditionalList,
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
                };

                return View("EngineerAdminDashBoard", dashboard);
            }

            return NotFound();
        }

        public IActionResult SuperUserDashBoard(string Clean, int jobNumber, string jobnumb = "0", int MyJobsPage = 1, int PendingToCrossJobPage = 1, int OnCrossJobPage = 1)
        {
            if (jobnumb != "0") jobNumber = int.Parse(jobnumb);
            if (jobNumber != 0) jobnumb = jobNumber.ToString();

            List<JobType> JobTyPeList = itemRepo.JobTypes.ToList();
            List<PO> POsList = repository.POs.ToList();
            List<JobAdditional> MyJobAdditionalList = repository.JobAdditionals.ToList();
            List<Job> MyjobsList = new List<Job>();
            List<Job> jobList = repository.Jobs.Where(m => m.JobNum == jobNumber).ToList();
            List<Job> OnCrossJobsList = repository.Jobs
                       .Where(j => j.Status == "On Cross Approval").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();
            

            List<Job> PendingToCrossJobList = repository.Jobs
                    .Where(j => j.Status == "Cross Approval Pending").OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();

            if (jobList != null && jobList.Count > 0)
            {
                MyjobsList = jobList
                    .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();
            }

            if (Clean == "true" || jobnumb == "0") MyjobsList = repository.Jobs
                    .OrderByDescending(m => m._JobAdditional.Priority).ThenBy(n => n.LatestFinishDate).ToList();


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

            if (jobNumber == 0) return View(dashboard);
            if (MyjobsList.Count > 0 && MyjobsList[0] != null) return View(dashboard);
            TempData["message"] = $"Does not exist any job with the JobNum #{jobNumber}, please try again.";
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
            if (UpdateStatus != null)
            {
                if (viewModel.buttonAction == "ToCross" && currentUser.EngID == UpdateStatus.EngID)
                {
                    if(UpdateStatus.Status == "Working on it")
                    {
                        UpdateStatus.Status = "Cross Approval Pending";
                        repository.SaveJob(UpdateStatus);

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
            if ((job.EngID != viewModel.CurrentEngID) && (job.CrossAppEngID != 0 && job.CrossAppEngID != viewModel.CurrentCrosAppEngID))
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
            else if (job.CrossAppEngID != 0 && job.CrossAppEngID != viewModel.CurrentCrosAppEngID)
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

        public string JobTypeName(int ID)
        {
            return itemRepo.JobTypes.FirstOrDefault(m => m.JobTypeID == ID).Name;
        }

    }

}
