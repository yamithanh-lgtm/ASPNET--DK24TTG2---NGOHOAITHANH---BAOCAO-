using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreaming.Data;
using MusicStreaming.Models;

namespace MusicStreaming.Controllers
{
    public class SongsController : Controller
    {
        private readonly MusicStreamingContext _context;

        public SongsController(MusicStreamingContext context)
        {
            _context = context;
        }

        // GET: Songs
        public async Task<IActionResult> Index(string search)
        {
            var songs = _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .Include(s => s.Genre)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                songs = songs.Where(s => s.SongName.Contains(search) || s.Artist.ArtistName.Contains(search));
            }

            return View(await songs.ToListAsync());
        }

        // GET: Songs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Album)
                .Include(s => s.Genre)
                .Include(s => s.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(m => m.SongId == id);

            if (song == null)
            {
                return NotFound();
            }

            // Tăng lượt nghe (đơn giản)
            song.PlayCount++;
            _context.Update(song);
            await _context.SaveChangesAsync();

            // Lấy thông tin đánh giá
            var ratings = await _context.Ratings.Where(r => r.SongId == id).ToListAsync();
            var averageRating = ratings.Any() ? ratings.Average(r => r.RatingValue) : 0;
            var totalRatings = ratings.Count;

            ViewBag.AverageRating = averageRating;
            ViewBag.TotalRatings = totalRatings;

            // Kiểm tra user hiện tại đã thích/đánh giá chưa
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == id);
                ViewBag.IsFavorite = favorite != null;

                var userRating = await _context.Ratings.FirstOrDefaultAsync(r => r.UserId == userId && r.SongId == id);
                ViewBag.UserRating = userRating?.RatingValue ?? 0;
            }

            // Bài hát liên quan (cùng thể loại)
            var relatedSongs = await _context.Songs
                .Include(s => s.Artist)
                .Where(s => s.GenreId == song.GenreId && s.SongId != song.SongId)
                .Take(5)
                .ToListAsync();
            ViewBag.RelatedSongs = relatedSongs;

            return View(song);
        }

        // POST: Songs/ToggleFavorite
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int songId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thêm yêu thích" });
            }

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.SongId == songId);

            bool isFavorite;
            if (favorite != null)
            {
                // Xóa khỏi yêu thích
                _context.Favorites.Remove(favorite);
                isFavorite = false;
            }
            else
            {
                // Thêm vào yêu thích
                var newFavorite = new Favorite
                {
                    UserId = userId.Value,
                    SongId = songId,
                    CreatedDate = DateTime.Now
                };
                _context.Favorites.Add(newFavorite);
                isFavorite = true;
            }

            await _context.SaveChangesAsync();

            return Json(new { success = true, isFavorite = isFavorite });
        }

        // POST: Songs/Rate
        [HttpPost]
        public async Task<IActionResult> Rate(int songId, int score)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để đánh giá" });
            }

            if (score < 1 || score > 5)
            {
                return Json(new { success = false, message = "Điểm đánh giá không hợp lệ" });
            }

            var rating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.SongId == songId);

            if (rating != null)
            {
                rating.RatingValue = score;
                rating.CreatedDate = DateTime.Now;
                _context.Update(rating);
            }
            else
            {
                rating = new Rating
                {
                    UserId = userId.Value,
                    SongId = songId,
                    RatingValue = score,
                    CreatedDate = DateTime.Now
                };
                _context.Add(rating);
            }

            await _context.SaveChangesAsync();

            // Tính lại điểm trung bình
            var average = await _context.Ratings.Where(r => r.SongId == songId).AverageAsync(r => r.RatingValue);
            var total = await _context.Ratings.CountAsync(r => r.SongId == songId);

            return Json(new { success = true, average = average, total = total });
        }

        // POST: Songs/Comment
        [HttpPost]
        public async Task<IActionResult> Comment(int songId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để bình luận" });
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                return Json(new { success = false, message = "Nội dung bình luận không được để trống" });
            }

            var comment = new Comment
            {
                UserId = userId.Value,
                SongId = songId,
                Content = content,
                CreatedDate = DateTime.Now
            };

            _context.Add(comment);
            await _context.SaveChangesAsync();

            // Lấy thông tin user để trả về hiển thị ngay
            var user = await _context.Users.FindAsync(userId.Value);

            return Json(new { 
                success = true, 
                username = user?.Username, 
                avatarUrl = user?.AvatarUrl,
                createdDate = comment.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                content = comment.Content
            });
        }
    }
}
