using System.ComponentModel.DataAnnotations;

namespace AIPlayground.ApiService.Models;

public class Genre
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public virtual ICollection<Story> Stories { get; set; } = new List<Story>();
}