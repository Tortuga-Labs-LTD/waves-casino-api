
using WavesCasinoAPI.Models;

namespace WavesCasinoAPI.Areas.Roulette.Models
{
    public class RouletteGame : Game
    {
        public string? Result { get; set; }
        public IEnumerable<RouletteBet> Bets { get; set; }
        public DateTime CreatedOnChainOn { get; set; }
        public DateTime LastModifiedOnChainOn { get; set; }
    }
}
