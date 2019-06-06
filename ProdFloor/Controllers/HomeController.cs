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
    [Authorize(Roles ="Admin,Engineer")]
    public class HomeController : Controller
    {
        private IJobRepository repository;
        public int PageSize = 3;
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

        public ActionResult Index(string filtrado, string Sort = "default", int MyJobsPage = 1, int OnCrossJobPage = 1, int PendingToCrossJobPage = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            bool engineer = GetCurrentUserRole("Engineer").Result;
            if (filtrado != null) Sort = filtrado;
            if (engineer)
            {

                List<Job> MyjobsList = repository.Jobs
                    .Where(j => j.EngID == currentUser.EngID)
                    .Where(j => j.Status == "Incomplete" || j.Status == "Not Reviewed" || j.Status == "Working on it" || j.Status == "Cross Approval Complete").ToList();

                List<Job> OnCrossJobsList = repository.Jobs
                        .Where(j => j.CrossAppEngID == currentUser.EngID)
                        .Where(j => j.Status == "On Cross Approval").ToList();

                List<Job> PendingToCrossJobList = repository.Jobs
                        .Where(j => j.EngID != currentUser.EngID)
                        .Where(j => j.Status == "Cross Approval Pending").ToList();

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
                    MyJobsPagingInfo = new PagingInfo
                    {
                        CurrentPage = MyJobsPage,
                        ItemsPerPage = PageSize,
                        TotalItems = MyjobsList.Count(),
                        sort = Sort != "default" ? Sort : "deafult"
                    },

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

            return View("AdminDashBoard", new DashboardIndexViewModel
            {
                MyJobs = repository.Jobs
                  .OrderBy(p => p.JobID)
                  .Skip((MyJobsPage - 1) * PageSize)
                  .Take(PageSize),
                MyJobsPagingInfo = new PagingInfo
                {
                    CurrentPage = MyJobsPage,
                    ItemsPerPage = PageSize,
                    TotalItems = repository.Jobs.Count()
                },
                OnCrossJobs = repository.Jobs
                  .OrderBy(p => p.JobID)
                  .Skip((OnCrossJobPage - 1) * PageSize)
                  .Take(PageSize),
                OnCrossJobsPagingInfo = new PagingInfo
                {
                    CurrentPage = PendingToCrossJobPage,
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

        [HttpPost]
        public IActionResult ToCross(int JobCrossID, DashboardIndexViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            Job UpdateStatus = repository.Jobs.FirstOrDefault(j => j.JobID == JobCrossID);
            if (UpdateStatus != null)
            {
                if (viewModel.buttonAction == "ToCross" && currentUser.EngID == UpdateStatus.EngID)
                {
                    UpdateStatus.Status = "Cross Approval Pending";
                    repository.SaveJob(UpdateStatus);

                    TempData["message"] = $"You have released the Job #{UpdateStatus.JobNum} to Cross Approval";
                }
                else if (viewModel.buttonAction == "CrossApproved" && currentUser.EngID == UpdateStatus.CrossAppEngID)
                {
                    UpdateStatus.Status = "Cross Approval Complete";
                    repository.SaveJob(UpdateStatus);

                    TempData["message"] = $"You have approved the Job #{UpdateStatus.JobNum}";
                }
                else if (viewModel.buttonAction == "ToProduction" && currentUser.EngID == UpdateStatus.EngID)
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
    }

}
