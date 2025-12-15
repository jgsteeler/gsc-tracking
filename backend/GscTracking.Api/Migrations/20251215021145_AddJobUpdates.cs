using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GscTracking.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddJobUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "Job",
                newName: "Job",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Customer",
                newName: "Customer",
                newSchema: "public");

            migrationBuilder.CreateTable(
                name: "JobUpdate",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    JobId = table.Column<int>(type: "INTEGER", nullable: false),
                    UpdateText = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobUpdate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobUpdate_Job_JobId",
                        column: x => x.JobId,
                        principalSchema: "public",
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobUpdate_JobId",
                schema: "public",
                table: "JobUpdate",
                column: "JobId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobUpdate",
                schema: "public");

            migrationBuilder.RenameTable(
                name: "Job",
                schema: "public",
                newName: "Job");

            migrationBuilder.RenameTable(
                name: "Customer",
                schema: "public",
                newName: "Customer");
        }
    }
}
