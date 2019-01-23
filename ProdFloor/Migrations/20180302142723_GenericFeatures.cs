using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace ProdFloor.Migrations
{
    public partial class GenericFeatures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GenericFeaturesList",
                columns: table => new
                {
                    GenericFeaturesID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Attendant = table.Column<bool>(nullable: false),
                    CallEnable = table.Column<bool>(nullable: false),
                    CarCallCodeSecurity = table.Column<string>(nullable: true),
                    CarToLobby = table.Column<bool>(nullable: false),
                    EMT = table.Column<bool>(nullable: false),
                    EP = table.Column<bool>(nullable: false),
                    EQ = table.Column<bool>(nullable: false),
                    FLO = table.Column<bool>(nullable: false),
                    FRON2 = table.Column<bool>(nullable: false),
                    Hosp = table.Column<bool>(nullable: false),
                    IDS = table.Column<bool>(nullable: false),
                    IMon = table.Column<bool>(nullable: false),
                    INA = table.Column<bool>(nullable: false),
                    INCP = table.Column<bool>(nullable: false),
                    Ind = table.Column<bool>(nullable: false),
                    JobID = table.Column<int>(nullable: false),
                    LoadWeigher = table.Column<bool>(nullable: false),
                    MView = table.Column<bool>(nullable: false),
                    SpecialInstructions = table.Column<string>(nullable: true),
                    SwitchStyle = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericFeaturesList", x => x.GenericFeaturesID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GenericFeaturesList");
        }
    }
}
