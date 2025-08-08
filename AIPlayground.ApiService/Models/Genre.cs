using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AIPlayground.ApiService.Models;

public class Genre
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Navigation property for the many-to-many relationship
    [JsonIgnore]
    public ICollection<Story> Stories { get; set; } = new List<Story>();
}