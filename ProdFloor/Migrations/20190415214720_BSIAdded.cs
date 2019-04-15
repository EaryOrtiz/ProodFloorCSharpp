using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class BSIAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Ind",
                table: "GenericFeaturesList",
                newName: "Pit");

            migrationBuilder.AddColumn<bool>(
                name: "BSI",
                table: "GenericFeaturesList",
                nullable: false,
                defaultValue: false);
            /*
            migrationBuilder.CreateIndex(
                name: "IX_Jobs_PO",
                table: "Jobs",
                column: "PO",
                unique: true);
                */
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Jobs_PO",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "BSI",
                table: "GenericFeaturesList");

            migrationBuilder.RenameColumn(
                name: "Pit",
                table: "GenericFeaturesList",
                newName: "Ind");
        }
    }
}
