using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatrickBotman.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gifs",
                columns: table => new
                {
                    GifId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GifUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gifs", x => x.GifId);
                });

            migrationBuilder.CreateTable(
                name: "GifRatings",
                columns: table => new
                {
                    GifRatingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GifId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    Vote = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GifRatings", x => x.GifRatingId);
                    table.ForeignKey(
                        name: "FK_GifRatings_Gifs_GifId",
                        column: x => x.GifId,
                        principalTable: "Gifs",
                        principalColumn: "GifId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GifRatings_GifId",
                table: "GifRatings",
                column: "GifId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GifRatings");

            migrationBuilder.DropTable(
                name: "Gifs");
        }
    }
}
