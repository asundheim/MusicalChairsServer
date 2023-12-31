using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MusicalChairs.Spotify.Models;

public class SpotifySimpleTrackCollection
{
    [JsonPropertyName("limit")]
    public required int Limit { get; set; }

    [JsonPropertyName("next")]
    public string? Next { get; set; }

    [JsonPropertyName("previous")]
    public required string Previous { get; set; }

    [JsonPropertyName("offset")]
    public required int Offset { get; set; }

    [JsonPropertyName("total")]
    public required int Total { get; set; }

    [JsonPropertyName("items")]
    public required List<SpotifySimpleTrack> Tracks { get; set; }

    public bool IsEnumerated() => Tracks.Count == Total;

    public async Task EnumerateAllTracks(SpotifyService spotifyService, string albumId)
    {
        Debug.Assert(this.Next is not null && this.Limit < this.Total, "All tracks already enumerated");

        while (this.Tracks.Count < this.Total)
        {
            Offset = Tracks.Count;
            SpotifySimpleTrackCollection? result = await spotifyService.Get<SpotifySimpleTrackCollection>($"albums/{albumId}/tracks?offset={Offset}&limit={Limit}");
            if (result is null)
            {
                break;
            }
            else
            {
                Tracks.AddRange(result.Tracks);
            }
        }
    }
}
