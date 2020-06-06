using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using tictactoe_service.Data;
using tictactoe_service.Logic;
using tictactoe_service.Models;

namespace tictactoe_service.CQRS.Commands.UpdateGameCommand
{
    public class UpdateGameRequestHandler : IRequestHandler<UpdateGameRequest, UpdateGameResponse>
    {
        private readonly ILogger<UpdateGameRequestHandler> _logger;
        private readonly IGameEvaluator _gameEvaluator;
        private readonly IRepository _repo;

        public UpdateGameRequestHandler(IRepository repo, IGameEvaluator gameEvaluator, ILogger<UpdateGameRequestHandler> logger)
        {
            _logger = logger;
            _gameEvaluator = gameEvaluator;
            _repo = repo;
        }

        public async Task<UpdateGameResponse> Handle(UpdateGameRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var game = await _repo.GetById(request.Id, cancellationToken);
                if (game == null)
                {
                    return new UpdateGameResponse { IsNotFound = true, IsValid = false, Errors = new List<string> {"game is not found" } };
                }

                if (game.Status != GameKnownStatuses.InProgress)
                {
                    return new UpdateGameResponse { IsValid = false, Errors = new List<string> { "current game is already completed" }, Game = game };
                }

                if (request.Player==1 && game.WaitingForPlayer!= GameKnownWaitingForPlayer.WaitingForPlayer1)
                {
                    return new UpdateGameResponse { IsValid = false, Errors = new List<string> { $"Player {request.Player} is allowed to perform action." }, Game = game };
                }

                if (request.Player == 2 && game.WaitingForPlayer != GameKnownWaitingForPlayer.WaitingForPlayer2)
                {
                    return new UpdateGameResponse { IsValid = false, Errors = new List<string> { $"Player {request.Player} is allowed to perform action." }, Game = game };
                }

                int x, y;
                if (!ParseCoordinate(request.MoveCoordinate, out x, out y))
                {
                    return new UpdateGameResponse { IsValid = false, Errors = new List<string> { "unable to determine coordinates" }, Game = game };
                }

                if (!string.IsNullOrEmpty(game.Board[x, y]))
                {
                    return new UpdateGameResponse { IsValid = false, Errors = new List<string> { $"not allowed to use cell [{x},{y}] as it is already used." }, Game = game };
                }

                game.Board[x, y] = (request.Player == 1) ? "X":"O";

                var evaluationStatus = _gameEvaluator.EvaluateBoard(game.Board);

                if (!string.IsNullOrEmpty(evaluationStatus))
                {
                    //game is over
                    game.Status = GameKnownStatuses.Completed;
                    game.WaitingForPlayer = GameKnownWaitingForPlayer.GameIsNotActive;

                    switch (evaluationStatus)
                    {
                        case "X":
                            {
                                game.Result = GameKnownResults.Player1Won;
                                break;
                            }
                        case "O":
                            {
                                game.Result = GameKnownResults.Player2Won;
                                break;
                            }
                        default:
                            {
                                game.Result = GameKnownResults.Draw;
                                break;
                            }
                    }

                }
                else
                {
                    //game is not over, so switching to another player
                    if (request.Player == 1)
                    {
                        game.WaitingForPlayer = GameKnownWaitingForPlayer.WaitingForPlayer2;
                    }
                    else
                    {
                        game.WaitingForPlayer = GameKnownWaitingForPlayer.WaitingForPlayer1;
                    }
                }

                game.LastUpdatedUtc = DateTimeOffset.UtcNow;

                await _repo.UpdateById(request.Id, game, cancellationToken);

                return new UpdateGameResponse { Game = game };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception");
                throw;
            }
        }

        public static bool ParseCoordinate(string coordinates, out int x, out int y)
        {
            x = -1;
            y = -1;
            if (coordinates == null)
            {
                x = -1;
                return false;
            }
            var trimmed = coordinates.Trim();
            if (string.IsNullOrEmpty(trimmed))
            {
                return false;
            }

            var splitted = trimmed.Split(',');

            if (splitted.Length != 2)
            {
                return false;
            }

            if (!int.TryParse(splitted[1].Trim(), out x))
            {
                return false;
            }

            if (!int.TryParse(splitted[0].Trim(), out y))
            {
                return false;
            }

            if (x<0 || x > 2)
            {
                return false;
            }

            if (y<0 || y > 2)
            {
                return false;
            }

            return true;

        }
    }
}
