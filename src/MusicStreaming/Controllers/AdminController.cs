using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreaming.Data;
using MusicStreaming.Models;

namespace MusicStreaming.Controllers
{
    public class AdminController : Controller
    {
        private readonly MusicStreamingContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminController(MusicStreamingContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Role") == "Admin";
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            // Thống kê
            ViewBag.TotalSongs = await _context.Songs.CountAsync();
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalArtists = await _context.Artists.CountAsync();
            ViewBag.TotalPlaylists = await _context.Playlists.CountAsync();

            // Bài hát mới thêm
            var recentSongs = await _context.Songs
                .Include(s => s.Artist)
                .OrderByDescending(s => s.CreatedDate)
                .Take(5)
                .ToListAsync();

            return View(recentSongs);
        }

        // --- SONGS MANAGEMENT ---
        public async Task<IActionResult> Songs()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var songs = await _context.Songs.Include(s => s.Artist).Include(s => s.Genre).Include(s => s.Album).ToListAsync();
            return View(songs);
        }

        public async Task<IActionResult> CreateSong()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            ViewBag.Artists = await _context.Artists.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();
            ViewBag.Albums = await _context.Albums.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSong(Song song, IFormFile audioFile, IFormFile imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                // Upload Audio
                if (audioFile != null && audioFile.Length > 0)
                {
                    var audioName = Guid.NewGuid().ToString() + Path.GetExtension(audioFile.FileName);
                    var audioPath = Path.Combine(_environment.WebRootPath, "audio", audioName);
                    using (var stream = new FileStream(audioPath, FileMode.Create))
                    {
                        await audioFile.CopyToAsync(stream);
                    }
                    song.AudioFileUrl = "/audio/" + audioName;
                }

                // Upload Image
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var imagePath = Path.Combine(_environment.WebRootPath, "images", "songs", imageName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    song.CoverImageUrl = "/images/songs/" + imageName;
                }

                song.CreatedDate = DateTime.Now;
                _context.Add(song);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm bài hát thành công";
                return RedirectToAction(nameof(Songs));
            }

