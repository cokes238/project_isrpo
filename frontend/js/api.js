// Если фронт открыт со встроенного хостинга бэка (wwwroot) — используем тот же origin.
// Если фронт открыт отдельно (Live Server и т.п.) — стучимся на localhost:5185 по умолчанию.
const API_BASE = (() => {
    const sameOrigin = `${window.location.origin}/api`;
    if (window.location.port === '5185' || window.location.port === '7289' || window.location.protocol === 'file:') {
        return 'http://localhost:5185/api';
    }
    return sameOrigin;
})();

async function request(path, options = {}) {
    const res = await fetch(`${API_BASE}${path}`, {
        headers: { 'Content-Type': 'application/json' },
        ...options,
    });

    let body = null;
    try { body = await res.json(); } catch (_) { /* пустой ответ */ }

    if (!res.ok || (body && body.success === false)) {
        const msg = body?.message || `Ошибка ${res.status}`;
        const errs = body?.errors?.join('; ');
        const fullMsg = errs ? `${msg}: ${errs}` : msg;
        throw new Error(fullMsg);
    }

    return body;
}

const api = {
    // ===== Genres =====
    getGenres: () => request('/Genres'),
    createGenre: (data) => request('/Genres', { method: 'POST', body: JSON.stringify(data) }),
    updateGenre: (id, data) => request(`/Genres/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
    deleteGenre: (id) => request(`/Genres/${id}`, { method: 'DELETE' }),

    // ===== Movies =====
    getMovies: (filter = {}) => {
        const params = new URLSearchParams();
        Object.entries(filter).forEach(([k, v]) => {
            if (v !== undefined && v !== null && v !== '') params.append(k, v);
        });
        const qs = params.toString();
        return request(`/Movies${qs ? `?${qs}` : ''}`);
    },
    createMovie: (data) => request('/Movies', { method: 'POST', body: JSON.stringify(data) }),
    updateMovie: (id, data) => request(`/Movies/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
    deleteMovie: (id) => request(`/Movies/${id}`, { method: 'DELETE' }),
    toggleWatched: (id) => request(`/Movies/${id}/toggle-watched`, { method: 'PATCH' }),
    toggleFavorite: (id) => request(`/Movies/${id}/toggle-favorite`, { method: 'PATCH' }),
};
