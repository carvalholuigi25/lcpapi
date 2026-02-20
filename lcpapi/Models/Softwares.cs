using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace lcpapi.Models;

public class Software
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int SoftwareId { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [MinLength(1, ErrorMessage = "Title must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Version { get; set; }
    public string? Image { get; set; } = "assets/images/softwares";
    public string? DownloadUrl { get; set; }
    public DateTime? ReleaseDate { get; set; } = DateTime.Now;
    public bool? IsActive { get; set; } = true;
    public ESoftwareCategory? Category { get; set; } = ESoftwareCategory.Other;
    public ESoftwareLicense? License { get; set; } = ESoftwareLicense.Other;
    public virtual ICollection<SoftwaresMediaInfo>? SoftwaresMediaInfos { get; set; } = new List<SoftwaresMediaInfo>();
}

public class SoftwaresMediaInfo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    [JsonIgnore]
    public int MediaId { get; set; } = 1;

    public int SoftwareId { get; set; } = 1;

    public string Url { get; set; } = string.Empty;
    public bool? IsFeatured { get; set; } = false;
    public string? TypeMedia { get; set; } = "screenshot";
    // public string? TypeMime { get; set; } = "image/png";
}

public enum ESoftwareCategory
{
    [EnumMember(Value = "Utility")] Utility,
    [EnumMember(Value = "Game")] Game,
    [EnumMember(Value = "Productivity")] Productivity,
    [EnumMember(Value = "Education")] Education,
    [EnumMember(Value = "Entertainment")] Entertainment,
    [EnumMember(Value = "Other")] Other
}

public enum ESoftwareLicense
{
    [EnumMember(Value = "Free")] Free,
    [EnumMember(Value = "Open Source")] OpenSource,
    [EnumMember(Value = "Commercial")] Commercial,
    [EnumMember(Value = "Subscription")] Subscription,
    [EnumMember(Value = "Other")] Other
}