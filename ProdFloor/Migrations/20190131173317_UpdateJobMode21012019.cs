using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class UpdateJobMode21012019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoorBrand",
                table: "JobsExtensions");

            migrationBuilder.DropColumn(
                name: "DoorStyle",
                table: "JobsExtensions");

            migrationBuilder.DropColumn(
                name: "CountryID",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "StateID",
                table: "Jobs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DoorBrand",
                table: "JobsExtensions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DoorStyle",
                table: "JobsExtensions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CountryID",
                table: "Jobs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StateID",
                table: "Jobs",
                nullable: false,
                defaultValue: 0);
        }
    }
}
