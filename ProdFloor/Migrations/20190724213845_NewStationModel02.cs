using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class NewStationModel02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Stations_StationID",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_StationID",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "StationID",
                table: "Jobs");

           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestJobs_Stations_StationID",
                table: "TestJobs");

            migrationBuilder.DropIndex(
                name: "IX_TestJobs_StationID",
                table: "TestJobs");

            migrationBuilder.AddColumn<int>(
                name: "StationID",
                table: "Jobs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_StationID",
                table: "Jobs",
                column: "StationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Stations_StationID",
                table: "Jobs",
                column: "StationID",
                principalTable: "Stations",
                principalColumn: "StationID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
