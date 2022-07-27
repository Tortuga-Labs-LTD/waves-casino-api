using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WavesCasinoAPI.Areas.State.Models;
using WavesCasinoAPI.Data;

namespace WavesCasinoAPI.Areas.Activity
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActivityController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{gameId}/lastloaded")]
        public async Task<ActionResult<Activity>> GetLastLoaded(string gameId)
        {
            if (_context.GameStates == null)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(gameId))
            {
                return NotFound();
            }
            var gameState = await _context.GameStates.FindAsync(gameId);

            if (gameState == null)
            {
                return NotFound();
            }

            return new Activity() { GameId = gameState.Id, LastLoaded = gameState.LastLoaded };
        }

        [HttpPut("{gameId}/lastloaded")]
        public async Task<IActionResult> PutLastLoaded(string gameId)
        {
            if (_context.GameStates == null)
            {
                return NotFound();
            }
            if (string.IsNullOrEmpty(gameId))
            {
                return NotFound();
            }
            var gameState = await _context.GameStates.FindAsync(gameId);
            if (gameState == null)
            {
                return NotFound();
            }
            gameState.LastLoaded = DateTime.UtcNow;

            _context.Entry(gameState).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool GameStateExists(string id)
        {
            return (_context.GameStates?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}