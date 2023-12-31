using MusicalChairs.Spotify.Models;
using System.Text.Json;

namespace MusicalChairs.Spotify;

public class SpotifyService
{
    private readonly HttpClient _httpClient;
    private readonly SpotifyTokenService _tokenService;

    public SpotifyService(HttpClient httpClient, SpotifyTokenService tokenService)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://api.spotify.com/v1/");

        _tokenService = tokenService;
    }

    public async Task<SpotifySimpleTrackCollection?> GetTracksForAlbumId(string albumId)
    {
        SpotifySimpleTrackCollection? result = await Get<SpotifySimpleTrackCollection>($"albums/{albumId}/tracks");

        if (result is null)
        {
            return null;
        }

        if (!result.IsEnumerated())
        {
            await result.EnumerateAllTracks(this, albumId);
        }

        return result;
    }

    public async Task<T?> Get<T>(string uriFragment) where T : class
    {
        try
        {
            string authToken = await _tokenService.GetToken();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uriFragment)
            {
                Headers =
                {
                    { "Authorization", $"Bearer {authToken}" }
                }
            };

            var response = await _httpClient.SendAsync(request);
            using (var contentStream = await response.Content.ReadAsStreamAsync())
            {
                T result = await JsonSerializer.DeserializeAsync<T>(contentStream) ?? throw new Exception($"failed to deserialize {typeof(T)}");

                return result;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }
    }
}