            ViewBag.Artists = await _context.Artists.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();
            ViewBag.Albums = await _context.Albums.ToListAsync();
            return View(song);
        }

        public async Task<IActionResult> EditSong(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var song = await _context.Songs.FindAsync(id);
            if (song == null) return NotFound();

            ViewBag.Artists = await _context.Artists.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();
            ViewBag.Albums = await _context.Albums.ToListAsync();
            return View(song);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSong(int id, Song song, IFormFile? audioFile, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id != song.SongId) return NotFound();

            // Bỏ qua validate AudioFileUrl nếu không upload file mới (giữ nguyên file cũ)
            if (audioFile == null)
            {
                 ModelState.Remove("AudioFileUrl");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSong = await _context.Songs.AsNoTracking().FirstOrDefaultAsync(s => s.SongId == id);
                    if (existingSong == null) return NotFound();

                    // Giữ nguyên URL cũ nếu không upload mới
                    song.AudioFileUrl = existingSong.AudioFileUrl;
                    song.CoverImageUrl = existingSong.CoverImageUrl;
                    song.CreatedDate = existingSong.CreatedDate;

                    // Upload Audio Mới
                    if (audioFile != null && audioFile.Length > 0)
                    {
                        var audioName = Guid.NewGuid().ToString() + Path.GetExtension(audioFile.FileName);
                        var audioPath = Path.Combine(_environment.WebRootPath, "audio", audioName);
                        using (var stream = new FileStream(audioPath, FileMode.Create))
                        {
                            await audioFile.CopyToAsync(stream);
                        }
                        song.AudioFileUrl = "/audio/" + audioName;
                    }

                    // Upload Image Mới
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        var imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        var imagePath = Path.Combine(_environment.WebRootPath, "images", "songs", imageName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }
                        song.CoverImageUrl = "/images/songs/" + imageName;
                    }

                    _context.Update(song);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật bài hát thành công";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Songs.Any(e => e.SongId == song.SongId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Songs));
            }
            ViewBag.Artists = await _context.Artists.ToListAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();
            ViewBag.Albums = await _context.Albums.ToListAsync();
            return View(song);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSong(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var song = await _context.Songs.FindAsync(id);
            if (song != null)
            {
                _context.Songs.Remove(song);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa bài hát thành công";
            }
            return RedirectToAction(nameof(Songs));
        }

        // --- ARTISTS MANAGEMENT ---
        public async Task<IActionResult> Artists()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var artists = await _context.Artists.Include(a => a.Songs).ToListAsync();
            return View(artists);
        }

        public IActionResult CreateArtist()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateArtist(Artist artist, IFormFile imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                 if (imageFile != null && imageFile.Length > 0)
                {
                    var imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var imagePath = Path.Combine(_environment.WebRootPath, "images", "artists", imageName);
                    
                    // Ensure directory exists
                    var dir = Path.GetDirectoryName(imagePath);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir!);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    artist.ImageUrl = "/images/artists/" + imageName;
                }

                _context.Add(artist);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm nghệ sĩ thành công";
                return RedirectToAction(nameof(Artists));
            }
            return View(artist);
        }

        public async Task<IActionResult> EditArtist(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null) return NotFound();
            return View(artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArtist(int id, Artist artist, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id != artist.ArtistId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingArtist = await _context.Artists.AsNoTracking().FirstOrDefaultAsync(a => a.ArtistId == id);
                    if (existingArtist != null)
                    {
                        artist.ImageUrl = existingArtist.ImageUrl; // Keep old image by default

                        if (imageFile != null && imageFile.Length > 0)
                        {
                            var imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                            var imagePath = Path.Combine(_environment.WebRootPath, "images", "artists", imageName);
                            
                            var dir = Path.GetDirectoryName(imagePath);
                            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir!);

                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(stream);
                            }
                            artist.ImageUrl = "/images/artists/" + imageName;
                        }
                    }

                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật nghệ sĩ thành công";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Artists.Any(e => e.ArtistId == artist.ArtistId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Artists));
            }
            return View(artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var artist = await _context.Artists.FindAsync(id);
            if (artist != null)
            {
                _context.Artists.Remove(artist);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa nghệ sĩ thành công";
            }
            return RedirectToAction(nameof(Artists));
        }


        // --- GENRES MANAGEMENT ---
        public async Task<IActionResult> Genres()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var genres = await _context.Genres.ToListAsync();
            return View(genres);
        }

        public IActionResult CreateGenre()
        {
             if (!IsAdmin()) return RedirectToAction("Login", "Account");
             return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGenre(Genre genre)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                _context.Add(genre);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm thể loại thành công";
                return RedirectToAction(nameof(Genres));
            }
            return View(genre);
        }

        public async Task<IActionResult> EditGenre(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGenre(int id, Genre genre)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id != genre.GenreId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(genre);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thể loại thành công";
                }
                catch (DbUpdateConcurrencyException)
                {
                     if (!_context.Genres.Any(e => e.GenreId == genre.GenreId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Genres));
            }
            return View(genre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa thể loại thành công";
            }
            return RedirectToAction(nameof(Genres));
        }

        // --- ALBUMS MANAGEMENT ---
        public async Task<IActionResult> Albums()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var albums = await _context.Albums.Include(a => a.Artist).ToListAsync();
            return View(albums);
        }

        public async Task<IActionResult> CreateAlbum()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            ViewBag.Artists = await _context.Artists.ToListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAlbum(Album album, IFormFile imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var imagePath = Path.Combine(_environment.WebRootPath, "images", "albums", imageName);
                    
                    var dir = Path.GetDirectoryName(imagePath);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir!);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    album.CoverImageUrl = "/images/albums/" + imageName;
                }
                
                album.ReleaseDate = DateTime.Now; // Default release date
                _context.Add(album);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm album thành công";
                return RedirectToAction(nameof(Albums));
            }
            ViewBag.Artists = await _context.Artists.ToListAsync();
            return View(album);
        }

        public async Task<IActionResult> EditAlbum(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();
            var album = await _context.Albums.FindAsync(id);
            if (album == null) return NotFound();
            ViewBag.Artists = await _context.Artists.ToListAsync();
            return View(album);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAlbum(int id, Album album, IFormFile? imageFile)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id != album.AlbumId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingAlbum = await _context.Albums.AsNoTracking().FirstOrDefaultAsync(a => a.AlbumId == id);
                    if (existingAlbum != null)
                    {
                        album.CoverImageUrl = existingAlbum.CoverImageUrl;
                        album.ReleaseDate = existingAlbum.ReleaseDate;

                        if (imageFile != null && imageFile.Length > 0)
                        {
                            var imageName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                            var imagePath = Path.Combine(_environment.WebRootPath, "images", "albums", imageName);
                            
                            var dir = Path.GetDirectoryName(imagePath);
                            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir!);

                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(stream);
                            }
                            album.CoverImageUrl = "/images/albums/" + imageName;
                        }
                    }

                    _context.Update(album);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật album thành công";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Albums.Any(e => e.AlbumId == album.AlbumId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Albums));
            }
            ViewBag.Artists = await _context.Artists.ToListAsync();
            return View(album);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var album = await _context.Albums.FindAsync(id);
            if (album != null)
            {
                _context.Albums.Remove(album);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa album thành công";
            }
            return RedirectToAction(nameof(Albums));
        }

        // --- USERS MANAGEMENT ---
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                // Không cho phép khóa chính mình
                var currentUserId = HttpContext.Session.GetInt32("UserId");
                if (user.UserId == currentUserId)
                {
                    TempData["Error"] = "Không thể khóa tài khoản của chính mình";
                    return RedirectToAction(nameof(Users));
                }

                user.IsActive = !user.IsActive;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Đã {(user.IsActive ? "mở khóa" : "khóa")} tài khoản {user.Username}";
            }
            return RedirectToAction(nameof(Users));
        }

        // --- COMMENTS MANAGEMENT ---
        public async Task<IActionResult> Comments()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var comments = await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Song)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            return View(comments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa bình luận thành công";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy bình luận";
            }

            return RedirectToAction(nameof(Comments));
        }
    }
}
