using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using tictactoe_service.Data;

namespace tictactoe_service.CQRS.Queries.GetGamesQuery
{
    public class GetGamesQueryRequestHandler : IRequestHandler<GetGamesQueryRequest, GetGamesQueryResponse>
    {
        private readonly ILogger<GetGamesQueryRequestHandler> _logger;
        private readonly IRepository _repo;

        public GetGamesQueryRequestHandler(IRepository repo, ILogger<GetGamesQueryRequestHandler> logger)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<GetGamesQueryResponse> Handle(GetGamesQueryRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var list = await _repo.GetList(cancellationToken);
                return new GetGamesQueryResponse { List = list };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception");
                throw;
            }
        }
    }
}
