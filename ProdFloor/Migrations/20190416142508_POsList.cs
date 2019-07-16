using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class POsList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropIndex(
                name: "IX_Jobs_PO",
                table: "Jobs");
                */
            migrationBuilder.DropColumn(
                name: "PO",
                table: "Jobs");

            migrationBuilder.CreateTable(
                name: "POs",
                columns: table => new
                {
                    POID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    JobID = table.Column<int>(nullable: false),
                    PONumb = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_POs", x => x.POID);
                    table.ForeignKey(
                        name: "FK_POs_Jobs_JobID",
                        column: x => x.JobID,
                        principalTable: "Jobs",
                        principalColumn: "JobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_POs_JobID",
                table: "POs",
                column: "JobID");

            migrationBuilder.CreateIndex(
                name: "IX_POs_PONumb",
                table: "POs",
                column: "PONumb",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "POs");

            migrationBuilder.AddColumn<int>(
                name: "PO",
                table: "Jobs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_PO",
                table: "Jobs",
                column: "PO",
                unique: true);
        }
    }
}
