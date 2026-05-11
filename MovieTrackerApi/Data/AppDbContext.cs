using Microsoft.EntityFrameworkCore;
using MovieTrackerApi.Models;

namespace MovieTrackerApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Genre> Genres => Set<Genre>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Movie>()
            .HasOne(m => m.Genre)
            .WithMany(g => g.Movies)
            .HasForeignKey(m => m.GenreId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Genre>()
            .HasIndex(g => g.Name)
            .IsUnique();

        var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Боевик",     Description = "Экшен, погони, перестрелки", Color = "#ef4444", CreatedAt = seedDate },
            new Genre { Id = 2, Name = "Драма",      Description = "Серьёзные истории о людях",   Color = "#8b5cf6", CreatedAt = seedDate },
            new Genre { Id = 3, Name = "Комедия",    Description = "Юмор и хорошее настроение",   Color = "#f59e0b", CreatedAt = seedDate },
            new Genre { Id = 4, Name = "Фантастика", Description = "Будущее и невероятные миры",  Color = "#3b82f6", CreatedAt = seedDate },
            new Genre { Id = 5, Name = "Ужасы",      Description = "Страшно и атмосферно",        Color = "#1f2937", CreatedAt = seedDate }
        );
    }
}
