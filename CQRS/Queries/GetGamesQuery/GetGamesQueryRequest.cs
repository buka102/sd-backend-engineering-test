using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tictactoe_service.CQRS.Queries.GetGamesQuery
{
    public class GetGamesQueryRequest: IRequest<GetGamesQueryResponse>
    {
    }
}
