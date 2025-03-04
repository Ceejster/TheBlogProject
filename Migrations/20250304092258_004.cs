using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TheBlogProject.Migrations
{
    /// <inheritdoc />
    public partial class _004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Alt",
                table: "Posts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Alt",
                table: "Destination",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Alt",
                table: "Blogs",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Alt",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Alt",
                table: "Destination");

            migrationBuilder.DropColumn(
                name: "Alt",
                table: "Blogs");
        }
    }
}
