using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class RelacionesJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_JobsExtensions_JobID",
                table: "JobsExtensions",
                column: "JobID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Indicators_JobID",
                table: "Indicators",
                column: "JobID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HydroSpecifics_JobID",
                table: "HydroSpecifics",
                column: "JobID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoistWayDatas_JobID",
                table: "HoistWayDatas",
                column: "JobID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GenericFeaturesList_JobID",
                table: "GenericFeaturesList",
                column: "JobID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GenericFeaturesList_Jobs_JobID",
                table: "GenericFeaturesList",
                column: "JobID",
                principalTable: "Jobs",
                principalColumn: "JobID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoistWayDatas_Jobs_JobID",
                table: "HoistWayDatas",
                column: "JobID",
                principalTable: "Jobs",
                principalColumn: "JobID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HydroSpecifics_Jobs_JobID",
                table: "HydroSpecifics",
                column: "JobID",
                principalTable: "Jobs",
                principalColumn: "JobID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Indicators_Jobs_JobID",
                table: "Indicators",
                column: "JobID",
                principalTable: "Jobs",
                principalColumn: "JobID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobsExtensions_Jobs_JobID",
                table: "JobsExtensions",
                column: "JobID",
                principalTable: "Jobs",
                principalColumn: "JobID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GenericFeaturesList_Jobs_JobID",
                table: "GenericFeaturesList");

            migrationBuilder.DropForeignKey(
                name: "FK_HoistWayDatas_Jobs_JobID",
                table: "HoistWayDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_HydroSpecifics_Jobs_JobID",
                table: "HydroSpecifics");

            migrationBuilder.DropForeignKey(
                name: "FK_Indicators_Jobs_JobID",
                table: "Indicators");

            migrationBuilder.DropForeignKey(
                name: "FK_JobsExtensions_Jobs_JobID",
                table: "JobsExtensions");

            migrationBuilder.DropIndex(
                name: "IX_JobsExtensions_JobID",
                table: "JobsExtensions");

            migrationBuilder.DropIndex(
                name: "IX_Indicators_JobID",
                table: "Indicators");

            migrationBuilder.DropIndex(
                name: "IX_HydroSpecifics_JobID",
                table: "HydroSpecifics");

            migrationBuilder.DropIndex(
                name: "IX_HoistWayDatas_JobID",
                table: "HoistWayDatas");

            migrationBuilder.DropIndex(
                name: "IX_GenericFeaturesList_JobID",
                table: "GenericFeaturesList");
        }
    }
}
