using Microsoft.EntityFrameworkCore;
using MovieTrackerApi.Data;
using MovieTrackerApi.Models;
using MovieTrackerApi.Models.DTOs;

namespace MovieTrackerApi.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly AppDbContext _db;

    public MovieRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(IEnumerable<Movie> Items, int Total)> GetAllAsync(MovieFilterDto filter)
    {
        IQueryable<Movie> query = _db.Movies.Include(m => m.Genre);

        if (filter.GenreId.HasValue)
            query = query.Where(m => m.GenreId == filter.GenreId.Value);

        if (filter.IsWatched.HasValue)
            query = query.Where(m => m.IsWatched == filter.IsWatched.Value);

        if (filter.IsFavorite.HasValue)
            query = query.Where(m => m.IsFavorite == filter.IsFavorite.Value);

        if (filter.MinRating.HasValue)
            query = query.Where(m => m.Rating >= filter.MinRating.Value);

        if (filter.MinYear.HasValue)
            query = query.Where(m => m.ReleaseYear >= filter.MinYear.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var s = filter.Search.ToLower();
            query = query.Where(m =>
                m.Title.ToLower().Contains(s) ||
                m.Director.ToLower().Contains(s));
        }

        query = (filter.SortBy?.ToLower()) switch
        {
            "title"       => filter.Descending ? query.OrderByDescending(m => m.Title)        : query.OrderBy(m => m.Title),
            "rating"      => filter.Descending ? query.OrderByDescending(m => m.Rating)       : query.OrderBy(m => m.Rating),
            "year"        => filter.Descending ? query.OrderByDescending(m => m.ReleaseYear)  : query.OrderBy(m => m.ReleaseYear),
            "updatedat"   => filter.Descending ? query.OrderByDescending(m => m.UpdatedAt)    : query.OrderBy(m => m.UpdatedAt),
            _             => filter.Descending ? query.OrderByDescending(m => m.CreatedAt)    : query.OrderBy(m => m.CreatedAt),
        };

        var total = await query.CountAsync();

        var page = filter.Page < 1 ? 1 : filter.Page;
        var size = filter.PageSize < 1 ? 10 : filter.PageSize;

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return (items, total);
    }

    public async Task<Movie?> GetByIdAsync(int id)
    {
        return await _db.Movies
            .Include(m => m.Genre)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Movie> AddAsync(Movie movie)
    {
        movie.CreatedAt = DateTime.UtcNow;
        movie.UpdatedAt = DateTime.UtcNow;
        _db.Movies.Add(movie);
        await _db.SaveChangesAsync();

        await _db.Entry(movie).Reference(m => m.Genre).LoadAsync();
        return movie;
    }

    public async Task<Movie?> UpdateAsync(int id, Movie updated)
    {
        var existing = await _db.Movies.Include(m => m.Genre).FirstOrDefaultAsync(m => m.Id == id);
        if (existing is null) return null;

        existing.Title = updated.Title;
        existing.Director = updated.Director;
        existing.ReleaseYear = updated.ReleaseYear;
        existing.Rating = updated.Rating;
        existing.Description = updated.Description;
        existing.PosterUrl = updated.PosterUrl;
        existing.GenreId = updated.GenreId;
        existing.IsWatched = updated.IsWatched;
        existing.IsFavorite = updated.IsFavorite;
        existing.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        await _db.Entry(existing).Reference(m => m.Genre).LoadAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var movie = await _db.Movies.FindAsync(id);
        if (movie is null) return false;

        _db.Movies.Remove(movie);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Movie?> ToggleWatchedAsync(int id)
    {
        var movie = await _db.Movies.Include(m => m.Genre).FirstOrDefaultAsync(m => m.Id == id);
        if (movie is null) return null;

        movie.IsWatched = !movie.IsWatched;
        movie.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return movie;
    }

    public async Task<Movie?> ToggleFavoriteAsync(int id)
    {
        var movie = await _db.Movies.Include(m => m.Genre).FirstOrDefaultAsync(m => m.Id == id);
        if (movie is null) return null;

        movie.IsFavorite = !movie.IsFavorite;
        movie.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return movie;
    }
}
