using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreaming.Data;
using MusicStreaming.Models;

namespace MusicStreaming.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MusicStreamingContext _context;

    public HomeController(ILogger<HomeController> logger, MusicStreamingContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Lấy playlists nổi bật
        var featuredPlaylists = await _context.Playlists
            .Where(p => p.IsFeatured && p.IsPublic)
            .Include(p => p.PlaylistSongs)
            .ThenInclude(ps => ps.Song)
            .Take(6)
            .ToListAsync();

        ViewBag.FeaturedPlaylists = featuredPlaylists;

        // Lấy bài hát mới nhất
        var newSongs = await _context.Songs
            .Include(s => s.Artist)
            .Include(s => s.Genre)
            .OrderByDescending(s => s.CreatedDate)
            .Take(12)
            .ToListAsync();

        ViewBag.NewSongs = newSongs;

        // Lấy bài hát trending (nhiều lượt nghe nhất)
        var trendingSongs = await _context.Songs
            .Include(s => s.Artist)
            .Include(s => s.Genre)
            .OrderByDescending(s => s.PlayCount)
            .Take(10)
            .ToListAsync();

        ViewBag.TrendingSongs = trendingSongs;

        // Lấy thể loại
        var genres = await _context.Genres.ToListAsync();
        ViewBag.Genres = genres;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
