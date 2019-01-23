using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class RelacionesExtra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryID",
                table: "States",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DoorOperatorID",
                table: "JobsExtensions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CityID",
                table: "Jobs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FireCodeID",
                table: "Jobs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "JobTypeID",
                table: "Jobs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LandingSystemID",
                table: "HoistWayDatas",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FirecodeID",
                table: "Cities",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StateID",
                table: "Cities",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_States_CountryID",
                table: "States",
                column: "CountryID");

            migrationBuilder.CreateIndex(
                name: "IX_JobsExtensions_DoorOperatorID",
                table: "JobsExtensions",
                column: "DoorOperatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CityID",
                table: "Jobs",
                column: "CityID");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_FireCodeID",
                table: "Jobs",
                column: "FireCodeID");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_JobTypeID",
                table: "Jobs",
                column: "JobTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_HoistWayDatas_LandingSystemID",
                table: "HoistWayDatas",
                column: "LandingSystemID");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_FirecodeID",
                table: "Cities",
                column: "FirecodeID");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_StateID",
                table: "Cities",
                column: "StateID");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_FireCodes_FirecodeID",
                table: "Cities",
                column: "FirecodeID",
                principalTable: "FireCodes",
                principalColumn: "FireCodeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_States_StateID",
                table: "Cities",
                column: "StateID",
                principalTable: "States",
                principalColumn: "StateID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoistWayDatas_LandingSystems_LandingSystemID",
                table: "HoistWayDatas",
                column: "LandingSystemID",
                principalTable: "LandingSystems",
                principalColumn: "LandingSystemID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_Cities_CityID",
                table: "Jobs",
                column: "CityID",
                principalTable: "Cities",
                principalColumn: "CityID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_FireCodes_FireCodeID",
                table: "Jobs",
                column: "FireCodeID",
                principalTable: "FireCodes",
                principalColumn: "FireCodeID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_JobTypes_JobTypeID",
                table: "Jobs",
                column: "JobTypeID",
                principalTable: "JobTypes",
                principalColumn: "JobTypeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_JobsExtensions_DoorOperators_DoorOperatorID",
                table: "JobsExtensions",
                column: "DoorOperatorID",
                principalTable: "DoorOperators",
                principalColumn: "DoorOperatorID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_States_Countries_CountryID",
                table: "States",
                column: "CountryID",
                principalTable: "Countries",
                principalColumn: "CountryID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_FireCodes_FirecodeID",
                table: "Cities");

            migrationBuilder.DropForeignKey(
                name: "FK_Cities_States_StateID",
                table: "Cities");

            migrationBuilder.DropForeignKey(
                name: "FK_HoistWayDatas_LandingSystems_LandingSystemID",
                table: "HoistWayDatas");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_Cities_CityID",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_FireCodes_FireCodeID",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_JobTypes_JobTypeID",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_JobsExtensions_DoorOperators_DoorOperatorID",
                table: "JobsExtensions");

            migrationBuilder.DropForeignKey(
                name: "FK_States_Countries_CountryID",
                table: "States");

            migrationBuilder.DropIndex(
                name: "IX_States_CountryID",
                table: "States");

            migrationBuilder.DropIndex(
                name: "IX_JobsExtensions_DoorOperatorID",
                table: "JobsExtensions");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_CityID",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_FireCodeID",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_JobTypeID",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_HoistWayDatas_LandingSystemID",
                table: "HoistWayDatas");

            migrationBuilder.DropIndex(
                name: "IX_Cities_FirecodeID",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_StateID",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "CountryID",
                table: "States");

            migrationBuilder.DropColumn(
                name: "DoorOperatorID",
                table: "JobsExtensions");

            migrationBuilder.DropColumn(
                name: "CityID",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "FireCodeID",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "JobTypeID",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "LandingSystemID",
                table: "HoistWayDatas");

            migrationBuilder.DropColumn(
                name: "FirecodeID",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "StateID",
                table: "Cities");
        }
    }
}
