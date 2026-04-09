using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace dmb_backend.Models;

[Table("news_items")]
public class NewsItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(140)]
    public string Title { get; set; } = "";

    [Required]
    [Column("Content")]
    [MaxLength(4000)]
    public string Text { get; set; } = "";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public string? CreatedByUserId { get; set; }
}