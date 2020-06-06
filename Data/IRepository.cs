using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using tictactoe_service.Models;

namespace tictactoe_service.Data
{
    public interface IRepository
    {
        Task<string> CreateNew(GameEntity gameEntity, CancellationToken cancellationToken);
        Task<List<GamesListItem>> GetList(CancellationToken cancellationToken);

        Task<GameEntity> GetById(string id, CancellationToken cancellationToken);

        Task<bool> UpdateById(string id, GameEntity updatedEntity, CancellationToken cancellationToken);
       
    }
}
