using Microsoft.AspNetCore.Mvc;
using MovieTrackerApi.Helpers;
using MovieTrackerApi.Models;
using MovieTrackerApi.Models.DTOs;
using MovieTrackerApi.Repositories;

namespace MovieTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieRepository _movies;
    private readonly IGenreRepository _genres;

    public MoviesController(IMovieRepository movies, IGenreRepository genres)
    {
        _movies = movies;
        _genres = genres;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetAll([FromQuery] MovieFilterDto filter)
    {
        var (items, total) = await _movies.GetAllAsync(filter);
        var dtos = items.Select(MapToDto).ToList();

        var payload = new
        {
            items = dtos,
            total,
            page = filter.Page,
            pageSize = filter.PageSize,
            totalPages = (int)Math.Ceiling((double)total / filter.PageSize)
        };

        return Ok(ApiResponse<object>.Ok(payload, "Список фильмов получен"));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<MovieResponseDto>>> GetById(int id)
    {
        var movie = await _movies.GetByIdAsync(id);
        if (movie is null)
            return NotFound(ApiResponse<MovieResponseDto>.Fail($"Фильм с ID {id} не найден"));

        return Ok(ApiResponse<MovieResponseDto>.Ok(MapToDto(movie)));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<MovieResponseDto>>> Create([FromBody] CreateMovieDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<MovieResponseDto>.Fail("Ошибки валидации", errors));
        }

        if (!await _genres.ExistsAsync(dto.GenreId))
            return BadRequest(ApiResponse<MovieResponseDto>.Fail($"Жанр с ID {dto.GenreId} не найден"));

        var movie = new Movie
        {
            Title = dto.Title,
            Director = dto.Director,
            ReleaseYear = dto.ReleaseYear,
            Rating = dto.Rating,
            Description = dto.Description,
            PosterUrl = dto.PosterUrl,
            GenreId = dto.GenreId
        };

        var created = await _movies.AddAsync(movie);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<MovieResponseDto>.Ok(MapToDto(created), "Фильм добавлен"));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<MovieResponseDto>>> Update(int id, [FromBody] UpdateMovieDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<MovieResponseDto>.Fail("Ошибки валидации", errors));
        }

        if (!await _genres.ExistsAsync(dto.GenreId))
            return BadRequest(ApiResponse<MovieResponseDto>.Fail($"Жанр с ID {dto.GenreId} не найден"));

        var updated = await _movies.UpdateAsync(id, new Movie
        {
            Title = dto.Title,
            Director = dto.Director,
            ReleaseYear = dto.ReleaseYear,
            Rating = dto.Rating,
            Description = dto.Description,
            PosterUrl = dto.PosterUrl,
            GenreId = dto.GenreId,
            IsWatched = dto.IsWatched,
            IsFavorite = dto.IsFavorite
        });

        if (updated is null)
            return NotFound(ApiResponse<MovieResponseDto>.Fail($"Фильм с ID {id} не найден"));

        return Ok(ApiResponse<MovieResponseDto>.Ok(MapToDto(updated), "Фильм обновлён"));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _movies.DeleteAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<object>.Fail($"Фильм с ID {id} не найден"));

        return Ok(ApiResponse<object>.Ok(new { id }, "Фильм удалён"));
    }

    [HttpPatch("{id:int}/toggle-watched")]
    public async Task<ActionResult<ApiResponse<MovieResponseDto>>> ToggleWatched(int id)
    {
        var movie = await _movies.ToggleWatchedAsync(id);
        if (movie is null)
            return NotFound(ApiResponse<MovieResponseDto>.Fail($"Фильм с ID {id} не найден"));

        return Ok(ApiResponse<MovieResponseDto>.Ok(MapToDto(movie),
            movie.IsWatched ? "Отмечен как просмотренный" : "Отметка о просмотре снята"));
    }

    [HttpPatch("{id:int}/toggle-favorite")]
    public async Task<ActionResult<ApiResponse<MovieResponseDto>>> ToggleFavorite(int id)
    {
        var movie = await _movies.ToggleFavoriteAsync(id);
        if (movie is null)
            return NotFound(ApiResponse<MovieResponseDto>.Fail($"Фильм с ID {id} не найден"));

        return Ok(ApiResponse<MovieResponseDto>.Ok(MapToDto(movie),
            movie.IsFavorite ? "Добавлен в избранное" : "Убран из избранного"));
    }

    private static MovieResponseDto MapToDto(Movie m) => new()
    {
        Id = m.Id,
        Title = m.Title,
        Director = m.Director,
        ReleaseYear = m.ReleaseYear,
        Rating = m.Rating,
        Description = m.Description,
        PosterUrl = m.PosterUrl,
        IsWatched = m.IsWatched,
        IsFavorite = m.IsFavorite,
        CreatedAt = m.CreatedAt,
        UpdatedAt = m.UpdatedAt,
        GenreId = m.GenreId,
        GenreName = m.Genre?.Name ?? string.Empty,
        GenreColor = m.Genre?.Color ?? string.Empty
    };
}
