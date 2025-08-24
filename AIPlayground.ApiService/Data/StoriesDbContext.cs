using Microsoft.EntityFrameworkCore;
using AIPlayground.ApiService.Models;

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

        // Configure Genre entity
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedDate).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Story entity
        modelBuilder.Entity<Story>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Author).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(10000);
            entity.Property(e => e.PublicationDate).IsRequired();
            entity.Property(e => e.CreatedDate).IsRequired();

            // Configure the relationship
            entity.HasOne(e => e.Genre)
                  .WithMany(g => g.Stories)
                  .HasForeignKey(e => e.GenreId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed some initial data
        modelBuilder.Entity<Genre>().HasData(
            new Genre { Id = 1, Name = "Adventure", Description = "Exciting stories of exploration and discovery", CreatedDate = DateTime.UtcNow },
            new Genre { Id = 2, Name = "Fantasy", Description = "Magical tales with mythical creatures", CreatedDate = DateTime.UtcNow },
            new Genre { Id = 3, Name = "Educational", Description = "Stories that teach important lessons", CreatedDate = DateTime.UtcNow }
        );

        modelBuilder.Entity<Story>().HasData(
            new Story 
            { 
                Id = 1, 
                Title = "The Little Explorer", 
                Author = "Jane Smith", 
                Content = "Once upon a time, there was a brave little mouse who loved to explore...", 
                PublicationDate = new DateTime(2023, 1, 15),
                GenreId = 1,
                CreatedDate = DateTime.UtcNow
            },
            new Story 
            { 
                Id = 2, 
                Title = "The Magic Garden", 
                Author = "Bob Jones", 
                Content = "In a secret garden hidden behind ivy walls, magical flowers bloomed...", 
                PublicationDate = new DateTime(2023, 3, 22),
                GenreId = 2,
                CreatedDate = DateTime.UtcNow
            }
        );
    }
}