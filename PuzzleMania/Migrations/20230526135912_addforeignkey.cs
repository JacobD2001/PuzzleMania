using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuzzleMania.Migrations
{
    /// <inheritdoc />
    public partial class addforeignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          /*  migrationBuilder.AddForeignKey(
            name: "FK_AspNetUsers_Teams_TeamId",
            table: "AspNetUsers",
            column: "TeamId",
            principalTable: "Teams",
            principalColumn: "TeamId",
            onDelete: ReferentialAction.Restrict);*/
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
