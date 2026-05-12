using Microsoft.EntityFrameworkCore;
using MovieTrackerApi.Data;
using MovieTrackerApi.Models;

namespace MovieTrackerApi.Repositories;

public class GenreRepository : IGenreRepository
{
    private readonly AppDbContext _db;

    public GenreRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        return await _db.Genres
            .Include(g => g.Movies)
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await _db.Genres
            .Include(g => g.Movies)
            .FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Genre> AddAsync(Genre genre)
    {
        _db.Genres.Add(genre);
        await _db.SaveChangesAsync();
        return genre;
    }

    public async Task<Genre?> UpdateAsync(int id, Genre updated)
    {
        var existing = await _db.Genres.FindAsync(id);
        if (existing is null) return null;

        existing.Name = updated.Name;
        existing.Description = updated.Description;
        existing.Color = updated.Color;

        await _db.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var genre = await _db.Genres.Include(g => g.Movies).FirstOrDefaultAsync(g => g.Id == id);
        if (genre is null) return false;
        if (genre.Movies.Any()) return false;

        _db.Genres.Remove(genre);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(int id) => _db.Genres.AnyAsync(g => g.Id == id);

    public Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        var query = _db.Genres.Where(g => g.Name.ToLower() == name.ToLower());
        if (excludeId.HasValue) query = query.Where(g => g.Id != excludeId.Value);
        return query.AnyAsync();
    }

    public Task<int> CountMoviesAsync(int genreId) =>
        _db.Movies.CountAsync(m => m.GenreId == genreId);
}
