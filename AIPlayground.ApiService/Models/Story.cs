using System.ComponentModel.DataAnnotations;

namespace AIPlayground.ApiService.Models;

public class Story
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(200, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Author { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
    
    [Required]
    public DateTime PublicationDate { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    [Required]
    public int GenreId { get; set; }
    
    // Navigation property
    public virtual Genre Genre { get; set; } = null!;
}