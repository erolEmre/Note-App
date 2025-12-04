using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace NoteAppMVCPattern.Migrations
{
    /// <inheritdoc />
    public partial class notebook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NotebookId",
                table: "Notes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Notebook",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notebook", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notebook_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Notebook",
                columns: new[] { "Id", "Color", "CreatedAt", "Description", "Name", "UpdatedAt", "UserId" },
                values: new object[] { 1, "bg-secondary", new DateTime(2025, 10, 23, 0, 0, 0, 0, DateTimeKind.Utc), "varsayılan notebook açıklaması", "Default", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null });
            
            migrationBuilder.Sql(
            @"UPDATE ""Notes"" 
            SET ""NotebookId"" = 1 
            WHERE ""NotebookId"" = 0;"
            );
            migrationBuilder.CreateIndex(
                name: "IX_Notes_NotebookId",
                table: "Notes",
                column: "NotebookId");

            migrationBuilder.CreateIndex(
                name: "IX_Notebook_UserId",
                table: "Notebook",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Notebook_NotebookId",
                table: "Notes",
                column: "NotebookId",
                principalTable: "Notebook",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Notebook_NotebookId",
                table: "Notes");

            migrationBuilder.DropTable(
                name: "Notebook");

            migrationBuilder.DropIndex(
                name: "IX_Notes_NotebookId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "NotebookId",
                table: "Notes");
        }
    }
}
