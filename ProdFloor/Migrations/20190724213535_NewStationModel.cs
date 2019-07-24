using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class NewStationModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Station",
                table: "TestJobs");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDate",
                table: "TestJobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "TestJobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "StationID",
                table: "TestJobs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AuxStationID",
                table: "Stops",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AuxTechnicianID",
                table: "Stops",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Critical",
                table: "Stops",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AuxStationID",
                table: "StepsForJobs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AuxTechnicianID",
                table: "StepsForJobs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Obsolete",
                table: "StepsForJobs",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StationID",
                table: "Jobs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Stations",
                columns: table => new
                {
                    StationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobTypeID = table.Column<int>(nullable: false),
                    Label = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.StationID);
                    table.ForeignKey(
                        name: "FK_Stations_JobTypes_JobTypeID",
                        column: x => x.JobTypeID,
                        principalTable: "JobTypes",
                        principalColumn: "JobTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_StationID",
                table: "Jobs",
                column: "StationID");

            migrationBuilder.CreateIndex(
                name: "IX_Stations_JobTypeID",
                table: "Stations",
                column: "JobTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Stations_StationID",
                table: "Jobs",
                column: "StationID",
                principalTable: "Stations",
                principalColumn: "StationID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Stations_StationID",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_StationID",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CompletedDate",
                table: "TestJobs");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "TestJobs");

            migrationBuilder.DropColumn(
                name: "StationID",
                table: "TestJobs");

            migrationBuilder.DropColumn(
                name: "AuxStationID",
                table: "Stops");

            migrationBuilder.DropColumn(
                name: "AuxTechnicianID",
                table: "Stops");

            migrationBuilder.DropColumn(
                name: "Critical",
                table: "Stops");

            migrationBuilder.DropColumn(
                name: "AuxStationID",
                table: "StepsForJobs");

            migrationBuilder.DropColumn(
                name: "AuxTechnicianID",
                table: "StepsForJobs");

            migrationBuilder.DropColumn(
                name: "Obsolete",
                table: "StepsForJobs");

            migrationBuilder.DropColumn(
                name: "StationID",
                table: "Jobs");

            migrationBuilder.AddColumn<string>(
                name: "Station",
                table: "TestJobs",
                nullable: true);
        }
    }
}
