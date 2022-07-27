using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WavesCasinoAPI.Models
{
    public class BaseEntity
    {
        public DateTime CreatedOn { get; set; }
        public DateTime LastModifiedOn { get; set; }
    }
}
