using System.Net.WebSockets;

namespace MusicalChairs.Game.Interfaces;

public interface IGameService
{
    /// <summary>
    /// Retrieve a <see cref="Game"/> from it's id.
    /// </summary>
    /// <param name="gameId">The <see cref="Game.Id"/></param>
    /// <returns>The <see cref="Game"/></returns>
    public Game? GetGame(Guid gameId);

    /// <summary>
    /// Create a new game and registers it to the game service.
    /// </summary>
    /// <param name="clientId">player 1 (creator) id</param>
    /// <param name="webSocket">The transport for game messages.</param>
    /// <returns>The <see cref="Game.Id"/> of the created game.</returns>
    public Guid CreateNewGame(string clientId, WebSocket webSocket);

    /// <summary>
    /// Attempt to join a created game.
    /// </summary>
    /// <param name="gameId">The game id</param>
    /// <param name="playerId">The joining player id</param>
    /// <param name="webSocket">The transport for game messages.</param>
    /// <returns>If the game was joined successfully.</returns>
    public bool JoinGame(Guid gameId, Guid playerId, WebSocket webSocket);
}
