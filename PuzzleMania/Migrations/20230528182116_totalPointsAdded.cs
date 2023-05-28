using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuzzleMania.Migrations
{
    /// <inheritdoc />
    public partial class totalPointsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalPoints",
                table: "Teams",
                type: "int",
                nullable: true,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPoints",
                table: "Teams");
        }
    }
}
