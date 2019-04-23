using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class CustomModelAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomSoftwares",
                columns: table => new
                {
                    CustomSoftwareID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomSoftwares", x => x.CustomSoftwareID);
                });

            migrationBuilder.CreateTable(
                name: "CustomFeatures",
                columns: table => new
                {
                    CustomFeatureID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomSoftwareID = table.Column<int>(nullable: false),
                    JobID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFeatures", x => x.CustomFeatureID);
                    table.ForeignKey(
                        name: "FK_CustomFeatures_CustomSoftwares_CustomSoftwareID",
                        column: x => x.CustomSoftwareID,
                        principalTable: "CustomSoftwares",
                        principalColumn: "CustomSoftwareID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomFeatures_Jobs_JobID",
                        column: x => x.JobID,
                        principalTable: "Jobs",
                        principalColumn: "JobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TriggeringCustSofts",
                columns: table => new
                {
                    TriggeringCustSoftID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CustomSoftwareID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    isSelected = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggeringCustSofts", x => x.TriggeringCustSoftID);
                    table.ForeignKey(
                        name: "FK_TriggeringCustSofts_CustomSoftwares_CustomSoftwareID",
                        column: x => x.CustomSoftwareID,
                        principalTable: "CustomSoftwares",
                        principalColumn: "CustomSoftwareID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomFeatures_CustomSoftwareID",
                table: "CustomFeatures",
                column: "CustomSoftwareID");

            migrationBuilder.CreateIndex(
                name: "IX_CustomFeatures_JobID",
                table: "CustomFeatures",
                column: "JobID");

            migrationBuilder.CreateIndex(
                name: "IX_TriggeringCustSofts_CustomSoftwareID",
                table: "TriggeringCustSofts",
                column: "CustomSoftwareID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomFeatures");

            migrationBuilder.DropTable(
                name: "TriggeringCustSofts");

            migrationBuilder.DropTable(
                name: "CustomSoftwares");
        }
    }
}
