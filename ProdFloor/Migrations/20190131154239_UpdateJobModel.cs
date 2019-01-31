using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class UpdateJobModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "JobStateID",
                table: "Jobs",
                newName: "StateID");

            migrationBuilder.RenameColumn(
                name: "JobCountryID",
                table: "Jobs",
                newName: "CountryID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StateID",
                table: "Jobs",
                newName: "JobStateID");

            migrationBuilder.RenameColumn(
                name: "CountryID",
                table: "Jobs",
                newName: "JobCountryID");
        }
    }
}
