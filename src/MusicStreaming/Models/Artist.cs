using System.ComponentModel.DataAnnotations;

namespace MusicStreaming.Models
{
    public class Artist
    {
        [Key]
        public int ArtistId { get; set; }

        [Required(ErrorMessage = "Tên nghệ sĩ là bắt buộc")]
        [StringLength(200)]
        [Display(Name = "Tên nghệ sĩ")]
        public string ArtistName { get; set; } = string.Empty;

        [Display(Name = "Tiểu sử")]
        public string? Biography { get; set; }

        [StringLength(500)]
        [Display(Name = "Hình ảnh")]
        public string? ImageUrl { get; set; }

        [StringLength(100)]
        [Display(Name = "Quốc gia")]
        public string? Country { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Song>? Songs { get; set; }
        public virtual ICollection<Album>? Albums { get; set; }
    }
}
