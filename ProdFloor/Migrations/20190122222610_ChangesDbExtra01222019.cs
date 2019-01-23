using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class ChangesDbExtra01222019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "States");

            migrationBuilder.DropColumn(
                name: "JobCity",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "JobType",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SafetyCode",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "LandingSystem",
                table: "HoistWayDatas");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "CurrentFireCode",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Cities");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "States",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobCity",
                table: "Jobs",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobType",
                table: "Jobs",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SafetyCode",
                table: "Jobs",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LandingSystem",
                table: "HoistWayDatas",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Cities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrentFireCode",
                table: "Cities",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Cities",
                nullable: true);
        }
    }
}
