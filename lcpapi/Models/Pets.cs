using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace lcpapi.Models;

public class Pet
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int PetsId { get; set; }

    [Required(ErrorMessage = "Name is required.")]
    [MinLength(1, ErrorMessage = "Name must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime DateOfBirthday { get; set; } = DateTime.Now;

    public string? Description { get; set; }

    public bool? isFavorite { get; set; }
}

