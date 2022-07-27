using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WavesCasinoAPI.Models
{
    public class Game : BaseEntity
    {
        /// <summary>
        /// DAPP_G_NUMBER
        /// </summary>
        public string Id { get; set; }
        public long Number { get; set; }
        public string? DAppAddress { get; set; }
    }
}
