using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class NewStationModel03 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestJobs_Stations_StationID",
                table: "TestJobs");

            migrationBuilder.DropIndex(
                name: "IX_TestJobs_StationID",
                table: "TestJobs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TestJobs_StationID",
                table: "TestJobs",
                column: "StationID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestJobs_Stations_StationID",
                table: "TestJobs",
                column: "StationID",
                principalTable: "Stations",
                principalColumn: "StationID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
