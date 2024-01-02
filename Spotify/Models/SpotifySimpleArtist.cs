using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace MusicalChairs.Spotify.Models;

public record SpotifySimpleArtist(
    [property: JsonPropertyName("href")] string Href,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("uri")] string Uri) : IEqualityComparer<SpotifySimpleArtist>
{
    public bool Equals(SpotifySimpleArtist? x, SpotifySimpleArtist? y)
    {
        return x?.Id.Equals(y?.Id ?? string.Empty, StringComparison.Ordinal) ?? false;
    }

    public int GetHashCode([DisallowNull] SpotifySimpleArtist artist)
    {
        return artist.Id.GetHashCode();
    }
}
