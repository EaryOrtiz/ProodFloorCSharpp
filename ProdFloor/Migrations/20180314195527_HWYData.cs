using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class HWYData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StatusIndicators");

            migrationBuilder.CreateTable(
                name: "HoistWayDatas",
                columns: table => new
                {
                    HoistWayDataID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Capacity = table.Column<int>(nullable: false),
                    DownSpeed = table.Column<int>(nullable: false),
                    FrontEightServed = table.Column<bool>(nullable: false),
                    FrontEleventhServed = table.Column<bool>(nullable: false),
                    FrontFifteenthServed = table.Column<bool>(nullable: false),
                    FrontFifthServed = table.Column<bool>(nullable: false),
                    FrontFirstServed = table.Column<bool>(nullable: false),
                    FrontFourteenthServed = table.Column<bool>(nullable: false),
                    FrontFourthServed = table.Column<bool>(nullable: false),
                    FrontNinthServed = table.Column<bool>(nullable: false),
                    FrontSecondServed = table.Column<bool>(nullable: false),
                    FrontSeventhServed = table.Column<bool>(nullable: false),
                    FrontSexthServed = table.Column<bool>(nullable: false),
                    FrontSixteenthServed = table.Column<bool>(nullable: false),
                    FrontTenthServed = table.Column<bool>(nullable: false),
                    FrontThirdServed = table.Column<bool>(nullable: false),
                    FrontThirteenthServed = table.Column<bool>(nullable: false),
                    FrontTwelvethServed = table.Column<bool>(nullable: false),
                    JobID = table.Column<int>(nullable: false),
                    LandingSystem = table.Column<string>(nullable: true),
                    RearEightServed = table.Column<bool>(nullable: false),
                    RearEleventhServed = table.Column<bool>(nullable: false),
                    RearFifteenthServed = table.Column<bool>(nullable: false),
                    RearFifthServed = table.Column<bool>(nullable: false),
                    RearFirstServed = table.Column<bool>(nullable: false),
                    RearFourteenthServed = table.Column<bool>(nullable: false),
                    RearFourthServed = table.Column<bool>(nullable: false),
                    RearNinthServed = table.Column<bool>(nullable: false),
                    RearSecondServed = table.Column<bool>(nullable: false),
                    RearSeventhServed = table.Column<bool>(nullable: false),
                    RearSexthServed = table.Column<bool>(nullable: false),
                    RearSixteenthServed = table.Column<bool>(nullable: false),
                    RearTenthServed = table.Column<bool>(nullable: false),
                    RearThirdServed = table.Column<bool>(nullable: false),
                    RearThirteenthServed = table.Column<bool>(nullable: false),
                    RearTwelvethServed = table.Column<bool>(nullable: false),
                    TotalTravel = table.Column<int>(nullable: false),
                    UpSpeed = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoistWayDatas", x => x.HoistWayDataID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoistWayDatas");

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
    }
}
