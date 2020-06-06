using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using tictactoe_service.Models;

namespace tictactoe_service.Data
{
    public class DynamoDbRepository : IRepository
    {
        private ILogger<DynamoDbRepository> _logger;
        private AmazonDynamoDBClient _dbClient;
        private IConfiguration _configuration;
        private Table _table;
        private readonly string _tableName;

        public DynamoDbRepository(IConfiguration configuration, AmazonDynamoDBClient dbClient, ILogger<DynamoDbRepository> logger)
        {
            _logger = logger;
            _dbClient = dbClient;
            _configuration = configuration;
            _tableName = _configuration["AWS:Table"];
        }

        private Table GameCatalog
        {
            get
            {
                if (_table == null)
                {
                    _table = Table.LoadTable(_dbClient, _tableName);
                }
                return _table;
            }
        }

        public async Task<string> CreateNew(GameEntity gameEntity, CancellationToken cancellationToken)
        {
            await PutItem(gameEntity, cancellationToken);

            return gameEntity.Id;
        }

        public async Task<GameEntity> GetById(string id, CancellationToken cancellationToken)
        {

            GetItemOperationConfig config = new GetItemOperationConfig
            {
                AttributesToGet = new List<string> { "id","status","result","board","waiting_for_player","create_utc","update_utc" },
                ConsistentRead = true
            };
            Document document = await GameCatalog.GetItemAsync(id, config, cancellationToken);

            if (document != null)
            {

                var g = new GameEntity();
                g.Id = document["id"].AsString();
                g.Status = document["status"].AsString();
                g.Result = document["result"].AsString();
                g.WaitingForPlayer = document["waiting_for_player"].AsString();

                g.Board = JsonConvert.DeserializeObject<string[,]>(document["board"].AsString());

                var create_utc_str = document["create_utc"].AsString();
                if (DateTimeOffset.TryParse(create_utc_str, out DateTimeOffset createdUtc))
                {
                    g.CreatedUtc = createdUtc;
                }

                var updated_utc_str = document["update_utc"].AsString();
                if (DateTimeOffset.TryParse(updated_utc_str, out DateTimeOffset updatedUtc))
                {
                    g.LastUpdatedUtc = updatedUtc;
                }
                return g;
            }
            else
            {
                return null;
            }













            //var request = new GetItemRequest
            //{
            //    TableName = _tableName,
            //    Key = new Dictionary<string, AttributeValue>()
            //{
            //    { "id", new AttributeValue {
            //          S = id
            //      } }
            //},
            //    ProjectionExpression = "id, status, result, board, waiting_for_player, create_utc, create_utc, update_utc",
            //    ConsistentRead = true
            //};

            //var response = await _dbClient.GetItemAsync(request, cancellationToken);

            //if (response.IsItemSet)
            //{

            //    var r = response.Item;
            //    var g = new GameEntity();
            //    g.Id = r["id"].S;
            //    g.Status = r["status"].S;
            //    g.Result = r["result"].S;
            //    g.WaitingForPlayer = r["waiting_for_player"].S;

            //    g.Board = JsonConvert.DeserializeObject<string[,]>(r["board"].AsString());

            //    var create_utc_str = r["create_utc"].S;
            //    if (DateTimeOffset.TryParse(create_utc_str, out DateTimeOffset createdUtc))
            //    {
            //        g.CreatedUtc = createdUtc;
            //    }

            //    var updated_utc_str = r["update_utc"].S;
            //    if (DateTimeOffset.TryParse(updated_utc_str, out DateTimeOffset updatedUtc))
            //    {
            //        g.LastUpdatedUtc = updatedUtc;
            //    }
            //    return g;
            //}
            //else
            //{
            //    return null;
            //}


        }

        public async Task<List<GamesListItem>> GetList(CancellationToken cancellationToken)
        {

            var request = new ScanRequest
            {
                TableName = _tableName
            };

            var response = await _dbClient.ScanAsync(request, cancellationToken);

            var games = response.Items.Select(r =>
            {
                var g = new GamesListItem();
                g.Id = r["id"].S;
                g.Status = r["status"].S;
                g.Result = r["result"].S;
                g.WaitingForPlayer = r["waiting_for_player"].S;
                var create_utc_str = r["create_utc"].S;
                if (DateTimeOffset.TryParse(create_utc_str, out DateTimeOffset createdUtc))
                {
                    g.CreatedUtc = createdUtc;
                }

                var updated_utc_str = r["update_utc"].S;
                if (DateTimeOffset.TryParse(updated_utc_str, out DateTimeOffset updatedUtc))
                {
                    g.LastUpdatedUtc = updatedUtc;
                }

                return g;
            });

            return games.ToList();

        }

        public async Task<bool> UpdateById(string id, GameEntity updatedEntity, CancellationToken cancellationToken)
        {
            await PutItem(updatedEntity, cancellationToken);
            return true;
        }

        private async Task PutItem(GameEntity entity, CancellationToken cancellationToken)
        {
            var boardSerialized = JsonConvert.SerializeObject(entity.Board);

            var gameDoc = new Document();
            gameDoc["id"] = entity.Id;
            gameDoc["status"] = entity.Status;
            gameDoc["result"] = entity.Result;
            gameDoc["board"] = boardSerialized;
            gameDoc["waiting_for_player"] = entity.WaitingForPlayer;
            gameDoc["create_utc"] = entity.CreatedUtc.ToString("o");
            gameDoc["update_utc"] = entity.LastUpdatedUtc.ToString("o");

            await GameCatalog.PutItemAsync(gameDoc, cancellationToken);
        }
    }
}
