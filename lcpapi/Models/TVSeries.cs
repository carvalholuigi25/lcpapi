using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace lcpapi.Models;

public class Tvseries
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int TvserieId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(1, ErrorMessage = "Title must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; } = "assets/images/tvseries";
    public string? Artwork { get; set; } = "assets/images/tvseries/artworks";
    public string? Studio { get; set; }
    public bool? IsFeatured { get; set; } = false;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ScoreRating { get; set; } = 0;
    public int? NumSeasons { get; set; } = 0;
    public int? NumEpisodes { get; set; } = 0;
    public DateTime? ReleaseDate { get; set; } = DateTime.Now;
    public IList<ETvserieGenre>? Genre { get; set; } = new List<ETvserieGenre>();
    public IList<ETvserieFormat>? Format { get; set; } = new List<ETvserieFormat>();

    [JsonIgnore]
    public virtual ICollection<TvseriesSeasonsInfo>? SeasonsInfo { get; set; } = [];

    [JsonIgnore]
    public virtual ICollection<TvseriesEpisodesInfo>? EpisodesInfo { get; set; } = [];

    [JsonIgnore]
    public virtual ICollection<TvseriesMediaInfo>? MediasInfo { get; set; } = [];

    [JsonIgnore]
    public virtual ICollection<TvseriesReviewsInfo>? ReviewsInfos { get; set; } = [];
}

public class TvseriesSeasonsInfo
{
    // [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int SeasonsId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(1, ErrorMessage = "Title must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string SeasonsTitle { get; set; } = string.Empty;
    public string? SeasonsDescription { get; set; }
    public string? SeasonsImage { get; set; }
    public string? SeasonsStudio { get; set; }
    public bool? SeasonsIsFeatured { get; set; } = false;
    public bool? SeasonsIsWatched { get; set; } = false;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? SeasonsScoreRating { get; set; } = 0;
    public DateTime? SeasonsReleaseDate { get; set; } = DateTime.Now;

    [DefaultValue(1)]
    public int? TvserieId { get; set; } = 1;

    // [JsonIgnore]
    // public virtual ICollection<TvseriesEpisodesInfo> EpisodesInfo { get; set; } = [];

    // [DefaultValue(1)]
    // public int? TvserieId { get; set; } = 1;

    // [JsonIgnore]
    // public Tvseries? Tvseries { get; set; }
}

public class TvseriesEpisodesInfo
{
    // [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int EpisodesId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(1, ErrorMessage = "Title must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string EpisodesTitle { get; set; } = string.Empty;
    public string? EpisodesDescription { get; set; }
    public string? EpisodesImage { get; set; }
    public string? EpisodesStudio { get; set; }
    public bool? EpisodesIsFeatured { get; set; } = false;
    public bool? EpisodesIsWatched { get; set; } = false;
    [Column(TypeName = "decimal(18,2)")]
    public decimal? EpisodesScoreRating { get; set; } = 0;
    public DateTime? EpisodesReleaseDate { get; set; } = DateTime.Now;

    [DefaultValue(1)]
    public int? SeasonsId { get; set; } = 1;

    [DefaultValue(1)]
    public int? TvserieId { get; set; } = 1;

    [JsonIgnore]
    public virtual ICollection<TvseriesReactions>? ReactionsInfo { get; set; } = [];

    [JsonIgnore]
    public virtual ICollection<TvseriesCommentsInfo>? CommentsInfo { get; set; } = [];

    // [JsonIgnore]
    // public virtual ICollection<TvseriesSeasonsInfo>? SeasonsInfo { get; set; } = [];
}

public class TvseriesReviewsInfo
{
    // [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int ReviewsId { get; set; }
    public string ReviewsTitle { get; set; } = null!;
    public string ReviewsDescription { get; set; } = null!;
    public string? ReviewsImage { get; set; } = "images/tvseries/reviews/main.png";
    public string? ReviewsCover { get; set; } = "images/tvseries/reviews/covers/main.png";
    public bool? ReviewsIsFeatured { get; set; } = false;
    public DateTime? ReviewsDateTime { get; set; } = DateTime.UtcNow;
    public TvseriesReviewsStatus? ReviewsStatus { get; set; } = TvseriesReviewsStatus.draft;

