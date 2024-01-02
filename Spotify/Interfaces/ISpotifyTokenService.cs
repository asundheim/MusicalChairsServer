namespace MusicalChairs.Spotify.Interfaces;

public interface ISpotifyTokenService
{
    public Task<string> GetToken();
}
