using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lcpapi.Models;

public class Settings
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int SettingsId { get; set; }

    [Required(ErrorMessage = "Theme setting name is required.")]
    [MinLength(1, ErrorMessage = "Theme setting name must be at least 1 character long.")]
    [MaxLength(255, ErrorMessage = "Theme setting name cannot exceed 255 characters.")]
    [Display(Name = "Theme Setting Name", Description = "The name of the theme setting.")]
    public string ThemeSettingName { get; set; } = null!;

    [Display(Name = "Real Data Time Enabled", Description = "Indicates whether real data time is enabled.")]
    [DefaultValue(false)]
    public bool? RealDataTimeEnabled { get; set; }

    [Display(Name = "Dark Mode", Description = "Indicates whether dark mode is enabled.")]
    [DefaultValue(false)]
    public bool? IsDarkMode { get; set; }

    [Display(Name = "Auto Refresh Interval", Description = "The interval at which the data should be refreshed automatically.")]
    [Required(ErrorMessage = "Auto refresh interval setting is required.")]
    [DefaultValue(0)]
    public int? AutoRefreshInterval { get; set; } = 0;

    [Display(Name = "Notifications Enabled", Description = "Indicates whether notifications are enabled.")]
    [DefaultValue(false)]
    public bool? NotificationsEnabled { get; set; }

    [Display(Name = "Logging Enabled", Description = "Indicates whether logging is enabled.")]
    [DefaultValue(false)]
    public bool? EnableLogging { get; set; }

    [Display(Name = "User ID", Description = "The ID of the user associated with these settings.")]
    [DefaultValue(1)]
    public int? UserId { get; set; }
}