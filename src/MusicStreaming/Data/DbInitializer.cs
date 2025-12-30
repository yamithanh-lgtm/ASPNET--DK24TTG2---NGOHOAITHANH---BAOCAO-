using MusicStreaming.Models;

namespace MusicStreaming.Data
{
    public static class DbInitializer
    {
        public static void Initialize(MusicStreamingContext context)
        {
            context.Database.EnsureCreated();

            // Kiểm tra xem đã có dữ liệu chưa
            if (context.Songs.Any())
            {
                return;   // DB đã được seed
            }

            // Seed Genres
            var genres = new Genre[]
            {
                new Genre { GenreName = "Pop", Description = "Nhạc Pop hiện đại", ImageUrl = "/images/genres/pop.jpg" },
                new Genre { GenreName = "Rock", Description = "Nhạc Rock mạnh mẽ", ImageUrl = "/images/genres/rock.jpg" },
                new Genre { GenreName = "Ballad", Description = "Nhạc Ballad nhẹ nhàng", ImageUrl = "/images/genres/ballad.jpg" },
                new Genre { GenreName = "EDM", Description = "Electronic Dance Music", ImageUrl = "/images/genres/edm.jpg" },
                new Genre { GenreName = "Hip Hop", Description = "Nhạc Hip Hop", ImageUrl = "/images/genres/hiphop.jpg" },
                new Genre { GenreName = "Jazz", Description = "Nhạc Jazz cổ điển", ImageUrl = "/images/genres/jazz.jpg" },
                new Genre { GenreName = "Acoustic", Description = "Nhạc Acoustic", ImageUrl = "/images/genres/acoustic.jpg" },
                new Genre { GenreName = "Vpop", Description = "Nhạc Việt Pop", ImageUrl = "/images/genres/vpop.jpg" }
            };
            context.Genres.AddRange(genres);
            context.SaveChanges();

            // Seed Artists
            var artists = new Artist[]
            {
                new Artist { ArtistName = "Sơn Tùng M-TP", Biography = "Ca sĩ, nhạc sĩ người Việt Nam", ImageUrl = "/images/artists/sontung.jpg", Country = "Việt Nam" },
                new Artist { ArtistName = "Hòa Minzy", Biography = "Ca sĩ nữ người Việt Nam", ImageUrl = "/images/artists/hoaminzy.jpg", Country = "Việt Nam" },
                new Artist { ArtistName = "Đen Vâu", Biography = "Rapper người Việt Nam", ImageUrl = "/images/artists/denvau.jpg", Country = "Việt Nam" },
                new Artist { ArtistName = "Mỹ Tâm", Biography = "Ca sĩ nữ người Việt Nam", ImageUrl = "/images/artists/mytam.jpg", Country = "Việt Nam" },
                new Artist { ArtistName = "Noo Phước Thịnh", Biography = "Ca sĩ nam người Việt Nam", ImageUrl = "/images/artists/noophuocthinh.jpg", Country = "Việt Nam" },
                new Artist { ArtistName = "Chi Pu", Biography = "Ca sĩ, diễn viên người Việt Nam", ImageUrl = "/images/artists/chipu.jpg", Country = "Việt Nam" },
                new Artist { ArtistName = "Bích Phương", Biography = "Ca sĩ nữ người Việt Nam", ImageUrl = "/images/artists/bichphuong.jpg", Country = "Việt Nam" },
                new Artist { ArtistName = "Erik", Biography = "Ca sĩ nam người Việt Nam", ImageUrl = "/images/artists/erik.jpg", Country = "Việt Nam" }
            };
            context.Artists.AddRange(artists);
            context.SaveChanges();

            // Seed Albums
            var albums = new Album[]
            {
                new Album { AlbumName = "Sky Tour", ArtistId = artists[0].ArtistId, ReleaseDate = DateTime.Parse("2019-07-01"), CoverImageUrl = "/images/albums/skytour.jpg", Description = "Album Sky Tour của Sơn Tùng M-TP" },
                new Album { AlbumName = "Rời Bỏ", ArtistId = artists[1].ArtistId, ReleaseDate = DateTime.Parse("2020-03-15"), CoverImageUrl = "/images/albums/roibo.jpg", Description = "Album Rời Bỏ của Hòa Minzy" },
                new Album { AlbumName = "Ai Mà Biết Được", ArtistId = artists[2].ArtistId, ReleaseDate = DateTime.Parse("2021-05-20"), CoverImageUrl = "/images/albums/aimabietduoc.jpg", Description = "Album của Đen Vâu" },
                new Album { AlbumName = "Tâm 9", ArtistId = artists[3].ArtistId, ReleaseDate = DateTime.Parse("2018-12-01"), CoverImageUrl = "/images/albums/tam9.jpg", Description = "Album Tâm 9 của Mỹ Tâm" },
                new Album { AlbumName = "The Best Of", ArtistId = artists[4].ArtistId, ReleaseDate = DateTime.Parse("2020-06-10"), CoverImageUrl = "/images/albums/bestof.jpg", Description = "Tuyển tập của Noo Phước Thịnh" }
            };
            context.Albums.AddRange(albums);
            context.SaveChanges();

            // Seed Songs
            var songs = new Song[]
            {
                new Song { SongName = "Lạc Trôi", ArtistId = artists[0].ArtistId, AlbumId = albums[0].AlbumId, GenreId = genres[0].GenreId, Duration = 240, AudioFileUrl = "/audio/lac-troi.mp3", CoverImageUrl = "/images/songs/lac-troi.jpg", Lyrics = "[Lời bài hát Lạc Trôi...]", ReleaseDate = DateTime.Parse("2017-01-01"), PlayCount = 1500000 },
                new Song { SongName = "Chúng Ta Không Thuộc Về Nhau", ArtistId = artists[0].ArtistId, AlbumId = albums[0].AlbumId, GenreId = genres[0].GenreId, Duration = 280, AudioFileUrl = "/audio/chung-ta-khong-thuoc-ve-nhau.mp3", CoverImageUrl = "/images/songs/chung-ta.jpg", Lyrics = "[Lời bài hát...]", ReleaseDate = DateTime.Parse("2019-04-01"), PlayCount = 2000000 },
                new Song { SongName = "Rời Bỏ", ArtistId = artists[1].ArtistId, AlbumId = albums[1].AlbumId, GenreId = genres[1].GenreId, Duration = 260, AudioFileUrl = "/audio/roi-bo.mp3", CoverImageUrl = "/images/songs/roi-bo.jpg", Lyrics = "[Lời bài hát Rời Bỏ...]", ReleaseDate = DateTime.Parse("2020-03-15") },
                new Song { SongName = "Anh Là Ngoại Lệ Của Em", ArtistId = artists[1].ArtistId, AlbumId = albums[1].AlbumId, GenreId = genres[1].GenreId, Duration = 245, AudioFileUrl = "/audio/anh-la-ngoai-le.mp3", CoverImageUrl = "/images/songs/ngoai-le.jpg", Lyrics = "[Lời bài hát...]", ReleaseDate = DateTime.Parse("2021-02-14") },
                new Song { SongName = "Lối Nhỏ", ArtistId = artists[2].ArtistId, AlbumId = albums[2].AlbumId, GenreId = genres[2].GenreId, Duration = 300, AudioFileUrl = "/audio/loi-nho.mp3", CoverImageUrl = "/images/songs/loi-nho.jpg", Lyrics = "[Lời bài hát Lối Nhỏ...]", ReleaseDate = DateTime.Parse("2018-06-01"), PlayCount = 1200000 },
                new Song { SongName = "Bài Này Chill Phết", ArtistId = artists[2].ArtistId, AlbumId = albums[2].AlbumId, GenreId = genres[2].GenreId, Duration = 220, AudioFileUrl = "/audio/bai-nay-chill-phet.mp3", CoverImageUrl = "/images/songs/chill-phet.jpg", Lyrics = "[Lời bài hát...]", ReleaseDate = DateTime.Parse("2020-09-10"), PlayCount = 1800000 },
                new Song { SongName = "Đừng Hỏi Em", ArtistId = artists[3].ArtistId, AlbumId = albums[3].AlbumId, GenreId = genres[3].GenreId, Duration = 290, AudioFileUrl = "/audio/dung-hoi-em.mp3", CoverImageUrl = "/images/songs/dung-hoi-em.jpg", Lyrics = "[Lời bài hát...]", ReleaseDate = DateTime.Parse("2018-12-01") },
                new Song { SongName = "Người Hãy Quên Em Đi", ArtistId = artists[3].ArtistId, AlbumId = albums[3].AlbumId, GenreId = genres[3].GenreId, Duration = 310, AudioFileUrl = "/audio/nguoi-hay-quen-em-di.mp3", CoverImageUrl = "/images/songs/nguoi-hay-quen.jpg", Lyrics = "[Lời bài hát...]", ReleaseDate = DateTime.Parse("2019-05-20") },
                new Song { SongName = "Cause I Love You", ArtistId = artists[4].ArtistId, AlbumId = albums[4].AlbumId, GenreId = genres[4].GenreId, Duration = 235, AudioFileUrl = "/audio/cause-i-love-you.mp3", CoverImageUrl = "/images/songs/cause-i-love-you.jpg", Lyrics = "[Lời bài hát...]", ReleaseDate = DateTime.Parse("2020-06-10") },
                new Song { SongName = "Thương", ArtistId = artists[4].ArtistId, AlbumId = albums[4].AlbumId, GenreId = genres[4].GenreId, Duration = 265, AudioFileUrl = "/audio/thuong.mp3", CoverImageUrl = "/images/songs/thuong.jpg", Lyrics = "[Lời bài hát...]", ReleaseDate = DateTime.Parse("2019-11-15") },
                new Song { SongName = "Đóa Hoa Hồng", ArtistId = artists[5].ArtistId, GenreId = genres[0].GenreId, Duration = 200, AudioFileUrl = "/audio/doa-hoa-hong.mp3", CoverImageUrl = "/images/songs/doa-hoa-hong.jpg", Lyrics = "[Lời bài hát...]", ReleaseDate = DateTime.Parse("2021-07-01") },
                new Song { SongName = "Bùa Yêu", ArtistId = artists[6].ArtistId, GenreId = genres[0].GenreId, Duration = 215, AudioFileUrl = "/audio/bua-yeu.mp3", CoverImageUrl = "/images/songs/bua-yeu.jpg", Lyrics = "[Lời bài hát...]", ReleaseDate = DateTime.Parse("2017-05-20") },
                new Song { SongName = "Ghen Cô Vy", ArtistId = artists[7].ArtistId, GenreId = genres[0].GenreId, Duration = 180, AudioFileUrl = "/audio/ghen-co-vy.mp3", CoverImageUrl = "/images/songs/ghen-co-vy.jpg", Lyrics = "[Lời bài hát...]", ReleaseDate = DateTime.Parse("2020-03-01") }
            };
            context.Songs.AddRange(songs);
            context.SaveChanges();

            // Seed Users
            var users = new User[]
            {
                new User { Username = "admin", Email = "admin@music.com", PasswordHash = "123456", FullName = "Quản Trị Viên", Role = "Admin", AvatarUrl = "/images/avatars/admin.jpg", IsActive = true },
                new User { Username = "user1", Email = "user1@music.com", PasswordHash = "123456", FullName = "Nguyễn Văn A", Role = "User", AvatarUrl = "/images/avatars/user1.jpg", IsActive = true },
                new User { Username = "user2", Email = "user2@music.com", PasswordHash = "123456", FullName = "Trần Thị B", Role = "User", AvatarUrl = "/images/avatars/user2.jpg", IsActive = true }
            };
            context.Users.AddRange(users);
            context.SaveChanges();

            // Seed Playlists
            var playlists = new Playlist[]
            {
                new Playlist { PlaylistName = "Top Hits Việt Nam", UserId = users[0].UserId, Description = "Những bài hát hot nhất Việt Nam", IsPublic = true, IsFeatured = true, CoverImageUrl = "/images/playlists/top-hits.jpg" },
                new Playlist { PlaylistName = "Ballad Buồn", UserId = users[0].UserId, Description = "Những bản ballad da diết", IsPublic = true, IsFeatured = true, CoverImageUrl = "/images/playlists/ballad-buon.jpg" },
                new Playlist { PlaylistName = "Chill Với Đen", UserId = users[0].UserId, Description = "Playlist chill cùng Đen Vâu", IsPublic = true, IsFeatured = true, CoverImageUrl = "/images/playlists/chill-den.jpg" },
                new Playlist { PlaylistName = "Yêu Thích Của Tôi", UserId = users[1].UserId, Description = "Playlist cá nhân", IsPublic = false, IsFeatured = false, CoverImageUrl = "/images/playlists/default.jpg" }
            };
            context.Playlists.AddRange(playlists);
            context.SaveChanges();

            // Seed PlaylistSongs
            var playlistSongs = new PlaylistSong[]
            {
                new PlaylistSong { PlaylistId = playlists[0].PlaylistId, SongId = songs[0].SongId, OrderIndex = 1 },
                new PlaylistSong { PlaylistId = playlists[0].PlaylistId, SongId = songs[1].SongId, OrderIndex = 2 },
                new PlaylistSong { PlaylistId = playlists[0].PlaylistId, SongId = songs[2].SongId, OrderIndex = 3 },
                new PlaylistSong { PlaylistId = playlists[0].PlaylistId, SongId = songs[10].SongId, OrderIndex = 4 },
                new PlaylistSong { PlaylistId = playlists[0].PlaylistId, SongId = songs[11].SongId, OrderIndex = 5 },
                new PlaylistSong { PlaylistId = playlists[1].PlaylistId, SongId = songs[2].SongId, OrderIndex = 1 },
                new PlaylistSong { PlaylistId = playlists[1].PlaylistId, SongId = songs[6].SongId, OrderIndex = 2 },
                new PlaylistSong { PlaylistId = playlists[1].PlaylistId, SongId = songs[7].SongId, OrderIndex = 3 },
                new PlaylistSong { PlaylistId = playlists[1].PlaylistId, SongId = songs[9].SongId, OrderIndex = 4 },
                new PlaylistSong { PlaylistId = playlists[2].PlaylistId, SongId = songs[4].SongId, OrderIndex = 1 },
                new PlaylistSong { PlaylistId = playlists[2].PlaylistId, SongId = songs[5].SongId, OrderIndex = 2 }
            };
            context.PlaylistSongs.AddRange(playlistSongs);
            context.SaveChanges();

            // Seed Favorites
            var favorites = new Favorite[]
            {
                new Favorite { UserId = users[1].UserId, SongId = songs[0].SongId },
                new Favorite { UserId = users[1].UserId, SongId = songs[4].SongId },
                new Favorite { UserId = users[1].UserId, SongId = songs[5].SongId },
                new Favorite { UserId = users[1].UserId, SongId = songs[11].SongId },
                new Favorite { UserId = users[2].UserId, SongId = songs[1].SongId },
                new Favorite { UserId = users[2].UserId, SongId = songs[2].SongId },
                new Favorite { UserId = users[2].UserId, SongId = songs[6].SongId }
            };
            context.Favorites.AddRange(favorites);
            context.SaveChanges();

            // Seed Comments
            var comments = new Comment[]
            {
                new Comment { SongId = songs[0].SongId, UserId = users[1].UserId, Content = "Bài hát hay quá!" },
                new Comment { SongId = songs[0].SongId, UserId = users[2].UserId, Content = "Lạc Trôi là huyền thoại!" },
                new Comment { SongId = songs[4].SongId, UserId = users[1].UserId, Content = "Lối Nhỏ chill phết!" },
                new Comment { SongId = songs[5].SongId, UserId = users[2].UserId, Content = "Đen Vâu là tài năng thực sự" }
            };
            context.Comments.AddRange(comments);
            context.SaveChanges();

            // Seed Ratings
            var ratings = new Rating[]
            {
                new Rating { SongId = songs[0].SongId, UserId = users[1].UserId, RatingValue = 5 },
                new Rating { SongId = songs[0].SongId, UserId = users[2].UserId, RatingValue = 5 },
                new Rating { SongId = songs[4].SongId, UserId = users[1].UserId, RatingValue = 5 },
                new Rating { SongId = songs[5].SongId, UserId = users[2].UserId, RatingValue = 4 },
                new Rating { SongId = songs[2].SongId, UserId = users[1].UserId, RatingValue = 4 },
                new Rating { SongId = songs[6].SongId, UserId = users[2].UserId, RatingValue = 5 }
            };
            context.Ratings.AddRange(ratings);
            context.SaveChanges();
        }
    }
}
