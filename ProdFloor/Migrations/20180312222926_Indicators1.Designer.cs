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
    [Migration("20180312222926_Indicators1")]
    partial class Indicators1
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

                    b.Property<string>("Country");

                    b.Property<string>("CurrentFireCode");

                    b.Property<string>("Name");

                    b.Property<string>("State");

                    b.HasKey("CityID");

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

                    b.Property<bool>("CallEnable");

                    b.Property<string>("CarCallCodeSecurity");

                    b.Property<bool>("CarToLobby");

                    b.Property<bool>("EMT");

                    b.Property<bool>("EP");

                    b.Property<bool>("EQ");

                    b.Property<bool>("FLO");

                    b.Property<bool>("FRON2");

                    b.Property<bool>("Hosp");

                    b.Property<bool>("IDS");

                    b.Property<bool>("IMon");

                    b.Property<bool>("INA");

                    b.Property<bool>("INCP");

                    b.Property<bool>("Ind");

                    b.Property<int>("JobID");

                    b.Property<bool>("LoadWeigher");

                    b.Property<bool>("MView");

                    b.Property<string>("SpecialInstructions");

                    b.Property<string>("SwitchStyle");

                    b.HasKey("GenericFeaturesID");

                    b.ToTable("GenericFeaturesList");
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

                    b.Property<string>("Starter");

                    b.Property<bool>("VCI");

                    b.Property<string>("ValveBrand");

                    b.Property<int>("ValveCoils");

                    b.Property<string>("ValveModel");

                    b.Property<int>("ValveNum");

                    b.Property<int>("ValveVoltage");

                    b.HasKey("HydroSpecificID");

                    b.ToTable("HydroSpecifics");
                });

            modelBuilder.Entity("ProdFloor.Models.Job", b =>
                {
                    b.Property<int>("JobID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Contractor")
                        .IsRequired();

                    b.Property<string>("Cust")
                        .IsRequired();

                    b.Property<string>("JobCity")
                        .IsRequired();

                    b.Property<string>("JobCountry")
                        .IsRequired();

                    b.Property<int>("JobNum");

                    b.Property<string>("JobState")
                        .IsRequired();

                    b.Property<string>("JobType")
                        .IsRequired();

                    b.Property<DateTime>("LatestFinishDate");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<int>("PO");

                    b.Property<string>("SafetyCode")
                        .IsRequired();

                    b.Property<DateTime>("ShipDate");

                    b.HasKey("JobID");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("ProdFloor.Models.JobExtension", b =>
                {
                    b.Property<int>("JobExtensionID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AUXCOP");

                    b.Property<bool>("CartopDoorButtons");

                    b.Property<string>("DoorBrand");

                    b.Property<string>("DoorGate");

                    b.Property<string>("DoorHoist");

                    b.Property<bool>("DoorHold");

                    b.Property<string>("DoorModel");

                    b.Property<bool>("HeavyDoors");

                    b.Property<bool>("InfDetector");

                    b.Property<int>("InputFrecuency");

                    b.Property<int>("InputPhase");

                    b.Property<int>("InputVoltage");

                    b.Property<int>("JobID");

                    b.Property<string>("JobTypeAdd");

                    b.Property<string>("JobTypeMain");

                    b.Property<bool>("MechSafEdge");

                    b.Property<bool>("Nudging");

                    b.Property<int>("NumOfStops");

                    b.Property<bool>("SCOP");

                    b.Property<bool>("SHC");

                    b.Property<int>("SHCRisers");

                    b.HasKey("JobExtensionID");

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

                    b.Property<string>("Country");

                    b.Property<string>("Name");

                    b.HasKey("StateID");

                    b.ToTable("States");
                });

            modelBuilder.Entity("ProdFloor.Models.StatusIndicator", b =>
                {
                    b.Property<int>("StatusIndicatorID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("JobID");

                    b.Property<string>("Name");

                    b.Property<int>("Voltage");

                    b.Property<string>("VoltageType");

                    b.HasKey("StatusIndicatorID");

                    b.ToTable("StatusIndicators");
                });
#pragma warning restore 612, 618
        }
    }
}
