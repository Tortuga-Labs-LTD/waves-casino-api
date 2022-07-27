using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WavesCasinoAPI.Migrations
{
    public partial class AddActivityLastLoaded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoaded",
                table: "GameStates",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLoaded",
                table: "GameStates");
        }
    }
}
