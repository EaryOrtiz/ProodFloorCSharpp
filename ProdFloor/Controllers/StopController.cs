using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels;
using ProdFloor.Models.ViewModels.TestJob;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin,Technician,Enginner")]
    public class StopController : Controller
    {
        private IJobRepository jobRepo;
        private ITestingRepository testingRepo;
        private UserManager<AppUser> userManager;
        public int PageSize = 10;

        public StopController(ITestingRepository repo, IJobRepository repo2, UserManager<AppUser> userMgr)
        {
            jobRepo = repo2;
            testingRepo = repo;
            userManager = userMgr;
        }

        public ViewResult List(int page = 1)
            => View(new TestJobViewModel
            {
                StopList = testingRepo.Stops
                .OrderBy(p => p.StopID)
                .Skip((page - 1) * PageSize)
                .Take(PageSize).ToList(),
                JobList = jobRepo.Jobs.ToList(),
                TestJobList = testingRepo.TestJobs.ToList(),
                Reasons1List = testingRepo.Reasons1.ToList(),
                Reasons2List = testingRepo.Reasons2.ToList(),
                Reasons3List = testingRepo.Reasons3.ToList(),
                Reasons4List = testingRepo.Reasons4.ToList(),
                Reasons5List = testingRepo.Reasons5.ToList(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = testingRepo.Stops.Count()
                }
            });

        private async Task<AppUser> GetCurrentUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            return user;
        }

        public ViewResult NewStop(int ID)
        {
            return View(new Stop { TestJobID = ID});
        }

        [HttpPost]
        public ViewResult NewStop(Stop Stop)
        {
            bool admin = GetCurrentUserRole("Admin").Result;
            Stop NewtStop = new Stop
            {
                TestJobID = Stop.TestJobID,
                Reason1 = Stop.Reason1,
                Reason2 = 0,
                Reason3 = 0,
                Reason4 = 0,
                Reason5ID = 0,
                Description = null,
                StartDate = DateTime.Now,
                StopDate = DateTime.Now,
                Elapsed = new DateTime(1, 1, 1, 0, 0, 0)
        };

            testingRepo.SaveStop(NewtStop);
            Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.StopID == testingRepo.Stops.Max(x => x.StopID));
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStop.TestJobID);
            testJob.Status = "Stoped";
            testingRepo.SaveTestJob(testJob);
            Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
            if (admin)
            {
                Stop CurrentStop2 = testingRepo.Stops.FirstOrDefault(p => p.StopID == CurrentStop.StopID);
                TestJob testJob2 = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStop.TestJobID);
                Job job2 = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
                return View("Edit", new TestJobViewModel { Job = job2, Stop = CurrentStop2, TestJob = testJob2 });
            }
            return View("WaitingForRestar", new TestJobViewModel {  Job = job, Stop = CurrentStop, TestJob = testJob});
        }

        public ViewResult WaitingForRestar(int ID)
        {
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == ID);
            Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.TestJobID == testJob.TestJobID && p.StopID == testingRepo.Stops.Max(x => x.StopID));
            testJob.Status = "Stoped";
            testingRepo.SaveTestJob(testJob);
            Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
            return View(new TestJobViewModel { Job = job, Stop = CurrentStop });
        }

        public ViewResult RestarTestJob(int ID)
        {
            Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.StopID == ID);
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStop.TestJobID);
            Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
            return View(new TestJobViewModel { Job = job, Stop = CurrentStop, TestJob = testJob});
        }

        [HttpPost]
        public IActionResult RestarTestJob(TestJobViewModel viewModel)
        {
            TimeSpan auxTime = (DateTime.Now - viewModel.Stop.StartDate);
            viewModel.Stop.Elapsed += auxTime;
            Stop UpdatedStop = new Stop
            {
                StopID = viewModel.Stop.StopID,
                TestJobID = viewModel.Stop.TestJobID,
                Reason1 = viewModel.Stop.Reason1,
                Reason2 = viewModel.Stop.Reason2,
                Reason3 = viewModel.Stop.Reason3,
                Reason4 = viewModel.Stop.Reason4,
                Reason5ID = viewModel.Stop.Reason5ID,
                Description = viewModel.Stop.Description,
                StartDate = viewModel.Stop.StartDate,
                StopDate = DateTime.Now,
                Elapsed = viewModel.Stop.Elapsed
            };
            testingRepo.SaveStop(UpdatedStop);
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == UpdatedStop.TestJobID);
            testJob.Status = "Working on it";
            testingRepo.SaveTestJob(testJob);
            List<StepsForJob> StepsForJobList = testingRepo.StepsForJobs.FromSql("select * from dbo.StepsForJobs where dbo.StepsForJobs.StepsForJobID " +
               "IN( select  Max(dbo.StepsForJobs.StepsForJobID ) from dbo.StepsForJobs where dbo.StepsForJobs.TestJobID = {0} group by dbo.StepsForJobs.Consecutivo)", testJob.TestJobID).ToList();
            var AllStepsForJobInfo = testingRepo.Steps.Where(m => StepsForJobList.Any(s => s.StepID == m.StepID)).ToList();
            StepsForJob CurrentStep = StepsForJobList.FirstOrDefault(m => m.Complete == false); CurrentStep.Start = DateTime.Now;
            testingRepo.SaveStepsForJob(CurrentStep);
            var stepInfo = testingRepo.Steps.FirstOrDefault(m => m.StepID == CurrentStep.StepID);
            var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStep.TestJobID);
            var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);
            return View("StepsForJob", new TestJobViewModel { StepsForJob = CurrentStep, Step = stepInfo, Job = job, TestJob = testjobinfo, StepList = AllStepsForJobInfo, StepsForJobList = StepsForJobList });
        }

        public ViewResult Edit(int ID)
        {
            Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.StopID == ID);
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStop.TestJobID);
            Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
            return View(new TestJobViewModel { Job = job, Stop = CurrentStop, TestJob = testJob });
        }

        [HttpPost]
        public IActionResult Edit(Stop stop)
        {
            if (ModelState.IsValid)
            {
                TimeSpan auxTime = (DateTime.Now - stop.StartDate);
                stop.Elapsed += auxTime;
                testingRepo.SaveStop(stop);
                TempData["message"] = $"{stop.Description} has been saved..";
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(stop);
            }
        }

        [HttpPost]
        public IActionResult Delete(int ID)
        {
            Stop deletedStop = testingRepo.DeleteStop(ID);

            if (deletedStop != null)
            {
                TempData["message"] = $"{deletedStop.StopID} was deleted";
            }
            return RedirectToAction("List");
        }

        private async Task<bool> GetCurrentUserRole(string role)
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);

            bool isInRole = await userManager.IsInRoleAsync(user, role);

            return isInRole;
        }
    }
}