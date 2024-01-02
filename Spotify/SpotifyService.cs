using MusicalChairs.Spotify.Interfaces;
using MusicalChairs.Spotify.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace MusicalChairs.Spotify;

public class SpotifyService : ISpotifyService
{
    private readonly HttpClient _httpClient;
    private readonly ISpotifyTokenService _tokenService;

    private static readonly string[] trackNameSeparators = [" ", ",", ", ", ":", ": "];
    private static readonly string[] invalidTokenLinks = ["the", "and", "for", "you"];

    public SpotifyService(IHttpClientFactory httpClientFactory, ISpotifyTokenService tokenService)
    {
        _httpClient = httpClientFactory.CreateClient();
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

    public Task<SpotifyAlbum?> GetAlbum(string albumId) => Get<SpotifyAlbum>($"albums/{albumId}");

    public async Task<IEnumerable<SpotifyLink>> TryLinkAlbum(string fromAlbumId, string toAlbumId)
    {
        SpotifyAlbum? fromAlbum = await GetAlbum(fromAlbumId);
        SpotifyAlbum? toAlbum = await GetAlbum(toAlbumId);

        if (fromAlbum is null || toAlbum is null)
        {
            return Enumerable.Empty<SpotifyLink>();
        }
        else
        {
            List<SpotifyLink> links = new List<SpotifyLink>();

            HashSet<SpotifySimpleArtist> fromArtists = new HashSet<SpotifySimpleArtist>(fromAlbum.Artists);
            HashSet<SpotifySimpleArtist> toArtists = new HashSet<SpotifySimpleArtist>(toAlbum.Artists);
            IEnumerable<SpotifySimpleArtist> artistIntersection = fromArtists.Intersect(toArtists);

            foreach (SpotifySimpleArtist artist in artistIntersection)
            {
                links.Add(new SpotifyLink() { LinkType = SpotifyLinkType.Artist, Artist = artist });
            }

            if (!fromAlbum.TracksCollection.IsEnumerated())
            {
                await fromAlbum.TracksCollection.EnumerateAllTracks(this, fromAlbumId);
            }

            if (!toAlbum.TracksCollection.IsEnumerated())
            {
                await toAlbum.TracksCollection.EnumerateAllTracks(this, toAlbumId);
            }
            
            foreach (SpotifySimpleTrack fromTrack in fromAlbum.TracksCollection.Tracks)
            {
                HashSet<string> trackNameTokens = new HashSet<string>(fromTrack.Name.Split(trackNameSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Where(t => t.Length > 2));

                foreach (SpotifySimpleTrack toTrack in toAlbum.TracksCollection.Tracks)
                {
                    HashSet<string> toTrackNameTokens = new HashSet<string>(toTrack.Name.Split(trackNameSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Where(t => t.Length > 2));

                    IEnumerable<string> tokenIntersection = trackNameTokens.Intersect(toTrackNameTokens);
                    foreach (string intersectToken in tokenIntersection)
                    {
                        if (invalidTokenLinks.Any(invalidToken => invalidToken.Equals(intersectToken, StringComparison.OrdinalIgnoreCase)))
                        {
                            continue;
                        }

                        links.Add(new SpotifyLink() 
                        { 
                            LinkType = SpotifyLinkType.Song,
                            FromSong = fromTrack, 
                            ToSong = toTrack, 
                            SongNameLink = intersectToken 
                        });

                        break;
                    }
                }
            }

            return links;
        }
    }
}
