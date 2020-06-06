using MediatR;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using tictactoe_service.Data;
using tictactoe_service.Models;

namespace tictactoe_service.CQRS.Commands.StartGameCommand
{
    public class StartGameRequestHandler : IRequestHandler<StartGameRequest, StartGameResponse>
    {
        private readonly ILogger<StartGameRequestHandler> _logger;
        private readonly IRepository _repo;

        public StartGameRequestHandler(IRepository repo, ILogger<StartGameRequestHandler> logger)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<StartGameResponse> Handle(StartGameRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var utc = DateTimeOffset.UtcNow;
                var newGame = new GameEntity();
                newGame.Board = new string[3, 3];
                for (var i = 0; i <= 2; i++)
                    for (var j = 0; j <= 2; j++)
                        newGame.Board[i, j] = string.Empty;
                newGame.Id = ToShortGuid(Guid.NewGuid());
                newGame.CreatedUtc = utc;
                newGame.LastUpdatedUtc = utc;
                newGame.Status = GameKnownStatuses.InProgress;
                newGame.WaitingForPlayer = GameKnownWaitingForPlayer.WaitingForPlayer1;
                newGame.Result = GameKnownResults.InProgress;

                await _repo.CreateNew(newGame, cancellationToken);

                return new StartGameResponse { Created = newGame };

            } catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception");
                throw;
            }
        }

        private string ToShortGuid(Guid newGuid)
        {
            string modifiedBase64 = Convert.ToBase64String(newGuid.ToByteArray())
                .Replace('+', '-').Replace('/', '_') // avoid invalid URL characters
                .Substring(0, 22);
            return modifiedBase64;
        }
    }
}
