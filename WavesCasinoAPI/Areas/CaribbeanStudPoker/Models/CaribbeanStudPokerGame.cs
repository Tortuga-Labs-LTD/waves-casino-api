using WavesCasinoAPI.Models;

namespace WavesCasinoAPI.Areas.CaribbeanStudPoker.Models
{
    public class CaribbeanStudPokerGame : Game
    {
        public string TxId { get; set; }
        
        public string? PayoutTxId { get; set; }
        public string? Caller { get; set; }
        public string? PlayerStartGameTxId { get; set; }
        public string? PlayerCardRevealTxId { get; set; }
        public string? DealerCardRevealTxId { get; set; }
        public string? PlayerSortedCards { get; set; }
        public string? DealerSortedCards { get; set; }
        public Int64 Amount { get; set; }
        public Int64 Payout { get; set; }
        public int State { get; set; }
        public DateTime CreatedOnChainOn { get; set; }
        public DateTime LastModifiedOnChainOn { get; set; }
    }
}
