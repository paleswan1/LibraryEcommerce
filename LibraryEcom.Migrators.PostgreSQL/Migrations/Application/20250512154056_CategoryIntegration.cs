using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryEcom.Migrators.PostgreSQL.Migrations.Application
{
    /// <inheritdoc />
    public partial class CategoryIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAwarded",
                table: "Books",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBestSeller",
                table: "Books",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isFeatured",
                table: "Books",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAwarded",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsBestSeller",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "isFeatured",
                table: "Books");
        }
    }
}
