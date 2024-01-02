using MusicalChairs.Spotify.Models;

namespace MusicalChairs.Spotify.Interfaces;

public interface ISpotifyService
{
    public Task<SpotifySimpleTrackCollection?> GetTracksForAlbumId(string albumId);

    public Task<IEnumerable<SpotifyLink>> TryLinkAlbum(string fromAlbumId, string toAlbumId);

    public Task<T?> Get<T>(string uriFragment) where T : class;
}
