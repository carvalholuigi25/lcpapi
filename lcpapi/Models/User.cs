using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using lcpapi.Models.Enums;
using lcpapi.Models.UsersAuth;

namespace lcpapi.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Username is required.")]
    [MinLength(1, ErrorMessage = "Username must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(1, ErrorMessage = "Password must be at least 1 character long.")]
    [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
    public string Password { get; set; } = null!;

    [MinLength(1, ErrorMessage = "Email must be at least 1 character long.")]
    [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? Avatar { get; set; } = "images/avatars/u1.png";
    public string? Cover { get; set; } = "images/covers/u1.png";
    public string? DateBirthday { get; set; } = DateTime.Now.ToString();
    public EUsersRoles? Role { get; set; } = EUsersRoles.user;
    public EUserPrivacy? Privacy { get; set; } = EUserPrivacy.locked;
    public int? UsersInfoId { get; set; } = 1;

    [JsonIgnore]
    public UserPermissionsSettings? UserPermissionsSettings { get; set; }

    [JsonIgnore]
    public UserModerationSettings? UserModerationSettings { get; set; }

    [JsonIgnore]
    public List<RefreshToken>? RefreshTokens { get; set; }

    // OTP (TOTP) properties
    [JsonIgnore]
    public string? OtpSecret { get; set; }

    [DefaultValue(false)]
    public bool OtpEnabled { get; set; } = false;
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

public class UserPermissionsSettings
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int UserPermissionsSettingsId { get; set; }
    public int UserId { get; set; } = 1;
    public string TypePermissions { get; set; } = null!;
    public int? CodePermissions { get; set; }
    public string? DescPermissions { get; set; }
}

public class UserModerationSettings
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int UserModerationSettingsId { get; set; }
    public int UserId { get; set; } = 1;
    public int AdminId { get; set; } = 1;
    public int CurAttempts { get; set; } = 0;
    public bool IsUserConfirmed { get; set; } = true;
    public bool? IsUserBanned { get; set; } = false;
    public bool? IsUserAuthLocked { get; set; } = false;
    public string? TimeCreation { get; set; } = DateTime.UtcNow.ToString();
    public string? TimeUserAuthLocked { get; set; } = DateTime.UtcNow.ToString();
}

public enum EUserPermissionsSettings {
    [EnumMember(Value = "READ_DATA")]
    READ_DATA = 0,
    [EnumMember(Value = "CREATE_DATA")]
    CREATE_DATA = 1,
    [EnumMember(Value = "UPDATE_DATA")]
    UPDATE_DATA = 2,
    [EnumMember(Value = "DELETE_DATA")]
    DELETE_DATA = 3,
    [EnumMember(Value = "LOCK_DATA")]
    LOCK_DATA = 4,
    [EnumMember(Value = "BAN_BAD_USERS_ONLY")]
    BAN_BAD_USERS_ONLY = 5
}