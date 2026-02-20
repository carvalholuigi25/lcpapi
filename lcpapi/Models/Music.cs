using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace lcpapi.Models;

public class Music
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int MusicId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(1, ErrorMessage = "Title must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; } = "assets/images/musics";
    public string? Artwork { get; set; } = "assets/images/musics/artworks";
    public string? Artist { get; set; }
    public string? Album { get; set; }
    public bool? IsFeatured { get; set; } = false;
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ScoreRating { get; set; } = 0;
    public DateTime? ReleaseDate { get; set; } = DateTime.Now;
    public IList<EMusicGenre>? Genre { get; set; } = new List<EMusicGenre>();
    public IList<EMusicFormat>? Format { get; set; } = new List<EMusicFormat>();
    public virtual ICollection<MusicsMediaInfo>? MusicsMediasInfo { get; set; } = new List<MusicsMediaInfo>();
}

public class MusicsMediaInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int MediaId { get; set; } = 1;

    public int MusicId { get; set; } = 1;

    public string Url { get; set; } = string.Empty;
    public bool? IsFeatured { get; set; } = false;
    public string? TypeMedia { get; set; } = "audio";
    // public string? TypeMime { get; set; } = "audio/mpeg";
}

public enum EMusicGenre
{
    Rock,
    Pop,
    Jazz,
    Classical,
    HipHop,
    Electronic,
    Country,
    Reggae,
    Blues,
    Metal,
    Folk,
    Punk,
    Disco,
    Funk,
    Soul,
    RnB,
    Gospel,
    Opera,
    Ska
}

public enum EMusicFormat
{
    MP3,
    WAV,
    FLAC,
    AAC,
    OGG,
    ALAC,
    WMA,
    AIFF
}