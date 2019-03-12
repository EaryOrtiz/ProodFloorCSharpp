using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class EnginnerRefTablesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Overloads",
                columns: table => new
                {
                    OverloadID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AMPMax = table.Column<float>(nullable: false),
                    AMPMin = table.Column<float>(nullable: false),
                    MCPart = table.Column<string>(nullable: false),
                    OverTableNum = table.Column<int>(nullable: false),
                    SiemensPart = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Overloads", x => x.OverloadID);
                });

            migrationBuilder.CreateTable(
                name: "Slowdowns",
                columns: table => new
                {
                    SlowdownID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    A = table.Column<int>(nullable: false),
                    CarSpeedFPM = table.Column<int>(nullable: false),
                    Distance = table.Column<int>(nullable: false),
                    MiniumFloorHeight = table.Column<int>(nullable: false),
                    SlowLimit = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slowdowns", x => x.SlowdownID);
                });

            migrationBuilder.CreateTable(
                name: "Starters",
                columns: table => new
                {
                    StarterID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Brand = table.Column<string>(nullable: false),
                    FLA = table.Column<string>(nullable: false),
                    HP = table.Column<float>(nullable: false),
                    MCPart = table.Column<string>(nullable: false),
                    NewManufacturerPart = table.Column<string>(nullable: false),
                    OverloadTable = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Volts = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Starters", x => x.StarterID);
                });

            migrationBuilder.CreateTable(
                name: "WireTypesSizes",
                columns: table => new
                {
                    WireTypesSizeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AMPRating = table.Column<int>(nullable: false),
                    Size = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WireTypesSizes", x => x.WireTypesSizeID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Overloads");

            migrationBuilder.DropTable(
                name: "Slowdowns");

            migrationBuilder.DropTable(
                name: "Starters");

            migrationBuilder.DropTable(
                name: "WireTypesSizes");
        }
    }
}
