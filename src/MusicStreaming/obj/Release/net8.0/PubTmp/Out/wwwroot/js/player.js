// Music Player JavaScript
let currentSong = null;
let isPlaying = false;
let isShuffle = false;
let repeatMode = 0; // 0: no repeat, 1: repeat all, 2: repeat one
let playlist = [];
let currentIndex = 0;

const audioPlayer = document.getElementById('audioPlayer');
const musicPlayer = document.getElementById('musicPlayer');
const btnPlayPause = document.getElementById('btnPlayPause');
const btnPrevious = document.getElementById('btnPrevious');
const btnNext = document.getElementById('btnNext');
const btnShuffle = document.getElementById('btnShuffle');
const btnRepeat = document.getElementById('btnRepeat');
const progressBar = document.getElementById('progressBar');
const volumeBar = document.getElementById('volumeBar');
const currentTimeEl = document.getElementById('currentTime');
const durationEl = document.getElementById('duration');
const btnClosePlayer = document.getElementById('btnClosePlayer');
const btnRestorePlayer = document.getElementById('btnRestorePlayer');

// Close (Minimize) Player
if (btnClosePlayer) {
    btnClosePlayer.addEventListener('click', function () {
        musicPlayer.classList.remove('d-flex');
        musicPlayer.style.display = 'none';
        if (btnRestorePlayer) btnRestorePlayer.style.display = 'flex';
    });
}

// Restore Player
if (btnRestorePlayer) {
    btnRestorePlayer.addEventListener('click', function () {
        musicPlayer.classList.add('d-flex');
        musicPlayer.style.display = 'flex';
        btnRestorePlayer.style.display = 'none';
    });
}

// Play song function
function playSong(songId, songName, artistName, audioUrl, coverUrl) {
    if (btnRestorePlayer) btnRestorePlayer.style.display = 'none';

    currentSong = {
        id: songId,
        name: songName,
        artist: artistName,
        audio: audioUrl,
        cover: coverUrl
    };

    // Update player UI
    const songNameEl = document.getElementById('playerSongName');
    const artistNameEl = document.getElementById('playerArtistName');
    const coverEl = document.getElementById('playerCover');

    if (songNameEl) songNameEl.textContent = songName;
    if (artistNameEl) artistNameEl.textContent = artistName;
    if (coverEl) coverEl.src = coverUrl || '/images/default-cover.jpg';

    // Load and play audio
    if (audioPlayer) {
        audioPlayer.src = audioUrl;
        audioPlayer.play().catch(e => console.error("Error playing audio:", e));
    }

    isPlaying = true;

    // Show player
    if (musicPlayer) {
        musicPlayer.classList.add('d-flex');
        musicPlayer.style.display = 'flex';
    }

    // Update play button
    if (btnPlayPause) btnPlayPause.innerHTML = '<i class="fas fa-pause"></i>';

    // Record play count
    // fetch(`/Songs/Play/${songId}`, { method: 'POST' });
}

// Play/Pause toggle
if (btnPlayPause) {
    btnPlayPause.addEventListener('click', function () {
        if (isPlaying) {
            audioPlayer.pause();
            btnPlayPause.innerHTML = '<i class="fas fa-play"></i>';
            isPlaying = false;
        } else {
            audioPlayer.play();
            btnPlayPause.innerHTML = '<i class="fas fa-pause"></i>';
            isPlaying = true;
        }
    });
}

// Update progress bar
if (audioPlayer) {
    audioPlayer.addEventListener('timeupdate', function () {
        if (!isNaN(audioPlayer.duration)) {
            const progress = (audioPlayer.currentTime / audioPlayer.duration) * 100;
            progressBar.value = progress || 0;
            currentTimeEl.textContent = formatTime(audioPlayer.currentTime);
        }
    });

    audioPlayer.addEventListener('loadedmetadata', function () {
        durationEl.textContent = formatTime(audioPlayer.duration);
    });

    audioPlayer.addEventListener('ended', function () {
        if (repeatMode === 2) {
            // Repeat one
            audioPlayer.currentTime = 0;
            audioPlayer.play();
        } else if (repeatMode === 1 || currentIndex < playlist.length - 1) {
            // Repeat all or has next song
            playNext();
        } else {
            // Stop
            isPlaying = false;
            btnPlayPause.innerHTML = '<i class="fas fa-play"></i>';
        }
    });
}

// Seek in progress bar
if (progressBar) {
    progressBar.addEventListener('input', function () {
        const seekTime = (progressBar.value / 100) * audioPlayer.duration;
        audioPlayer.currentTime = seekTime;
    });
}

// Volume control
if (volumeBar) {
    volumeBar.addEventListener('input', function () {
        audioPlayer.volume = volumeBar.value / 100;
    });
    audioPlayer.volume = 0.7;
}

// Shuffle toggle
if (btnShuffle) {
    btnShuffle.addEventListener('click', function () {
        isShuffle = !isShuffle;
        btnShuffle.style.color = isShuffle ? 'var(--primary-color)' : 'var(--text-secondary)';
    });
}

