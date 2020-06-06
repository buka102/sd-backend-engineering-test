using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using tictactoe_service.CQRS.Shared;
using tictactoe_service.Models;

namespace tictactoe_service.CQRS.Queries.GetGamesQuery
{
    public class GetGamesQueryResponse : BaseResponse
    {
        [JsonProperty("list")]
        public List<GamesListItem> List { get; set; }
    }
}
