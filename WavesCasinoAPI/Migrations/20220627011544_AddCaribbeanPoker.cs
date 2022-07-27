using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WavesCasinoAPI.Migrations
{
    public partial class AddCaribbeanPoker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaribbeanStudPokerGames",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TxId = table.Column<string>(type: "text", nullable: false),
                    PayoutTxId = table.Column<string>(type: "text", nullable: true),
                    Caller = table.Column<string>(type: "text", nullable: true),
                    PlayerStartGameTxId = table.Column<string>(type: "text", nullable: true),
                    PlayerCardRevealTxId = table.Column<string>(type: "text", nullable: true),
                    DealerCardRevealTxId = table.Column<string>(type: "text", nullable: true),
                    PlayerSortedCards = table.Column<string>(type: "text", nullable: true),
                    DealerSortedCards = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Payout = table.Column<long>(type: "bigint", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    CreatedOnChainOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOnChainOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Number = table.Column<long>(type: "bigint", nullable: false),
                    DAppAddress = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaribbeanStudPokerGames", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaribbeanStudPokerGames");
        }
    }
}
