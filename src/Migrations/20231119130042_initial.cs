using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace patrick_botman.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Gifs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GifUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gifs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Blacklists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<long>(type: "INTEGER", nullable: false),
                    GifId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blacklists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Blacklists_Gifs_GifId",
                        column: x => x.GifId,
                        principalTable: "Gifs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Blacklists_GifId",
                table: "Blacklists",
                column: "GifId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Blacklists");

            migrationBuilder.DropTable(
                name: "Gifs");
        }
    }
}
