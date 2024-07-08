using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InntalerSchachfreunde.Migrations
{
    /// <inheritdoc />
    public partial class AddedTournamentPw : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TournamentPw",
                table: "Tournaments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TournamentPw",
                table: "Tournaments");
        }
    }
}
