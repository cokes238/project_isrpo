namespace MovieTrackerApi.Models.DTOs;

public class GenreResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int MoviesCount { get; set; }
}