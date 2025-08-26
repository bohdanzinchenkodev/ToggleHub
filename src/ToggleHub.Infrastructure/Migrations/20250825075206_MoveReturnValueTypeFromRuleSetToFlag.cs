using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToggleHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveReturnValueTypeFromRuleSetToFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReturnValueType",
                table: "RuleSets");

            migrationBuilder.AddColumn<string>(
                name: "DefaultValueOffRaw",
                table: "Flags",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultValueOnRaw",
                table: "Flags",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReturnValueType",
                table: "Flags",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultValueOffRaw",
                table: "Flags");

            migrationBuilder.DropColumn(
                name: "DefaultValueOnRaw",
                table: "Flags");

            migrationBuilder.DropColumn(
                name: "ReturnValueType",
                table: "Flags");

            migrationBuilder.AddColumn<int>(
                name: "ReturnValueType",
                table: "RuleSets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
