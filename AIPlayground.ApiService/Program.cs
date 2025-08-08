using Microsoft.EntityFrameworkCore;
using AIPlayground.ApiService.Data;
using AIPlayground.ApiService.Models;
using AIPlayground.ApiService.DTOs;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add Entity Framework with SQLite for easier testing
builder.Services.AddDbContext<StoriesDbContext>(options =>
    options.UseSqlite("Data Source=stories.db"));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<StoriesDbContext>();
    context.Database.EnsureCreated();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

// Group children's stories endpoints
var storiesGroup = app.MapGroup("/api/stories")
    .WithTags("Stories");

// 1. Get paginated list of children's stories with genres
storiesGroup.MapGet("/", async (StoriesDbContext db, int page = 1, int pageSize = 10) =>
{
    if (page < 1) page = 1;
    if (pageSize < 1 || pageSize > 100) pageSize = 10;

    var totalCount = await db.Stories.CountAsync();
    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
    
    var stories = await db.Stories
        .Include(s => s.Genre)
        .OrderByDescending(s => s.CreatedDate)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(s => new StoryResponse
        {
            Id = s.Id,
            Title = s.Title,
            Author = s.Author,
            Content = s.Content,
            PublicationDate = s.PublicationDate,
            CreatedDate = s.CreatedDate,
            Genre = new GenreResponse
            {
                Id = s.Genre.Id,
                Name = s.Genre.Name,
                Description = s.Genre.Description,
                CreatedDate = s.Genre.CreatedDate
            }
        })
        .ToListAsync();

    return Results.Ok(new PaginatedResponse<StoryResponse>
    {
        Items = stories,
        Page = page,
        PageSize = pageSize,
        TotalCount = totalCount,
        TotalPages = totalPages,
        HasNextPage = page < totalPages,
        HasPreviousPage = page > 1
    });
});

// 2. Add a new story
storiesGroup.MapPost("/", async (StoriesDbContext db, CreateStoryRequest request) =>
{
    // Validate the request
    var validationContext = new ValidationContext(request);
    var validationResults = new List<ValidationResult>();
    
    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        return Results.BadRequest(new { 
            Message = "Validation failed", 
            Errors = validationResults.Select(vr => vr.ErrorMessage) 
        });
    }

    // Check if genre exists
    var genre = await db.Genres.FindAsync(request.GenreId);
    if (genre == null)
    {
        return Results.BadRequest(new { Message = "Genre not found" });
    }

    var story = new Story
    {
        Title = request.Title,
        Author = request.Author,
        Content = request.Content,
        PublicationDate = request.PublicationDate,
        GenreId = request.GenreId,
        CreatedDate = DateTime.UtcNow
    };

    db.Stories.Add(story);
    await db.SaveChangesAsync();

    // Return the created story with genre information
    var createdStory = await db.Stories
        .Include(s => s.Genre)
        .Where(s => s.Id == story.Id)
        .Select(s => new StoryResponse
        {
            Id = s.Id,
            Title = s.Title,
            Author = s.Author,
            Content = s.Content,
            PublicationDate = s.PublicationDate,
            CreatedDate = s.CreatedDate,
            Genre = new GenreResponse
            {
                Id = s.Genre.Id,
                Name = s.Genre.Name,
                Description = s.Genre.Description,
                CreatedDate = s.Genre.CreatedDate
            }
        })
        .FirstAsync();

    return Results.Created($"/api/stories/{story.Id}", createdStory);
});

// Group genres endpoints
var genresGroup = app.MapGroup("/api/genres")
    .WithTags("Genres");

// 3. Add a new genre
genresGroup.MapPost("/", async (StoriesDbContext db, CreateGenreRequest request) =>
{
    // Validate the request
    var validationContext = new ValidationContext(request);
    var validationResults = new List<ValidationResult>();
    
    if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
    {
        return Results.BadRequest(new { 
            Message = "Validation failed", 
            Errors = validationResults.Select(vr => vr.ErrorMessage) 
        });
    }

    // Check if genre with the same name already exists
    var existingGenre = await db.Genres.FirstOrDefaultAsync(g => g.Name.ToLower() == request.Name.ToLower());
    if (existingGenre != null)
    {
        return Results.Conflict(new { Message = "A genre with this name already exists" });
    }

    var genre = new Genre
    {
        Name = request.Name,
        Description = request.Description,
        CreatedDate = DateTime.UtcNow
    };

    db.Genres.Add(genre);
    await db.SaveChangesAsync();

    var response = new GenreResponse
    {
        Id = genre.Id,
        Name = genre.Name,
        Description = genre.Description,
        CreatedDate = genre.CreatedDate
    };

    return Results.Created($"/api/genres/{genre.Id}", response);
});

// Get all genres (helper endpoint)
genresGroup.MapGet("/", async (StoriesDbContext db) =>
{
    var genres = await db.Genres
        .OrderBy(g => g.Name)
        .Select(g => new GenreResponse
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            CreatedDate = g.CreatedDate
        })
        .ToListAsync();

    return Results.Ok(genres);
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
