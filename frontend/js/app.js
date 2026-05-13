// ====== Состояние ======
const state = {
    genres: [],
    editingMovieId: null,
};

// ====== DOM helpers ======
const $ = (id) => document.getElementById(id);

function showToast(message, type = 'info') {
    const toast = $('toast');
    toast.textContent = message;
    toast.className = `toast toast--${type}`;
    setTimeout(() => toast.classList.add('hidden'), 50);
    toast.classList.remove('hidden');
    setTimeout(() => toast.classList.add('hidden'), 3500);
}

function escapeHtml(str) {
    return String(str ?? '')
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');
}

// ====== Загрузка жанров ======
async function loadGenres() {
    try {
        const res = await api.getGenres();
        state.genres = res.data || [];
        renderGenres();
        fillGenreSelects();
    } catch (e) {
        showToast(`Не удалось загрузить жанры: ${e.message}`, 'error');
    }
}

function renderGenres() {
    const list = $('genreList');
    list.innerHTML = state.genres.map(g => `
        <li style="background:${escapeHtml(g.color)}">
            ${escapeHtml(g.name)} <small>(${g.moviesCount})</small>
            <button class="chip-del" title="Удалить" data-id="${g.id}">×</button>
        </li>
    `).join('');

    list.querySelectorAll('.chip-del').forEach(btn => {
        btn.addEventListener('click', () => deleteGenre(Number(btn.dataset.id)));
    });
}

function fillGenreSelects() {
    const opts = state.genres.map(g => `<option value="${g.id}">${escapeHtml(g.name)}</option>`).join('');
    $('genreId').innerHTML = opts;
    $('filterGenre').innerHTML = '<option value="">Все</option>' + opts;
}

async function deleteGenre(id) {
    if (!confirm('Удалить этот жанр?')) return;
    try {
        await api.deleteGenre(id);
        showToast('Жанр удалён', 'success');
        await loadGenres();
        await loadMovies();
    } catch (e) {
        showToast(e.message, 'error');
    }
}

// ====== Форма жанра ======
$('genreForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    try {
        await api.createGenre({
            name: $('genreName').value.trim(),
            description: $('genreDescription').value.trim(),
            color: $('genreColor').value,
        });
        showToast('Жанр добавлен', 'success');
        $('genreForm').reset();
        $('genreColor').value = '#667eea';
        await loadGenres();
    } catch (err) {
        showToast(err.message, 'error');
    }
});

// ====== Загрузка фильмов ======
async function loadMovies() {
    const filter = {
        search: $('filterSearch').value.trim(),
        genreId: $('filterGenre').value,
        isWatched: $('filterWatched').value,
        isFavorite: $('filterFav').value,
        minRating: $('filterRating').value,
        sortBy: $('sortBy').value,
        descending: $('sortDir').value,
        pageSize: 100,
    };
    try {
        const res = await api.getMovies(filter);
        const items = res.data?.items || [];
        renderMovies(items);
        updateStats(items);
    } catch (e) {
        showToast(`Не удалось загрузить фильмы: ${e.message}`, 'error');
    }
}

function renderMovies(items) {
    const list = $('movieList');
    const empty = $('emptyMsg');

    if (!items.length) {
        list.innerHTML = '';
        empty.classList.remove('hidden');
        return;
    }
    empty.classList.add('hidden');

    list.innerHTML = items.map(m => {
        const poster = m.posterUrl
            ? `<div class="movie-card__poster" style="background-image:url('${escapeHtml(m.posterUrl)}')"></div>`
            : `<div class="movie-card__poster">🎬</div>`;

        const badges = `
            ${m.isWatched ? '<span class="badge badge--watched">Просмотрено</span>' : ''}
            ${m.isFavorite ? '<span class="badge badge--fav">★ Избранное</span>' : ''}
        `;

        return `
            <div class="movie-card" data-id="${m.id}">
                ${poster}
                <div class="movie-card__body">
                    <h3 class="movie-card__title">${escapeHtml(m.title)}</h3>
                    <div class="movie-card__meta">
                        <span class="genre-chip" style="background:${escapeHtml(m.genreColor)}">${escapeHtml(m.genreName)}</span>
                        <span>${escapeHtml(m.releaseYear)}</span>
                        <span class="movie-card__rating">★ ${m.rating}/10</span>
                    </div>
                    <div class="movie-card__meta">${badges}</div>
                    ${m.director ? `<div class="movie-card__meta">Реж.: ${escapeHtml(m.director)}</div>` : ''}
                    ${m.description ? `<p class="movie-card__desc">${escapeHtml(m.description)}</p>` : ''}
                    <div class="movie-card__actions">
                        <button class="btn btn--small" data-act="watch" data-id="${m.id}">${m.isWatched ? 'Не просмотрен' : 'Просмотрен'}</button>
                        <button class="btn btn--small" data-act="fav" data-id="${m.id}">${m.isFavorite ? '★' : '☆'}</button>
                        <button class="btn btn--small" data-act="edit" data-id="${m.id}">Изм.</button>
                        <button class="btn btn--small btn--danger" data-act="del" data-id="${m.id}">×</button>
                    </div>
                </div>
            </div>
        `;
    }).join('');

    list.querySelectorAll('button[data-act]').forEach(btn => {
        btn.addEventListener('click', () => handleMovieAction(btn.dataset.act, Number(btn.dataset.id)));
    });
}

