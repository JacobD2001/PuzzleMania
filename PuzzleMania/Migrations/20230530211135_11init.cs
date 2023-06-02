using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuzzleMania.Migrations
{
    /// <inheritdoc />
    public partial class _11init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //fail bc there is already a gameid in riddles to fix later TODO
    /*        migrationBuilder.AddColumn<int>(
                name: "GameId",
                table: "Riddles",
                type: "int",
                nullable: true,
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
                onDelete: ReferentialAction.Cascade);*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Riddles_Games_GameId",
                table: "Riddles");

            migrationBuilder.DropIndex(
                name: "IX_Riddles_GameId",
                table: "Riddles");

            migrationBuilder.DropColumn(
                name: "GameId",
                table: "Riddles");
        }
    }
}
