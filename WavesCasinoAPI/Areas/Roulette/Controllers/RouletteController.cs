using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WavesCasinoAPI.Data;

namespace WavesCasinoAPI.Areas.Roulette.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouletteController : ControllerBase
    {
        private ApplicationDbContext _context;
        public RouletteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{dappAddress}/games/{gameNumber}/bets")]
        public async Task<IActionResult> GetGameBets(string dappAddress, long gameNumber)
        {
            return Ok(await _context.RouletteBets.Where(rb => rb.GameId == dappAddress + "_G_" + gameNumber).ToListAsync());
        }

        [HttpGet("{dappAddress}/addresses/{address}/bets")]
        public async Task<IActionResult> GetGameBets(string dappAddress,string address)
        {
            return Ok(await _context.RouletteBets.Where(rb => rb.GameId.Contains(dappAddress) && rb.Caller == address)
                .Include(rb => rb.Game).OrderByDescending(b => b.CreatedOnChainOn).Take(1000).ToListAsync());
        }
    }
}
