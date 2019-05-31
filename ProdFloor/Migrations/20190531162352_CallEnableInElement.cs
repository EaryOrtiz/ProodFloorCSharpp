using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class CallEnableInElement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ElementHydros",
                columns: table => new
                {
                    ElementHydroID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FLA = table.Column<int>(nullable: false),
                    HP = table.Column<int>(nullable: false),
                    JobID = table.Column<int>(nullable: false),
                    SPH = table.Column<int>(nullable: false),
                    Starter = table.Column<string>(nullable: true),
                    ValveBrand = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementHydros", x => x.ElementHydroID);
                    table.ForeignKey(
                        name: "FK_ElementHydros_Jobs_JobID",
                        column: x => x.JobID,
                        principalTable: "Jobs",
                        principalColumn: "JobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Elements",
                columns: table => new
                {
                    ElementID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CRO = table.Column<bool>(nullable: false),
                    CReg = table.Column<bool>(nullable: false),
                    CTINSPST = table.Column<bool>(nullable: false),
                    CallEnable = table.Column<bool>(nullable: false),
                    Capacity = table.Column<int>(nullable: false),
                    CarCardReader = table.Column<bool>(nullable: false),
                    CarKey = table.Column<bool>(nullable: false),
                    DHLD = table.Column<bool>(nullable: false),
                    DoorGate = table.Column<string>(maxLength: 50, nullable: false),
                    DoorOperatorID = table.Column<int>(nullable: false),
                    EMT = table.Column<bool>(nullable: false),
                    EP = table.Column<bool>(nullable: false),
                    Egress = table.Column<bool>(nullable: false),
                    Frequency = table.Column<int>(nullable: false),
                    HAPS = table.Column<bool>(nullable: false),
                    HCRO = table.Column<bool>(nullable: false),
                    HallCardReader = table.Column<bool>(nullable: false),
                    HallKey = table.Column<bool>(nullable: false),
                    INA = table.Column<string>(nullable: true),
                    INCP = table.Column<bool>(nullable: false),
                    JobID = table.Column<int>(nullable: false),
                    LoadWeigher = table.Column<string>(nullable: true),
                    PHECutOut = table.Column<bool>(nullable: false),
                    PSS = table.Column<bool>(nullable: false),
                    PTFLD = table.Column<bool>(nullable: false),
                    Phase = table.Column<int>(nullable: false),
                    Speed = table.Column<int>(nullable: false),
                    Traveler = table.Column<bool>(nullable: false),
                    VCI = table.Column<bool>(nullable: false),
                    Voltage = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Elements", x => x.ElementID);
                    table.ForeignKey(
                        name: "FK_Elements_DoorOperators_DoorOperatorID",
                        column: x => x.DoorOperatorID,
                        principalTable: "DoorOperators",
                        principalColumn: "DoorOperatorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Elements_Jobs_JobID",
                        column: x => x.JobID,
                        principalTable: "Jobs",
                        principalColumn: "JobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ElementTractions",
                columns: table => new
                {
                    ElementTractionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Contact = table.Column<string>(nullable: true),
                    Current = table.Column<int>(nullable: false),
                    Encoder = table.Column<bool>(nullable: false),
                    FLA = table.Column<int>(nullable: false),
                    HP = table.Column<int>(nullable: false),
                    HoldVoltage = table.Column<int>(nullable: false),
                    ISO = table.Column<bool>(nullable: false),
                    JobID = table.Column<int>(nullable: false),
                    MachineLocation = table.Column<string>(nullable: true),
                    MotorBrand = table.Column<string>(nullable: true),
                    PickVoltage = table.Column<int>(nullable: false),
                    Resistance = table.Column<int>(nullable: false),
                    VVVF = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ElementTractions", x => x.ElementTractionID);
                    table.ForeignKey(
                        name: "FK_ElementTractions_Jobs_JobID",
                        column: x => x.JobID,
                        principalTable: "Jobs",
                        principalColumn: "JobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ElementHydros_JobID",
                table: "ElementHydros",
                column: "JobID");

            migrationBuilder.CreateIndex(
                name: "IX_Elements_DoorOperatorID",
                table: "Elements",
                column: "DoorOperatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Elements_JobID",
                table: "Elements",
                column: "JobID");

            migrationBuilder.CreateIndex(
                name: "IX_ElementTractions_JobID",
                table: "ElementTractions",
                column: "JobID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ElementHydros");

            migrationBuilder.DropTable(
                name: "Elements");

            migrationBuilder.DropTable(
                name: "ElementTractions");
        }
    }
}
