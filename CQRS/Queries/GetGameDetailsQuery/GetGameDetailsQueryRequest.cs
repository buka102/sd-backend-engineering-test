using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tictactoe_service.CQRS.Queries.GetGameDetailsQuery
{
    public class GetGameDetailsQueryRequest: IRequest<GetGameDetailsQueryResponse>
    {
        public string Id { get; set; }
    }
}
