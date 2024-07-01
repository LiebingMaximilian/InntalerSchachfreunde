using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InntalerSchachfreunde.Migrations
{
    /// <inheritdoc />
    public partial class AddedNameToImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Images",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Images");
        }
    }
}
