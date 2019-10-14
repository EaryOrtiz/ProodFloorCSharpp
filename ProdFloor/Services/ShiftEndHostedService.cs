using Microsoft.Extensions.Hosting;
using ProdFloor.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProdFloor.Services
{
    public class ShiftEndHostedService : IHostedService, IDisposable
    {
        private TestJobController testController;
        private readonly IHostingEnvironment environment;
        private Timer timer;

        public ShiftEndHostedService(TestJobController testCtrl, IHostingEnvironment environment)
        {
            testController = testCtrl;
            this.environment = environment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.FromMinutes(30), TimeSpan.FromHours(10));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            testController.AutomaticShiftEnd();
            Console.WriteLine("ShiftEnd completed");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("ShiftEnd stoped");
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
