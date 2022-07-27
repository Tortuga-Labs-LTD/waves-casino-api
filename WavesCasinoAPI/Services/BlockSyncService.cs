using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;
using WavesCasinoAPI.Areas.Roulette.Models;
using WavesCasinoAPI.Areas.State.Models;
using WavesCasinoAPI.Clients;
using WavesCasinoAPI.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using WavesCasinoAPI.Areas.CaribbeanStudPoker.Models;

namespace WavesCasinoAPI.Services
{
    public class BlockSyncService
    {
        public List<Task> SyncTasks { get; set; } = new List<Task>();
        NodeClient _node;
        GamesAPIOptions _options;
        ILogger<BlockSyncService> _logger;
        IServiceScopeFactory _scopeFactory;

        public BlockSyncService(IServiceScopeFactory scopeFactory,
            NodeClient node,
            IOptions<GamesAPIOptions> options,
            ILogger<BlockSyncService> logger)
        {
            _node = node;
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _logger = logger;
            StartSync();
        }

        public void StartSync()
        {
            foreach (var games in _options.Options.ToList())
            {
                SyncTasks.Add(Task.Run(async () =>
                {
                    while (true)
                    {
                        _logger.LogDebug("Syncing " + games.Game + " at " + games.DAppAddress);
                        try
                        {
                            using (var _context = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>())
                            {
                                if (await _context.GameStates.AsNoTracking().Where(gs => gs.Id.ToLower() == games.DAppAddress.ToLower()).FirstOrDefaultAsync() == null)
                                {
                                    _logger.LogInformation("Creating game state for " + games.DAppAddress);
                                    var state = _context.Add(new GameState()
                                    {
                                        LastSyncedHeight = 0,
                                        Id = games.DAppAddress
                                    });
                                    await _context.SaveChangesAsync();
                                    _context.DetachAllEntities();
                                }
                                else
                                {
                                    _logger.LogInformation("Found game state for " + games.DAppAddress);
                                }
                                while (true)
                                {
                                    List<JObject> totalTransactions = new List<JObject>();
                                    var gameState = await _context.GameStates.AsNoTracking().Where(gs => gs.Id.ToLower() == games.DAppAddress.ToLower()).FirstOrDefaultAsync();
                                    var lastTx = "";
                                    var continuePaging = true;
                                    while (continuePaging)
                                    {
                                        var transactions = await _node.GetTransactionsByAddress(gameState.Id, lastTx, games.TxCacheSize);
                                        foreach (var tx in transactions)
                                        {
                                            if (tx["id"].Value<string>() != lastTx && tx["id"].Value<string>() != gameState.LastSyncedTx && tx["height"].Value<long>() >= games.FromBlock)
                                            {
                                                if (totalTransactions.Count > games.TxCacheSize)
                                                {
                                                    totalTransactions.RemoveAt(0);
                                                }
                                                totalTransactions.Add(tx);
                                            }
                                            else
                                            {
                                                continuePaging = false;
                                                break;
                                            }
                                        }
                                        if (transactions.Count() == 0)
                                        {
                                            continuePaging = false;
                                        }
                                        else
                                        {
                                            lastTx = transactions.Last()["id"].Value<string>();
                                        }
                                    }

                                    totalTransactions.Reverse();

                                    foreach (var tx in totalTransactions)
                                    {
                                        _logger.LogDebug("Adding DB Transactions for tx " + tx["id"].Value<string>());
                                        var saveChanges = ProcessTransaction(games.Game, tx, _context);
                                        gameState = await _context.GameStates.AsNoTracking().Where(gs => gs.Id.ToLower() == games.DAppAddress.ToLower()).FirstOrDefaultAsync();
                                        gameState.LastSyncedTx = tx["id"].Value<string>();
                                        gameState.LastSyncedHeight = tx["height"].Value<long>();
                                        _context.Update(gameState);
                                        await _context.SaveChangesAsync();
                                        _context.DetachAllEntities();
                                        _logger.LogDebug("Successfully added DB Transactions for tx " + tx["id"].Value<string>());
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.Message);
                        }

                        await Task.Delay(games.SleepTimeMs);
                    }
                }));
            }

        }

        public bool ProcessTransaction(string game, JObject tx, ApplicationDbContext context)
        {
            switch (game)
            {
                case "roulette":
                    if (tx["type"].Value<int>() == 16)
                    {
                        switch (tx["call"]["function"].Value<string>())
                        {
                            case "placeBet":
                                var currentBet = GetStateChangeKey<long>("G_TOTALBETS", tx);
                                var betGame = GetStateChangeKey<long>("B_" + currentBet + "_GAME", tx);
                                var id = tx["dApp"].Value<string>() + "_B_" + currentBet;
                                if (context.RouletteBets.Where(b => b.Id == id).Count() == 0)
                                {
                                    context.Add(new RouletteBet()
                                    {
                                        Id = tx["dApp"].Value<string>() + "_B_" + currentBet,
                                        Amount = long.Parse(GetStateChangeKey<string>("B_" + currentBet + "_BETDETAILS", tx).Split("-")[1]),
                                        TxId = tx["id"].ToString(),
                                        GameId = tx["dApp"].Value<string>() + "_G_" + betGame,
                                        Bet = GetInvokeArgsKey<string>(0, tx),
                                        Caller = tx["sender"].Value<string>(),
                                        CreatedOnChainOn = DateTimeOffset.FromUnixTimeMilliseconds(tx["timestamp"].ToObject<long>()).UtcDateTime,
                                        LastModifiedOnChainOn = DateTimeOffset.FromUnixTimeMilliseconds(tx["timestamp"].ToObject<long>()).UtcDateTime
                                    });
                                }
                                else
                                {
                                    _logger.LogInformation("Skipping " + id + " as it already exists.");
                                }
                                break;
                            case "processNextBet":
                                var processedBets = GetStateChangeKey<long>("G_PROCESSEDBETS", tx);
                                var foundBet = context.RouletteBets.AsNoTracking().Where(rb => rb.Id == tx["dApp"].Value<string>() + "_B_" + processedBets).FirstOrDefault();
                                if (foundBet != null)
                                {
                                    foundBet.Payout = GetStateChangeKey<long>("B_" + processedBets + "_PAYOUT", tx);
                                    foundBet.LastModifiedOnChainOn = DateTimeOffset.FromUnixTimeMilliseconds(tx["timestamp"].ToObject<long>()).UtcDateTime;
                                    foundBet.PaymentId = tx["id"].Value<string>();
                                    context.Update(foundBet);
                                }
                                else
                                {
                                    _logger.LogError("BET " + processedBets + " for DAPP " + tx["dApp"].Value<string>() + " cannot be found in DB to be ended.");
                                }
                                break;
                            case "startGame":
                                var rouletteGameId = tx["dApp"].Value<string>() + "_G_" + GetStateChangeKey<long>("G_GAMESCOUNTER", tx);
                                if (context.RouletteGames.Where(b => b.Id == rouletteGameId).Count() == 0)
                                {
                                    context.Add(new RouletteGame()
                                    {
                                        Id = rouletteGameId,
                                        DAppAddress = tx["dApp"].Value<string>(),
                                        Number = GetStateChangeKey<long>("G_GAMESCOUNTER", tx),
                                        CreatedOnChainOn = DateTimeOffset.FromUnixTimeMilliseconds(tx["timestamp"].ToObject<long>()).UtcDateTime,
                                        LastModifiedOnChainOn = DateTimeOffset.FromUnixTimeMilliseconds(tx["timestamp"].ToObject<long>()).UtcDateTime
                                    });
                                }
                                else
                                {
                                    _logger.LogInformation("Skipping " + rouletteGameId + " as it already exists.");
                                }
                                break;
                            case "endGame":
                                var endedGame = GetInvokeArgsKey<long>(1, tx);
                                var foundGame = context.RouletteGames.AsNoTracking().Where(rg => rg.Id == tx["dApp"].Value<string>() + "_G_" + endedGame).FirstOrDefault();
                                if (foundGame != null)
                                {
                                    foundGame.Result = "" + GetStateChangeKey<long>("G_" + endedGame + "_RESULT", tx);
                                    foundGame.LastModifiedOnChainOn = DateTimeOffset.FromUnixTimeMilliseconds(tx["timestamp"].ToObject<long>()).UtcDateTime;
                                    context.Update(foundGame);
                                }
                                else
                                {
                                    _logger.LogError("Game " + endedGame + " for DAPP " + tx["dApp"].Value<string>() + " cannot be found in DB to be ended.");
                                }
                                break;
                            default:
                                return false;
                        }
                        return true;
                    }
                    break;
                case "studPoker":
                    if (tx["type"].Value<int>() == 16)
                    {
                        var dAppAddress = tx["dApp"].Value<string>();
                        var timestamp = DateTimeOffset.FromUnixTimeMilliseconds(tx["timestamp"].ToObject<long>()).DateTime.ToUniversalTime();
                        switch (tx["call"]["function"].Value<string>())
                        {
                            case "setupGame":
                                var gameNumber = GetStateChangeKey<long>("G_SETUPGAMESCOUNTER", tx);
                                var rouletteGameId = tx["dApp"].Value<string>() + "_G_" + gameNumber;
                                context.Add(new CaribbeanStudPokerGame()
                                {
                                    Id = rouletteGameId,
                                    CreatedOnChainOn = timestamp,
                                    LastModifiedOnChainOn = timestamp,
                                    TxId = tx["id"].Value<string>(),
                                    State = GetStateChangeKey<int>("G_" + gameNumber + "_STATE", tx),
                                    DAppAddress = dAppAddress
                                });
                                break;
                            case "startGame":
                                gameNumber = GetStateChangeKey<long>("G_USEDGAMESCOUNTER", tx);
                                rouletteGameId = tx["dApp"].Value<string>() + "_G_" + gameNumber;
                                var existingGame = context.CaribbeanStudPokerGames.Where(x => x.DAppAddress == dAppAddress
                                                     && x.Id == rouletteGameId).FirstOrDefault();
                                existingGame.LastModifiedOnChainOn = timestamp;
                                existingGame.PlayerStartGameTxId = tx["id"].Value<string>();
                                existingGame.State = GetStateChangeKey<int>("G_" + gameNumber + "_STATE", tx);
                                existingGame.Caller = GetStateChangeKey<string>("G_" + gameNumber + "_PLAYER", tx);
                                existingGame.Amount = GetStateChangeKey<long>("G_" + gameNumber + "_ANTE", tx);
                                existingGame.Number = gameNumber;
                                context.Update(existingGame);
                                break;
                            case "revealCards":
                                gameNumber = tx["call"]["args"][0]["value"].Value<int>();
                                rouletteGameId = tx["dApp"].Value<string>() + "_G_" + gameNumber;
                                existingGame = context.CaribbeanStudPokerGames.Where(x => x.DAppAddress == dAppAddress
                                                     && x.Id == rouletteGameId).FirstOrDefault();
                                existingGame.LastModifiedOnChainOn = timestamp;
                                existingGame.PlayerCardRevealTxId = tx["id"].Value<string>();
                                existingGame.PlayerSortedCards = GetStateChangeKey<string>("G_" + gameNumber + "_PLAYER_SORTEDHAND", tx);
                                existingGame.State = GetStateChangeKey<int>("G_" + gameNumber + "_STATE", tx);
                                context.Update(existingGame);
                                break;
                            case "foldOrRaise":
                                gameNumber = long.Parse(GetStateChangeKeyText("_STATE", tx).Split("_")[1]);
                                rouletteGameId = tx["dApp"].Value<string>() + "_G_" + gameNumber;
                                existingGame = context.CaribbeanStudPokerGames.Where(x => x.DAppAddress == dAppAddress
                                                     && x.Id == rouletteGameId).FirstOrDefault();
                                existingGame.State = GetStateChangeKey<int>("G_" + gameNumber + "_STATE", tx);
                                if (tx["call"]["args"][0]["value"].Value<string>() == "raise")
                                {
                                    existingGame.Amount = existingGame.Amount * 3;
                                }
                                context.Update(existingGame);
                                break;
                            case "revealResults":
                                gameNumber = tx["call"]["args"][0]["value"].Value<int>();
                                rouletteGameId = tx["dApp"].Value<string>() + "_G_" + gameNumber;
                                existingGame = context.CaribbeanStudPokerGames.Where(x => x.DAppAddress == dAppAddress
                                                     && x.Id == rouletteGameId).FirstOrDefault();
                                existingGame.State = GetStateChangeKey<int>("G_" + gameNumber + "_STATE", tx);
                                existingGame.Payout = GetStateChangeKey<long>("G_" + gameNumber + "_PAYOUT", tx);
                                existingGame.DealerCardRevealTxId = tx["id"].Value<string>();
                                existingGame.DealerSortedCards = GetStateChangeKey<string>("G_" + gameNumber + "_DEALER_SORTEDHAND", tx);
                                existingGame.PayoutTxId = tx["id"].Value<string>();
                                context.Update(existingGame);
                                break;
                            default:
                                return false;
                        }
                        return true;
                    }
                    break;
            }
            _logger.LogError("Game " + game + " not found");
            return false;
        }

        public string GetStateChangeKeyText(string contains, JObject response)
        {
            var allDataChanges = response["stateChanges"]["data"].ToObject<List<JObject>>();
            var foundData = allDataChanges.Where(adc => adc["key"].Value<string>().ToLower().Contains(contains.ToLower())).FirstOrDefault();
            if (foundData == null)
            {
                return null;
            }
            else
            {
                return foundData["key"].Value<string>();
            }
        }

        public T GetStateChangeKey<T>(string key, JObject response)
        {
            var allDataChanges = response["stateChanges"]["data"].ToObject<List<JObject>>(); 
            var foundData = allDataChanges.FirstOrDefault(adc => adc["key"].Value<string>().ToLower().StartsWith(key?.ToLower()));
            if (foundData == null)
            {
                return default(T);
            }
            else
            {
                return foundData["value"].Value<T>();
            }
        }

        public T GetInvokeArgsKey<T>(int index, JObject response)
        {
            var allDataChanges = response["call"]["args"].ToObject<List<JObject>>();
            var foundData = allDataChanges[index];
            if (foundData == null)
            {
                return default(T);
            }
            else
            {
                return foundData["value"].Value<T>();
            }
        }

    }
}
