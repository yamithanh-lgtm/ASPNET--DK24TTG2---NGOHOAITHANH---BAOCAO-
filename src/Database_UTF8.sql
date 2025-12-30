-- ================================================
-- Database: MusicStreamingDB
-- Description: Database cho website nghe nhạc trực tuyến
-- Encoding: UTF-8
-- ================================================

USE master;
GO

-- Xóa database nếu đã tồn tại
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'MusicStreamingDB')
BEGIN
    ALTER DATABASE MusicStreamingDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MusicStreamingDB;
END
GO

-- Tạo database mới
CREATE DATABASE MusicStreamingDB;
GO

USE MusicStreamingDB;
GO

-- ================================================
-- Bảng: Genres (Thể loại)
-- ================================================
CREATE TABLE Genres (
    GenreId INT PRIMARY KEY IDENTITY(1,1),
    GenreName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    ImageUrl NVARCHAR(500),
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- ================================================
-- Bảng: Artists (Nghệ sĩ)
-- ================================================
CREATE TABLE Artists (
    ArtistId INT PRIMARY KEY IDENTITY(1,1),
    ArtistName NVARCHAR(200) NOT NULL,
    Biography NVARCHAR(MAX),
    ImageUrl NVARCHAR(500),
    Country NVARCHAR(100),
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- ================================================
-- Bảng: Albums (Album)
-- ================================================
CREATE TABLE Albums (
    AlbumId INT PRIMARY KEY IDENTITY(1,1),
    AlbumName NVARCHAR(200) NOT NULL,
    ArtistId INT FOREIGN KEY REFERENCES Artists(ArtistId),
    ReleaseDate DATE,
    CoverImageUrl NVARCHAR(500),
    Description NVARCHAR(MAX),
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- ================================================
-- Bảng: Songs (Bài hát)
-- ================================================
CREATE TABLE Songs (
    SongId INT PRIMARY KEY IDENTITY(1,1),
    SongName NVARCHAR(200) NOT NULL,
    ArtistId INT FOREIGN KEY REFERENCES Artists(ArtistId),
    AlbumId INT FOREIGN KEY REFERENCES Albums(AlbumId),
    GenreId INT FOREIGN KEY REFERENCES Genres(GenreId),
    Duration INT, -- Thời lượng (giây)
    AudioFileUrl NVARCHAR(500) NOT NULL,
    CoverImageUrl NVARCHAR(500),
    Lyrics NVARCHAR(MAX),
    PlayCount INT DEFAULT 0,
    ReleaseDate DATE,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- ================================================
-- Bảng: Users (Người dùng)
-- ================================================
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(500) NOT NULL,
    FullName NVARCHAR(200),
    AvatarUrl NVARCHAR(500),
    Role NVARCHAR(20) DEFAULT 'User', -- User, Admin
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    LastLoginDate DATETIME
);

-- ================================================
-- Bảng: Playlists (Danh sách phát)
-- ================================================
CREATE TABLE Playlists (
    PlaylistId INT PRIMARY KEY IDENTITY(1,1),
    PlaylistName NVARCHAR(200) NOT NULL,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    Description NVARCHAR(500),
    CoverImageUrl NVARCHAR(500),
    IsPublic BIT DEFAULT 0,
    IsFeatured BIT DEFAULT 0, -- Playlist nổi bật
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- ================================================
-- Bảng: PlaylistSongs (Bài hát trong playlist)
-- ================================================
CREATE TABLE PlaylistSongs (
    PlaylistSongId INT PRIMARY KEY IDENTITY(1,1),
    PlaylistId INT FOREIGN KEY REFERENCES Playlists(PlaylistId) ON DELETE CASCADE,
    SongId INT FOREIGN KEY REFERENCES Songs(SongId) ON DELETE CASCADE,
    OrderIndex INT DEFAULT 0,
    AddedDate DATETIME DEFAULT GETDATE()
);

-- ================================================
-- Bảng: Favorites (Bài hát yêu thích)
-- ================================================
CREATE TABLE Favorites (
    FavoriteId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId) ON DELETE CASCADE,
    SongId INT FOREIGN KEY REFERENCES Songs(SongId) ON DELETE CASCADE,
    CreatedDate DATETIME DEFAULT GETDATE(),
    UNIQUE(UserId, SongId)
);

-- ================================================
-- Bảng: ListeningHistory (Lịch sử nghe nhạc)
-- ================================================
CREATE TABLE ListeningHistory (
    HistoryId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId) ON DELETE CASCADE,
    SongId INT FOREIGN KEY REFERENCES Songs(SongId) ON DELETE CASCADE,
    ListenedDate DATETIME DEFAULT GETDATE()
);

-- ================================================
-- Bảng: Comments (Bình luận)
-- ================================================
CREATE TABLE Comments (
    CommentId INT PRIMARY KEY IDENTITY(1,1),
    SongId INT FOREIGN KEY REFERENCES Songs(SongId) ON DELETE CASCADE,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    Content NVARCHAR(1000) NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE()
);

-- ================================================
-- Bảng: Ratings (Đánh giá)
-- ================================================
CREATE TABLE Ratings (
    RatingId INT PRIMARY KEY IDENTITY(1,1),
    SongId INT FOREIGN KEY REFERENCES Songs(SongId) ON DELETE CASCADE,
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    RatingValue INT CHECK (RatingValue BETWEEN 1 AND 5),
    CreatedDate DATETIME DEFAULT GETDATE(),
    UNIQUE(SongId, UserId)
);

-- ================================================
-- INSERT DỮ LIỆU MẪU
-- ================================================

-- Thêm Genres
INSERT INTO Genres (GenreName, Description, ImageUrl) VALUES
(N'Pop', N'Nhạc Pop hiện đại', '/images/genres/pop.jpg'),
(N'Rock', N'Nhạc Rock mạnh mẽ', '/images/genres/rock.jpg'),
(N'Ballad', N'Nhạc Ballad nhẹ nhàng', '/images/genres/ballad.jpg'),
(N'EDM', N'Electronic Dance Music', '/images/genres/edm.jpg'),
(N'Hip Hop', N'Nhạc Hip Hop', '/images/genres/hiphop.jpg'),
(N'Jazz', N'Nhạc Jazz cổ điển', '/images/genres/jazz.jpg'),
(N'Acoustic', N'Nhạc Acoustic', '/images/genres/acoustic.jpg'),
(N'Vpop', N'Nhạc Việt Pop', '/images/genres/vpop.jpg');

-- Thêm Artists
INSERT INTO Artists (ArtistName, Biography, ImageUrl, Country) VALUES
(N'Sơn Tùng M-TP', N'Ca sĩ, nhạc sĩ người Việt Nam', '/images/artists/sontung.jpg', N'Việt Nam'),
(N'Hòa Minzy', N'Ca sĩ nữ người Việt Nam', '/images/artists/hoaminzy.jpg', N'Việt Nam'),
(N'Đen Vâu', N'Rapper người Việt Nam', '/images/artists/denvau.jpg', N'Việt Nam'),
(N'Mỹ Tâm', N'Ca sĩ nữ người Việt Nam', '/images/artists/mytam.jpg', N'Việt Nam'),
(N'Noo Phước Thịnh', N'Ca sĩ nam người Việt Nam', '/images/artists/noophuocthinh.jpg', N'Việt Nam'),
(N'Chi Pu', N'Ca sĩ, diễn viên người Việt Nam', '/images/artists/chipu.jpg', N'Việt Nam'),
(N'Bích Phương', N'Ca sĩ nữ người Việt Nam', '/images/artists/bichphuong.jpg', N'Việt Nam'),
(N'Erik', N'Ca sĩ nam người Việt Nam', '/images/artists/erik.jpg', N'Việt Nam');

-- Thêm Albums
INSERT INTO Albums (AlbumName, ArtistId, ReleaseDate, CoverImageUrl, Description) VALUES
(N'Sky Tour', 1, '2019-07-01', '/images/albums/skytour.jpg', N'Album Sky Tour của Sơn Tùng M-TP'),
(N'Rời Bỏ', 2, '2020-03-15', '/images/albums/roibo.jpg', N'Album Rời Bỏ của Hòa Minzy'),
(N'Ai Mà Biết Được', 3, '2021-05-20', '/images/albums/aimabietduoc.jpg', N'Album của Đen Vâu'),
(N'Tâm 9', 4, '2018-12-01', '/images/albums/tam9.jpg', N'Album Tâm 9 của Mỹ Tâm'),
(N'The Best Of', 5, '2020-06-10', '/images/albums/bestof.jpg', N'Tuyển tập của Noo Phước Thịnh');

-- Thêm Songs
INSERT INTO Songs (SongName, ArtistId, AlbumId, GenreId, Duration, AudioFileUrl, CoverImageUrl, Lyrics, ReleaseDate) VALUES
(N'Lạc Trôi', 1, 1, 8, 240, '/audio/lac-troi.mp3', '/images/songs/lac-troi.jpg', N'[Lời bài hát Lạc Trôi...]', '2017-01-01'),
(N'Chúng Ta Không Thuộc Về Nhau', 1, 1, 3, 280, '/audio/chung-ta-khong-thuoc-ve-nhau.mp3', '/images/songs/chung-ta.jpg', N'[Lời bài hát...]', '2019-04-01'),
(N'Rời Bỏ', 2, 2, 3, 260, '/audio/roi-bo.mp3', '/images/songs/roi-bo.jpg', N'[Lời bài hát Rời Bỏ...]', '2020-03-15'),
(N'Anh Là Ngoại Lệ Của Em', 2, 2, 8, 245, '/audio/anh-la-ngoai-le.mp3', '/images/songs/ngoai-le.jpg', N'[Lời bài hát...]', '2021-02-14'),
(N'Lối Nhỏ', 3, 3, 7, 300, '/audio/loi-nho.mp3', '/images/songs/loi-nho.jpg', N'[Lời bài hát Lối Nhỏ...]', '2018-06-01'),
(N'Bài Này Chill Phết', 3, 3, 5, 220, '/audio/bai-nay-chill-phet.mp3', '/images/songs/chill-phet.jpg', N'[Lời bài hát...]', '2020-09-10'),
(N'Đừng Hỏi Em', 4, 4, 3, 290, '/audio/dung-hoi-em.mp3', '/images/songs/dung-hoi-em.jpg', N'[Lời bài hát...]', '2018-12-01'),
(N'Người Hãy Quên Em Đi', 4, 4, 3, 310, '/audio/nguoi-hay-quen-em-di.mp3', '/images/songs/nguoi-hay-quen.jpg', N'[Lời bài hát...]', '2019-05-20'),
(N'Cause I Love You', 5, 5, 1, 235, '/audio/cause-i-love-you.mp3', '/images/songs/cause-i-love-you.jpg', N'[Lời bài hát...]', '2020-06-10'),
(N'Thương', 5, 5, 3, 265, '/audio/thuong.mp3', '/images/songs/thuong.jpg', N'[Lời bài hát...]', '2019-11-15'),
(N'Đóa Hoa Hồng', 6, NULL, 1, 200, '/audio/doa-hoa-hong.mp3', '/images/songs/doa-hoa-hong.jpg', N'[Lời bài hát...]', '2021-07-01'),
(N'Bùa Yêu', 7, NULL, 8, 215, '/audio/bua-yeu.mp3', '/images/songs/bua-yeu.jpg', N'[Lời bài hát...]', '2017-05-20'),
(N'Ghen Cô Vy', 8, NULL, 1, 180, '/audio/ghen-co-vy.mp3', '/images/songs/ghen-co-vy.jpg', N'[Lời bài hát...]', '2020-03-01');

-- Thêm Users (mật khẩu: 123456 - clear text)
INSERT INTO Users (Username, Email, PasswordHash, FullName, Role, AvatarUrl) VALUES
('admin', 'admin@music.com', '123456', N'Quản Trị Viên', 'Admin', '/images/avatars/admin.jpg'),
('user1', 'user1@music.com', '123456', N'Nguyễn Văn A', 'User', '/images/avatars/user1.jpg'),
('user2', 'user2@music.com', '123456', N'Trần Thị B', 'User', '/images/avatars/user2.jpg');

-- Thêm Playlists
INSERT INTO Playlists (PlaylistName, UserId, Description, IsPublic, IsFeatured, CoverImageUrl) VALUES
(N'Top Hits Việt Nam', 1, N'Những bài hát hot nhất Việt Nam', 1, 1, '/images/playlists/top-hits.jpg'),
(N'Ballad Buồn', 1, N'Những bản ballad da diết', 1, 1, '/images/playlists/ballad-buon.jpg'),
(N'Chill Với Đen', 1, N'Playlist chill cùng Đen Vâu', 1, 1, '/images/playlists/chill-den.jpg'),
(N'Yêu Thích Của Tôi', 2, N'Playlist cá nhân', 0, 0, '/images/playlists/default.jpg');

-- Thêm PlaylistSongs
INSERT INTO PlaylistSongs (PlaylistId, SongId, OrderIndex) VALUES
(1, 1, 1), (1, 2, 2), (1, 3, 3), (1, 11, 4), (1, 12, 5),
(2, 3, 1), (2, 7, 2), (2, 8, 3), (2, 10, 4),
(3, 5, 1), (3, 6, 2);

-- Thêm Favorites
INSERT INTO Favorites (UserId, SongId) VALUES
(2, 1), (2, 5), (2, 6), (2, 12),
(3, 2), (3, 3), (3, 7);

-- Thêm Comments
INSERT INTO Comments (SongId, UserId, Content) VALUES
(1, 2, N'Bài hát hay quá!'),
(1, 3, N'Lạc Trôi là huyền thoại!'),
(5, 2, N'Lối Nhỏ chill phết!'),
(6, 3, N'Đen Vâu là tài năng thực sự');

-- Thêm Ratings
INSERT INTO Ratings (SongId, UserId, RatingValue) VALUES
(1, 2, 5), (1, 3, 5),
(5, 2, 5), (6, 3, 4),
(3, 2, 4), (7, 3, 5);

-- Cập nhật PlayCount
UPDATE Songs SET PlayCount = 1500000 WHERE SongId = 1;
UPDATE Songs SET PlayCount = 2000000 WHERE SongId = 2;
UPDATE Songs SET PlayCount = 1200000 WHERE SongId = 5;
UPDATE Songs SET PlayCount = 1800000 WHERE SongId = 6;

GO

PRINT N'✅ Database MusicStreamingDB đã được tạo thành công!';
PRINT N'✅ Dữ liệu mẫu đã được thêm vào!';
GO
