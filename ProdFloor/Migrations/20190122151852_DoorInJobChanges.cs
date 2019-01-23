using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class DoorInJobChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoorBrand",
                table: "JobsExtensions");

            migrationBuilder.DropColumn(
                name: "DoorModel",
                table: "JobsExtensions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DoorBrand",
                table: "JobsExtensions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DoorModel",
                table: "JobsExtensions",
                nullable: false,
                defaultValue: "");
        }
    }
}
