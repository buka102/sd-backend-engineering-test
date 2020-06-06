using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tictactoe_service.CQRS.Commands.UpdateGameCommand
{
    public class UpdateGameRequest : IRequest<UpdateGameResponse>
    {
        public string Id { get; set; }
        public int Player { get; set; }
        public string MoveCoordinate { get; set; }
    }
}
