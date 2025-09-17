using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToggleHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Environments_Projects_ProjectId",
                table: "Environments");

            migrationBuilder.DropIndex(
                name: "IX_Projects_OrganizationId",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OrganizationId_Slug",
                table: "Projects",
                columns: new[] { "OrganizationId", "Slug" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Environments_Projects_ProjectId",
                table: "Environments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Environments_Projects_ProjectId",
                table: "Environments");

            migrationBuilder.DropIndex(
                name: "IX_Projects_OrganizationId_Slug",
                table: "Projects");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OrganizationId",
                table: "Projects",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Environments_Projects_ProjectId",
                table: "Environments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
