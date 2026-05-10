using System.ComponentModel.DataAnnotations;

namespace MovieTrackerApi.Models.DTOs;

public class CreateGenreDto
{
    [Required(ErrorMessage = "Название обязательно")]
    [MaxLength(50, ErrorMessage = "Максимум 50 символов")]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    
    [MaxLength(7)]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Цвет должен быть в формате HEX: #RRGGBB")]
    public string Color { get; set; } = "#667eea";
}