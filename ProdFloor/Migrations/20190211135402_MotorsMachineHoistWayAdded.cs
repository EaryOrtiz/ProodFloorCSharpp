using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class MotorsMachineHoistWayAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MotorsDisconnect",
                table: "HydroSpecifics",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HoistWaysNumber",
                table: "HoistWayDatas",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MachineRooms",
                table: "HoistWayDatas",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MotorsDisconnect",
                table: "HydroSpecifics");

            migrationBuilder.DropColumn(
                name: "HoistWaysNumber",
                table: "HoistWayDatas");

            migrationBuilder.DropColumn(
                name: "MachineRooms",
                table: "HoistWayDatas");
        }
    }
}
