using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class StepsOneToManyStepsforjob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StepsForJobs_StepID",
                table: "StepsForJobs");

            migrationBuilder.CreateIndex(
                name: "IX_StepsForJobs_StepID",
                table: "StepsForJobs",
                column: "StepID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StepsForJobs_StepID",
                table: "StepsForJobs");

            migrationBuilder.CreateIndex(
                name: "IX_StepsForJobs_StepID",
                table: "StepsForJobs",
                column: "StepID",
                unique: true);
        }
    }
}
