using System.Text.Json.Serialization;

namespace MusicalChairs.Spotify.Models;

public record SpotifyAccessToken(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_in")] int ExpiresInMs,
    [property: JsonPropertyName("token_type")] string TokenType);
