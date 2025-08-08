using System.ComponentModel.DataAnnotations;

namespace AIPlayground.ApiService.Models;

public class Story
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Author { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public DateTime PublicationDate { get; set; }

    // Navigation property for the many-to-many relationship
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
}