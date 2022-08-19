using Microsoft.EntityFrameworkCore.Migrations;

namespace ProdFloor.Migrations
{
    public partial class OneToManyPXP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WiringPXPs_JobID",
                table: "WiringPXPs");

            migrationBuilder.CreateIndex(
                name: "IX_WiringPXPs_JobID",
                table: "WiringPXPs",
                column: "JobID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WiringPXPs_JobID",
                table: "WiringPXPs");

            migrationBuilder.CreateIndex(
                name: "IX_WiringPXPs_JobID",
                table: "WiringPXPs",
                column: "JobID",
                unique: true);
        }
    }
}
