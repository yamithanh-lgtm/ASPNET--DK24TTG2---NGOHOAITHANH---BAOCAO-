using System.ComponentModel.DataAnnotations;

namespace MusicStreaming.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Tên thể loại là bắt buộc")]
        [StringLength(100)]
        [Display(Name = "Tên thể loại")]
        public string GenreName { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [StringLength(500)]
        [Display(Name = "Hình ảnh")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Song>? Songs { get; set; }
    }
}
