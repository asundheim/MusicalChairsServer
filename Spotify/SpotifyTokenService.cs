using MusicalChairs.Spotify.Interfaces;
using MusicalChairs.Spotify.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace MusicalChairs.Spotify;

public class SpotifyTokenService : ISpotifyTokenService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    private string _token = string.Empty;
    private DateTime? _tokenExpirationDate = DateTime.MinValue;

    public SpotifyTokenService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://accounts.spotify.com/api/token");
        _configuration = configuration;
    }

    public async Task<string> GetToken()
    {
        if (DateTime.Now >= _tokenExpirationDate)
        {
            await RefreshToken();
        }

        return _token;
    }

    private async Task RefreshToken()
    {
        IEnumerable<KeyValuePair<string, string>> tokenRequestContent = new Dictionary<string, string>()
        {
            ["grant_type"] = "client_credentials"
        };

        HttpRequestMessage tokenRequest = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            Content = new FormUrlEncodedContent(tokenRequestContent)
        };

        tokenRequest.Headers.Add("Authorization", 
            $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_configuration["Spotify:ClientId"]}:{_configuration["Spotify:ClientSecret"]}"))}");

        try
        {
            var response = await _httpClient.SendAsync(tokenRequest);
            response.EnsureSuccessStatusCode();

            using (var contentStream = await response.Content.ReadAsStreamAsync())
            {
                var accessToken = await JsonSerializer.DeserializeAsync<SpotifyAccessToken>(contentStream) ?? throw new Exception("failed to deserialize access token");

                Debug.Assert(accessToken.TokenType.Equals("bearer", comparisonType: StringComparison.OrdinalIgnoreCase), "Token type not bearer?");
                _tokenExpirationDate = DateTime.Now.AddMilliseconds(accessToken.ExpiresInMs);
                _token = accessToken.AccessToken;
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
