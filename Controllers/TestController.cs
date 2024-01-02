using Microsoft.AspNetCore.Mvc;
using MusicalChairs.Spotify;
using MusicalChairs.Spotify.Interfaces;

namespace MusicalChairs.Controllers;

public class TestController : Controller
{
    private readonly ISpotifyTokenService _tokenService;
    private readonly ISpotifyService _spotifyService;

    public TestController(ISpotifyTokenService spotifyTokenService, ISpotifyService spotifyService)
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
            return BadRequest(new List<string>());
        }

        List<string> tracks = result.Tracks.Select(t => t.Name).ToList();

        return Ok(tracks);
    }

    [HttpGet]
    [Route("/linkalbum")]
    public async Task<ActionResult<List<string>>> GetAlbumLinks(string fromAlbumId = "5Dbax7G8SWrP9xyzkOvy2F", string toAlbumId = "4LH4d3cOWNNsVw41Gqt2kv")
    {
        IEnumerable<SpotifyLink> result = await _spotifyService.TryLinkAlbum(fromAlbumId, toAlbumId);
        if (result is null)
        {
            return BadRequest(new List<string>());
        }

        List<string> links = result.Select(l => l.ToString()).ToList();

        return Ok(links);
    }
}
