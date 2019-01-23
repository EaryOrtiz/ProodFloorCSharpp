using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    CityID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Country = table.Column<string>(nullable: true),
                    CurrentFireCode = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.CityID);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryID);
                });

            migrationBuilder.CreateTable(
                name: "DoorOperators",
                columns: table => new
                {
                    DoorOperatorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Brand = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Style = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoorOperators", x => x.DoorOperatorID);
                });

            migrationBuilder.CreateTable(
                name: "FireCodes",
                columns: table => new
                {
                    FireCodeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FireCodes", x => x.FireCodeID);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    JobID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Contractor = table.Column<string>(nullable: false),
                    Cust = table.Column<string>(nullable: false),
                    JobNum = table.Column<int>(nullable: false),
                    JobState = table.Column<string>(nullable: false),
                    JobType = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    PO = table.Column<int>(nullable: false),
                    SafetyCode = table.Column<string>(nullable: false),
                    ShipDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.JobID);
                });

            migrationBuilder.CreateTable(
                name: "JobsExtensions",
                columns: table => new
                {
                    JobExtensionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AUXCOP = table.Column<bool>(nullable: false),
                    CartopDoorButtons = table.Column<bool>(nullable: false),
                    DoorBrand = table.Column<string>(nullable: true),
                    DoorGate = table.Column<string>(nullable: true),
                    DoorHoist = table.Column<string>(nullable: true),
                    DoorHold = table.Column<bool>(nullable: false),
                    DoorModel = table.Column<string>(nullable: true),
                    DoorStyle = table.Column<string>(nullable: true),
                    HeavyDoors = table.Column<bool>(nullable: false),
                    InfDetector = table.Column<bool>(nullable: false),
                    InputFrecuency = table.Column<int>(nullable: false),
                    InputPhase = table.Column<int>(nullable: false),
                    InputVoltage = table.Column<int>(nullable: false),
                    JobID = table.Column<int>(nullable: false),
                    JobTypeAdd = table.Column<string>(nullable: true),
                    JobTypeMain = table.Column<string>(nullable: true),
                    MechSafEdge = table.Column<bool>(nullable: false),
                    Nudging = table.Column<bool>(nullable: false),
                    NumOfStops = table.Column<int>(nullable: false),
                    SCOP = table.Column<bool>(nullable: false),
                    SHC = table.Column<bool>(nullable: false),
                    SHCRisers = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobsExtensions", x => x.JobExtensionID);
                });

            migrationBuilder.CreateTable(
                name: "JobTypes",
                columns: table => new
                {
                    JobTypeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTypes", x => x.JobTypeID);
                });

            migrationBuilder.CreateTable(
                name: "LandingSystems",
                columns: table => new
                {
                    LandingSystemID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    UsedIn = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandingSystems", x => x.LandingSystemID);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    StateID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Country = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.StateID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "DoorOperators");

            migrationBuilder.DropTable(
                name: "FireCodes");

            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "JobsExtensions");

            migrationBuilder.DropTable(
                name: "JobTypes");

            migrationBuilder.DropTable(
                name: "LandingSystems");

            migrationBuilder.DropTable(
                name: "States");
        }
    }
}