// Repeat toggle
if (btnRepeat) {
    btnRepeat.addEventListener('click', function () {
        repeatMode = (repeatMode + 1) % 3;
        if (repeatMode === 0) {
            btnRepeat.innerHTML = '<i class="fas fa-redo"></i>';
            btnRepeat.style.color = 'var(--text-secondary)';
        } else if (repeatMode === 1) {
            btnRepeat.innerHTML = '<i class="fas fa-redo"></i>';
            btnRepeat.style.color = 'var(--primary-color)';
        } else {
            btnRepeat.innerHTML = '<i class="fas fa-redo"></i> <span style="font-size: 0.7rem;">1</span>';
            btnRepeat.style.color = 'var(--primary-color)';
        }
    });
}

// Previous/Next
if (btnPrevious) btnPrevious.addEventListener('click', playPrevious);
if (btnNext) btnNext.addEventListener('click', playNext);

function playNext() {
    if (playlist.length === 0) return;
    if (isShuffle) {
        currentIndex = Math.floor(Math.random() * playlist.length);
    } else {
        currentIndex = (currentIndex + 1) % playlist.length;
    }
    const song = playlist[currentIndex];
    playSong(song.id, song.name, song.artist, song.audio, song.cover);
}

function playPrevious() {
    if (playlist.length === 0) return;
    if (isShuffle) {
        currentIndex = Math.floor(Math.random() * playlist.length);
    } else {
        currentIndex = (currentIndex - 1 + playlist.length) % playlist.length;
    }
    const song = playlist[currentIndex];
    playSong(song.id, song.name, song.artist, song.audio, song.cover);
}

// Format time helper
function formatTime(seconds) {
    if (isNaN(seconds)) return '0:00';
    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins}:${secs.toString().padStart(2, '0')}`;
}

// Keyboard shortcuts
document.addEventListener('keydown', function (e) {
    if (e.code === 'Space' && e.target.tagName !== 'INPUT' && e.target.tagName !== 'TEXTAREA') {
        e.preventDefault();
        if (btnPlayPause) btnPlayPause.click();
    }
});

// --- API Functions (Updated) ---

// Add to favorites
function toggleFavorite(songId, button) {
    const formData = new FormData();
    formData.append('songId', songId);

    fetch('/Songs/ToggleFavorite', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const icon = button.querySelector('i');
                if (data.isFavorite) {
                    icon.classList.remove('far');
                    icon.classList.add('fas');
                    icon.style.color = 'var(--primary-color)';
                } else {
                    icon.classList.remove('fas');
                    icon.classList.add('far');
                    icon.style.color = '';
                }
            } else {
                alert(data.message || 'Có lỗi xảy ra');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Có lỗi xảy ra khi thêm vào yêu thích');
        });
}

// Add rating
function addRating(songId, score) {
    const formData = new FormData();
    formData.append('songId', songId);
    formData.append('score', score);

    fetch('/Songs/Rate', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Update rating display
                const avgEl = document.getElementById('averageRating');
                const totalEl = document.getElementById('totalRatings');
                if (avgEl) avgEl.textContent = data.average.toFixed(1);
                if (totalEl) totalEl.textContent = data.total;

                // Update stars
                updateStars(score);
                alert('Đánh giá thành công!');
            } else {
                alert(data.message || 'Có lỗi xảy ra');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Có lỗi xảy ra khi đánh giá');
        });
}

function updateStars(rating) {
    for (let i = 1; i <= 5; i++) {
        const star = document.getElementById(`star${i}`);
        if (star) {
            if (i <= rating) {
                star.classList.remove('far');
                star.classList.add('fas');
                star.style.color = '#ffc107';
            } else {
                star.classList.remove('fas');
                star.classList.add('far');
                star.style.color = 'var(--text-secondary)';
            }
        }
    }
}

// Add comment
function addComment(songId) {
    const contentInput = document.getElementById('commentContent');
    const content = contentInput.value;

    if (!content.trim()) {
        alert('Vui lòng nhập nội dung bình luận');
        return;
    }

    const formData = new FormData();
    formData.append('songId', songId);
    formData.append('content', content);

    fetch('/Songs/Comment', {
        method: 'POST',
        body: formData
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Add comment to list
                const commentsList = document.getElementById('commentsList');

                // Remove "No comments" message if exists
                const noCommentsMsg = commentsList.querySelector('p.text-center');
                if (noCommentsMsg) noCommentsMsg.remove();

                const commentHtml = `
                <div class="comment-item mb-3 p-3" style="background: rgba(0,0,0,0.03); border-radius: 10px;">
                    <div class="d-flex align-items-start">
                        <div class="comment-avatar me-3">
                            <img src="${data.avatarUrl || '/images/avatars/default.png'}" class="rounded-circle" width="40" height="40" style="object-fit: cover;">
                        </div>
                        <div class="flex-grow-1">
                            <div class="comment-header mb-2">
                                <strong>${data.username}</strong>
                                <span class="text-muted ms-2">${data.createdDate}</span>
                            </div>
                            <div class="comment-content text-dark">${data.content}</div>
                        </div>
                    </div>
                </div>
            `;
                commentsList.insertAdjacentHTML('afterbegin', commentHtml);
                contentInput.value = '';
            } else {
                alert(data.message || 'Có lỗi xảy ra');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            alert('Có lỗi xảy ra khi thêm bình luận');
        });
}

// Make functions global
window.playSong = playSong;
window.toggleFavorite = toggleFavorite;
window.addRating = addRating;
window.addComment = addComment;
