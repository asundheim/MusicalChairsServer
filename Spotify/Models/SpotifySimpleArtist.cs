using System.Text.Json.Serialization;

namespace MusicalChairs.Spotify.Models;

public record SpotifySimpleArtist(
    [property: JsonPropertyName("href")] string Href,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("uri")] string Uri);
