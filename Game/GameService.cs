using MusicalChairs.Game.Interfaces;
using MusicalChairs.Spotify.Interfaces;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace MusicalChairs.Game;

public class GameService : IGameService
{
    private static readonly ConcurrentDictionary<Guid, Game> _games = new ConcurrentDictionary<Guid, Game>();
    private readonly ISpotifyService _spotifyService;

    public GameService(ISpotifyService spotifyService)
    {
        _spotifyService = spotifyService;
    }

    public Game? GetGame(Guid gameId)
    {
        if (_games.TryGetValue(gameId, out var game))
        {
            return game;
        }

        return null;
    }

    public Guid CreateNewGame(string clientId, WebSocket webSocket)
    {
        Game newGame = new Game(clientId, webSocket, _spotifyService);
        _games[newGame.Id] = newGame;

        return newGame.Id;
    }

    public bool JoinGame(Guid gameId, Guid clientId, WebSocket webSocket)
    {
        Game? game = GetGame(gameId);
        if (game is null)
        {
            return false;
        }

        return game.AddPlayer2(clientId);
    }
}
