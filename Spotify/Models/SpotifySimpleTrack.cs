using System.Text.Json.Serialization;

namespace MusicalChairs.Spotify.Models;

public record SpotifySimpleTrack(
    [property: JsonPropertyName("artists")] List<SpotifySimpleArtist> Artists,
    [property: JsonPropertyName("href")] string Href,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("uri")] string Uri,
    [property: JsonPropertyName("name")] string Name);
