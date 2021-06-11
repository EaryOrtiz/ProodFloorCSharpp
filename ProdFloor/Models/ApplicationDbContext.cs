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
        public DbSet<PlanningReport> PlanningReports { get; set; }
        public DbSet<PlanningReportRow> PlanningReportRows { get; set; }

        //Enginner Refernces Tables

        public DbSet<PO> POs { get; set; }
        public DbSet<StatusPO> StatusPOs { get; set; }
        public DbSet<JobAdditional> JobAdditionals { get; set; }
        public DbSet<CustomSoftware> CustomSoftwares { get; set; }
        public DbSet<TriggeringCustSoft> TriggeringCustSofts { get; set; }
        public DbSet<CustomFeature> CustomFeatures { get; set; }
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
        public DbSet<Station> Stations { get; set; }

        //Nuevos Modelos
        public DbSet<Element> Elements { get; set; }
        public DbSet<ElementHydro> ElementHydros { get; set; }
        public DbSet<ElementTraction> ElementTractions { get; set; }


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

        //WirerPXP
        public DbSet<WiringPXP> WiringPXPs { get; set; }
        public DbSet<PXPError> PXPErrors { get; set; }
        public DbSet<PXPReason> PXPReasons { get; set; }
        public DbSet<WirersPXPInvolved> WirersPXPInvolveds { get; set; }


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
                Program.CreateWebHostBuilder(args).Build().Services
                    .GetRequiredService<ApplicationDbContext>();
        }
    }
}
