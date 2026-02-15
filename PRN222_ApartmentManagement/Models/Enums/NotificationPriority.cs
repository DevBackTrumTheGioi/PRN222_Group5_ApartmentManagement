using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum NotificationPriority
{
    [Display(Name = "Thấp")]
    Low = 1,

    [Display(Name = "Bình thường")]
    Normal = 2,

    [Display(Name = "Cao")]
    High = 3,

    [Display(Name = "Khẩn cấp")]
    Critical = 4
}

