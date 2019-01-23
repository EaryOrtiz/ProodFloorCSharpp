using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class HydroSpecifics : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HydroSpecifics",
                columns: table => new
                {
                    HydroSpecificID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Battery = table.Column<bool>(nullable: false),
                    BatteryBrand = table.Column<string>(nullable: true),
                    FLA = table.Column<int>(nullable: false),
                    HP = table.Column<int>(nullable: false),
                    JobID = table.Column<int>(nullable: false),
                    LOS = table.Column<bool>(nullable: false),
                    LifeJacket = table.Column<bool>(nullable: false),
                    MotorsNum = table.Column<int>(nullable: false),
                    OilCool = table.Column<bool>(nullable: false),
                    OilTank = table.Column<bool>(nullable: false),
                    PSS = table.Column<bool>(nullable: false),
                    Resync = table.Column<bool>(nullable: false),
                    Roped = table.Column<bool>(nullable: false),
                    SPH = table.Column<int>(nullable: false),
                    Starter = table.Column<string>(nullable: true),
                    VCI = table.Column<bool>(nullable: false),
                    ValveBrand = table.Column<string>(nullable: true),
                    ValveCoils = table.Column<int>(nullable: false),
                    ValveModel = table.Column<string>(nullable: true),
                    ValveNum = table.Column<int>(nullable: false),
                    ValveVoltage = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HydroSpecifics", x => x.HydroSpecificID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HydroSpecifics");
        }
    }
}
