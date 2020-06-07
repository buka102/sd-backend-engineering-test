using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Runtime.Internal.Util;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using tictactoe_service.Configuration;
using tictactoe_service.CQRS.Commands.StartGameCommand;
using tictactoe_service.CQRS.Commands.UpdateGameCommand;
using tictactoe_service.CQRS.Queries.GetGameDetailsQuery;
using tictactoe_service.CQRS.Queries.GetGamesQuery;
using tictactoe_service.CQRS.Shared;
using tictactoe_service.Models.ViewModels;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace tictactoe_service.Lambda
{
    public class Functions
    {
        private readonly IMediator _mediator;
        private readonly ILogger<Functions> _logger;
        private ServiceProvider serviceProvider;

        public Functions()
        {
            ConfigureServices();
            _mediator = serviceProvider.GetRequiredService<IMediator>();
            _logger = serviceProvider.GetRequiredService<ILogger<Functions>>();
        }

        public async Task<APIGatewayProxyResponse> GetList(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var cts = new CancellationTokenSource(context.RemainingTime);
            var result = await _mediator.Send(new GetGamesQueryRequest(), cts.Token);

            return result.APIGatewayResponse();
        }

        public async Task<APIGatewayProxyResponse> GetById(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var id = request.PathParameters["id"];
            var cts = new CancellationTokenSource(context.RemainingTime);
            var result = await _mediator.Send(new GetGameDetailsQueryRequest { Id = id }, cts.Token);

            return result.APIGatewayResponse();
        }

        public async Task<APIGatewayProxyResponse> CreateNew(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var cts = new CancellationTokenSource(context.RemainingTime);
            var result = await _mediator.Send(new StartGameRequest() , cts.Token);

            return result.APIGatewayResponse();
        }

        public async Task<APIGatewayProxyResponse> UpdateById(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var id = request.PathParameters["id"];
            try
            {
                var requestBody = JsonConvert.DeserializeObject<UpdateGameViewModel>(request.Body);
                var cts = new CancellationTokenSource(context.RemainingTime);
                var result = await _mediator.Send(new UpdateGameRequest { Id = id, Player = requestBody.Player, MoveCoordinate = requestBody.MoveCoordinate }, cts.Token);

                return result.APIGatewayResponse();
            }
            catch(Exception)
            {
                var err = new BaseResponse { IsValid = false, Errors = new List<string>() { "Something is wrong wiht your request" } };
                return err.APIGatewayResponse();
            }
        }

        private void ConfigureServices()
        {
            var serviceCollection = new ServiceCollection();
            Startup.GlobalConfigureServices(serviceCollection, null);

            serviceCollection.AddSingleton<AWSDynamoConfig>(ctx => {

                var strAccessKey =  Environment.GetEnvironmentVariable("Dynamo_AccessKey");
                var strSecretKey = Environment.GetEnvironmentVariable("Dynamo_SecretKey");
                var strRegion = Environment.GetEnvironmentVariable("Dynamo_Region");
                var strTable = Environment.GetEnvironmentVariable("Dynamo_Table");
                return new AWSDynamoConfig { Region = strRegion, AccessKey = strAccessKey, SecretKey = strSecretKey, Table = strTable };
            });


            serviceProvider = serviceCollection.BuildServiceProvider();
        }

    }
}
