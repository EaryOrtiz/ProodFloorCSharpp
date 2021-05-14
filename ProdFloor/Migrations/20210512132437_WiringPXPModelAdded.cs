using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProdFloor.Migrations
{
    public partial class WiringPXPModelAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PXPReasons",
                columns: table => new
                {
                    PXPReasonID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PXPReasons", x => x.PXPReasonID);
                });

            migrationBuilder.CreateTable(
                name: "WirersPXPInvolveds",
                columns: table => new
                {
                    WirersPXPInvolvedID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WiringPXPID = table.Column<int>(nullable: false),
                    WirerPXPID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WirersPXPInvolveds", x => x.WirersPXPInvolvedID);
                });

            migrationBuilder.CreateTable(
                name: "WiringPXPs",
                columns: table => new
                {
                    WiringPXPID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobID = table.Column<int>(nullable: false),
                    StationID = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    SinglePO = table.Column<int>(nullable: false),
                    WirerPXPID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WiringPXPs", x => x.WiringPXPID);
                    table.ForeignKey(
                        name: "FK_WiringPXPs_Jobs_JobID",
                        column: x => x.JobID,
                        principalTable: "Jobs",
                        principalColumn: "JobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PXPErrors",
                columns: table => new
                {
                    PXPErrorID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WiringPXPID = table.Column<int>(nullable: false),
                    PXPReasonID = table.Column<int>(nullable: false),
                    GuiltyWirerID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PXPErrors", x => x.PXPErrorID);
                    table.ForeignKey(
                        name: "FK_PXPErrors_WiringPXPs_WiringPXPID",
                        column: x => x.WiringPXPID,
                        principalTable: "WiringPXPs",
                        principalColumn: "WiringPXPID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PXPErrors_WiringPXPID",
                table: "PXPErrors",
                column: "WiringPXPID");

            migrationBuilder.CreateIndex(
                name: "IX_WiringPXPs_JobID",
                table: "WiringPXPs",
                column: "JobID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PXPErrors");

            migrationBuilder.DropTable(
                name: "PXPReasons");

            migrationBuilder.DropTable(
                name: "WirersPXPInvolveds");

            migrationBuilder.DropTable(
                name: "WiringPXPs");
        }
    }
}
