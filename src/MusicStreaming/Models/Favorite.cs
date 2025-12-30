using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicStreaming.Models
{
    public class Favorite
    {
        [Key]
        public int FavoriteId { get; set; }

        [Display(Name = "Người dùng")]
        public int UserId { get; set; }

        [Display(Name = "Bài hát")]
        public int SongId { get; set; }

        [Display(Name = "Ngày thêm")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("SongId")]
        public virtual Song? Song { get; set; }
    }
}
