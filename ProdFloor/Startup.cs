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

namespace ProdFloor
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                Configuration["Data:ProdFloorJobs:ConnectionString"]));

            services.AddDbContext<AppIdentityDbContext>(options =>
            options.UseSqlServer(
                Configuration["Data:ProdFloorIdentity:ConnectionString"]));

            services.AddIdentity<AppUser, IdentityRole>(opts => {
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = true;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireDigit = true;})
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IJobRepository, EFJobRepository>();
            services.AddTransient<IItemRepository, EFItemRepository>();
            services.AddMvc();
            services.AddMemoryCache();
            services.AddSession();
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });
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
                app.UseExceptionHandler("/error");
            }

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
                    template: "{MyJobsPage:int}_{OnCrossJobPage:int}_{PendingToCrossJobPage:int}_{Sort}",
                    defaults: new { controller = "Home", action = "Index" });

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
                    template: "CrossHub/{pendingJobPage:int}_{productionJobPage:int}_{onCrossJobPage:int}",
                    defaults: new { controller = "Home", action = "CrossHub" });
                routes.MapRoute(
                    name: null,
                    template: "{pendingJobPage:int}_{productionJobPage:int}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: null,
                    template: "CrossHub/{pendingJobPage:int}_{productionJobPage:int}",
                    defaults: new { controller = "Home", action = "CrossHub" });

                routes.MapRoute(
                    name: null,
                    template: "MyjobsList/Page{jobPage:int}",
                    defaults: new
                    {
                        controller = "Job",
                        action = "MyjobsList",
                        jobPage = 1
                    }
                );

                routes.MapRoute(
                    name: null,
                    template: "List/Page{jobPage:int}",
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

                routes.MapRoute(name: null, template: "{controller}/{action}/Page{page:int}");

                routes.MapRoute(name: null, template: "{controller}/{action}/{id?}");

                routes.MapRoute(
                 name: null,
                    template: "{controller=Job}/{action=JobSearchList}/{id?}");
            });
            //SeedData.EnsurePopulated(app);
            //IdentitySeedData.EnsurePopulated(app);
        }
    }
}
