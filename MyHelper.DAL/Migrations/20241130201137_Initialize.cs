using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyHelper.DAL.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Challenges",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "varchar(300)", nullable: false),
                    Note = table.Column<string>(nullable: true),
                    InProgress = table.Column<bool>(nullable: false),
                    Start = table.Column<DateTime>(type: "date", nullable: true),
                    End = table.Column<DateTime>(type: "date", nullable: true),
                    Duration = table.Column<string>(type: "varchar(20)", nullable: true),
                    Regularity = table.Column<string>(type: "varchar(20)", nullable: true),
                    AdditionalRegularity = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Challenges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TargetsGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "varchar(300)", nullable: false),
                    Year = table.Column<uint>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetsGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Check",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    NumberDay = table.Column<int>(nullable: false),
                    Checked = table.Column<bool>(nullable: false),
                    ChallengeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Check", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Check_Challenges_ChallengeId",
                        column: x => x.ChallengeId,
                        principalTable: "Challenges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Target",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "varchar(300)", nullable: false),
                    Note = table.Column<string>(nullable: true),
                    TargetsGroupId = table.Column<int>(nullable: false),
                    IsComplete = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Target", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Target_TargetsGroups_TargetsGroupId",
                        column: x => x.TargetsGroupId,
                        principalTable: "TargetsGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Check_ChallengeId",
                table: "Check",
                column: "ChallengeId");

            migrationBuilder.CreateIndex(
                name: "IX_Target_TargetsGroupId",
                table: "Target",
                column: "TargetsGroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Check");

            migrationBuilder.DropTable(
                name: "Target");

            migrationBuilder.DropTable(
                name: "Challenges");

            migrationBuilder.DropTable(
                name: "TargetsGroups");
        }
    }
}
