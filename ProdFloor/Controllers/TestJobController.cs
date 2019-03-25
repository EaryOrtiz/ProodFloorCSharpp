using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.TestJob;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TestJobController : Controller
    {
        private IJobRepository jobRepo;
        private ITestingRepository testingRepo;
        private UserManager<AppUser> userManager;
        public int PageSize = 10;

        public TestJobController(ITestingRepository repo, IJobRepository repo2, UserManager<AppUser> userMgr)
        {
            jobRepo = repo2;
            testingRepo = repo;
            userManager = userMgr;
        }

        public ViewResult List(int page = 1)
        {
            AppUser currentUser = GetCurrentUser().Result;
            TestJobViewModel testJobView = new TestJobViewModel
            {
                TestJobList = testingRepo.TestJobs
                .Where(m => m.TechnicianID == currentUser.EngID)
                .OrderBy(p => p.TechnicianID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = testingRepo.TestJobs.Count()
                }
            };
            return View(testJobView);
        }

        public ViewResult NewTestJob()
        {
            return View(new TestJobViewModel());
        }

        public IActionResult SearchJob(TestJobViewModel viewModel)
        {
            var jobSearch = jobRepo.Jobs.AsQueryable();
            TestJobViewModel testJobSearchAux = new TestJobViewModel { };

            if (viewModel.NumJobSearch > 0)
            {
                var _jobSearch = jobSearch.Where(m => m.JobNum == viewModel.NumJobSearch);

                if (_jobSearch.Count() > 0)
                {
                    List<Job> Jobs = new List<Job>();
                    foreach (Job job in _jobSearch) if (job.Status != "Incomplete") Jobs.Add(job);

                    if (Jobs.Count() != 0)
                    {
                        TestJobViewModel testJobView = new TestJobViewModel
                        {
                            JobList = Jobs,
                            PagingInfo = new PagingInfo
                            {
                                CurrentPage = 1,
                                ItemsPerPage = PageSize,
                                TotalItems = Jobs.Count()
                            }
                        };

                        return View("NewTestJob",testJobView);
                    }

                    TempData["alert"] = $"alert-danger";
                    TempData["message"] = $"These jobs aren't completed, please finish it or create a dummy job and try again";


                    return RedirectToAction("NewTestJob", testJobSearchAux);

                }

                TempData["alert"] = $"alert-danger";
                TempData["message"] = $"That job doesn't exist, please enter a new one or create a dummy job";

                return RedirectToAction("NewTestJob", testJobSearchAux);
            }
            else
            {
                return View(testJobSearchAux);
            }
        }

        private async Task<AppUser> GetCurrentUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            return user;
        }

    }
}