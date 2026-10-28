using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalAccessibilityResearch1.Migrations
{
    /// <inheritdoc />
    public partial class AddSurveyResponses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SurveyResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    EaseOfUse = table.Column<int>(type: "INTEGER", nullable: false),
                    Readability = table.Column<int>(type: "INTEGER", nullable: false),
                    Navigation = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskCompletion = table.Column<int>(type: "INTEGER", nullable: false),
                    Difficulties = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    SubmittedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyResponses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyResponses");
        }
    }
}
