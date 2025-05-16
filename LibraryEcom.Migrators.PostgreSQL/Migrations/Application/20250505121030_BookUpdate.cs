using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryEcom.Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class BookUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookAuthors_Books_BookId1",
                table: "BookAuthors");

            migrationBuilder.DropIndex(
                name: "IX_BookAuthors_BookId1",
                table: "BookAuthors");

            migrationBuilder.DropColumn(
                name: "BookId1",
                table: "BookAuthors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BookId1",
                table: "BookAuthors",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthors_BookId1",
                table: "BookAuthors",
                column: "BookId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BookAuthors_Books_BookId1",
                table: "BookAuthors",
                column: "BookId1",
                principalTable: "Books",
                principalColumn: "Id");
        }
    }
}
