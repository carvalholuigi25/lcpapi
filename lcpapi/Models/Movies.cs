using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace lcpapi.Models;

public class Movies
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int MovieId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(1, ErrorMessage = "Title must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; } = "assets/images/movies";
    public string? Artwork { get; set; } = "assets/images/movies/artworks";
    public string? Studio { get; set; }
    public bool? IsFeatured { get; set; } = false;
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ScoreRating { get; set; } = 0;
    public DateTime? ReleaseDate { get; set; } = DateTime.Now;
    public IList<EMovieGenre>? Genre { get; set; } = new List<EMovieGenre>();
    public IList<EMovieFormat>? Format { get; set; } = new List<EMovieFormat>();
    public virtual ICollection<MoviesMediaInfo>? MoviesMediasInfo { get; set; } = new List<MoviesMediaInfo>();
}

public class MoviesMediaInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int MediaId { get; set; } = 1;

    public int MovieId { get; set; } = 1;

    public string Url { get; set; } = string.Empty;
    public bool? IsFeatured { get; set; } = false;
    public string? TypeMedia { get; set; } = "screenshot";
    // public string? TypeMime { get; set; } = "image/png";
}

public class NMoviesMediaInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int MediaId { get; set; } = 1;

    public int MovieId { get; set; } = 1;
    public Movies? Movies { get; set; }

    public int ScreenshotId { get; set; } = 1;
    public ScreenshotInfo? ScreenshotsInfo { get; set; }

    public int VideoId { get; set; } = 1;
    public VideoInfo? VideosInfo { get; set; }
}

public enum EMovieFormat
{
    [EnumMember(Value = "Movie")] Movie,
    [EnumMember(Value = "Short Film")] ShortFilm,
    [EnumMember(Value = "Documentary")] Documentary,
    [EnumMember(Value = "Series")] Series
}

public enum EMovieGenre
{
    [EnumMember(Value = "Action")] Action,
    [EnumMember(Value = "Adventure")] Adventure,
    [EnumMember(Value = "Comedy")] Comedy,
    [EnumMember(Value = "Drama")] Drama,
    [EnumMember(Value = "Fantasy")] Fantasy,
    [EnumMember(Value = "Horror")] Horror,
    [EnumMember(Value = "Mystery")] Mystery,
    [EnumMember(Value = "Romance")] Romance,
    [EnumMember(Value = "SciFi")] SciFi,
    [EnumMember(Value = "Thriller")] Thriller,
    [EnumMember(Value = "Animation")] Animation,
    [EnumMember(Value = "Family")] Family,
    [EnumMember(Value = "Musical")] Musical,
   [EnumMember(Value = "Crimne")] Crime,
    [EnumMember(Value = "Biography")] Biography,
    [EnumMember(Value = "History")] History,
    [EnumMember(Value = "War")] War,
    [EnumMember(Value = "Western")] Western
}