using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatrickBotman.Common.Migrations
{
    public partial class _9dbcfc50e20f4e408cf3671350558bdf : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Closed",
                table: "PollData",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Closed",
                table: "PollData");
        }
    }
}
