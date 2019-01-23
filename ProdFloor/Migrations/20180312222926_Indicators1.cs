using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class Indicators1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Indicators");

            migrationBuilder.CreateTable(
                name: "StatusIndicators",
                columns: table => new
                {
                    StatusIndicatorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Voltage = table.Column<int>(nullable: false),
                    VoltageType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusIndicators", x => x.StatusIndicatorID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusIndicators");

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
    }
}
