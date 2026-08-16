using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace lcpapi.Models;

public class Book
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int BookId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(1, ErrorMessage = "Title must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; } = "assets/images/books";
    public string? Artwork { get; set; } = "assets/images/books/artworks";
    public string? Author { get; set; }
    public bool? IsFeatured { get; set; } = false;
    [Column(TypeName = "decimal(18,2)")]
    public decimal? ScoreRating { get; set; } = 0;
    public DateTime? ReleaseDate { get; set; } = DateTime.Now;
    public IList<EBookGenre>? Genre { get; set; } = new List<EBookGenre>();
    public virtual ICollection<BooksMediaInfo>? BooksMediasInfo { get; set; }
}

public class BooksMediaInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int BooksMediaInfoId { get; set; }

    [ForeignKey("Book")]
    public int BookId { get; set; }
    public string? MediaType { get; set; }
    public string? Link { get; set; }

    [JsonIgnore]
    public virtual Book? Book { get; set; }
}

public enum EBookGenre
{
    [EnumMember(Value = "Fantasy")] Fantasy,
    [EnumMember(Value = "Science Fiction")] ScienceFiction,
    [EnumMember(Value = "Mystery")] Mystery,
    [EnumMember(Value = "Thriller")] Thriller,
    [EnumMember(Value = "Romance")] Romance,
    [EnumMember(Value = "Western")]  Western,
    [EnumMember(Value = "Dystopian")] Dystopian,
    [EnumMember(Value = "Contemporary")] Contemporary,
    [EnumMember(Value = "Historical Fiction")] HistoricalFiction,
    [EnumMember(Value = "Horror")] Horror,
    [EnumMember(Value = "Literary Fiction")] LiteraryFiction
}