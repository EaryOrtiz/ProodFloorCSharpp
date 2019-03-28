using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class StarterTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Starters");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Starters",
                newName: "_Starter");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "_Starter",
                table: "Starters",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Starters",
                nullable: false,
                defaultValue: "");
        }
    }
}
