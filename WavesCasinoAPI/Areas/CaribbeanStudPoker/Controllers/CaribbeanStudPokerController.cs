using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WavesCasinoAPI.Data;

namespace WavesCasinoAPI.Areas.Poker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaribbeanStudPokerController : ControllerBase
    {
        public ApplicationDbContext _context;
        public CaribbeanStudPokerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{dappAddress}/addresses/{address}/games")]
        public async Task<IActionResult> GetPlayerGames(string dappAddress, string address)
        {
            return Ok(await _context.CaribbeanStudPokerGames.Where(c => c.DAppAddress == dappAddress && c.Caller == address).OrderByDescending(b => b.CreatedOnChainOn).Take(100).ToListAsync());
        }
    }
}
