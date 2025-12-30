using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicStreaming.Models
{
    public class Song
    {
        [Key]
        public int SongId { get; set; }

        [Required(ErrorMessage = "Tên bài hát là bắt buộc")]
        [StringLength(200)]
        [Display(Name = "Tên bài hát")]
        public string SongName { get; set; } = string.Empty;

        [Display(Name = "Nghệ sĩ")]
        public int? ArtistId { get; set; }

        [Display(Name = "Album")]
        public int? AlbumId { get; set; }

        [Display(Name = "Thể loại")]
        public int? GenreId { get; set; }

        [Display(Name = "Thời lượng (giây)")]
        public int? Duration { get; set; }

        [Required(ErrorMessage = "File nhạc là bắt buộc")]
        [StringLength(500)]
        [Display(Name = "File nhạc")]
        public string AudioFileUrl { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Ảnh bìa")]
        public string? CoverImageUrl { get; set; }

        [Display(Name = "Lời bài hát")]
        public string? Lyrics { get; set; }

        [Display(Name = "Lượt nghe")]
        public int PlayCount { get; set; } = 0;

        [Display(Name = "Ngày phát hành")]
        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        [ForeignKey("ArtistId")]
        public virtual Artist? Artist { get; set; }

        [ForeignKey("AlbumId")]
        public virtual Album? Album { get; set; }

        [ForeignKey("GenreId")]
        public virtual Genre? Genre { get; set; }

        public virtual ICollection<PlaylistSong>? PlaylistSongs { get; set; }
        public virtual ICollection<Favorite>? Favorites { get; set; }
        public virtual ICollection<ListeningHistory>? ListeningHistories { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<Rating>? Ratings { get; set; }
    }
}
