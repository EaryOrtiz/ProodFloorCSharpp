using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class JobCorrections01232019 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobCountry",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "JobState",
                table: "Jobs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobCountry",
                table: "Jobs",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobState",
                table: "Jobs",
                nullable: false,
                defaultValue: "");
        }
    }
}
