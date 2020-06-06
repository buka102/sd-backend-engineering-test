using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace tictactoe_service.Models
{
    public class GameEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("board")]
        public string[,] Board { get; set; }
        [JsonProperty("result")]
        public string Result { get; set; }
        [JsonProperty("created_utc")]
        public DateTimeOffset CreatedUtc { get; set; }
        [JsonProperty("last_updated_utc")]
        public DateTimeOffset LastUpdatedUtc { get; set; }
        [JsonProperty("waiting_for_player", NullValueHandling = NullValueHandling.Ignore)]
        public string WaitingForPlayer { get; set; }
    }

    public static class GameKnownStatuses
    {
        public const string InProgress = "in_progress";
        public const string Completed = "completed";
    }

    public static class GameKnownResults
    {
        public const string InProgress = "game_is_in_progress";
        public const string Player1Won = "won_by_player_1";
        public const string Player2Won = "won_by_player_2";
        public const string Draw = "draw";
    }

    public static class GameKnownWaitingForPlayer
    {
        public const string GameIsNotActive = "none";
        public const string WaitingForPlayer1 = "waiting_for_player_1";
        public const string WaitingForPlayer2 = "waiting_for_player_2";
    }

}
