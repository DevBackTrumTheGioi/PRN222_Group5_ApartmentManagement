using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum RequestPriority
{
    [Display(Name = "Thấp")]
    Low = 1,

    [Display(Name = "Bình thường")]
    Normal = 2,

    [Display(Name = "Cao")]
    High = 3,

    [Display(Name = "Khẩn cấp")]
    Emergency = 4
}

