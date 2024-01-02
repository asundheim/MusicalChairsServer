using MusicalChairs.Spotify.Models;

namespace MusicalChairs.Spotify;

public enum SpotifyLinkType { Invalid, Artist, Song }
public class SpotifyLink
{
    public SpotifyLinkType LinkType { get; set; } = SpotifyLinkType.Invalid;

    public SpotifySimpleArtist? Artist { get; set; }

    public SpotifySimpleTrack? FromSong { get; set; }

    public SpotifySimpleTrack? ToSong { get; set; }

    public string? SongNameLink { get; set; }

    public override string ToString()
    {
        if (Artist is not null)
        {
            return Artist.Name;
        }
        
        if (FromSong is not null && ToSong is not null && SongNameLink is not null)
        {
            return $"{FromSong.Name} -> {ToSong.Name} ({SongNameLink})";
        }

        return string.Empty;
    }
}
