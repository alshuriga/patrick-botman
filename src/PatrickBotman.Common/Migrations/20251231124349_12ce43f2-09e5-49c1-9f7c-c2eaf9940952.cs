using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatrickBotman.Common.Migrations
{
    public partial class _12ce43f209e549c19f7cc2eaf9940952 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MessageId",
                table: "PollData",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "PollData");
        }
    }
}
