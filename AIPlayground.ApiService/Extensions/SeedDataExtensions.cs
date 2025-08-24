using AIPlayground.ApiService.Data;
using AIPlayground.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace AIPlayground.ApiService.Extensions;

public static class SeedDataExtensions
{
    public static async Task SeedSampleStoriesAsync(this StoriesDbContext context)
    {
        if (await context.Stories.AnyAsync())
        {
            return; // Already seeded
        }

        var adventureGenre = await context.Genres.FirstAsync(g => g.Name == "Adventure");
        var fantasyGenre = await context.Genres.FirstAsync(g => g.Name == "Fantasy");
        var fairyTaleGenre = await context.Genres.FirstAsync(g => g.Name == "Fairy Tale");

        var stories = new List<Story>
        {
            new Story
            {
                Title = "The Brave Little Mouse",
                Author = "Jane Doe",
                Content = "Once upon a time, in a small village, there lived a brave little mouse who dreamed of great adventures...",
                PublicationDate = new DateTime(2023, 1, 15),
                Genres = new List<Genre> { adventureGenre, fairyTaleGenre }
            },
            new Story
            {
                Title = "The Magic Forest",
                Author = "John Smith",
                Content = "Deep in the enchanted forest, magical creatures lived in harmony until one day...",
                PublicationDate = new DateTime(2023, 3, 22),
                Genres = new List<Genre> { fantasyGenre, fairyTaleGenre }
            },
            new Story
            {
                Title = "Captain Luna's Space Adventure",
                Author = "Sarah Johnson",
                Content = "Captain Luna and her crew discovered a mysterious planet filled with friendly aliens...",
                PublicationDate = new DateTime(2023, 6, 10),
                Genres = new List<Genre> { adventureGenre, fantasyGenre }
            }
        };

        context.Stories.AddRange(stories);
        await context.SaveChangesAsync();
    }
}