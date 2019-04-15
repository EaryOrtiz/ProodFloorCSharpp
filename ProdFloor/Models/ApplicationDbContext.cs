using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace ProdFloor.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        //Job
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobExtension> JobsExtensions { get; set; }
        public DbSet<HydroSpecific> HydroSpecifics { get; set; }
        public DbSet<GenericFeatures> GenericFeaturesList { get; set; }
        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<HoistWayData> HoistWayDatas { get; set; }

        //Items
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<DoorOperator> DoorOperators { get; set; }
        public DbSet<FireCode> FireCodes { get; set; }
        public DbSet<JobType> JobTypes { get; set; }
        public DbSet<LandingSystem> LandingSystems { get; set; }
        public DbSet<SpecialFeatures> SpecialFeatures { get; set; }


        //Enginner Refernces Tables
        public DbSet<Slowdown> Slowdowns { get; set; }
        public DbSet<WireTypesSize> WireTypesSizes { get; set; }
        public DbSet<Starter> Starters { get; set; }
        public DbSet<Overload> Overloads { get; set; }

        //Testing
        public DbSet<TestJob> TestJobs { get; set; }
        public DbSet<TestFeature> TestFeatures { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<TriggeringFeature> TriggeringFeatures { get; set; }
        public DbSet<StepsForJob> StepsForJobs { get; set; }
        public DbSet<Reason1> Reasons1 { get; set; }
        public DbSet<Reason2> Reasons2 { get; set; }
        public DbSet<Reason3> Reasons3 { get; set; }
        public DbSet<Reason4> Reasons4 { get; set; }
        public DbSet<Reason5> Reasons5 { get; set; }
        public DbSet<Stop> Stops { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Job>()
                .HasIndex(u => u.PO)
                .IsUnique();
        }
        public class ApplicationDbContextFactory
            : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            public ApplicationDbContext CreateDbContext(string[] args) =>
                Program.BuildWebHost(args).Services
                    .GetRequiredService<ApplicationDbContext>();
        }
    }
}
