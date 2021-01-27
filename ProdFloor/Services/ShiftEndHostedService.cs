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
        private int count = 0;

        public ShiftEndHostedService(TestJobController testCtrl, IHostingEnvironment environment)
        {
            testController = testCtrl;
            this.environment = environment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            if (count == 0)
                timer.Change(TimeSpan.FromMilliseconds(0), TimeSpan.FromMinutes(1));

            testController.AutomaticShiftEnd();
            Console.WriteLine("ShiftEnd completed");
            count++;
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
