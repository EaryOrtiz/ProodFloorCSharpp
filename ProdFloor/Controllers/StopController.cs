using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProdFloor.Models;
using ProdFloor.Models.ViewModels.TestJob;

namespace ProdFloor.Controllers
{
    [Authorize(Roles = "Admin")]
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
            Stop NewtStop = new Stop
            {
                TestJobID = Stop.TestJobID,
                Reason1ID = Stop.Reason1ID,
                Reason2ID = 0,
                Reason3ID = 0,
                Reason4ID = 0,
                Reason5ID = 0,
                Description = null,
                StartDate = DateTime.Now,
                StopDate = DateTime.Now,
                Elapsed = new TimeSpan(0,0,0)
            };

            testingRepo.SaveStop(NewtStop);
            Stop CurrentStop = testingRepo.Stops.FirstOrDefault(p => p.StopID == testingRepo.Stops.Max(x => x.StopID));
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStop.TestJobID);
            testJob.Status = "Stoped";
            testingRepo.SaveTestJob(testJob);
            Job job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testJob.JobID);
            return View("WaitingForRestar", new TestJobViewModel {  Job = job, Stop = CurrentStop });
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
            return View(new TestJobViewModel { Job = job, Stop = CurrentStop});
        }

        [HttpPost]
        public IActionResult RestarTestJob(TestJobViewModel viewModel)
        {
            TimeSpan auxTime = (DateTime.Now - viewModel.Stop.StartDate);
            Stop UpdatedStop = new Stop
            {
                StopID = viewModel.Stop.StopID,
                TestJobID = viewModel.Stop.TestJobID,
                Reason1ID = viewModel.Stop.Reason1ID,
                Reason2ID = viewModel.Stop.Reason2ID,
                Reason3ID = viewModel.Stop.Reason3ID,
                Reason4ID = viewModel.Stop.Reason4ID,
                Reason5ID = viewModel.Stop.Reason5ID,
                StartDate = viewModel.Stop.StartDate,
                StopDate = DateTime.Now,
                Elapsed = auxTime
            };
            testingRepo.SaveStop(UpdatedStop);
            TestJob testJob = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == UpdatedStop.TestJobID);
            testJob.Status = "Working on it";
            testingRepo.SaveTestJob(testJob);
            List<StepsForJob> stepsList = testingRepo.StepsForJobs.Where(m => m.TestJobID == testJob.TestJobID).OrderBy(m => m.Consecutivo).ToList();
            StepsForJob CurrentStep = stepsList.FirstOrDefault(m => m.Complete == false); CurrentStep.Start = DateTime.Now;
            testingRepo.SaveStepsForJob(CurrentStep);
            var stepInfo = testingRepo.Steps.FirstOrDefault(m => m.StepID == CurrentStep.StepID);
            var testjobinfo = testingRepo.TestJobs.FirstOrDefault(m => m.TestJobID == CurrentStep.TestJobID);
            var job = jobRepo.Jobs.FirstOrDefault(m => m.JobID == testjobinfo.JobID);
            return View("StepsForJob", new TestJobViewModel { StepsForJob = CurrentStep, Step = stepInfo, Job = job, TestJob = testjobinfo });
        }
    }
}