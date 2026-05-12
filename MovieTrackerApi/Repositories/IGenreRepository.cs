using MovieTrackerApi.Models;

namespace MovieTrackerApi.Repositories;

public interface IGenreRepository
{
    Task<IEnumerable<Genre>> GetAllAsync();
    Task<Genre?> GetByIdAsync(int id);
    Task<Genre> AddAsync(Genre genre);
    Task<Genre?> UpdateAsync(int id, Genre genre);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> NameExistsAsync(string name, int? excludeId = null);
    Task<int> CountMoviesAsync(int genreId);
}
