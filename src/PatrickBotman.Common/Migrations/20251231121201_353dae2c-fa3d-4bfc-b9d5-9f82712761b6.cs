using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatrickBotman.Common.Migrations
{
    public partial class _353dae2cfa3d4bfcb9d59f82712761b6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "PollData",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                table: "PollData");
        }
    }
}
