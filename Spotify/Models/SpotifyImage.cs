using System.Text.Json.Serialization;

namespace MusicalChairs.Spotify.Models;

public record SpotifyImage(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("height")] int Height,
    [property: JsonPropertyName("width")] int Width);
