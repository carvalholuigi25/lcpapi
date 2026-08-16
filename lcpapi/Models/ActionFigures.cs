using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace lcpapi.Models;

public class ActionFigure
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int ActionFigureId { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MinLength(1, ErrorMessage = "Name must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public string? Manufacturer { get; set; }
    public bool? IsFeatured { get; set; } = false;
    public decimal? Price { get; set; } = 0.0M;
    public DateTime? ReleaseDate { get; set; } = DateTime.Now;
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ScoreRating { get; set; } = 0;
    public IList<EActionFigureCategory>? Category { get; set; } = new List<EActionFigureCategory>();
    public virtual ICollection<ActionFiguresMediaInfo>? ActionFiguresMediasInfo { get; set; } = new List<ActionFiguresMediaInfo>();
}

public class ActionFiguresMediaInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int MediaId { get; set; } = 1;

    public int ActionFigureId { get; set; } = 1;

    public string Url { get; set; } = string.Empty;
    public bool? IsFeatured { get; set; } = false;
    public string? TypeMedia { get; set; } = "image";
    // public string? TypeMime { get; set; } = "image/png";
}

public enum EActionFigureCategory
{
    [EnumMember(Value = "Anime")]
    Anime,
    [EnumMember(Value = "Comic")]
    Comic,
    [EnumMember(Value = "Movie")]
    Movie,
    [EnumMember(Value = "Video Game")]
    VideoGame,
    [EnumMember(Value = "TV Series")]
    TVSeries,
    [EnumMember(Value = "Other")]
    Other
}