using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class BooleanToGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AltRis",
                table: "JobsExtensions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BackUpDisp",
                table: "JobsExtensions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SwingOp",
                table: "JobsExtensions",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltRis",
                table: "JobsExtensions");

            migrationBuilder.DropColumn(
                name: "BackUpDisp",
                table: "JobsExtensions");

            migrationBuilder.DropColumn(
                name: "SwingOp",
                table: "JobsExtensions");
        }
    }
}
