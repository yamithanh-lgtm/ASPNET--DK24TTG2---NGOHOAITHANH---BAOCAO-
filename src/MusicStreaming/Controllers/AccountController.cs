using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreaming.Data;
using MusicStreaming.Models;
using System.Security.Cryptography;
using System.Text;

namespace MusicStreaming.Controllers
{
    public class AccountController : Controller
    {
        private readonly MusicStreamingContext _context;

        public AccountController(MusicStreamingContext context)
        {
            _context = context;
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);

            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng";
                return View();
            }

            if (!user.IsActive)
            {
                ViewBag.Error = "Tài khoản đã bị khóa";
                return View();
            }

            // Lưu Session
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            if (!string.IsNullOrEmpty(user.FullName))
            {
                HttpContext.Session.SetString("FullName", user.FullName);
            }
            if (!string.IsNullOrEmpty(user.AvatarUrl))
            {
                HttpContext.Session.SetString("AvatarUrl", user.AvatarUrl);
            }

            user.LastLoginDate = DateTime.Now;
            await _context.SaveChangesAsync();

            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            if (HttpContext.Session.GetInt32("UserId").HasValue)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string email, string password, string confirmPassword, string? fullName)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp";
                return View();
            }

            if (password.Length < 6)
            {
                ViewBag.Error = "Mật khẩu phải có ít nhất 6 ký tự";
                return View();
            }

            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại";
                return View();
            }

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                ViewBag.Error = "Email đã được sử dụng";
                return View();
            }

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = password,
                FullName = fullName,
                Role = "User",
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ViewBag.Success = "Đăng ký thành công! Vui lòng đăng nhập.";
            return View("Login");
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            var favoritesCount = await _context.Favorites.CountAsync(f => f.UserId == userId);
            var playlistsCount = await _context.Playlists.CountAsync(p => p.UserId == userId);
            var commentsCount = await _context.Comments.CountAsync(c => c.UserId == userId);

            ViewBag.FavoritesCount = favoritesCount;
            ViewBag.PlaylistsCount = playlistsCount;
            ViewBag.CommentsCount = commentsCount;

            var favorites = await _context.Favorites
                .Include(f => f.Song)
                    .ThenInclude(s => s!.Artist)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedDate)
                .Take(10)
                .ToListAsync();

            ViewBag.Favorites = favorites;

            var playlists = await _context.Playlists
                .Include(p => p.PlaylistSongs)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            ViewBag.Playlists = playlists;

            return View(user);
        }

        // POST: Account/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string? fullName, string? email)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(email) && email != user.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email == email && u.UserId != userId))
                {
                    TempData["Error"] = "Email đã được sử dụng";
                    return RedirectToAction("Profile");
                }
                user.Email = email;
            }

            user.FullName = fullName;
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(fullName))
            {
                HttpContext.Session.SetString("FullName", fullName);
            }

            TempData["Success"] = "Cập nhật thông tin thành công";
            return RedirectToAction("Profile");
        }

        // POST: Account/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu mới không khớp";
                return RedirectToAction("Profile");
            }

            if (newPassword.Length < 6)
            {
                TempData["Error"] = "Mật khẩu phải có ít nhất 6 ký tự";
                return RedirectToAction("Profile");
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            if (user.PasswordHash != currentPassword)
            {
                TempData["Error"] = "Mật khẩu hiện tại không đúng";
                return RedirectToAction("Profile");
            }

            user.PasswordHash = newPassword;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đổi mật khẩu thành công";
            return RedirectToAction("Profile");
        }

        // POST: Account/UpdateAvatar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAvatar(IFormFile avatarFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(avatarFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    TempData["Error"] = "Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .gif)";
                    return RedirectToAction("Profile");
                }

                var fileName = $"avatar_{userId}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "avatars", fileName);

                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory!);
                }

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }

                var user = await _context.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    user.AvatarUrl = "/images/avatars/" + fileName;
                    await _context.SaveChangesAsync();

                    HttpContext.Session.SetString("AvatarUrl", user.AvatarUrl);
                    
                    TempData["Success"] = "Cập nhật ảnh đại diện thành công";
                }
            }
            else
            {
                TempData["Error"] = "Vui lòng chọn file ảnh";
            }

            return RedirectToAction("Profile");
        }
    }
}
