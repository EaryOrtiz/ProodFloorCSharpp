using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class UpdateFkSpecialFaetures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpecialFeatures_JobID",
                table: "SpecialFeatures");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialFeatures_JobID",
                table: "SpecialFeatures",
                column: "JobID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SpecialFeatures_JobID",
                table: "SpecialFeatures");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialFeatures_JobID",
                table: "SpecialFeatures",
                column: "JobID",
                unique: true);
        }
    }
}
