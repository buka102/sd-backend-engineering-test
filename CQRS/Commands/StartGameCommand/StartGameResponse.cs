using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tictactoe_service.CQRS.Shared;
using tictactoe_service.Models;

namespace tictactoe_service.CQRS.Commands.StartGameCommand
{
    public class StartGameResponse : BaseResponse
    {
        [JsonProperty("created")]
        public GameEntity Created { get; set; }
    }
}
