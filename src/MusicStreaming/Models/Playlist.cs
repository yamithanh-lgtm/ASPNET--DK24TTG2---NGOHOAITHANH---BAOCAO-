using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicStreaming.Models
{
    public class Playlist
    {
        [Key]
        public int PlaylistId { get; set; }

        [Required(ErrorMessage = "Tên playlist là bắt buộc")]
        [StringLength(200)]
        [Display(Name = "Tên playlist")]
        public string PlaylistName { get; set; } = string.Empty;

        [Display(Name = "Người tạo")]
        public int? UserId { get; set; }

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [StringLength(500)]
        [Display(Name = "Ảnh bìa")]
        public string? CoverImageUrl { get; set; }

        [Display(Name = "Công khai")]
        public bool IsPublic { get; set; } = false;

        [Display(Name = "Nổi bật")]
        public bool IsFeatured { get; set; } = false;

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public virtual ICollection<PlaylistSong>? PlaylistSongs { get; set; }
    }
}
