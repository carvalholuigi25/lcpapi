using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lcpapi.Models;

public class Settings
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Theme setting name is required.")]
    [MinLength(1, ErrorMessage = "Theme setting name must be at least 1 character long.")]
    [MaxLength(255, ErrorMessage = "Theme setting name cannot exceed 255 characters.")]
    [Display(Name = "Theme Setting Name", Description = "The name of the theme setting.")]
    public string ThemeSettingName { get; set; } = null!;

    [Display(Name = "Real Data Time Enabled", Description = "Indicates whether real data time is enabled.")]
    public string? RealDataTimeEnabled { get; set; } = "false";

    [Display(Name = "Dark Mode", Description = "Indicates whether dark mode is enabled.")]
    public string? IsDarkMode { get; set; } = "false";

    [Display(Name = "Auto Refresh Interval", Description = "The interval at which the data should be refreshed automatically.")]
    [Required(ErrorMessage = "Auto refresh interval setting is required.")]
    [MaxLength(10000, ErrorMessage = "Auto refresh interval cannot exceed 10000.")]
    [DefaultValue(0)]
    public int? AutoRefreshInterval { get; set; } = 0;

    [Display(Name = "Notifications Enabled", Description = "Indicates whether notifications are enabled.")]
    public string? NotificationsEnabled { get; set; } = "false";

    [Display(Name = "Logging Enabled", Description = "Indicates whether logging is enabled.")]
    public string? EnableLogging { get; set; } = "false";

    [Display(Name = "User ID", Description = "The ID of the user associated with these settings.")]
    public int? UserId { get; set; } = 1;
}