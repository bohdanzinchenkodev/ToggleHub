using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToggleHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReturnValuesToRuleSets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BucketingSeed",
                table: "Flags");

            migrationBuilder.DropColumn(
                name: "DefaultValueRaw",
                table: "Flags");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Flags");

            migrationBuilder.AlterColumn<int>(
                name: "Percentage",
                table: "RuleSets",
                type: "int",
                nullable: false,
                defaultValue: 100,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<Guid>(
                name: "BucketingSeed",
                table: "RuleSets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OffReturnValueRaw",
                table: "RuleSets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReturnValueRaw",
                table: "RuleSets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReturnValueType",
                table: "RuleSets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BucketingSeed",
                table: "RuleSets");

            migrationBuilder.DropColumn(
                name: "OffReturnValueRaw",
                table: "RuleSets");

            migrationBuilder.DropColumn(
                name: "ReturnValueRaw",
                table: "RuleSets");

            migrationBuilder.DropColumn(
                name: "ReturnValueType",
                table: "RuleSets");

            migrationBuilder.AlterColumn<int>(
                name: "Percentage",
                table: "RuleSets",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 100);

            migrationBuilder.AddColumn<Guid>(
                name: "BucketingSeed",
                table: "Flags",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "DefaultValueRaw",
                table: "Flags",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Flags",
                type: "int",
                maxLength: 100,
                nullable: false,
                defaultValue: 0);
        }
    }
}
