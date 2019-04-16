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
        public DbSet<PO> POs { get; set; }
        public DbSet<Slowdown> Slowdowns { get; set; }
        public DbSet<WireTypesSize> WireTypesSizes { get; set; }
        public DbSet<Starter> Starters { get; set; }
        public DbSet<Overload> Overloads { get; set; }

        //ItemHistory
        public DbSet<Country_audit> Country_Audits { get; set; }
        public DbSet<State_audit> State_Audits { get; set; }
        public DbSet<City_audit> City_Audits { get; set; }
        public DbSet<FireCode_audit> FireCode_Audits { get; set; }
        public DbSet<DoorOperator_audit> DoorOperator_Audits { get; set; }
        public DbSet<JobType_audit> JobType__Audits { get; set; }
        public DbSet<LandingSystem_audit> LandingSystem_Audits { get; set; }
        public DbSet<Starter_audit> Starter_Audits { get; set; }
        public DbSet<Slowdown_audit> Slowdown__Audits { get; set; }
        public DbSet<Overload_audit> Overload_Audits { get; set; }
        public DbSet<WireTypeSize_audit> WireTypeSize_Audits { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<PO>()
                .HasIndex(u => u.PONumb)
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
