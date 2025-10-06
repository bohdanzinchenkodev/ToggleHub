using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToggleHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.RenameTable(
                name: "RuleSets",
                newName: "RuleSets",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "RuleConditions",
                newName: "RuleConditions",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "RuleConditionItems",
                newName: "RuleConditionItems",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "Projects",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "OrgMembers",
                newName: "OrgMembers",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Organizations",
                newName: "Organizations",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "OrganizationInvites",
                newName: "OrganizationInvites",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Flags",
                newName: "Flags",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Environments",
                newName: "Environments",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "AuditLogs",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "ApiKeys",
                newName: "ApiKeys",
                newSchema: "app");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "RuleSets",
                schema: "app",
                newName: "RuleSets");

            migrationBuilder.RenameTable(
                name: "RuleConditions",
                schema: "app",
                newName: "RuleConditions");

            migrationBuilder.RenameTable(
                name: "RuleConditionItems",
                schema: "app",
                newName: "RuleConditionItems");

            migrationBuilder.RenameTable(
                name: "Projects",
                schema: "app",
                newName: "Projects");

            migrationBuilder.RenameTable(
                name: "OrgMembers",
                schema: "app",
                newName: "OrgMembers");

            migrationBuilder.RenameTable(
                name: "Organizations",
                schema: "app",
                newName: "Organizations");

            migrationBuilder.RenameTable(
                name: "OrganizationInvites",
                schema: "app",
                newName: "OrganizationInvites");

            migrationBuilder.RenameTable(
                name: "Flags",
                schema: "app",
                newName: "Flags");

            migrationBuilder.RenameTable(
                name: "Environments",
                schema: "app",
                newName: "Environments");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                schema: "app",
                newName: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "ApiKeys",
                schema: "app",
                newName: "ApiKeys");
        }
    }
}
