using WavesCasinoAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace WavesCasinoAPI.Areas.Roulette.Models
{
    public class RouletteBet: BaseEntity
    {
        /// <summary>
        /// DAPP_B_NUMBER
        /// </summary>
        public string Id { get; set; }
        public string? TxId { get; set; }
        [ForeignKey("Game")]
        public string? GameId { get; set; }
        public RouletteGame? Game { get; set; }
        public string? Bet { get; set; }
        public string? Caller { get; set; }
        public Int64 Amount { get; set; }
        public Int64 Payout { get; set; }
        public string? PaymentId { get; set; }
        public DateTime CreatedOnChainOn { get; set; }
        public DateTime LastModifiedOnChainOn { get; set; }
    }
}
