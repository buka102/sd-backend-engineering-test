### Author:   Irina Ivahno
### Date:     06/06/2020


# Description

Solution in  C# .Net Core 3.1 API. Provides API endpoints for Tic Tac Toe game.

When player 1 makes a move, it's always "X".
Player 2 puts "0"



### Endpoints:

    - Get all games:
        GET /games/

    - Create new game/board:
        POST: /games/ 

    - Get game details:
        GET: /games/<game-id>

    - Player plays:
        PUT: /games/<game-id>

        Body of the request should be in following format:
        {
            "player":<1-or-2>,
            "move_coordinates":"1,1"
        }

Board is two-dimentional array with 0-based coordinates.

Resposne will always return status of the game:
```
{
    "game": {
        "id": "TcMZN3nCv0S57WO0SPCw",
        "status": "in_progress",
        "board": [
            [
                "X",
                "",
                "X"
            ],
            [
                "X",
                "O",
                "X"
            ],
            [
                "O",
                "O",
                ""
            ]
        ],
        "result": "game_is_in_progress",
        "created_utc": "2020-06-06T20:59:25.933099+00:00",
        "last_updated_utc": "2020-06-06T21:00:53.21745+00:00",
        "waiting_for_player": "waiting_for_player_2"
    },
    "is_valid": false,
    "errors": [
        "not allowed to use cell [2,0] as it is already used."
    ]
}
```


## Database

DynamoDB is used for data. It has following attributes:

    - string id
    - string status
    - string board
    - string result
    - string create_utc
    - string update_utc
    - string waiting_for_player

DynamoDB will be provisioned on deployment.


## Configuration 

Configue values for DynamoDB in serverless.yml file:

```  
environment:
    Dynamo_AccessKey: "<your-key>"
    Dynamo_SecretKey: "<your-secret>"
```

Configure settings in appsettings file: 
``` 
"AWS": {
    "Region": "us-west-1",
    "AccessKey": "<your-key>",
    "SecretKey": "<your-secret>",
    "Table": "GameCatalog"
  }
```

OR provide env variables with CLI:

```
export AWS_ACCESS_KEY_ID=<your-key>
export AWS_SECRET_ACCESS_KEY=<your-secret>
```


## Run Locally:

```
sh build.sh 
dotnet build
dotnet run
```
Running on:
http://localhost:5000/games


## Test

Postman collection is provided "tictactoe.postman_collection.json"

Note: when new game is created, id should be used in subsequent requests.


## Deploy

```
sh build.sh 
sls deploy
```

Output:
``` 
...
endpoints:
  GET - https://<custom-url-id>.execute-api.us-west-1.amazonaws.com/dev/games
  GET - https://<custom-url-id>.execute-api.us-west-1.amazonaws.com/dev/games/{id}
  POST - https://<custom-url-id>.execute-api.us-west-1.amazonaws.com/dev/games
  PUT - https://<custom-url-id>.execute-api.us-west-1.amazonaws.com/dev/games/{id}
....
```

Use postman for testing

