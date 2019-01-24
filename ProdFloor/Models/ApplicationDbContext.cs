using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace ProdFloor.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<JobExtension> JobsExtensions { get; set; }

        public DbSet<HydroSpecific> HydroSpecifics { get; set; }

        public DbSet<GenericFeatures> GenericFeaturesList { get; set; }

        public DbSet<Indicator> Indicators { get; set; }

        public DbSet<HoistWayData> HoistWayDatas { get; set; }

        public DbSet<Country> Countries { get; set; }

        public DbSet<State> States { get; set; }

        public DbSet<City> Cities { get; set; }

        public DbSet<DoorOperator> DoorOperators { get; set; }

        public DbSet<FireCode> FireCodes { get; set; }

        public DbSet<JobType> JobTypes { get; set; }

        public DbSet<LandingSystem> LandingSystems { get; set; }

        public DbSet<SpecialFeatures> SpecialFeatures { get; set; }

        public class ApplicationDbContextFactory
            : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args) =>
                Program.BuildWebHost(args).Services
                    .GetRequiredService<ApplicationDbContext>();
        }
    }
}
