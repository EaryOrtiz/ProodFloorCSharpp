using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class flaOnStarterFloat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {/*
            migrationBuilder.CreateIndex(
                name: "IX_TestJobs_JobID",
                table: "TestJobs",
                column: "JobID");

            migrationBuilder.AddForeignKey(
                name: "FK_TestJobs_Jobs_JobID",
                table: "TestJobs",
                column: "JobID",
                principalTable: "Jobs",
                principalColumn: "JobID",
                onDelete: ReferentialAction.Cascade);
                */
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestJobs_Jobs_JobID",
                table: "TestJobs");

            migrationBuilder.DropIndex(
                name: "IX_TestJobs_JobID",
                table: "TestJobs");
        }
    }
}
