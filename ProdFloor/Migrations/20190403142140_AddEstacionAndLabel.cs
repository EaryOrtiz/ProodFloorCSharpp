using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class AddEstacionAndLabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "JobLabel",
                table: "TestJobs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Station",
                table: "TestJobs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobLabel",
                table: "TestJobs");

            migrationBuilder.DropColumn(
                name: "Station",
                table: "TestJobs");
           
        }
    }
}
