using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models;

public class SystemSetting
{
    [Key]
    public int SettingId { get; set; }

    [Required]
    [MaxLength(100)]
    public string SettingKey { get; set; } = string.Empty;

    [Required]
    public string SettingValue { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Description { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

