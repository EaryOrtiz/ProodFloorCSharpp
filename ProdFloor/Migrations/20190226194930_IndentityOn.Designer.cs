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
    [Migration("20190226194930_IndentityOn")]
    partial class IndentityOn
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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

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

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("CountryID");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("ProdFloor.Models.DoorOperator", b =>
                {
                    b.Property<int>("DoorOperatorID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Style")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("DoorOperatorID");

                    b.ToTable("DoorOperators");
                });

            modelBuilder.Entity("ProdFloor.Models.FireCode", b =>
                {
                    b.Property<int>("FireCodeID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("FireCodeID");

                    b.ToTable("FireCodes");
                });

            modelBuilder.Entity("ProdFloor.Models.GenericFeatures", b =>
                {
                    b.Property<int>("GenericFeaturesID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Attendant");

                    b.Property<bool>("BottomAccess");

                    b.Property<string>("BottomAccessLocation")
                        .HasMaxLength(25);

                    b.Property<bool>("CRO");

                    b.Property<bool>("CTINSPST");

                    b.Property<bool>("CallEnable");

                    b.Property<string>("CarCallCodeSecurity")
                        .HasMaxLength(50);

                    b.Property<bool>("CarCallRead");

                    b.Property<bool>("CarKey");

                    b.Property<bool>("CarToLobby");

                    b.Property<bool>("EMT");

                    b.Property<bool>("EP");

                    b.Property<string>("EPCarsNumber");

                    b.Property<string>("EPContact")
                        .HasMaxLength(50);

                    b.Property<bool>("EPOtherCars");

                    b.Property<bool>("EPSelect");

                    b.Property<bool>("EPVoltage");

                    b.Property<bool>("EQ");

                    b.Property<bool>("FLO");

                    b.Property<bool>("FRON2");

                    b.Property<string>("GovModel")
                        .HasMaxLength(50);

                    b.Property<bool>("HCRO");

                    b.Property<bool>("HallCallRead");

                    b.Property<bool>("HallKey");

                    b.Property<bool>("Hosp");

                    b.Property<bool>("INA");

                    b.Property<bool>("INCP");

                    b.Property<string>("INCPButtons")
                        .HasMaxLength(50);

                    b.Property<bool>("Ind");

                    b.Property<int>("JobID");

                    b.Property<bool>("LoadWeigher");

                    b.Property<string>("Monitoring")
                        .HasMaxLength(100);

                    b.Property<bool>("PTI");

                    b.Property<bool>("Roped");

                    b.Property<string>("SpecialInstructions")
                        .HasMaxLength(250);

                    b.Property<string>("SwitchStyle")
                        .HasMaxLength(50);

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

                    b.Property<int>("HoistWaysNumber");

                    b.Property<int>("JobID");

                    b.Property<int>("LandingSystemID");

                    b.Property<int>("MachineRooms");

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

                    b.Property<string>("BatteryBrand")
                        .HasMaxLength(50);

                    b.Property<int>("FLA");

                    b.Property<int>("HP");

                    b.Property<int>("JobID");

                    b.Property<bool>("LOS");

                    b.Property<bool>("LifeJacket");

                    b.Property<int>("MotorsDisconnect");

                    b.Property<int>("MotorsNum");

                    b.Property<bool>("OilCool");

                    b.Property<bool>("OilTank");

                    b.Property<bool>("PSS");

                    b.Property<bool>("Resync");

                    b.Property<bool>("Roped");

                    b.Property<int>("SPH");

                    b.Property<string>("Starter")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<bool>("VCI");

                    b.Property<string>("ValveBrand")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("ValveCoils");

                    b.Property<string>("ValveModel")
                        .IsRequired()
                        .HasMaxLength(50);

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
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<string>("CarCallsVoltage")
                        .IsRequired();

                    b.Property<string>("CarCallsVoltageType")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.Property<bool>("CarLanterns");

                    b.Property<string>("CarLanternsStyle")
                        .HasMaxLength(25);

                    b.Property<string>("CarLanternsType")
                        .HasMaxLength(10);

                    b.Property<bool>("CarPI");

                    b.Property<string>("CarPIDiscreteType")
                        .HasMaxLength(25);

                    b.Property<string>("CarPIType")
                        .HasMaxLength(25);

                    b.Property<string>("HallCallsType")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<string>("HallCallsVoltage")
                        .IsRequired();

                    b.Property<string>("HallCallsVoltageType")
                        .IsRequired()
                        .HasMaxLength(5);

                    b.Property<bool>("HallLanterns");

                    b.Property<string>("HallLanternsStyle")
                        .HasMaxLength(50);

                    b.Property<string>("HallLanternsType")
                        .HasMaxLength(10);

                    b.Property<bool>("HallPI");

                    b.Property<string>("HallPIDiscreteType")
                        .HasMaxLength(25);

                    b.Property<string>("HallPIType")
                        .HasMaxLength(25);

                    b.Property<int>("IndicatorsVoltage");

                    b.Property<string>("IndicatorsVoltageType")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("JobID");

                    b.Property<bool>("PassingFloor");

                    b.Property<string>("PassingFloorDiscreteType")
                        .HasMaxLength(25);

                    b.Property<bool>("PassingFloorEnable");

                    b.Property<string>("PassingFloorType")
                        .HasMaxLength(25);

                    b.Property<bool>("VoiceAnnunciationPI");

                    b.Property<string>("VoiceAnnunciationPIType")
                        .HasMaxLength(50);

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
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("CrossAppEngID");

                    b.Property<string>("Cust")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<int>("EngID");

                    b.Property<int>("FireCodeID");

                    b.Property<int>("JobNum");

                    b.Property<int>("JobTypeID");

                    b.Property<DateTime>("LatestFinishDate");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(78);

                    b.Property<int>("PO");

                    b.Property<DateTime>("ShipDate");

                    b.Property<string>("Status")
                        .HasMaxLength(26);

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
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("DoorHoist")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<bool>("DoorHold");

                    b.Property<int>("DoorOperatorID");

                    b.Property<bool>("HeavyDoors");

                    b.Property<bool>("InfDetector");

                    b.Property<int>("InputFrecuency");

                    b.Property<int>("InputPhase");

                    b.Property<int>("InputVoltage");

                    b.Property<int>("JobID");

                    b.Property<string>("JobTypeAdd")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("JobTypeMain")
                        .IsRequired()
                        .HasMaxLength(26);

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
                        .IsRequired()
                        .HasMaxLength(10);

                    b.HasKey("JobTypeID");

                    b.ToTable("JobTypes");
                });

            modelBuilder.Entity("ProdFloor.Models.LandingSystem", b =>
                {
                    b.Property<int>("LandingSystemID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20);

                    b.Property<string>("UsedIn")
                        .IsRequired()
                        .HasMaxLength(15);

                    b.HasKey("LandingSystemID");

                    b.ToTable("LandingSystems");
                });

            modelBuilder.Entity("ProdFloor.Models.SpecialFeatures", b =>
                {
                    b.Property<int>("SpecialFeaturesID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .HasMaxLength(250);

                    b.Property<int>("JobID");

                    b.HasKey("SpecialFeaturesID");

                    b.HasIndex("JobID");

                    b.ToTable("SpecialFeatures");
                });

            modelBuilder.Entity("ProdFloor.Models.State", b =>
                {
                    b.Property<int>("StateID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CountryID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("StateID");

                    b.HasIndex("CountryID");

                    b.ToTable("States");
                });

            modelBuilder.Entity("ProdFloor.Models.City", b =>
                {
                    b.HasOne("ProdFloor.Models.FireCode")
                        .WithMany("_Cities")
                        .HasForeignKey("FirecodeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProdFloor.Models.State")
                        .WithMany("_Cities")
                        .HasForeignKey("StateID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.GenericFeatures", b =>
                {
                    b.HasOne("ProdFloor.Models.Job")
                        .WithOne("_GenericFeatures")
                        .HasForeignKey("ProdFloor.Models.GenericFeatures", "JobID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.HoistWayData", b =>
                {
                    b.HasOne("ProdFloor.Models.Job")
                        .WithOne("_HoistWayData")
                        .HasForeignKey("ProdFloor.Models.HoistWayData", "JobID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProdFloor.Models.LandingSystem")
                        .WithMany("_HoistWayDatas")
                        .HasForeignKey("LandingSystemID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.HydroSpecific", b =>
                {
                    b.HasOne("ProdFloor.Models.Job")
                        .WithOne("_HydroSpecific")
                        .HasForeignKey("ProdFloor.Models.HydroSpecific", "JobID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.Indicator", b =>
                {
                    b.HasOne("ProdFloor.Models.Job")
                        .WithOne("_Indicator")
                        .HasForeignKey("ProdFloor.Models.Indicator", "JobID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.Job", b =>
                {
                    b.HasOne("ProdFloor.Models.City")
                        .WithMany("_Jobs")
                        .HasForeignKey("CityID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProdFloor.Models.FireCode")
                        .WithMany("_Jobs")
                        .HasForeignKey("FireCodeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProdFloor.Models.JobType")
                        .WithMany("_Jobs")
                        .HasForeignKey("JobTypeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.JobExtension", b =>
                {
                    b.HasOne("ProdFloor.Models.DoorOperator")
                        .WithMany("_JobExtensions")
                        .HasForeignKey("DoorOperatorID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ProdFloor.Models.Job")
                        .WithOne("_jobExtension")
                        .HasForeignKey("ProdFloor.Models.JobExtension", "JobID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.SpecialFeatures", b =>
                {
                    b.HasOne("ProdFloor.Models.Job", "Job")
                        .WithMany("_SpecialFeatureslist")
                        .HasForeignKey("JobID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ProdFloor.Models.State", b =>
                {
                    b.HasOne("ProdFloor.Models.Country")
                        .WithMany("_States")
                        .HasForeignKey("CountryID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
