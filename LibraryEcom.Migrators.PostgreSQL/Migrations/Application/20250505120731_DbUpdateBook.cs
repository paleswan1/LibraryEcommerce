using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryEcom.Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class DbUpdateBook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Discounts_BookId",
                table: "Discounts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Discounts",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Discounts",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<string>(
                name: "CoverImage",
                table: "Books",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BookId1",
                table: "BookAuthors",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_BookId",
                table: "Discounts",
                column: "BookId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookAuthors_Books_BookId1",
                table: "BookAuthors");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_BookId",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_BookAuthors_BookId1",
                table: "BookAuthors");

            migrationBuilder.DropColumn(
                name: "CoverImage",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BookId1",
                table: "BookAuthors");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Discounts",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_BookId",
                table: "Discounts",
                column: "BookId",
                unique: true);
        }
    }
}
