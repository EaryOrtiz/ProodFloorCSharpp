using Microsoft.EntityFrameworkCore.Migrations;

namespace ProdFloor.Migrations
{
    public partial class oneToManyWFeatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WiringFeatures_WiringID",
                table: "WiringFeatures");

            migrationBuilder.CreateIndex(
                name: "IX_WiringFeatures_WiringID",
                table: "WiringFeatures",
                column: "WiringID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WiringFeatures_WiringID",
                table: "WiringFeatures");

            migrationBuilder.CreateIndex(
                name: "IX_WiringFeatures_WiringID",
                table: "WiringFeatures",
                column: "WiringID",
                unique: true);
        }
    }
}
