using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoteAppMVCPattern.Migrations
{
    /// <inheritdoc />
    public partial class NoteStatus_TagUsageCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TagUsageCount",
                table: "Tag",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PlannedDate",
                table: "Notes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Notes",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagUsageCount",
                table: "Tag");

            migrationBuilder.DropColumn(
                name: "PlannedDate",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Notes");
        }
    }
}
