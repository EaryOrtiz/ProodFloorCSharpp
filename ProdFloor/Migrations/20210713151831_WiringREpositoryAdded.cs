using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProdFloor.Migrations
{
    public partial class WiringREpositoryAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WiringOptions",
                columns: table => new
                {
                    WiringOptionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    isDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiringOptions", x => x.WiringOptionID);
                });

            migrationBuilder.CreateTable(
                name: "WiringReasons1",
                columns: table => new
                {
                    WiringReason1ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiringReasons1", x => x.WiringReason1ID);
                });

            migrationBuilder.CreateTable(
                name: "Wirings",
                columns: table => new
                {
                    WiringID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    POID = table.Column<int>(nullable: false),
                    WirerID = table.Column<int>(nullable: false),
                    StationID = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    CompletedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wirings", x => x.WiringID);
                });

            migrationBuilder.CreateTable(
                name: "WiringSteps",
                columns: table => new
                {
                    WiringStepID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobTypeID = table.Column<int>(nullable: false),
                    Stage = table.Column<string>(nullable: false),
                    ExpectedTime = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiringSteps", x => x.WiringStepID);
                });

            migrationBuilder.CreateTable(
                name: "WiringReasons2",
                columns: table => new
                {
                    WiringReason2ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WiringReason1ID = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiringReasons2", x => x.WiringReason2ID);
                    table.ForeignKey(
                        name: "FK_WiringReasons2_WiringReasons1_WiringReason1ID",
                        column: x => x.WiringReason1ID,
                        principalTable: "WiringReasons1",
                        principalColumn: "WiringReason1ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WirersInvolveds",
                columns: table => new
                {
                    WirersInvolvedID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WiringID = table.Column<int>(nullable: false),
                    WirerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WirersInvolveds", x => x.WirersInvolvedID);
                    table.ForeignKey(
                        name: "FK_WirersInvolveds_Wirings_WiringID",
                        column: x => x.WiringID,
                        principalTable: "Wirings",
                        principalColumn: "WiringID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WiringFeatures",
                columns: table => new
                {
                    WiringFeaturesID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WiringID = table.Column<int>(nullable: false),
                    WiringOptionID = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiringFeatures", x => x.WiringFeaturesID);
                    table.ForeignKey(
                        name: "FK_WiringFeatures_Wirings_WiringID",
                        column: x => x.WiringID,
                        principalTable: "Wirings",
                        principalColumn: "WiringID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WiringStepsForJobs",
                columns: table => new
                {
                    WiringStepForJobID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WiringStepID = table.Column<int>(nullable: false),
                    WiringID = table.Column<int>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    Stop = table.Column<DateTime>(nullable: false),
                    Elapsed = table.Column<DateTime>(nullable: false),
                    Complete = table.Column<bool>(nullable: false),
                    Consecutivo = table.Column<int>(nullable: false),
                    Obsolete = table.Column<bool>(nullable: false),
                    AuxWirerID = table.Column<int>(nullable: false),
                    AuxStationID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiringStepsForJobs", x => x.WiringStepForJobID);
                    table.ForeignKey(
                        name: "FK_WiringStepsForJobs_Wirings_WiringID",
                        column: x => x.WiringID,
                        principalTable: "Wirings",
                        principalColumn: "WiringID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WiringStepsForJobs_WiringSteps_WiringStepID",
                        column: x => x.WiringStepID,
                        principalTable: "WiringSteps",
                        principalColumn: "WiringStepID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WiringTriggeringFeatures",
                columns: table => new
                {
                    WiringTriggeringFeatureID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WiringOptionID = table.Column<int>(nullable: false),
                    WiringStepID = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Equality = table.Column<string>(nullable: true),
                    IsSelected = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiringTriggeringFeatures", x => x.WiringTriggeringFeatureID);
                    table.ForeignKey(
                        name: "FK_WiringTriggeringFeatures_WiringSteps_WiringStepID",
                        column: x => x.WiringStepID,
                        principalTable: "WiringSteps",
                        principalColumn: "WiringStepID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WiringReasons3",
                columns: table => new
                {
                    WiringReason3ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WiringReason2ID = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiringReasons3", x => x.WiringReason3ID);
                    table.ForeignKey(
                        name: "FK_WiringReasons3_WiringReasons2_WiringReason2ID",
                        column: x => x.WiringReason2ID,
                        principalTable: "WiringReasons2",
                        principalColumn: "WiringReason2ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WiringReasons4",
                columns: table => new
                {
                    WiringReason4ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WiringReason3ID = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiringReasons4", x => x.WiringReason4ID);
                    table.ForeignKey(
                        name: "FK_WiringReasons4_WiringReasons3_WiringReason3ID",
                        column: x => x.WiringReason3ID,
                        principalTable: "WiringReasons3",
                        principalColumn: "WiringReason3ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WiringReasons5",
                columns: table => new
                {
                    WiringReason5ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WiringReason4ID = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiringReasons5", x => x.WiringReason5ID);
                    table.ForeignKey(
                        name: "FK_WiringReasons5_WiringReasons4_WiringReason4ID",
                        column: x => x.WiringReason4ID,
                        principalTable: "WiringReasons4",
                        principalColumn: "WiringReason4ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WiringStops",
                columns: table => new
                {
                    WiringStopID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WiringID = table.Column<int>(nullable: false),
                    Reason1 = table.Column<int>(nullable: false),
                    Reason2 = table.Column<int>(nullable: false),
                    Reason3 = table.Column<int>(nullable: false),
                    Reason4 = table.Column<int>(nullable: false),
                    Reason5ID = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    StopDate = table.Column<DateTime>(nullable: false),
                    Elapsed = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Critical = table.Column<bool>(nullable: false),
                    AuxWirerID = table.Column<int>(nullable: false),
                    AuxStationID = table.Column<int>(nullable: false),
                    WiringReason5ID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiringStops", x => x.WiringStopID);
                    table.ForeignKey(
                        name: "FK_WiringStops_Wirings_WiringID",
                        column: x => x.WiringID,
                        principalTable: "Wirings",
                        principalColumn: "WiringID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WiringStops_WiringReasons5_WiringReason5ID",
                        column: x => x.WiringReason5ID,
                        principalTable: "WiringReasons5",
                        principalColumn: "WiringReason5ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WirersInvolveds_WiringID",
                table: "WirersInvolveds",
                column: "WiringID");

            migrationBuilder.CreateIndex(
                name: "IX_WiringFeatures_WiringID",
                table: "WiringFeatures",
                column: "WiringID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WiringReasons2_WiringReason1ID",
                table: "WiringReasons2",
                column: "WiringReason1ID");

            migrationBuilder.CreateIndex(
                name: "IX_WiringReasons3_WiringReason2ID",
                table: "WiringReasons3",
                column: "WiringReason2ID");

            migrationBuilder.CreateIndex(
                name: "IX_WiringReasons4_WiringReason3ID",
                table: "WiringReasons4",
                column: "WiringReason3ID");

            migrationBuilder.CreateIndex(
                name: "IX_WiringReasons5_WiringReason4ID",
                table: "WiringReasons5",
                column: "WiringReason4ID");

            migrationBuilder.CreateIndex(
                name: "IX_WiringStepsForJobs_WiringID",
                table: "WiringStepsForJobs",
                column: "WiringID");

            migrationBuilder.CreateIndex(
                name: "IX_WiringStepsForJobs_WiringStepID",
                table: "WiringStepsForJobs",
                column: "WiringStepID");

            migrationBuilder.CreateIndex(
                name: "IX_WiringStops_WiringID",
                table: "WiringStops",
                column: "WiringID");

            migrationBuilder.CreateIndex(
                name: "IX_WiringStops_WiringReason5ID",
                table: "WiringStops",
                column: "WiringReason5ID");

            migrationBuilder.CreateIndex(
                name: "IX_WiringTriggeringFeatures_WiringStepID",
                table: "WiringTriggeringFeatures",
                column: "WiringStepID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WirersInvolveds");

            migrationBuilder.DropTable(
                name: "WiringFeatures");

            migrationBuilder.DropTable(
                name: "WiringOptions");

            migrationBuilder.DropTable(
                name: "WiringStepsForJobs");

            migrationBuilder.DropTable(
                name: "WiringStops");

            migrationBuilder.DropTable(
                name: "WiringTriggeringFeatures");

            migrationBuilder.DropTable(
                name: "Wirings");

            migrationBuilder.DropTable(
                name: "WiringReasons5");

            migrationBuilder.DropTable(
                name: "WiringSteps");

            migrationBuilder.DropTable(
                name: "WiringReasons4");

            migrationBuilder.DropTable(
                name: "WiringReasons3");

            migrationBuilder.DropTable(
                name: "WiringReasons2");

            migrationBuilder.DropTable(
                name: "WiringReasons1");
        }
    }
}
