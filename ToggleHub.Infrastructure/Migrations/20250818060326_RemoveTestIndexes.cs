using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToggleHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTestIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_OrgId_Slug",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_OrgMembers_OrgId_UserId",
                table: "OrgMembers");

            migrationBuilder.DropIndex(
                name: "IX_Flags_EnvironmentId_Key",
                table: "Flags");

            migrationBuilder.DropIndex(
                name: "IX_Environments_ProjectId_Name",
                table: "Environments");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_OrgId_CreatedAt",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_ApiKeys_Prefix",
                table: "ApiKeys");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Environments",
                newName: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OrgId",
                table: "Projects",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgMembers_OrgId",
                table: "OrgMembers",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Flags_EnvironmentId",
                table: "Flags",
                column: "EnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Environments_ProjectId",
                table: "Environments",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_OrgId",
                table: "AuditLogs",
                column: "OrgId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Projects_OrgId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_OrgMembers_OrgId",
                table: "OrgMembers");

            migrationBuilder.DropIndex(
                name: "IX_Flags_EnvironmentId",
                table: "Flags");

            migrationBuilder.DropIndex(
                name: "IX_Environments_ProjectId",
                table: "Environments");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_OrgId",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Environments",
                newName: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OrgId_Slug",
                table: "Projects",
                columns: new[] { "OrgId", "Slug" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrgMembers_OrgId_UserId",
                table: "OrgMembers",
                columns: new[] { "OrgId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Flags_EnvironmentId_Key",
                table: "Flags",
                columns: new[] { "EnvironmentId", "Key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Environments_ProjectId_Name",
                table: "Environments",
                columns: new[] { "ProjectId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_OrgId_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "OrgId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Prefix",
                table: "ApiKeys",
                column: "Prefix",
                unique: true);
        }
    }
}
