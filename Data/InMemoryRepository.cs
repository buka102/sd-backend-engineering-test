using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using tictactoe_service.Models;

namespace tictactoe_service.Data
{
    public class InMemoryRepository : IRepository
    {
        public static List<GameEntity> AllGames = new List<GameEntity>();
        static SemaphoreSlim _sem = new SemaphoreSlim(1); //ensure locking for update
        public InMemoryRepository()
        {

        }

        public Task<string> CreateNew(GameEntity gameEntity, CancellationToken cancellationToken)
        {
            AllGames.Add(gameEntity);
            return Task.FromResult(gameEntity.Id);
        }

        public Task<GameEntity> GetById(string id, CancellationToken cancellationToken)
        {
            var gameById = AllGames.Where(g => g.Id == id).FirstOrDefault();
            return Task.FromResult(gameById);
        }

        public Task<List<GamesListItem>> GetList(CancellationToken cancellationToken)
        {
            var gameList = AllGames.Select(g => new GamesListItem { Id = g.Id, CreatedUtc = g.CreatedUtc, LastUpdatedUtc = g.LastUpdatedUtc, Result = g.Result, Status = g.Status, WaitingForPlayer = g.WaitingForPlayer });
            return Task.FromResult(gameList.ToList());
        }

        public async Task<bool> UpdateById(string id, GameEntity updatedEntity, CancellationToken cancellationToken)
        {
            var result = false;
            try
            {
                await _sem.WaitAsync();
                var findGameIndex = AllGames.FindIndex(g => g.Id == id);
                if (findGameIndex != -1)
                {
                    AllGames[findGameIndex] = updatedEntity;
                    result = true;
                }
            }
            finally
            {
                _sem.Release();
            }
            return result;
        }
    }
}
