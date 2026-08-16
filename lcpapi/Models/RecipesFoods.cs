using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace lcpapi.Models;

public class RecipesFoods
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int RecipesFoodsId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(1, ErrorMessage = "Title must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public bool? IsFeatured { get; set; } = false;
    public virtual ICollection<RecipesFoodsMediaInfo>? RecipesFoodsMediasInfo { get; set; }
}

public class RecipesFoodsMediaInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int RecipesFoodsMediaInfoId { get; set; }

    [ForeignKey("RecipesFoods")]
    public int RecipesFoodsId { get; set; }
    public string? MediaType { get; set; }
    public string? Link { get; set; }

    [JsonIgnore]
    public virtual RecipesFoods? RecipesFoods { get; set; }
}