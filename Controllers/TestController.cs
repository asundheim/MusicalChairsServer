using Microsoft.AspNetCore.Mvc;
using MusicalChairs.Spotify;

namespace MusicalChairs.Controllers;

public class TestController : Controller
{
    private readonly SpotifyTokenService _tokenService;
    private readonly SpotifyService _spotifyService;

    public TestController(SpotifyTokenService spotifyTokenService, SpotifyService spotifyService)
    {
        _tokenService = spotifyTokenService;
        _spotifyService = spotifyService;
    }

    [HttpGet]
    [Route("/token")]
    public async Task<ActionResult<string>> GetToken()
    {
        return Ok(await _tokenService.GetToken());
    }

    [HttpGet]
    [Route("/tracksforalbum")]
    public async Task<ActionResult<List<string>>> GetTracksForAlbum(string albumId)
    {
        var result = await _spotifyService.GetTracksForAlbumId(albumId);
        if (result is null)
        {
            throw new Exception("failed to get tracks");
        }

        List<string> tracks = result.Tracks.Select(t => t.Name).ToList();

        return Ok(tracks);
    }
}
