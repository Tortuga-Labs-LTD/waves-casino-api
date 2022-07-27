using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WavesCasinoAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameStates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LastSyncedTx = table.Column<string>(type: "text", nullable: true),
                    LastSyncedHeight = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameStates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouletteGames",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: true),
                    CreatedOnChainOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOnChainOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Number = table.Column<long>(type: "bigint", nullable: false),
                    DAppAddress = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouletteGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RouletteBets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TxId = table.Column<string>(type: "text", nullable: true),
                    GameId = table.Column<string>(type: "text", nullable: true),
                    Bet = table.Column<string>(type: "text", nullable: true),
                    Caller = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Payout = table.Column<long>(type: "bigint", nullable: false),
                    PaymentId = table.Column<string>(type: "text", nullable: true),
                    CreatedOnChainOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOnChainOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouletteBets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouletteBets_RouletteGames_GameId",
                        column: x => x.GameId,
                        principalTable: "RouletteGames",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RouletteBets_GameId",
                table: "RouletteBets",
                column: "GameId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameStates");

            migrationBuilder.DropTable(
                name: "RouletteBets");

            migrationBuilder.DropTable(
                name: "RouletteGames");
        }
    }
}
