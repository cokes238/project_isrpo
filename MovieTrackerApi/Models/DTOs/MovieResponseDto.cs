namespace MovieTrackerApi.Models.DTOs;

public class MovieResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Director { get; set; } = string.Empty;
    public int ReleaseYear { get; set; }
    public int Rating { get; set; }
    public string Description { get; set; } = string.Empty;
    public string PosterUrl { get; set; } = string.Empty;
    public bool IsWatched { get; set; }
    public bool IsFavorite { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int GenreId { get; set; }
    public string GenreName { get; set; } = string.Empty;
    public string GenreColor { get; set; } = string.Empty;
}