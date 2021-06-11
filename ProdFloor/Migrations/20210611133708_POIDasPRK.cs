using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProdFloor.Migrations
{
    public partial class POIDasPRK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WiringPXPs_Jobs_JobID",
                table: "WiringPXPs");

            migrationBuilder.DropIndex(
                name: "IX_WiringPXPs_JobID",
                table: "WiringPXPs");

            migrationBuilder.RenameColumn(
                name: "JobID",
                table: "WiringPXPs",
                newName: "POID");

            migrationBuilder.CreateTable(
                name: "StatusPOs",
                columns: table => new
                {
                    POID = table.Column<int>(nullable: false),
                    StatusPOID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusPOs", x => x.StatusPOID);
                    table.ForeignKey(
                        name: "FK_StatusPOs_POs_POID",
                        column: x => x.POID,
                        principalTable: "POs",
                        principalColumn: "POID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WiringPXPs_POID",
                table: "WiringPXPs",
                column: "POID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatusPOs_POID",
                table: "StatusPOs",
                column: "POID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WiringPXPs_POs_POID",
                table: "WiringPXPs",
                column: "POID",
                principalTable: "POs",
                principalColumn: "POID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WiringPXPs_POs_POID",
                table: "WiringPXPs");

            migrationBuilder.DropTable(
                name: "StatusPOs");

            migrationBuilder.DropIndex(
                name: "IX_WiringPXPs_POID",
                table: "WiringPXPs");

            migrationBuilder.RenameColumn(
                name: "POID",
                table: "WiringPXPs",
                newName: "JobID");

            migrationBuilder.CreateIndex(
                name: "IX_WiringPXPs_JobID",
                table: "WiringPXPs",
                column: "JobID");

            migrationBuilder.AddForeignKey(
                name: "FK_WiringPXPs_Jobs_JobID",
                table: "WiringPXPs",
                column: "JobID",
                principalTable: "Jobs",
                principalColumn: "JobID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
