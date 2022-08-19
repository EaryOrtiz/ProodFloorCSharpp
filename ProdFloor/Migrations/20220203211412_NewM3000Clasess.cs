using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProdFloor.Migrations
{
    public partial class NewM3000Clasess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "M3000s",
                columns: table => new
                {
                    M3000ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobID = table.Column<int>(nullable: false),
                    InputVoltage = table.Column<int>(nullable: false),
                    InputPhase = table.Column<int>(nullable: false),
                    InputFrecuency = table.Column<int>(nullable: false),
                    Speed = table.Column<int>(nullable: false),
                    Length = table.Column<int>(nullable: false),
                    ECRCT = table.Column<bool>(nullable: false),
                    ControlType = table.Column<string>(nullable: false),
                    NEMA = table.Column<string>(nullable: false),
                    ControlPanel = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M3000s", x => x.M3000ID);
                    table.ForeignKey(
                        name: "FK_M3000s_Jobs_JobID",
                        column: x => x.JobID,
                        principalTable: "Jobs",
                        principalColumn: "JobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MotorInfos",
                columns: table => new
                {
                    MotorInfoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobID = table.Column<int>(nullable: false),
                    Voltage = table.Column<int>(nullable: false),
                    HP = table.Column<int>(nullable: false),
                    FLA = table.Column<float>(nullable: false),
                    Contactor = table.Column<string>(nullable: false),
                    MainBrake = table.Column<bool>(nullable: false),
                    MainBrakeType = table.Column<string>(nullable: true),
                    MainBrakeContact = table.Column<string>(nullable: true),
                    AuxBrake = table.Column<bool>(nullable: false),
                    AuxBrakeType = table.Column<string>(nullable: true),
                    AuxBrakeContact = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MotorInfos", x => x.MotorInfoID);
                    table.ForeignKey(
                        name: "FK_MotorInfos_Jobs_JobID",
                        column: x => x.JobID,
                        principalTable: "Jobs",
                        principalColumn: "JobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperatingFeatures",
                columns: table => new
                {
                    OperatingFeaturesID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobID = table.Column<int>(nullable: false),
                    TandemOperation = table.Column<bool>(nullable: false),
                    AutoChainLubrication = table.Column<bool>(nullable: false),
                    AutoChainLubriVoltage = table.Column<int>(nullable: false),
                    DisplayModule = table.Column<string>(nullable: false),
                    SmokeDetector = table.Column<bool>(nullable: false),
                    RemoteMonitoring = table.Column<bool>(nullable: false),
                    RemoteMonitoringType = table.Column<string>(nullable: true),
                    Thermistor = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatingFeatures", x => x.OperatingFeaturesID);
                    table.ForeignKey(
                        name: "FK_OperatingFeatures_Jobs_JobID",
                        column: x => x.JobID,
                        principalTable: "Jobs",
                        principalColumn: "JobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_M3000s_JobID",
                table: "M3000s",
                column: "JobID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MotorInfos_JobID",
                table: "MotorInfos",
                column: "JobID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperatingFeatures_JobID",
                table: "OperatingFeatures",
                column: "JobID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "M3000s");

            migrationBuilder.DropTable(
                name: "MotorInfos");

            migrationBuilder.DropTable(
                name: "OperatingFeatures");
        }
    }
}
