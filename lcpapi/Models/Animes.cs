using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace lcpapi.Models;

public class Anime
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int AnimeId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(1, ErrorMessage = "Title must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; } = "assets/images/animes";
    public string? Artwork { get; set; } = "assets/images/animes/artworks";
    public string? Studio { get; set; }
    public bool? IsFeatured { get; set; } = false;
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ScoreRating { get; set; } = 0;
    public DateTime? ReleaseDate { get; set; } = DateTime.Now;
    public IList<EAnimeGenre>? Genre { get; set; } = new List<EAnimeGenre>();
    public IList<EAnimeFormat>? Format { get; set; } = new List<EAnimeFormat>();
    public virtual ICollection<AnimesMediaInfo>? AnimesMediasInfo { get; set; } = new List<AnimesMediaInfo>();
}

public class AnimesMediaInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int MediaId { get; set; } = 1;

    public int AnimeId { get; set; } = 1;

    public string Url { get; set; } = string.Empty;
    public bool? IsFeatured { get; set; } = false;
    public string? TypeMedia { get; set; } = "screenshot";
    // public string? TypeMime { get; set; } = "image/png";
}

public class NAnimesMediaInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int MediaId { get; set; } = 1;

    public int AnimeId { get; set; } = 1;
    public Anime? Animes { get; set; }

    public int ScreenshotId { get; set; } = 1;
    public ScreenshotInfo? ScreenshotsInfo { get; set; }

    public int VideoId { get; set; } = 1;
    public VideoInfo? VideosInfo { get; set; }
}

public enum EAnimeFormat
{
    [EnumMember(Value = "TV")] TV,
    [EnumMember(Value = "Movie")] Movie,
    [EnumMember(Value = "OVA")] OVA,
    [EnumMember(Value = "ONA")] ONA,
    [EnumMember(Value = "TV Short")] TVShort,
    [EnumMember(Value = "Music")] Music
}

public enum EAnimeGenre
{
    [EnumMember(Value = "Action")] Action,
    [EnumMember(Value = "Adventure")] Adventure,
    [EnumMember(Value = "Comedy")] Comedy,
    [EnumMember(Value = "Drama")] Drama,
    [EnumMember(Value = "Ecchi")] Ecchi,
    [EnumMember(Value = "Fantasy")] Fantasy,
    [EnumMember(Value = "Hentai")] Hentai,
    [EnumMember(Value = "Horror")] Horror,
    [EnumMember(Value = "Mecha")] Mecha,
    [EnumMember(Value = "Music")] Music,
    [EnumMember(Value = "Mystery")] Mystery,
    [EnumMember(Value = "Psychological")] Psychological,
    [EnumMember(Value = "Romance")] Romance,
    [EnumMember(Value = "SciFi")] SciFi,
    [EnumMember(Value = "SliceOfLife")] SliceOfLife,
    [EnumMember(Value = "Sports")] Sports,
    [EnumMember(Value = "Supernatural")] Supernatural,
    [EnumMember(Value = "Thriller")] Thriller
}