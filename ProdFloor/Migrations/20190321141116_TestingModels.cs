using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class TestingModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reasons1",
                columns: table => new
                {
                    Reason1ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reasons1", x => x.Reason1ID);
                });

            migrationBuilder.CreateTable(
                name: "Steps",
                columns: table => new
                {
                    StepID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false),
                    ExpectedTime = table.Column<DateTime>(nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Stage = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Steps", x => x.StepID);
                });

            migrationBuilder.CreateTable(
                name: "TestJobs",
                columns: table => new
                {
                    TestJobID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobID = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TechnicianID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestJobs", x => x.TestJobID);
                    table.ForeignKey(
                        name: "FK_TestJobs_Jobs_JobID",
                        column: x => x.JobID,
                        principalTable: "Jobs",
                        principalColumn: "JobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reasons2",
                columns: table => new
                {
                    Reason2ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false),
                    Reason1ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reasons2", x => x.Reason2ID);
                    table.ForeignKey(
                        name: "FK_Reasons2_Reasons1_Reason1ID",
                        column: x => x.Reason1ID,
                        principalTable: "Reasons1",
                        principalColumn: "Reason1ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TriggeringFeatures",
                columns: table => new
                {
                    TriggeringFeatureID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsSelected = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    StepID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggeringFeatures", x => x.TriggeringFeatureID);
                    table.ForeignKey(
                        name: "FK_TriggeringFeatures_Steps_StepID",
                        column: x => x.StepID,
                        principalTable: "Steps",
                        principalColumn: "StepID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StepsForJobs",
                columns: table => new
                {
                    StepsForJobID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Complete = table.Column<bool>(nullable: false),
                    Consecutivo = table.Column<int>(nullable: false),
                    Elapsed = table.Column<DateTime>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    StepID = table.Column<int>(nullable: false),
                    Stop = table.Column<DateTime>(nullable: false),
                    TestJobID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepsForJobs", x => x.StepsForJobID);
                    table.ForeignKey(
                        name: "FK_StepsForJobs_Steps_StepID",
                        column: x => x.StepID,
                        principalTable: "Steps",
                        principalColumn: "StepID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StepsForJobs_TestJobs_TestJobID",
                        column: x => x.TestJobID,
                        principalTable: "TestJobs",
                        principalColumn: "TestJobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestFeatures",
                columns: table => new
                {
                    TestFeatureID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BrakeCoilVoltageMoreThan10 = table.Column<bool>(nullable: false),
                    CTL2 = table.Column<bool>(nullable: false),
                    Custom = table.Column<bool>(nullable: false),
                    EMCO = table.Column<bool>(nullable: false),
                    Group = table.Column<bool>(nullable: false),
                    Local = table.Column<bool>(nullable: false),
                    MBrake = table.Column<bool>(nullable: false),
                    MRL = table.Column<bool>(nullable: false),
                    Overlay = table.Column<bool>(nullable: false),
                    PC = table.Column<bool>(nullable: false),
                    R6 = table.Column<bool>(nullable: false),
                    ShortFloor = table.Column<bool>(nullable: false),
                    TestJobID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestFeatures", x => x.TestFeatureID);
                    table.ForeignKey(
                        name: "FK_TestFeatures_TestJobs_TestJobID",
                        column: x => x.TestJobID,
                        principalTable: "TestJobs",
                        principalColumn: "TestJobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reasons3",
                columns: table => new
                {
                    Reason3ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false),
                    Reason2ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reasons3", x => x.Reason3ID);
                    table.ForeignKey(
                        name: "FK_Reasons3_Reasons2_Reason2ID",
                        column: x => x.Reason2ID,
                        principalTable: "Reasons2",
                        principalColumn: "Reason2ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reasons4",
                columns: table => new
                {
                    Reason4ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false),
                    Reason3ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reasons4", x => x.Reason4ID);
                    table.ForeignKey(
                        name: "FK_Reasons4_Reasons3_Reason3ID",
                        column: x => x.Reason3ID,
                        principalTable: "Reasons3",
                        principalColumn: "Reason3ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reasons5",
                columns: table => new
                {
                    Reason5ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false),
                    Reason4ID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reasons5", x => x.Reason5ID);
                    table.ForeignKey(
                        name: "FK_Reasons5_Reasons4_Reason4ID",
                        column: x => x.Reason4ID,
                        principalTable: "Reasons4",
                        principalColumn: "Reason4ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stops",
                columns: table => new
                {
                    StopID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false),
                    Reason1ID = table.Column<int>(nullable: false),
                    Reason2ID = table.Column<int>(nullable: false),
                    Reason3ID = table.Column<int>(nullable: false),
                    Reason4ID = table.Column<int>(nullable: false),
                    Reason5ID = table.Column<int>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    StopDate = table.Column<DateTime>(nullable: false),
                    TestJobID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stops", x => x.StopID);
                    table.ForeignKey(
                        name: "FK_Stops_Reasons1_Reason1ID",
                        column: x => x.Reason1ID,
                        principalTable: "Reasons1",
                        principalColumn: "Reason1ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Stops_Reasons2_Reason2ID",
                        column: x => x.Reason2ID,
                        principalTable: "Reasons2",
                        principalColumn: "Reason2ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Stops_Reasons3_Reason3ID",
                        column: x => x.Reason3ID,
                        principalTable: "Reasons3",
                        principalColumn: "Reason3ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Stops_Reasons4_Reason4ID",
                        column: x => x.Reason4ID,
                        principalTable: "Reasons4",
                        principalColumn: "Reason4ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Stops_Reasons5_Reason5ID",
                        column: x => x.Reason5ID,
                        principalTable: "Reasons5",
                        principalColumn: "Reason5ID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Stops_TestJobs_TestJobID",
                        column: x => x.TestJobID,
                        principalTable: "TestJobs",
                        principalColumn: "TestJobID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reasons2_Reason1ID",
                table: "Reasons2",
                column: "Reason1ID");

            migrationBuilder.CreateIndex(
                name: "IX_Reasons3_Reason2ID",
                table: "Reasons3",
                column: "Reason2ID");

            migrationBuilder.CreateIndex(
                name: "IX_Reasons4_Reason3ID",
                table: "Reasons4",
                column: "Reason3ID");

            migrationBuilder.CreateIndex(
                name: "IX_Reasons5_Reason4ID",
                table: "Reasons5",
                column: "Reason4ID");

            migrationBuilder.CreateIndex(
                name: "IX_StepsForJobs_StepID",
                table: "StepsForJobs",
                column: "StepID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StepsForJobs_TestJobID",
                table: "StepsForJobs",
                column: "TestJobID");

            migrationBuilder.CreateIndex(
                name: "IX_Stops_Reason1ID",
                table: "Stops",
                column: "Reason1ID");

            migrationBuilder.CreateIndex(
                name: "IX_Stops_Reason2ID",
                table: "Stops",
                column: "Reason2ID");

            migrationBuilder.CreateIndex(
                name: "IX_Stops_Reason3ID",
                table: "Stops",
                column: "Reason3ID");

            migrationBuilder.CreateIndex(
                name: "IX_Stops_Reason4ID",
                table: "Stops",
                column: "Reason4ID");

            migrationBuilder.CreateIndex(
                name: "IX_Stops_Reason5ID",
                table: "Stops",
                column: "Reason5ID");

            migrationBuilder.CreateIndex(
                name: "IX_Stops_TestJobID",
                table: "Stops",
                column: "TestJobID");

            migrationBuilder.CreateIndex(
                name: "IX_TestFeatures_TestJobID",
                table: "TestFeatures",
                column: "TestJobID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TestJobs_JobID",
                table: "TestJobs",
                column: "JobID");

            migrationBuilder.CreateIndex(
                name: "IX_TriggeringFeatures_StepID",
                table: "TriggeringFeatures",
                column: "StepID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StepsForJobs");

            migrationBuilder.DropTable(
                name: "Stops");

            migrationBuilder.DropTable(
                name: "TestFeatures");

            migrationBuilder.DropTable(
                name: "TriggeringFeatures");

            migrationBuilder.DropTable(
                name: "Reasons5");

            migrationBuilder.DropTable(
                name: "TestJobs");

            migrationBuilder.DropTable(
                name: "Steps");

            migrationBuilder.DropTable(
                name: "Reasons4");

            migrationBuilder.DropTable(
                name: "Reasons3");

            migrationBuilder.DropTable(
                name: "Reasons2");

            migrationBuilder.DropTable(
                name: "Reasons1");
        }
    }
}
