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

        [HttpPost]
        public IActionResult SearchJob(TestJobViewModel viewModel)
        {
            AppUser currentUser = GetCurrentUser().Result;
            var jobSearch = jobRepo.Jobs.AsQueryable();
            TestJobViewModel testJobSearchAux = new TestJobViewModel { };

            if (viewModel.POJobSearch > 0)
            {
                var _jobSearch = jobSearch.First(m => m.PO == viewModel.POJobSearch);

                if (_jobSearch != null && _jobSearch.Status != "Incomplete")
                {

                    TestJob testJob = new TestJob {JobID = _jobSearch.JobID,TechnicianID = currentUser.EngID, Status = "Working on it"};
                    testingRepo.SaveTestJob(testJob);

                    var currentTestJob = testingRepo.TestJobs
                        .FirstOrDefault(p => p.TestJobID == testingRepo.TestJobs.Max(x => x.TestJobID));


                    TestJobViewModel testJobView = new TestJobViewModel
                    {
                        TestJob = currentTestJob,
                    };

                    return View("NewTestFeatures", testJobView);

                }
                else
                {
                    TempData["message"] = $"There seems to be errors in the form. Please validate....";
                    TempData["alert"] = $"alert-danger";
                    return RedirectToAction("NewTestFeatures", testJobSearchAux);
                }
            }
            else
            {
                return View(testJobSearchAux);
            }
        }

        [HttpPost]
        public IActionResult NewTestFeatures(TestJobViewModel testJobView)
        {
            if(testJobView.TestFeature != null)
            {
                testingRepo.SaveTestFeature(testJobView.TestFeature);

                return RedirectToAction(nameof(List));
            }

            return NotFound();
        }
        private async Task<AppUser> GetCurrentUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            return user;
        }

    }
}