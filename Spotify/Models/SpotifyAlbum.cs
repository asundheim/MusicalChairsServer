using System.Diagnostics;
using System.Text.Json.Serialization;

namespace MusicalChairs.Spotify.Models;

[DebuggerDisplay("{AlbumName} ({ReleaseDate}) {Id}")]
public record SpotifyAlbum(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("album_type")] string AlbumType,
    [property: JsonPropertyName("total_track")] int TotalTracks,
    [property: JsonPropertyName("name")] string AlbumName,
    [property: JsonPropertyName("release_date")] string ReleaseDate,
    [property: JsonPropertyName("images")] List<SpotifyImage> Images,
    [property: JsonPropertyName("uri")] string Uri,
    [property: JsonPropertyName("artists")] List<SpotifySimpleArtist> Artists,
    [property: JsonPropertyName("tracks")] SpotifySimpleTrackCollection TracksCollection
)
{

}
