using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class ShortLeadAndHallAllPI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HallPIAll",
                table: "Indicators",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "JobAdditionals",
                columns: table => new
                {
                    JobAdditionalID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Action = table.Column<string>(nullable: true),
                    ERDate = table.Column<DateTime>(nullable: false),
                    JobID = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobAdditionals", x => x.JobAdditionalID);
                    table.ForeignKey(
                        name: "FK_JobAdditionals_Jobs_JobID",
                        column: x => x.JobID,
                        principalTable: "Jobs",
                        principalColumn: "JobID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobAdditionals_JobID",
                table: "JobAdditionals",
                column: "JobID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobAdditionals");

            migrationBuilder.DropColumn(
                name: "HallPIAll",
                table: "Indicators");
        }
    }
}
