namespace MovieTrackerApi.Models.DTOs;

public class MovieFilterDto
{
    public int? GenreId { get; set; }
    public bool? IsWatched { get; set; }
    public bool? IsFavorite { get; set; }
    public int? MinRating { get; set; }
    public int? MinYear { get; set; }
    public string? Search { get; set; }
    public string SortBy { get; set; } = "createdAt";
    public bool Descending { get; set; } = true;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}