using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ProdFloor.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ProdFloor.Controllers;
using ProdFloor.Services;
using Microsoft.AspNetCore.Hosting.Internal;

namespace ProdFloor
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                Configuration["Data:ProdFloorJobs:ConnectionString"]));

            services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseSqlServer(
                Configuration["Data:ProdFloorIdentity:ConnectionString"]));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddIdentity<AppUser, IdentityRole>(opts => {
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = true;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireDigit = true;
            })
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IJobRepository, EFJobRepository>();
            services.AddTransient<IItemRepository, EFItemRepository>();
            services.AddTransient<ITestingRepository, EFTestingRepository>();
            services.AddTransient<ItemController>();
            services.AddTransient<AccountController>();
            services.AddTransient<JobController>();
            //services.AddSingleton<IHostingEnvironment>(new HostingEnvironment());
            services.AddTransient<TestJobController>();
            services.AddTransient<Microsoft.Extensions.Hosting.IHostedService, DailyJobsBackupHostedService>();
           // services.AddTransient<Microsoft.Extensions.Hosting.IHostedService, ShiftEndHostedService>();
            services.AddMvc()
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);
            services.AddMemoryCache();
            services.AddSession();
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });

            // Build the intermediate service provider
            var serviceProvider = services.BuildServiceProvider();
            //return the provider
            return serviceProvider;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            env.EnvironmentName = EnvironmentName.Development;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }


            app.UseHttpsRedirection();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            //app.UseDeveloperExceptionPage();
            app.UseStatusCodePagesWithReExecute("/StatusCode/{0}");
            //app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseMvc(routes => {


                routes.MapRoute(
                    name: null,
                    template: "{MyJobsPage:int}_{OnCrossJobPage:int}_{PendingToCrossJobPage:int}_{JobNumb}",
                    defaults: new { controller = "TestJob", action = "SearchTestJob" });

                routes.MapRoute(
                    name: null,
                    template: "Home/{MyJobsPage:int}_{OnCrossJobPage:int}_{PendingToCrossJobPage:int}_{ActiveJobPage:int}_{Sort}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: null,
                    template: "Home/{MyJobsPage:int}_{OnCrossJobPage:int}_{PendingToCrossJobPage:int}_{Sort}",
                    defaults: new { controller = "Home", action = "Index" });

               
                routes.MapRoute(
                    name: null,
                    template: "{ElmHydroPage:int}_{ElmTractionPage:int}_{M2000Page:int}_{M4000Page:int}_{JobTypeName}",
                    defaults: new { controller = "Step", action = "List" });

                routes.MapRoute(
                    name: null,
                    template: "{MyJobsPage:int}_{OnCrossJobPage:int}_{PendingToCrossJobPage:int}",
                    defaults: new{controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: null,
                    template: "{EngineerAdminDashBoard}/{MyJobsPage:int}_{OnCrossJobPage:int}_{PendingToCrossJobPage:int}",
                    defaults: new { controller = "Home", action = "EngineerAdminDashBoard" });

                routes.MapRoute(
                    name: null,
                    template: "{pendingJobPage:int}_{productionJobPage:int}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: null,
                    template: "AllStepsForJob/{ID:int}/{Page:int}",
                    defaults: new { controller = "TestJob", action = "AllStepsForJob" });

                routes.MapRoute(
                    name: null,
                    template: "StopsFromTestJob/{ID:int}/{Page:int}",
                    defaults: new { controller = "TestJob", action = "StopsFromTestJob" });

                routes.MapRoute(
                    name: null,
                    template: "TestStatsAux/{JobType}",
                    defaults: new { controller = "TestJob", action = "TestStatsAux" });

                routes.MapRoute(
                    name: null,
                    template: "TestStats/{JobType}",
                    defaults: new { controller = "TestJob", action = "TestStats" });


                routes.MapRoute(
                    name: null,
                    template: "CrossHub/{pendingJobPage:int}_{productionJobPage:int}",
                    defaults: new { controller = "Home", action = "CrossHub" });

                routes.MapRoute(
                    name: null,
                    template: "MyjobsList/Page{page:int}",
                    defaults: new
                    {
                        controller = "Job",
                        action = "MyjobsList",
                        jobPage = 1
                    }
                );

                routes.MapRoute(
                    name: null,
                    template: "Job/List/Page{page:int}",
                    defaults: new{ controller = "Job", action = "List",
                        jobPage = 1 }
                );

                routes.MapRoute(
                    name: null,
                    template: "{jobType}",
                    defaults: new { controller = "Job", action = "List",
                        jobPage = 1 }
                );

                routes.MapRoute(
                    name: null,
                    template: "",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(name: null, template: "{controller}/{action}/ReasonNumber{reasonNumber:int}/Page{page:int}");

                routes.MapRoute(name: null, template: "{controller}/{action}/Page{page:int}");


                routes.MapRoute(name: null, template: "{controller}/{action}/{id?}");

                routes.MapRoute(
                 name: null,
                    template: "{controller=Job}/{action=JobSearchList}/{id?}");
            });
            app.UseCookiePolicy();
            //SeedData.EnsurePopulated(app);
            //IdentitySeedData.EnsurePopulated(app);
        }
    }
}