function updateStats(items) {
    $('statTotal').textContent = items.length;
    $('statWatched').textContent = items.filter(m => m.isWatched).length;
    $('statFav').textContent = items.filter(m => m.isFavorite).length;
}

async function handleMovieAction(action, id) {
    try {
        switch (action) {
            case 'watch':
                await api.toggleWatched(id);
                showToast('Статус просмотра обновлён', 'success');
                break;
            case 'fav':
                await api.toggleFavorite(id);
                showToast('Избранное обновлено', 'success');
                break;
            case 'edit':
                await fillFormForEdit(id);
                return;
            case 'del':
                if (!confirm('Удалить фильм?')) return;
                await api.deleteMovie(id);
                showToast('Фильм удалён', 'success');
                break;
        }
        await loadMovies();
        await loadGenres();
    } catch (e) {
        showToast(e.message, 'error');
    }
}

async function fillFormForEdit(id) {
    const list = await api.getMovies({ pageSize: 1000 });
    const movie = list.data?.items?.find(m => m.id === id);
    if (!movie) return;

    state.editingMovieId = id;
    $('movieId').value = id;
    $('title').value = movie.title;
    $('director').value = movie.director;
    $('releaseYear').value = movie.releaseYear;
    $('rating').value = movie.rating;
    $('description').value = movie.description;
    $('posterUrl').value = movie.posterUrl;
    $('genreId').value = movie.genreId;
    $('submitBtn').textContent = 'Сохранить изменения';

    window.scrollTo({ top: 0, behavior: 'smooth' });
}

function resetForm() {
    state.editingMovieId = null;
    $('movieForm').reset();
    $('movieId').value = '';
    $('rating').value = 5;
    $('releaseYear').value = 2024;
    $('submitBtn').textContent = 'Добавить';
}

// ====== Форма фильма ======
$('movieForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const payload = {
        title: $('title').value.trim(),
        director: $('director').value.trim(),
        releaseYear: Number($('releaseYear').value),
        rating: Number($('rating').value),
        description: $('description').value.trim(),
        posterUrl: $('posterUrl').value.trim(),
        genreId: Number($('genreId').value),
    };

    try {
        if (state.editingMovieId) {
            const current = (await api.getMovies({ pageSize: 1000 })).data.items
                .find(m => m.id === state.editingMovieId);
            await api.updateMovie(state.editingMovieId, {
                ...payload,
                isWatched: current?.isWatched ?? false,
                isFavorite: current?.isFavorite ?? false,
            });
            showToast('Фильм обновлён', 'success');
        } else {
            await api.createMovie(payload);
            showToast('Фильм добавлен', 'success');
        }
        resetForm();
        await loadMovies();
        await loadGenres();
    } catch (err) {
        showToast(err.message, 'error');
    }
});

$('resetBtn').addEventListener('click', resetForm);

// ====== Фильтры ======
$('applyFilters').addEventListener('click', loadMovies);
$('clearFilters').addEventListener('click', () => {
    $('filterSearch').value = '';
    $('filterGenre').value = '';
    $('filterWatched').value = '';
    $('filterFav').value = '';
    $('filterRating').value = '';
    $('sortBy').value = 'createdAt';
    $('sortDir').value = 'true';
    loadMovies();
});

// Поиск по Enter
$('filterSearch').addEventListener('keydown', (e) => {
    if (e.key === 'Enter') loadMovies();
});

// ====== Старт ======
(async function init() {
    await loadGenres();
    await loadMovies();
})();
