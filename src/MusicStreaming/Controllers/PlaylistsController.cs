using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreaming.Data;
using MusicStreaming.Models;

namespace MusicStreaming.Controllers
{
    public class PlaylistsController : Controller
    {
        private readonly MusicStreamingContext _context;

        public PlaylistsController(MusicStreamingContext context)
        {
            _context = context;
        }

        // Helper: Lấy UserId từ Session
        private int? GetUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        // GET: Playlists (Danh sách playlist của tôi)
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var playlists = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            return View(playlists);
        }

        // GET: Playlists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var playlist = await _context.Playlists
                .Include(p => p.User)
                .Include(p => p.PlaylistSongs!)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s!.Artist)
                .Include(p => p.PlaylistSongs!)
                    .ThenInclude(ps => ps.Song)
                        .ThenInclude(s => s!.Album)
                .FirstOrDefaultAsync(m => m.PlaylistId == id);

            if (playlist == null) return NotFound();

            // Kiểm tra quyền truy cập (nếu private và không phải chủ sở hữu)
            var userId = GetUserId();
            if (!playlist.IsPublic && playlist.UserId != userId)
            {
                return Forbid();
            }

            ViewBag.IsOwner = playlist.UserId == userId;
            return View(playlist);
        }

        // GET: Playlists/Create
        public IActionResult Create()
        {
            if (!GetUserId().HasValue) return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: Playlists/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlaylistName,Description,IsPublic")] Playlist playlist)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                playlist.UserId = userId.Value;
                playlist.CreatedDate = DateTime.Now;
                playlist.CoverImageUrl = "/images/playlists/default.jpg"; // Ảnh mặc định

                _context.Add(playlist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(playlist);
        }

        // POST: Playlists/AddSong
        [HttpPost]
        public async Task<IActionResult> AddSong(int playlistId, int songId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thực hiện chức năng này" });
            }

            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist == null || playlist.UserId != userId)
            {
                return Json(new { success = false, message = "Playlist không tồn tại hoặc bạn không có quyền" });
            }

            // Kiểm tra bài hát đã có trong playlist chưa
            var exists = await _context.PlaylistSongs
                .AnyAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (exists)
            {
                return Json(new { success = false, message = "Bài hát đã có trong playlist này" });
            }

            var playlistSong = new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = songId,
                AddedDate = DateTime.Now,
                OrderIndex = 0 // Có thể xử lý logic order sau
            };

            _context.PlaylistSongs.Add(playlistSong);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã thêm vào playlist" });
        }

        // POST: Playlists/RemoveSong
        [HttpPost]
        public async Task<IActionResult> RemoveSong(int playlistId, int songId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập" });
            }

            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist == null || playlist.UserId != userId)
            {
                return Json(new { success = false, message = "Không có quyền thực hiện" });
            }

            var playlistSong = await _context.PlaylistSongs
                .FirstOrDefaultAsync(ps => ps.PlaylistId == playlistId && ps.SongId == songId);

            if (playlistSong != null)
            {
                _context.PlaylistSongs.Remove(playlistSong);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Đã xóa khỏi playlist" });
            }

            return Json(new { success = false, message = "Bài hát không tồn tại trong playlist" });
        }

        // POST: Playlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = GetUserId();
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            var playlist = await _context.Playlists.FindAsync(id);
            if (playlist != null && playlist.UserId == userId)
            {
                _context.Playlists.Remove(playlist);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        // Helper: Lấy danh sách playlist nhỏ gọn để hiển thị trong modal "Add to Playlist"
        [HttpGet]
        public async Task<IActionResult> GetMyPlaylists()
        {
            var userId = GetUserId();
            if (!userId.HasValue) return Json(new List<object>());

            var playlists = await _context.Playlists
                .Where(p => p.UserId == userId)
                .Select(p => new { p.PlaylistId, p.PlaylistName, Count = p.PlaylistSongs.Count })
                .ToListAsync();

            return Json(playlists);
        }
    }
}
