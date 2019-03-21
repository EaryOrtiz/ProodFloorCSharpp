using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class JobTypeInStep : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobTypeID",
                table: "Steps",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Steps_JobTypeID",
                table: "Steps",
                column: "JobTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Steps_JobTypes_JobTypeID",
                table: "Steps",
                column: "JobTypeID",
                principalTable: "JobTypes",
                principalColumn: "JobTypeID",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Steps_JobTypes_JobTypeID",
                table: "Steps");

            migrationBuilder.DropIndex(
                name: "IX_Steps_JobTypeID",
                table: "Steps");

            migrationBuilder.DropColumn(
                name: "JobTypeID",
                table: "Steps");
        }
    }
}
