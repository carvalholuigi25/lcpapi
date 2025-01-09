using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lcpapi.Models;

public class Post
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public DateTime? DateCreation { get; set; } = DateTime.Now;
    public PostStatus? Status { get; set; } = PostStatus.all;
    public int? UserId { get; set; }
}

public enum PostStatus {
    all = 0,
    locked = 1
}