using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPSSLGame.Api.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "game_rounds",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_choice = table.Column<int>(type: "integer", nullable: false),
                    computer_choice = table.Column<int>(type: "integer", nullable: false),
                    result = table.Column<int>(type: "integer", nullable: false),
                    played_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_rounds", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "game_rounds");
        }
    }
}
