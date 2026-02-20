using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace lcpapi.Models;

public class Game
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int GameId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(1, ErrorMessage = "Title must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; } = "assets/images/games";
    public string? Artwork { get; set; } = "assets/images/games/artworks";
    public string? Publisher { get; set; }
    public string? Developer { get; set; }
    public bool? IsFeatured { get; set; } = false;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ScoreRating { get; set; } = 0;
    public DateTime? ReleaseDate { get; set; } = DateTime.Now;
    public EGameESRB? ESRB { get; set; } = EGameESRB.RatingPending;
    public IList<EGameEdition>? Edition { get; set; } = new List<EGameEdition>();
    public IList<EGameGenre>? Genre { get; set; } = new List<EGameGenre>();
    public IList<EGamePlatform>? Platform { get; set; } = new List<EGamePlatform>();
    public virtual ICollection<GamesMediaInfo>? GamesMediasInfo { get; set; } = new List<GamesMediaInfo>();
}

public class NGamesMediaInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int MediaId { get; set; } = 1;

    public int GameId { get; set; } = 1;
    public Game? Games { get; set; }

    public int ScreenshotId { get; set; } = 1;
    public ScreenshotInfo? ScreenshotsInfo { get; set; }

    public int VideoId { get; set; } = 1;
    public VideoInfo? VideosInfo { get; set; }
}

public class GamesMediaInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int MediaId { get; set; } = 1;

    public int GameId { get; set; } = 1;

    public string Url { get; set; } = string.Empty;
    public bool? IsFeatured { get; set; } = false;
    public string? TypeMedia { get; set; } = "screenshot";
    // public string? TypeMime { get; set; } = "image/png";
}

public class ScreenshotInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int ScreenshotId { get; set; } = 1;
    public string Url { get; set; } = string.Empty;
    public bool? IsFeatured { get; set; } = false;

    public int GameId { get; set; } = 1;
}

public class VideoInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int VideoId { get; set; } = 1;
    public string Url { get; set; } = string.Empty;
    public bool? IsFeatured { get; set; } = false;
    public int GameId { get; set; } = 1;
}

// public class GamesDLCs
// {
//     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//     [Key]
//     public int DLCId { get; set; } = 1;

//     public int GameId { get; set; } = 1;
//     public string Title { get; set; } = string.Empty;
//     public string? Description { get; set; }
//     public DateTime? ReleaseDate { get; set; } = DateTime.Now;
//     public decimal? Price { get; set; } = 0.0M;
// }

// public class GamesReviews
// {
//     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//     [Key]
//     public int ReviewId { get; set; } = 1;

//     public int GameId { get; set; } = 1;
//     public string Username { get; set; } = string.Empty;
//     public string? ReviewText { get; set; }
//     public int Rating { get; set; } = 0;
//     public DateTime? ReviewDate { get; set; } = DateTime.Now;
// }

// public class GamesSaveData
// {
//     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//     [Key]
//     public int SaveDataId { get; set; } = 1;

//     public int GameId { get; set; } = 1;
//     public string Username { get; set; } = string.Empty;
//     public string? SaveDataPath { get; set; }
//     public DateTime? LastUpdated { get; set; } = DateTime.Now;
// }

// public class GamesAchievements
// {
//     [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//     [Key]
//     public int AchievementId { get; set; } = 1;

//     public int GameId { get; set; } = 1;
//     public string Title { get; set; } = string.Empty;
//     public string? Description { get; set; }
//     public int Points { get; set; } = 0;
//     public bool IsSecret { get; set; } = false;
// }

public enum EGameEdition
{
    [EnumMember(Value = "Standard")] Standard,
    [EnumMember(Value = "Deluxe")] Deluxe,
    [EnumMember(Value = "Premium")] Premium,
    [EnumMember(Value = "Collector")] Collector,
    [EnumMember(Value = "Game of the Year")] GameOfTheYear,
    [EnumMember(Value = "Ultimate")] Ultimate,
    [EnumMember(Value = "Other")] Other
}

public enum EGameESRB
{
    [EnumMember(Value = "Early Childhood")] EarlyChildhood,
    [EnumMember(Value = "Everyone")] Everyone,
    [EnumMember(Value = "Everyone 10 Plus")] Everyone10Plus,
    [EnumMember(Value = "Teen")] Teen,
    [EnumMember(Value = "Mature")] Mature,
    [EnumMember(Value = "Mature 17 Plus")] Mature17Plus,
    [EnumMember(Value = "Adults Only 18 Plus")] AdultsOnly18Plus,
    [EnumMember(Value = "Rating Pending")] RatingPending
}

public enum EGamePlatform
{
    [EnumMember(Value = "PC")] PC,
    [EnumMember(Value = "Playstation")] PlayStation,
    [EnumMember(Value = "Playstation 2")] PlayStation2,
    [EnumMember(Value = "Playstation Portable")] PlayStationPortable,
    [EnumMember(Value = "Playstation 3")] PlayStation3,
    [EnumMember(Value = "Xbox")] Xbox,
    [EnumMember(Value = "Xbox 360")] Xbox360,
    [EnumMember(Value = "Nintendo 64")] Nintendo64,
    [EnumMember(Value = "Nintendo Wii")] NintendoWii,
    [EnumMember(Value = "Nintendo Switch")] NintendoSwitch,
    [EnumMember(Value = "Nintendo Switch 2")] NintendoSwitch2,
    [EnumMember(Value = "Mobile")] Mobile,
    [EnumMember(Value = "Steam")] Steam,
    [EnumMember(Value = "EGS")] EGS,
    [EnumMember(Value = "Other")] Other
}

public enum EGameGenre
{
    [EnumMember(Value = "Action")] Action,
    [EnumMember(Value = "Adventure")] Adventure,
    [EnumMember(Value = "RPG")] RPG,
    [EnumMember(Value = "Strategy")] Strategy,
    [EnumMember(Value = "Simulation")] Simulation,
    [EnumMember(Value = "Sports")] Sports,
    [EnumMember(Value = "Puzzle")] Puzzle,
    [EnumMember(Value = "Horror")] Horror,
    [EnumMember(Value = "Platformer")] Platformer,
    [EnumMember(Value = "Shooter")] Shooter,
    [EnumMember(Value = "Fighting")] Fighting,
    [EnumMember(Value = "Racing")] Racing,
    [EnumMember(Value = "MMO")] MMO,
    [EnumMember(Value = "Sandbox")] Sandbox,
    [EnumMember(Value = "Survival")] Survival,
    [EnumMember(Value = "Stealth")] Stealth,
    [EnumMember(Value = "Other")] Other
}

// public enum EGameCondition
// {
//     [EnumMember(Value = "New")] New,
//     [EnumMember(Value = "Like New")] LikeNew,
//     [EnumMember(Value = "Very Good")] VeryGood,
//     [EnumMember(Value = "Good")] Good,
//     [EnumMember(Value = "Acceptable")] Acceptable,
//     [EnumMember(Value = "Poor")] Poor
// }