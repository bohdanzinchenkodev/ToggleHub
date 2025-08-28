using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToggleHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApiKeyStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Hash",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "Scopes",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ApiKeys");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ApiKeys",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "ApiKeys",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_Key",
                table: "ApiKeys",
                column: "Key",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiKeys_Key",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ApiKeys");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "ApiKeys");

            migrationBuilder.AddColumn<string>(
                name: "Hash",
                table: "ApiKeys",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "ApiKeys",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Scopes",
                table: "ApiKeys",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "ApiKeys",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
