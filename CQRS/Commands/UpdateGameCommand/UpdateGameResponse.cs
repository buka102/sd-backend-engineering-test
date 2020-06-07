using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tictactoe_service.CQRS.Shared;
using tictactoe_service.Models;

namespace tictactoe_service.CQRS.Commands.UpdateGameCommand
{
    public class UpdateGameResponse:BaseResponse
    {
        [JsonProperty("game", NullValueHandling = NullValueHandling.Ignore)]
        public GameEntity Game { get; set; }
    }
}
