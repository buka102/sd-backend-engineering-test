using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using tictactoe_service.CQRS.Commands.StartGameCommand;
using tictactoe_service.CQRS.Commands.UpdateGameCommand;
using tictactoe_service.CQRS.Queries.GetGameDetailsQuery;
using tictactoe_service.CQRS.Queries.GetGamesQuery;
using tictactoe_service.Models;
using tictactoe_service.Models.ViewModels;

namespace tictactoe_service.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {


        private readonly ILogger<GamesController> _logger;
        private readonly IMediator _mediator;

        public GamesController(IMediator mediator, ILogger<GamesController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<JsonResult> Get(CancellationToken cancellationToken)
        {

            var gameList = await _mediator.Send(new GetGamesQueryRequest(), cancellationToken);

            return gameList.JsonResult();

        }

        [HttpGet("{id}")]
        public async Task<JsonResult> GetById(string id, CancellationToken cancellationToken)
        {

            var game= await _mediator.Send(new GetGameDetailsQueryRequest { Id = id }, cancellationToken);

            return game.JsonResult();

        }

        [HttpPut("{id}")]
        public async Task<JsonResult> UpdateGame(string id, [FromBody] UpdateGameViewModel updatePayload, CancellationToken cancellationToken)
        {

            var updatedGame = await _mediator.Send(new UpdateGameRequest { Id = id, Player = updatePayload.Player, MoveCoordinate = updatePayload.MoveCoordinate }, cancellationToken);

            return updatedGame.JsonResult();

        }

        [HttpPost()]
        public async Task<JsonResult> CreateNewGame(CancellationToken cancellationToken)
        {

            var newGame = await _mediator.Send(new StartGameRequest {}, cancellationToken);

            return newGame.JsonResult(System.Net.HttpStatusCode.Created);

        }
    }
}
