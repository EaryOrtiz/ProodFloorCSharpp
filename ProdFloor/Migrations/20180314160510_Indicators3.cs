using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class Indicators3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Indicators",
                columns: table => new
                {
                    IndicatorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CarCallsType = table.Column<string>(nullable: true),
                    CarCallsVoltage = table.Column<int>(nullable: false),
                    CarCallsVoltageType = table.Column<string>(nullable: true),
                    CarLanterns = table.Column<bool>(nullable: false),
                    CarLanternsType = table.Column<string>(nullable: true),
                    CarLanternsVoltage = table.Column<int>(nullable: false),
                    CarLanternsVoltageType = table.Column<string>(nullable: true),
                    CarPI = table.Column<bool>(nullable: false),
                    CarPIDiscreteType = table.Column<string>(nullable: true),
                    CarPIDiscreteVoltage = table.Column<string>(nullable: true),
                    CarPIDiscreteVoltageType = table.Column<string>(nullable: true),
                    CarPIType = table.Column<string>(nullable: true),
                    HallCallsType = table.Column<string>(nullable: true),
                    HallCallsVoltage = table.Column<int>(nullable: false),
                    HallCallsVoltageType = table.Column<string>(nullable: true),
                    HallLanterns = table.Column<bool>(nullable: false),
                    HallLanternsType = table.Column<string>(nullable: true),
                    HallLanternsVoltage = table.Column<int>(nullable: false),
                    HallLanternsVoltageType = table.Column<string>(nullable: true),
                    HallPI = table.Column<bool>(nullable: false),
                    HallPIDiscreteType = table.Column<string>(nullable: true),
                    HallPIDiscreteVoltage = table.Column<string>(nullable: true),
                    HallPIDiscreteVoltageType = table.Column<string>(nullable: true),
                    HallPIType = table.Column<string>(nullable: true),
                    IndicatorsVoltage = table.Column<int>(nullable: false),
                    IndicatorsVoltageType = table.Column<string>(nullable: true),
                    JobID = table.Column<int>(nullable: false),
                    PassingFloor = table.Column<bool>(nullable: false),
                    PassingFloorDiscreteType = table.Column<string>(nullable: true),
                    PassingFloorDiscreteVoltage = table.Column<string>(nullable: true),
                    PassingFloorDiscreteVoltageType = table.Column<string>(nullable: true),
                    PassingFloorEnable = table.Column<bool>(nullable: false),
                    PassingFloorType = table.Column<string>(nullable: true),
                    VoiceAnnunciationPI = table.Column<bool>(nullable: false),
                    VoiceAnnunciationPIType = table.Column<string>(nullable: true)
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
        }
    }
}
