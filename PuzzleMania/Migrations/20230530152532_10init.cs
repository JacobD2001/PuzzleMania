using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuzzleMania.Migrations
{
    /// <inheritdoc />
    public partial class _10init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {/*
            migrationBuilder.DropForeignKey(
                name: "FK_Riddles_Games_GameId",
                table: "Riddles");

            migrationBuilder.DropIndex(
                name: "IX_Riddles_GameId",
                table: "Riddles");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Riddles");*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Riddles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Riddles_GameId",
                table: "Riddles",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_Riddles_Games_GameId",
                table: "Riddles",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
