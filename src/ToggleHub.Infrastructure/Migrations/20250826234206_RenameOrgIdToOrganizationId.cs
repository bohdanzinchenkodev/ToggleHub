using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToggleHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrgIdToOrganizationId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiKeys_Organizations_OrgId",
                table: "ApiKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Organizations_OrgId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Environments_Projects_ProjectId",
                table: "Environments");

            migrationBuilder.DropForeignKey(
                name: "FK_OrgMembers_Organizations_OrgId",
                table: "OrgMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Organizations_OrgId",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "OrgId",
                table: "Projects",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_OrgId",
                table: "Projects",
                newName: "IX_Projects_OrganizationId");

            migrationBuilder.RenameColumn(
                name: "OrgId",
                table: "OrgMembers",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_OrgMembers_OrgId",
                table: "OrgMembers",
                newName: "IX_OrgMembers_OrganizationId");

            migrationBuilder.RenameColumn(
                name: "OrgId",
                table: "AuditLogs",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_OrgId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_OrganizationId");

            migrationBuilder.RenameColumn(
                name: "OrgId",
                table: "ApiKeys",
                newName: "OrganizationId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiKeys_OrgId",
                table: "ApiKeys",
                newName: "IX_ApiKeys_OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKeys_Organizations_OrganizationId",
                table: "ApiKeys",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Organizations_OrganizationId",
                table: "AuditLogs",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Environments_Projects_ProjectId",
                table: "Environments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrgMembers_Organizations_OrganizationId",
                table: "OrgMembers",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Organizations_OrganizationId",
                table: "Projects",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiKeys_Organizations_OrganizationId",
                table: "ApiKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Organizations_OrganizationId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Environments_Projects_ProjectId",
                table: "Environments");

            migrationBuilder.DropForeignKey(
                name: "FK_OrgMembers_Organizations_OrganizationId",
                table: "OrgMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Organizations_OrganizationId",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "Projects",
                newName: "OrgId");

            migrationBuilder.RenameIndex(
                name: "IX_Projects_OrganizationId",
                table: "Projects",
                newName: "IX_Projects_OrgId");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "OrgMembers",
                newName: "OrgId");

            migrationBuilder.RenameIndex(
                name: "IX_OrgMembers_OrganizationId",
                table: "OrgMembers",
                newName: "IX_OrgMembers_OrgId");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "AuditLogs",
                newName: "OrgId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_OrganizationId",
                table: "AuditLogs",
                newName: "IX_AuditLogs_OrgId");

            migrationBuilder.RenameColumn(
                name: "OrganizationId",
                table: "ApiKeys",
                newName: "OrgId");

            migrationBuilder.RenameIndex(
                name: "IX_ApiKeys_OrganizationId",
                table: "ApiKeys",
                newName: "IX_ApiKeys_OrgId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiKeys_Organizations_OrgId",
                table: "ApiKeys",
                column: "OrgId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Organizations_OrgId",
                table: "AuditLogs",
                column: "OrgId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Environments_Projects_ProjectId",
                table: "Environments",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrgMembers_Organizations_OrgId",
                table: "OrgMembers",
                column: "OrgId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Organizations_OrgId",
                table: "Projects",
                column: "OrgId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
