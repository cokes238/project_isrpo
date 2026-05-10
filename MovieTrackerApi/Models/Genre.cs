using System.ComponentModel.DataAnnotations;

namespace MovieTrackerApi.Models;

public class Genre
{
    public int Id { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public List<Movie> Movies { get; set; } = new();
    
    [Required(ErrorMessage = "Название жанра обязательно")]
    [MaxLength(50, ErrorMessage = "Название не должно превышать 50 символов")]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(200, ErrorMessage = "Описание не должно превышать 200 символов")]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(7)]
    public string Color { get; set; } = "#667eea";
}