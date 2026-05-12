using MovieTrackerApi.Models;
using MovieTrackerApi.Models.DTOs;

namespace MovieTrackerApi.Repositories;

public interface IMovieRepository
{
    Task<(IEnumerable<Movie> Items, int Total)> GetAllAsync(MovieFilterDto filter);
    Task<Movie?> GetByIdAsync(int id);
    Task<Movie> AddAsync(Movie movie);
    Task<Movie?> UpdateAsync(int id, Movie movie);
    Task<bool> DeleteAsync(int id);
    Task<Movie?> ToggleWatchedAsync(int id);
    Task<Movie?> ToggleFavoriteAsync(int id);
}
