using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProdFloor.Migrations
{
    public partial class planningReportAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "JobNum",
                table: "Jobs",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "PlanningReports",
                columns: table => new
                {
                    PlanningReportID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PlanningDate = table.Column<DateTime>(nullable: false),
                    DateTimeLoad = table.Column<DateTime>(nullable: false),
                    Busy = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanningReports", x => x.PlanningReportID);
                });

            migrationBuilder.CreateTable(
                name: "PlanningReportRows",
                columns: table => new
                {
                    PlanningReportRowID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PlanningReportID = table.Column<int>(nullable: false),
                    Consecutive = table.Column<int>(nullable: false),
                    JobNumber = table.Column<string>(nullable: true),
                    PO = table.Column<int>(nullable: false),
                    JobName = table.Column<string>(nullable: true),
                    PreviousWorkCenter = table.Column<string>(nullable: true),
                    WorkCenter = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    Priority = table.Column<string>(nullable: true),
                    Material = table.Column<string>(nullable: true),
                    MRP = table.Column<string>(nullable: true),
                    SoldTo = table.Column<string>(nullable: true),
                    Custom = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanningReportRows", x => x.PlanningReportRowID);
                    table.ForeignKey(
                        name: "FK_PlanningReportRows_PlanningReports_PlanningReportID",
                        column: x => x.PlanningReportID,
                        principalTable: "PlanningReports",
                        principalColumn: "PlanningReportID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanningReportRows_PlanningReportID",
                table: "PlanningReportRows",
                column: "PlanningReportID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanningReportRows");

            migrationBuilder.DropTable(
                name: "PlanningReports");

            migrationBuilder.AlterColumn<string>(
                name: "JobNum",
                table: "Jobs",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
