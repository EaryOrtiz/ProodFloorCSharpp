using Microsoft.Extensions.Hosting;
using ProdFloor.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProdFloor.Services
{
    public class HostedServices : IHostedService, IDisposable
    {
        private TestJobController testController;
        private JobController jobController;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment environment;
        private Timer timer;
        private Timer timer2;

        public HostedServices(TestJobController testCtrl, JobController jobCtrl, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment)
        {
            testController = testCtrl;
            jobController = jobCtrl;
            this.environment = environment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(AutomaticShiftEnd, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            timer2 = new Timer(DailyJobs, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;
        }

        private void AutomaticShiftEnd(object state)
        {
            testController.AutomaticShiftEnd();
            Console.WriteLine("ShiftEnd completed");
        }

        private void DailyJobs(object state)
        {
            jobController.ExportJobsToXML();
            Console.WriteLine("Jobs Saved");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("ShiftEnd stoped");
            timer?.Change(Timeout.Infinite, 0);
            timer2?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
            timer2?.Dispose();
        }
    }
}
