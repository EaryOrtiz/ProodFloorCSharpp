using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class MeetingUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarLanternsVoltage",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "CarLanternsVoltageType",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "CarPIDiscreteVoltage",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "CarPIDiscreteVoltageType",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "HallLanternsVoltage",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "HallLanternsVoltageType",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "HallPIDiscreteVoltage",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "HallPIDiscreteVoltageType",
                table: "Indicators");

            migrationBuilder.RenameColumn(
                name: "PassingFloorDiscreteVoltageType",
                table: "Indicators",
                newName: "HallLanternsStyle");

            migrationBuilder.RenameColumn(
                name: "PassingFloorDiscreteVoltage",
                table: "Indicators",
                newName: "CarLanternsStyle");

            migrationBuilder.RenameColumn(
                name: "MView",
                table: "GenericFeaturesList",
                newName: "TopAccess");

            migrationBuilder.RenameColumn(
                name: "IMon",
                table: "GenericFeaturesList",
                newName: "Roped");

            migrationBuilder.RenameColumn(
                name: "IDS",
                table: "GenericFeaturesList",
                newName: "EPVoltage");

            migrationBuilder.AddColumn<bool>(
                name: "BottomAccess",
                table: "GenericFeaturesList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BottomAccessLocation",
                table: "GenericFeaturesList",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CTINSPST",
                table: "GenericFeaturesList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EPCarsNumber",
                table: "GenericFeaturesList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EPContact",
                table: "GenericFeaturesList",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EPOtherCars",
                table: "GenericFeaturesList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EPSelect",
                table: "GenericFeaturesList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovModel",
                table: "GenericFeaturesList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "INCPButtons",
                table: "GenericFeaturesList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Monitoring",
                table: "GenericFeaturesList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PTI",
                table: "GenericFeaturesList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TopAccessLocation",
                table: "GenericFeaturesList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BottomAccess",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "BottomAccessLocation",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "CTINSPST",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "EPCarsNumber",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "EPContact",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "EPOtherCars",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "EPSelect",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "GovModel",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "INCPButtons",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "Monitoring",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "PTI",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "TopAccessLocation",
                table: "GenericFeaturesList");

            migrationBuilder.RenameColumn(
                name: "HallLanternsStyle",
                table: "Indicators",
                newName: "PassingFloorDiscreteVoltageType");

            migrationBuilder.RenameColumn(
                name: "CarLanternsStyle",
                table: "Indicators",
                newName: "PassingFloorDiscreteVoltage");

            migrationBuilder.RenameColumn(
                name: "TopAccess",
                table: "GenericFeaturesList",
                newName: "MView");

            migrationBuilder.RenameColumn(
                name: "Roped",
                table: "GenericFeaturesList",
                newName: "IMon");

            migrationBuilder.RenameColumn(
                name: "EPVoltage",
                table: "GenericFeaturesList",
                newName: "IDS");

            migrationBuilder.AddColumn<string>(
                name: "CarLanternsVoltage",
                table: "Indicators",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarLanternsVoltageType",
                table: "Indicators",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarPIDiscreteVoltage",
                table: "Indicators",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarPIDiscreteVoltageType",
                table: "Indicators",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HallLanternsVoltage",
                table: "Indicators",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HallLanternsVoltageType",
                table: "Indicators",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HallPIDiscreteVoltage",
                table: "Indicators",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HallPIDiscreteVoltageType",
                table: "Indicators",
                nullable: true);
        }
    }
}
