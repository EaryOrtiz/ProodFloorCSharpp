using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class NoRopedInHydro : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropColumn(
                name: "Roped",
                table: "HydroSpecifics");
                */
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.AddColumn<bool>(
                name: "Roped",
                table: "HydroSpecifics",
                nullable: false,
                defaultValue: false);
                */
        }
    }
}
