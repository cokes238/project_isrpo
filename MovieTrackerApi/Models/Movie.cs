using System.ComponentModel.DataAnnotations;

namespace MovieTrackerApi.Models;

public class Movie
{
    public int Id { get; set; }
    
    // Внешний ключ и навигационное свойство
    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsWatched { get; set; } = false;
    public bool IsFavorite { get; set; } = false;
    
    [Required(ErrorMessage = "Название фильма обязательно")]
    [MaxLength(200, ErrorMessage = "Название не должно превышать 200 символов")]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(500, ErrorMessage = "Режиссёр не должен превышать 500 символов")]
    public string Director { get; set; } = string.Empty;
    
    [Range(1888, 2030, ErrorMessage = "Год должен быть от 1888 до 2030")]
    public int ReleaseYear { get; set; }
    
    [Range(1, 10, ErrorMessage = "Рейтинг должен быть от 1 до 10")]
    public int Rating { get; set; } = 5;
    
    [MaxLength(500, ErrorMessage = "Описание не должно превышать 500 символов")]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(500, ErrorMessage = "URL постера не должен превышать 500 символов")]
    public string PosterUrl { get; set; } = string.Empty;
}