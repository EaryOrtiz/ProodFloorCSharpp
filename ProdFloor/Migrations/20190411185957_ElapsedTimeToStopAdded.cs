using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class ElapsedTimeToStopAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Stops",
                newName: "StartDate");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Stops",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Elapsed",
                table: "Stops",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Elapsed",
                table: "Stops");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Stops",
                newName: "Start");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Stops",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

        }
    }
}
