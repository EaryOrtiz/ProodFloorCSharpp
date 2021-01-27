﻿using Microsoft.Extensions.Hosting;
using ProdFloor.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ProdFloor.Services
{
    public class DailyJobsBackupHostedService : IHostedService, IDisposable
    {
        private JobController jobController;
        private readonly IHostingEnvironment environment;
        private Timer timer;
        private int count = 0;

        public DailyJobsBackupHostedService(JobController jobCtrl, IHostingEnvironment environment)
        {
            jobController = jobCtrl;
            this.environment = environment;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer = new Timer(DoWork, null, TimeSpan.FromHours(1), TimeSpan.FromDays(7));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            if (count == 0)
                timer.Change(TimeSpan.FromMilliseconds(0), TimeSpan.FromDays(7)); 

            jobController.ExportJobsToXML();
            Console.WriteLine("Jobs Saved");
            count++;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Daily jobs backup stoped");
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
