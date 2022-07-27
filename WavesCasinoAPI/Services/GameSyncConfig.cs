using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WavesCasinoAPI.Services
{
    public class GameSyncConfig
    {
        public string Game { get; set; }
        public string DAppAddress { get; set; }
        public int SleepTimeMs { get; set; }
        public long FromBlock { get; set; }
        /// <summary>
        /// How many tx to store in memory
        /// </summary>
        public int TxCacheSize { get; set; }
    }
}
