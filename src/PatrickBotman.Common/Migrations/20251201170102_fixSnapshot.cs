using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PatrickBotman.Common.Migrations
{
    public partial class fixSnapshot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PollChatId",
                table: "PollData",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateTable(
                name: "PollVote",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PollDataId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Vote = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PollVote", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PollVote_PollData_PollDataId",
                        column: x => x.PollDataId,
                        principalTable: "PollData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PollVote_PollDataId",
                table: "PollVote",
                column: "PollDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PollVote");

            migrationBuilder.AlterColumn<long>(
                name: "PollChatId",
                table: "PollData",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "PollId",
                table: "PollData",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
