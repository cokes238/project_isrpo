using Microsoft.AspNetCore.Mvc;
using MovieTrackerApi.Helpers;
using MovieTrackerApi.Models;
using MovieTrackerApi.Models.DTOs;
using MovieTrackerApi.Repositories;

namespace MovieTrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenreRepository _repo;

    public GenresController(IGenreRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<GenreResponseDto>>>> GetAll()
    {
        var genres = await _repo.GetAllAsync();
        var dtos = genres.Select(MapToDto);
        return Ok(ApiResponse<IEnumerable<GenreResponseDto>>.Ok(dtos, "Список жанров получен"));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponse<GenreResponseDto>>> GetById(int id)
    {
        var genre = await _repo.GetByIdAsync(id);
        if (genre is null)
            return NotFound(ApiResponse<GenreResponseDto>.Fail($"Жанр с ID {id} не найден"));

        return Ok(ApiResponse<GenreResponseDto>.Ok(MapToDto(genre)));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<GenreResponseDto>>> Create([FromBody] CreateGenreDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<GenreResponseDto>.Fail("Ошибки валидации", errors));
        }

        if (await _repo.NameExistsAsync(dto.Name))
            return Conflict(ApiResponse<GenreResponseDto>.Fail($"Жанр с именем '{dto.Name}' уже существует"));

        var genre = new Genre
        {
            Name = dto.Name,
            Description = dto.Description,
            Color = dto.Color
        };

        var created = await _repo.AddAsync(genre);
        return CreatedAtAction(nameof(GetById), new { id = created.Id },
            ApiResponse<GenreResponseDto>.Ok(MapToDto(created), "Жанр создан"));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<GenreResponseDto>>> Update(int id, [FromBody] UpdateGenreDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<GenreResponseDto>.Fail("Ошибки валидации", errors));
        }

        if (!await _repo.ExistsAsync(id))
            return NotFound(ApiResponse<GenreResponseDto>.Fail($"Жанр с ID {id} не найден"));

        if (await _repo.NameExistsAsync(dto.Name, id))
            return Conflict(ApiResponse<GenreResponseDto>.Fail($"Другой жанр с именем '{dto.Name}' уже есть"));

        var updated = await _repo.UpdateAsync(id, new Genre
        {
            Name = dto.Name,
            Description = dto.Description,
            Color = dto.Color
        });

        return Ok(ApiResponse<GenreResponseDto>.Ok(MapToDto(updated!), "Жанр обновлён"));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        if (!await _repo.ExistsAsync(id))
            return NotFound(ApiResponse<object>.Fail($"Жанр с ID {id} не найден"));

        var moviesCount = await _repo.CountMoviesAsync(id);
        if (moviesCount > 0)
            return BadRequest(ApiResponse<object>.Fail($"Нельзя удалить жанр: к нему привязано {moviesCount} фильм(ов)"));

        await _repo.DeleteAsync(id);
        return Ok(ApiResponse<object>.Ok(new { id }, "Жанр удалён"));
    }

    private static GenreResponseDto MapToDto(Genre g) => new()
    {
        Id = g.Id,
        Name = g.Name,
        Description = g.Description,
        Color = g.Color,
        CreatedAt = g.CreatedAt,
        MoviesCount = g.Movies?.Count ?? 0
    };
}