    [DefaultValue(0)]
    public long? ReviewsViews { get; set; } = 0;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? ReviewsScoreRating { get; set; } = 0;

    [DefaultValue(1)]
    public int? EpisodesId { get; set; } = 1;

    [DefaultValue(1)]
    public int? SeasonsId { get; set; } = 1;

    [DefaultValue(1)]
    public int? TvserieId { get; set; } = 1;

    [DefaultValue(1)]
    public int? UserId { get; set; } = 1;
}

public class TvseriesCommentsInfo
{
    // [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int CommentsId { get; set; }
    public string CommentsTitle { get; set; } = null!;
    public string CommentsDescription { get; set; } = null!;
    public bool? CommentsIsFeatured { get; set; } = false;
    public DateTime? CommentsDateTime { get; set; } = DateTime.UtcNow;
    public TvseriesCommentsStatus? CommentsStatus { get; set; } = TvseriesCommentsStatus.draft;

    [DefaultValue(1)]
    public int? ReviewsId { get; set; } = 1;

    [DefaultValue(1)]
    public int? EpisodesId { get; set; } = 1;

    [DefaultValue(1)]
    public int? SeasonsId { get; set; } = 1;

    [DefaultValue(1)]
    public int? TvserieId { get; set; } = 1;

    [DefaultValue(1)]
    public int? UserId { get; set; } = 1;
}

public class TvseriesMediaInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int MediaId { get; set; }

    [DefaultValue(1)]
    public int EpisodesId { get; set; } = 1;

    [DefaultValue(1)]
    public int SeasonsId { get; set; } = 1;

    [DefaultValue(1)]
    public int TvserieId { get; set; } = 1;

    public string Url { get; set; } = string.Empty;
    public bool? IsFeatured { get; set; } = false;
    public string? TypeMedia { get; set; } = "screenshot";
    // public string? TypeMime { get; set; } = "image/png";
}

public class TvseriesReactions
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int TvseriesReactionsId { get; set; }

    public ETvserieReactionType? ReactionsType { get; set; } = ETvserieReactionType.like;
    public int ReviewId { get; set; } = 1;
    public int EpisodeId { get; set; } = 1;
    public int SeasonId { get; set; } = 1;
    public int TvserieId { get; set; } = 1;
    public int UserId { get; set; } = 1;
}

public enum ETvserieFormat
{
    [EnumMember(Value = "Tvserie")] Tvserie,
    [EnumMember(Value = "Short Film")] ShortFilm,
    [EnumMember(Value = "Documentary")] Documentary,
    [EnumMember(Value = "Series")] Series
}

public enum ETvserieGenre
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

public enum TvseriesReviewsStatus
{
    [EnumMember(Value = "draft")] draft,
    [EnumMember(Value = "published")] published,
    [EnumMember(Value = "locked")] locked,
    [EnumMember(Value = "updated")] updated,
    [EnumMember(Value = "trash")] trash,
    [EnumMember(Value = "deleted")] deleted
}

public enum TvseriesCommentsStatus
{
    [EnumMember(Value = "draft")] draft,
    [EnumMember(Value = "published")] published,
    [EnumMember(Value = "locked")] locked,
    [EnumMember(Value = "updated")] updated,
    [EnumMember(Value = "trash")] trash,
    [EnumMember(Value = "deleted")] deleted
}

public enum ETvserieReactionType
{
    [EnumMember(Value = "like")] like,
    [EnumMember(Value = "dislike")] dislike,
    [EnumMember(Value = "love")] love,
    [EnumMember(Value = "hate")] hate,
    [EnumMember(Value = "cry")] cry,
    [EnumMember(Value = "gross")] gross,
    [EnumMember(Value = "thinking")] thinking,
    [EnumMember(Value = "awesome")] awesome,
    [EnumMember(Value = "dontknow")] dontknow,
    [EnumMember(Value = "custom")] custom,
    [EnumMember(Value = "unknown")] unknown
}