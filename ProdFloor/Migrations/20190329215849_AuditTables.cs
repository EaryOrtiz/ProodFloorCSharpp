using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class AuditTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "City_Audits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AffectedItemID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Statement = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City_Audits", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Country_Audits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AffectedItemID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Statement = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Country_Audits", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DoorOperator_Audits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AffectedItemID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Statement = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoorOperator_Audits", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "FireCode_Audits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AffectedItemID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Statement = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FireCode_Audits", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "JobType__Audits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AffectedItemID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Statement = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobType__Audits", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "LandingSystem_Audits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AffectedItemID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Statement = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LandingSystem_Audits", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Overload_Audits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AffectedItemID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Statement = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Overload_Audits", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Slowdown__Audits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AffectedItemID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Statement = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slowdown__Audits", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Starter_Audits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AffectedItemID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Statement = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Starter_Audits", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "State_Audits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AffectedItemID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Statement = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_State_Audits", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WireTypeSize_Audits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AffectedItemID = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Statement = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WireTypeSize_Audits", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "City_Audits");

            migrationBuilder.DropTable(
                name: "Country_Audits");

            migrationBuilder.DropTable(
                name: "DoorOperator_Audits");

            migrationBuilder.DropTable(
                name: "FireCode_Audits");

            migrationBuilder.DropTable(
                name: "JobType__Audits");

            migrationBuilder.DropTable(
                name: "LandingSystem_Audits");

            migrationBuilder.DropTable(
                name: "Overload_Audits");

            migrationBuilder.DropTable(
                name: "Slowdown__Audits");

            migrationBuilder.DropTable(
                name: "Starter_Audits");

            migrationBuilder.DropTable(
                name: "State_Audits");

            migrationBuilder.DropTable(
                name: "WireTypeSize_Audits");
        }
    }
}
