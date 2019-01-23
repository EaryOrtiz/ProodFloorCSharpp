using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class Indicators : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoorStyle",
                table: "JobsExtensions");

            migrationBuilder.AddColumn<string>(
                name: "JobCity",
                table: "Jobs",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobCountry",
                table: "Jobs",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LatestFinishDate",
                table: "Jobs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Indicators",
                columns: table => new
                {
                    IndicatorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Voltage = table.Column<int>(nullable: false),
                    VotageType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Indicators", x => x.IndicatorID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Indicators");

            migrationBuilder.DropColumn(
                name: "JobCity",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "JobCountry",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "LatestFinishDate",
                table: "Jobs");

            migrationBuilder.AddColumn<string>(
                name: "DoorStyle",
                table: "JobsExtensions",
                nullable: true);
        }
    }
}
