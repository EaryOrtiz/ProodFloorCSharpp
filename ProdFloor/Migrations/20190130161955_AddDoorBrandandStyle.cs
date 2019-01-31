using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class AddDoorBrandandStyle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoorBrand",
                table: "JobsExtensions");

            migrationBuilder.DropColumn(
                name: "DoorStyle",
                table: "JobsExtensions");
        }
    }
}
