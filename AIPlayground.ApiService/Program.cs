using AIPlayground.ApiService.Data;
using AIPlayground.ApiService.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add Entity Framework
builder.Services.AddDbContext<StoriesDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? "Data Source=stories.db"));

var app = builder.Build();

// Apply migrations automatically in development environment
if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("Running in Development environment - applying migrations automatically");
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<StoriesDbContext>();
        try
        {
            dbContext.Database.Migrate();
            // Seed sample data
            await dbContext.SeedSampleStoriesAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating or seeding the database in development environment.");
        }
    }
}
else
{
    app.Logger.LogInformation("Running in {Environment} environment - skipping automatic migrations", app.Environment.EnvironmentName);
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

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

// Stories API endpoints
app.MapGet("/api/stories", async (StoriesDbContext db) =>
{
    return await db.Stories.Include(s => s.Genres).ToListAsync();
});

app.MapGet("/api/stories/{id}", async (int id, StoriesDbContext db) =>
{
    return await db.Stories.Include(s => s.Genres).FirstOrDefaultAsync(s => s.Id == id);
});

app.MapGet("/api/genres", async (StoriesDbContext db) =>
{
    return await db.Genres.ToListAsync();
});

app.MapDefaultEndpoints();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
