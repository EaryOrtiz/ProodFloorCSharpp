using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class Name2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Jobs",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 78);

            migrationBuilder.AddColumn<string>(
                name: "Name2",
                table: "Jobs",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name2",
                table: "Jobs");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Jobs",
                maxLength: 78,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);
        }
    }
}
