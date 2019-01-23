using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class NamesAndReq : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "JobTypeMain",
                table: "JobsExtensions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JobTypeAdd",
                table: "JobsExtensions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DoorModel",
                table: "JobsExtensions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DoorHoist",
                table: "JobsExtensions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DoorGate",
                table: "JobsExtensions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DoorBrand",
                table: "JobsExtensions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IndicatorsVoltageType",
                table: "Indicators",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IndicatorsVoltage",
                table: "Indicators",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "HallLanternsVoltage",
                table: "Indicators",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "HallCallsVoltageType",
                table: "Indicators",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HallCallsVoltage",
                table: "Indicators",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "HallCallsType",
                table: "Indicators",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CarLanternsVoltage",
                table: "Indicators",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "CarCallsVoltageType",
                table: "Indicators",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CarCallsVoltage",
                table: "Indicators",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "CarCallsType",
                table: "Indicators",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ValveModel",
                table: "HydroSpecifics",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ValveBrand",
                table: "HydroSpecifics",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Starter",
                table: "HydroSpecifics",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LandingSystem",
                table: "HoistWayDatas",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CRO",
                table: "GenericFeaturesList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CarCallRead",
                table: "GenericFeaturesList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CarKey",
                table: "GenericFeaturesList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HCRO",
                table: "GenericFeaturesList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HallCallRead",
                table: "GenericFeaturesList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HallKey",
                table: "GenericFeaturesList",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CRO",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "CarCallRead",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "CarKey",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "HCRO",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "HallCallRead",
                table: "GenericFeaturesList");

            migrationBuilder.DropColumn(
                name: "HallKey",
                table: "GenericFeaturesList");

            migrationBuilder.AlterColumn<string>(
                name: "JobTypeMain",
                table: "JobsExtensions",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "JobTypeAdd",
                table: "JobsExtensions",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "DoorModel",
                table: "JobsExtensions",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "DoorHoist",
                table: "JobsExtensions",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "DoorGate",
                table: "JobsExtensions",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "DoorBrand",
                table: "JobsExtensions",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "IndicatorsVoltageType",
                table: "Indicators",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "IndicatorsVoltage",
                table: "Indicators",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "HallLanternsVoltage",
                table: "Indicators",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HallCallsVoltageType",
                table: "Indicators",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "HallCallsVoltage",
                table: "Indicators",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "HallCallsType",
                table: "Indicators",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "CarLanternsVoltage",
                table: "Indicators",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CarCallsVoltageType",
                table: "Indicators",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "CarCallsVoltage",
                table: "Indicators",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "CarCallsType",
                table: "Indicators",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ValveModel",
                table: "HydroSpecifics",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "ValveBrand",
                table: "HydroSpecifics",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Starter",
                table: "HydroSpecifics",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "LandingSystem",
                table: "HoistWayDatas",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
