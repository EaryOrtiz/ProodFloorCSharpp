using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class StatusToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TestJobs",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "TestJobs",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
