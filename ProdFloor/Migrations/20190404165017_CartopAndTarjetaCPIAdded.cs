using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class CartopAndTarjetaCPIAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MBrake",
                table: "TestFeatures",
                newName: "TrajetaCPI");

            migrationBuilder.AddColumn<bool>(
                name: "Cartop",
                table: "TestFeatures",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EMBrake",
                table: "TestFeatures",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cartop",
                table: "TestFeatures");

            migrationBuilder.DropColumn(
                name: "EMBrake",
                table: "TestFeatures");

            migrationBuilder.RenameColumn(
                name: "TrajetaCPI",
                table: "TestFeatures",
                newName: "MBrake");
        }
    }
}
