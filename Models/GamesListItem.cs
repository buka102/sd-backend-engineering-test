using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tictactoe_service.Models
{
    public class GamesListItem
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("result")]
        public string Result { get; set; }
        [JsonProperty("created_utc")]
        public DateTimeOffset CreatedUtc { get; set; }
        [JsonProperty("last_updated_utc")]
        public DateTimeOffset LastUpdatedUtc { get; set; }
        [JsonProperty("waiting_for_player")]
        public string WaitingForPlayer { get; set; }
    }
}
