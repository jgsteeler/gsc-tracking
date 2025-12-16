using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GscTracking.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "JobUpdate",
                schema: "public",
                newName: "JobUpdate");

            migrationBuilder.RenameTable(
                name: "Job",
                schema: "public",
                newName: "Job");

            migrationBuilder.RenameTable(
                name: "Customer",
                schema: "public",
                newName: "Customer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "JobUpdate",
                newName: "JobUpdate",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Job",
                newName: "Job",
                newSchema: "public");

            migrationBuilder.RenameTable(
                name: "Customer",
                newName: "Customer",
                newSchema: "public");
        }
    }
}
