using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class NullStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Jobs",
                maxLength: 26,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 26);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Jobs",
                maxLength: 26,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 26,
                oldNullable: true);
        }
    }
}
