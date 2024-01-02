using MusicalChairs.Game.Interfaces;
using MusicalChairs.Spotify;
using MusicalChairs.Spotify.Interfaces;
using MusicalChairs.Spotify.Models;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace MusicalChairs.Game;

public class Game : IGame
{
    private readonly ISpotifyService _spotifyService;

    private string _player1Id;
    private string? _player2Id;
    private readonly WebSocket _webSocket;
    private readonly CancellationTokenSource _gameCancellationTokenSource = new CancellationTokenSource();

    private readonly BlockingCollection<string> _sendQueue = new BlockingCollection<string>();

    private const int CHECK_INTERVAL = 250;

    public Game(string player1Id, WebSocket webSocket, ISpotifyService spotifyService)
    {
        _spotifyService = spotifyService;
        _webSocket = webSocket;

        this._player1Id = player1Id;
    }

    public Guid Id { get; } = Guid.NewGuid();

    public List<SpotifyAlbum> AlbumsPlayed { get; set; } = new List<SpotifyAlbum>();

    public Dictionary<SpotifyLink, int> LinksPlayed { get; set; } = new Dictionary<SpotifyLink, int>();

    public async Task GameLoop()
    {
        // send gameId to client
        _sendQueue.Add(this.Id.ToString());

        // kick off send queue task
        _ = Task.Run(() => ProcessSendQueue(_gameCancellationTokenSource.Token));

        byte[] buffer = new byte[8192];
        while (!_gameCancellationTokenSource.IsCancellationRequested)
        {
            WebSocketReceiveResult receiveResult = await _webSocket.ReceiveAsync(buffer, _gameCancellationTokenSource.Token);
            if (receiveResult.MessageType == WebSocketMessageType.Close)
            {
                _gameCancellationTokenSource.Cancel();
                _sendQueue.CompleteAdding();

                await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            }
        }
    }

    private async Task ProcessSendQueue(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (_sendQueue.TryTake(out string? message, 0, cancellationToken))
                {
                    if (!string.IsNullOrEmpty(message))
                    {
                        await _webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, cancellationToken);
                    }
                }

                await Task.Delay(CHECK_INTERVAL, cancellationToken);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in {nameof(ProcessSendQueue)}: {ex}");
            }
        }
    }

    public IEnumerable<SpotifyLink> TryAddAlbum(string albumId)
    {

    }
}
