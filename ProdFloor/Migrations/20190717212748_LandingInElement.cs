using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class LandingInElement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LandingSystemID",
                table: "Elements",
                nullable: false,
                defaultValue: 5);

            migrationBuilder.CreateIndex(
                name: "IX_Elements_LandingSystemID",
                table: "Elements",
                column: "LandingSystemID");

            migrationBuilder.AddForeignKey(
                name: "FK_Elements_LandingSystems_LandingSystemID",
                table: "Elements",
                column: "LandingSystemID",
                principalTable: "LandingSystems",
                principalColumn: "LandingSystemID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Elements_LandingSystems_LandingSystemID",
                table: "Elements");

            migrationBuilder.DropIndex(
                name: "IX_Elements_LandingSystemID",
                table: "Elements");

            migrationBuilder.DropColumn(
                name: "LandingSystemID",
                table: "Elements");
        }
    }
}
