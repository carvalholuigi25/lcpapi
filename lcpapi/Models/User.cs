using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using lcpapi.Models.Enums;
using lcpapi.Models.UsersAuth;

namespace lcpapi.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? Avatar { get; set; } = "images/avatars/u1.png";
    public string? Cover { get; set; } = "images/covers/u1.png";
    public EUsersRoles? Role { get; set; } = EUsersRoles.user;
    public EUserPrivacy? Privacy { get; set; } = EUserPrivacy.locked;
    public int? UsersInfoId { get; set; } = 1;

    [JsonIgnore]
    public List<RefreshToken>? RefreshTokens { get; set; }
}

public class UsersInfo {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int? UsersInfoId { get; set; }
    public string? PhoneUser { get; set; }
    public string? AddressUser { get; set; }
    public string? CountryUser { get; set; }
    public string? CityUser { get; set; }
    public string? DistrictUser { get; set; }
    public string? DateBirthdayUser { get; set; }
}