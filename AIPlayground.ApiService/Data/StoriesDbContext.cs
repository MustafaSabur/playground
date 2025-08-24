using AIPlayground.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace AIPlayground.ApiService.Data;

public class StoriesDbContext : DbContext
{
    public StoriesDbContext(DbContextOptions<StoriesDbContext> options) : base(options)
    {
    }

    public DbSet<Story> Stories { get; set; }
    public DbSet<Genre> Genres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure the many-to-many relationship between Story and Genre
        modelBuilder.Entity<Story>()
            .HasMany(s => s.Genres)
            .WithMany(g => g.Stories)
            .UsingEntity<Dictionary<string, object>>(
                "StoryGenre",
                j => j.HasOne<Genre>().WithMany().HasForeignKey("GenreId"),
                j => j.HasOne<Story>().WithMany().HasForeignKey("StoryId"),
                j =>
                {
                    j.HasKey("StoryId", "GenreId");
                    j.ToTable("StoryGenres");
                });

        // Configure table names
        modelBuilder.Entity<Story>().ToTable("Stories");
        modelBuilder.Entity<Genre>().ToTable("Genres");

        // Seed some initial data
        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Adventure" },
            new Genre { Id = 2, Name = "Fantasy" },
            new Genre { Id = 3, Name = "Educational" },
            new Genre { Id = 4, Name = "Fairy Tale" },
            new Genre { Id = 5, Name = "Mystery" }
        );
    }
}