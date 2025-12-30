using System.ComponentModel.DataAnnotations;

namespace MusicStreaming.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [StringLength(200)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(500)]
        [Display(Name = "Mật khẩu")]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(200)]
        [Display(Name = "Họ tên")]
        public string? FullName { get; set; }

        [StringLength(500)]
        [Display(Name = "Ảnh đại diện")]
        public string? AvatarUrl { get; set; }

        [StringLength(20)]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "User";

        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Lần đăng nhập cuối")]
        public DateTime? LastLoginDate { get; set; }

        // Navigation properties
        public virtual ICollection<Playlist>? Playlists { get; set; }
        public virtual ICollection<Favorite>? Favorites { get; set; }
        public virtual ICollection<ListeningHistory>? ListeningHistories { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Rating>? Ratings { get; set; }
    }
}
