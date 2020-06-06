using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace tictactoe_service.CQRS.Shared
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
    {

        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger _logger;

        public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validators, ILoggerFactory loggerFactory)
        {
            _validators = validators;
            _logger = loggerFactory.CreateLogger("ValidatorBehavior");
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {

            var context = new ValidationContext(request);

            var validatorTasks = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context)));

            var failures = validatorTasks
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .ToList();

            if (failures.Any())
            {
                _logger.LogTrace("There are {0} validation errors triggered.", failures.Count());

                if (typeof(BaseResponse).IsAssignableFrom(typeof(TResponse)))
                {
                    try
                    {
                        var response = (TResponse)Activator.CreateInstance(typeof(TResponse));
                        var baseResponse = response as BaseResponse;
                        if (baseResponse != null)
                        {
                            baseResponse.IsValid = false;
                            baseResponse.Errors = failures.Select(e => e.ErrorMessage).ToList();
                            return response;
                        }
                        else
                        {
                            _logger.LogTrace("Failed to create an instance of {0} to return failures in BaseResponse", typeof(TResponse).Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogTrace(ex, "Failed to create an instance of {0} to return failures in BaseResponse", typeof(TResponse).Name);
                    }
                }


                //if we are here, then it failed to create an instance of the TResponse

                throw new ValidationException(failures);

            }

            return await next();
        }
    }
}
