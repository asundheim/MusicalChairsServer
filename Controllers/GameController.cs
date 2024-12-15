using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MusicalChairs.Game.Interfaces;
using System.Net;
using System.Net.WebSockets;
using System.Text;

namespace MusicalChairs.Controllers;

public class GameController : Controller
{
    private readonly IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [Route("/game/establishConnection/{clientId}")]
    public async Task EstablishGameConnection(string clientId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            // connection open. send gameid.
            Guid gameId = _gameService.CreateNewGame(clientId, webSocket);
        }
    }

    [Route("/game/{gameId}/join/{playerId}")]
    public async Task JoinGame(Guid gameId, Guid playerId)
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            _gameService.JoinGame(gameId,gameId, webSocket);
        }
    }
}
