﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using ProdFloor.Models;
using System;

namespace ProdFloor.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190122222610_ChangesDbExtra01222019")]
    partial class ChangesDbExtra01222019
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ProdFloor.Models.City", b =>
                {
                    b.Property<int>("CityID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("FirecodeID");

                    b.Property<string>("Name");

                    b.Property<int>("StateID");

                    b.HasKey("CityID");

                    b.HasIndex("FirecodeID");

                    b.HasIndex("StateID");

                    b.ToTable("Cities");
                });

            modelBuilder.Entity("ProdFloor.Models.Country", b =>
                {
                    b.Property<int>("CountryID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("CountryID");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("ProdFloor.Models.DoorOperator", b =>
                {
                    b.Property<int>("DoorOperatorID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Brand");

                    b.Property<string>("Name");

                    b.Property<string>("Style");

                    b.HasKey("DoorOperatorID");

                    b.ToTable("DoorOperators");
                });

            modelBuilder.Entity("ProdFloor.Models.FireCode", b =>
                {
                    b.Property<int>("FireCodeID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("FireCodeID");

                    b.ToTable("FireCodes");
                });

            modelBuilder.Entity("ProdFloor.Models.GenericFeatures", b =>
                {
                    b.Property<int>("GenericFeaturesID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Attendant");

                    b.Property<bool>("BottomAccess");

                    b.Property<string>("BottomAccessLocation");

                    b.Property<bool>("CRO");

                    b.Property<bool>("CTINSPST");

                    b.Property<bool>("CallEnable");

                    b.Property<string>("CarCallCodeSecurity");

                    b.Property<bool>("CarCallRead");

                    b.Property<bool>("CarKey");

                    b.Property<bool>("CarToLobby");

                    b.Property<bool>("EMT");

                    b.Property<bool>("EP");

                    b.Property<string>("EPCarsNumber");

                    b.Property<string>("EPContact");

                    b.Property<bool>("EPOtherCars");

                    b.Property<bool>("EPSelect");

                    b.Property<bool>("EPVoltage");

                    b.Property<bool>("EQ");

                    b.Property<bool>("FLO");

                    b.Property<bool>("FRON2");

                    b.Property<string>("GovModel");

                    b.Property<bool>("HCRO");

                    b.Property<bool>("HallCallRead");

                    b.Property<bool>("HallKey");

                    b.Property<bool>("Hosp");

                    b.Property<bool>("INA");

                    b.Property<bool>("INCP");

                    b.Property<string>("INCPButtons");

                    b.Property<bool>("Ind");

                    b.Property<int>("JobID");

                    b.Property<bool>("LoadWeigher");

                    b.Property<string>("Monitoring");

                    b.Property<bool>("PTI");

                    b.Property<bool>("Roped");

                    b.Property<string>("SpecialInstructions");

                    b.Property<string>("SwitchStyle");

                    b.Property<bool>("TopAccess");

                    b.Property<string>("TopAccessLocation");

                    b.HasKey("GenericFeaturesID");

                    b.HasIndex("JobID")
                        .IsUnique();

                    b.ToTable("GenericFeaturesList");
                });

            modelBuilder.Entity("ProdFloor.Models.HoistWayData", b =>
                {
                    b.Property<int>("HoistWayDataID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Capacity");

                    b.Property<int>("DownSpeed");

                    b.Property<bool>("FrontEightServed");

                    b.Property<bool>("FrontEleventhServed");

                    b.Property<bool>("FrontFifteenthServed");

                    b.Property<bool>("FrontFifthServed");

                    b.Property<bool>("FrontFirstServed");

                    b.Property<bool>("FrontFourteenthServed");

                    b.Property<bool>("FrontFourthServed");

                    b.Property<bool>("FrontNinthServed");

                    b.Property<bool>("FrontSecondServed");

                    b.Property<bool>("FrontSeventhServed");

                    b.Property<bool>("FrontSexthServed");

                    b.Property<bool>("FrontSixteenthServed");

                    b.Property<bool>("FrontTenthServed");

                    b.Property<bool>("FrontThirdServed");

                    b.Property<bool>("FrontThirteenthServed");

                    b.Property<bool>("FrontTwelvethServed");

                    b.Property<int>("JobID");

                    b.Property<int>("LandingSystemID");

                    b.Property<bool>("RearEightServed");

                    b.Property<bool>("RearEleventhServed");

                    b.Property<bool>("RearFifteenthServed");

                    b.Property<bool>("RearFifthServed");

                    b.Property<bool>("RearFirstServed");

                    b.Property<bool>("RearFourteenthServed");

                    b.Property<bool>("RearFourthServed");

                    b.Property<bool>("RearNinthServed");

                    b.Property<bool>("RearSecondServed");

                    b.Property<bool>("RearSeventhServed");

                    b.Property<bool>("RearSexthServed");

                    b.Property<bool>("RearSixteenthServed");

                    b.Property<bool>("RearTenthServed");

                    b.Property<bool>("RearThirdServed");

                    b.Property<bool>("RearThirteenthServed");

                    b.Property<bool>("RearTwelvethServed");

                    b.Property<int>("TotalTravel");

                    b.Property<int>("UpSpeed");

                    b.HasKey("HoistWayDataID");

                    b.HasIndex("JobID")
                        .IsUnique();

                    b.HasIndex("LandingSystemID");

                    b.ToTable("HoistWayDatas");
                });

            modelBuilder.Entity("ProdFloor.Models.HydroSpecific", b =>
                {
                    b.Property<int>("HydroSpecificID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Battery");

                    b.Property<string>("BatteryBrand");

                    b.Property<int>("FLA");

                    b.Property<int>("HP");

                    b.Property<int>("JobID");

                    b.Property<bool>("LOS");

                    b.Property<bool>("LifeJacket");

                    b.Property<int>("MotorsNum");

                    b.Property<bool>("OilCool");

                    b.Property<bool>("OilTank");

                    b.Property<bool>("PSS");

                    b.Property<bool>("Resync");

                    b.Property<bool>("Roped");

                    b.Property<int>("SPH");

                    b.Property<string>("Starter")
                        .IsRequired();

                    b.Property<bool>("VCI");

                    b.Property<string>("ValveBrand")
                        .IsRequired();

                    b.Property<int>("ValveCoils");

                    b.Property<string>("ValveModel")
                        .IsRequired();

                    b.Property<int>("ValveNum");

                    b.Property<int>("ValveVoltage");

                    b.HasKey("HydroSpecificID");

                    b.HasIndex("JobID")
                        .IsUnique();

                    b.ToTable("HydroSpecifics");
                });

            modelBuilder.Entity("ProdFloor.Models.Indicator", b =>
                {
                    b.Property<int>("IndicatorID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CarCallsType")
                        .IsRequired();

                    b.Property<string>("CarCallsVoltage")
                        .IsRequired();

                    b.Property<string>("CarCallsVoltageType")
                        .IsRequired();

                    b.Property<bool>("CarLanterns");

                    b.Property<string>("CarLanternsStyle");

                    b.Property<string>("CarLanternsType");

                    b.Property<bool>("CarPI");

                    b.Property<string>("CarPIDiscreteType");

                    b.Property<string>("CarPIType");

                    b.Property<string>("HallCallsType")
                        .IsRequired();

                    b.Property<string>("HallCallsVoltage")
                        .IsRequired();

                    b.Property<string>("HallCallsVoltageType")
                        .IsRequired();

                    b.Property<bool>("HallLanterns");

                    b.Property<string>("HallLanternsStyle");

                    b.Property<string>("HallLanternsType");

                    b.Property<bool>("HallPI");

                    b.Property<string>("HallPIDiscreteType");

                    b.Property<string>("HallPIType");

                    b.Property<string>("IndicatorsVoltage")
                        .IsRequired();

                    b.Property<string>("IndicatorsVoltageType")
                        .IsRequired();

                    b.Property<int>("JobID");

                    b.Property<bool>("PassingFloor");

                    b.Property<string>("PassingFloorDiscreteType");

                    b.Property<bool>("PassingFloorEnable");

                    b.Property<string>("PassingFloorType");

                    b.Property<bool>("VoiceAnnunciationPI");

                    b.Property<string>("VoiceAnnunciationPIType");

                    b.HasKey("IndicatorID");

                    b.HasIndex("JobID")
                        .IsUnique();

                    b.ToTable("Indicators");
                });

            modelBuilder.Entity("ProdFloor.Models.Job", b =>
                {
                    b.Property<int>("JobID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CityID");

                    b.Property<string>("Contractor")
                        .IsRequired();

                    b.Property<string>("Cust")
                        .IsRequired();

                    b.Property<int>("EngID");

                    b.Property<int>("FireCodeID");

                    b.Property<string>("JobCountry")
                        .IsRequired();

                    b.Property<int>("JobNum");

                    b.Property<string>("JobState")
                        .IsRequired();

                    b.Property<int>("JobTypeID");

                    b.Property<DateTime>("LatestFinishDate");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("PO");

                    b.Property<DateTime>("ShipDate");

                    b.Property<string>("Status");

                    b.HasKey("JobID");

                    b.HasIndex("CityID");

                    b.HasIndex("FireCodeID");

                    b.HasIndex("JobTypeID");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("ProdFloor.Models.JobExtension", b =>
                {
                    b.Property<int>("JobExtensionID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AUXCOP");

                    b.Property<bool>("CartopDoorButtons");

                    b.Property<string>("DoorGate")
                        .IsRequired();

                    b.Property<string>("DoorHoist")
                        .IsRequired();

                    b.Property<bool>("DoorHold");

                    b.Property<int>("DoorOperatorID");

                    b.Property<bool>("HeavyDoors");

                    b.Property<bool>("InfDetector");

                    b.Property<int>("InputFrecuency");

                    b.Property<int>("InputPhase");

                    b.Property<int>("InputVoltage");

                    b.Property<int>("JobID");

                    b.Property<string>("JobTypeAdd")
                        .IsRequired();

                    b.Property<string>("JobTypeMain")
                        .IsRequired();

                    b.Property<bool>("MechSafEdge");

                    b.Property<bool>("Nudging");

                    b.Property<int>("NumOfStops");

                    b.Property<bool>("SCOP");

                    b.Property<bool>("SHC");

                    b.Property<int>("SHCRisers");

                    b.HasKey("JobExtensionID");

                    b.HasIndex("DoorOperatorID");

                    b.HasIndex("JobID")
                        .IsUnique();

                    b.ToTable("JobsExtensions");
                });

            modelBuilder.Entity("ProdFloor.Models.JobType", b =>
                {
                    b.Property<int>("JobTypeID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("JobTypeID");

                    b.ToTable("JobTypes");
                });

            modelBuilder.Entity("ProdFloor.Models.LandingSystem", b =>
                {
                    b.Property<int>("LandingSystemID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("UsedIn");

                    b.HasKey("LandingSystemID");

                    b.ToTable("LandingSystems");
                });

            modelBuilder.Entity("ProdFloor.Models.State", b =>
                {
                    b.Property<int>("StateID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CountryID");

                    b.Property<string>("Name");

                    b.HasKey("StateID");

                    b.HasIndex("CountryID");

                    b.ToTable("States");
                });

            modelBuilder.Entity("ProdFloor.Models.City", b =>
                {
                    b.HasOne("ProdFloor.Models.FireCode")
                        .WithMany("Cities")
                        .HasForeignKey("FirecodeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProdFloor.Models.State")
                        .WithMany("Cities")
                        .HasForeignKey("StateID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.GenericFeatures", b =>
                {
                    b.HasOne("ProdFloor.Models.Job")
                        .WithOne("GenericFeatures")
                        .HasForeignKey("ProdFloor.Models.GenericFeatures", "JobID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.HoistWayData", b =>
                {
                    b.HasOne("ProdFloor.Models.Job")
                        .WithOne("HoistWayData")
                        .HasForeignKey("ProdFloor.Models.HoistWayData", "JobID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProdFloor.Models.LandingSystem")
                        .WithMany("HoistWayDatas")
                        .HasForeignKey("LandingSystemID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.HydroSpecific", b =>
                {
                    b.HasOne("ProdFloor.Models.Job")
                        .WithOne("HydroSpecific")
                        .HasForeignKey("ProdFloor.Models.HydroSpecific", "JobID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.Indicator", b =>
                {
                    b.HasOne("ProdFloor.Models.Job")
                        .WithOne("Indicator")
                        .HasForeignKey("ProdFloor.Models.Indicator", "JobID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.Job", b =>
                {
                    b.HasOne("ProdFloor.Models.City")
                        .WithMany("Jobs")
                        .HasForeignKey("CityID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProdFloor.Models.FireCode")
                        .WithMany("Jobs")
                        .HasForeignKey("FireCodeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProdFloor.Models.JobType")
                        .WithMany("Jobs")
                        .HasForeignKey("JobTypeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.JobExtension", b =>
                {
                    b.HasOne("ProdFloor.Models.DoorOperator")
                        .WithMany("JobExtensions")
                        .HasForeignKey("DoorOperatorID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProdFloor.Models.Job")
                        .WithOne("jobExtension")
                        .HasForeignKey("ProdFloor.Models.JobExtension", "JobID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.State", b =>
                {
                    b.HasOne("ProdFloor.Models.Country")
                        .WithMany("States")
                        .HasForeignKey("CountryID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
