using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tictactoe_service.Models.ViewModels
{
    public class UpdateGameViewModel
    {
        [JsonProperty("player")]
        public int Player { get; set; }
        [JsonProperty("move_coordinates")]
        public string MoveCoordinate { get; set; }
    }
}
