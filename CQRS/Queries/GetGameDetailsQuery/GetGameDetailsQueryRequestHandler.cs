using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using tictactoe_service.Data;

namespace tictactoe_service.CQRS.Queries.GetGameDetailsQuery
{
    public class GetGameDetailsQueryRequestHandler : IRequestHandler<GetGameDetailsQueryRequest, GetGameDetailsQueryResponse>
    {
        private readonly ILogger<GetGameDetailsQueryRequestHandler> _logger;
        private readonly IRepository _repo;

        public GetGameDetailsQueryRequestHandler(IRepository repo, ILogger<GetGameDetailsQueryRequestHandler> logger)
        {
            _logger = logger;
            _repo = repo;
        }

        public async Task<GetGameDetailsQueryResponse> Handle(GetGameDetailsQueryRequest request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var game = await _repo.GetById( request.Id, cancellationToken);
                if (game == null)
                {
                    return new GetGameDetailsQueryResponse { IsNotFound = true, IsValid = false, Errors = new List<string> { "game is not found" } };
                }
                return new GetGameDetailsQueryResponse { Game = game };

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected exception");
                throw;
            }
        }
    }
}
