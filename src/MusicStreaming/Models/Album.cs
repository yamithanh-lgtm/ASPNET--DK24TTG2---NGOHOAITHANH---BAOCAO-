using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicStreaming.Models
{
    public class Album
    {
        [Key]
        public int AlbumId { get; set; }

        [Required(ErrorMessage = "Tên album là bắt buộc")]
        [StringLength(200)]
        [Display(Name = "Tên album")]
        public string AlbumName { get; set; } = string.Empty;

        [Display(Name = "Nghệ sĩ")]
        public int? ArtistId { get; set; }

        [Display(Name = "Ngày phát hành")]
        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Ảnh bìa")]
        public string? CoverImageUrl { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("ArtistId")]
        public virtual Artist? Artist { get; set; }
        public virtual ICollection<Song>? Songs { get; set; }
    }
}
