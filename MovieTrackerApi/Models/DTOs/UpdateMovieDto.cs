using System.ComponentModel.DataAnnotations;

namespace MovieTrackerApi.Models.DTOs;

public class UpdateMovieDto
{
    [Required(ErrorMessage = "Название обязательно")]
    [MaxLength(200, ErrorMessage = "Максимум 200 символов")]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string Director { get; set; } = string.Empty;
    
    [Range(1888, 2030)]
    public int ReleaseYear { get; set; }
    
    [Range(1, 10)]
    public int Rating { get; set; } = 5;
    
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string PosterUrl { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Жанр обязателен")]
    public int GenreId { get; set; }
    
    public bool IsWatched { get; set; }
    
    public bool IsFavorite { get; set; }
}