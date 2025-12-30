using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicStreaming.Models
{
    public class PlaylistSong
    {
        [Key]
        public int PlaylistSongId { get; set; }

        [Display(Name = "Playlist")]
        public int PlaylistId { get; set; }

        [Display(Name = "Bài hát")]
        public int SongId { get; set; }

        [Display(Name = "Thứ tự")]
        public int OrderIndex { get; set; } = 0;

        [Display(Name = "Ngày thêm")]
        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("PlaylistId")]
        public virtual Playlist? Playlist { get; set; }

        [ForeignKey("SongId")]
        public virtual Song? Song { get; set; }
    }
}
