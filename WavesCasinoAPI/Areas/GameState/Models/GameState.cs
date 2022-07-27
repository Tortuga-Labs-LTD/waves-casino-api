namespace WavesCasinoAPI.Areas.State.Models
{
    public class GameState
    {
        /// <summary>
        /// This is the DApp Address
        /// </summary>
        public string Id { get; set; }
        public string? LastSyncedTx { get; set; }
        public long LastSyncedHeight { get; set; }
        public DateTime LastLoaded { get; set; } = DateTime.MinValue.ToUniversalTime();
    }
}
