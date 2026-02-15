using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum CardStatus
{
    [Display(Name = "Hoạt động")]
    Active,

    [Display(Name = "Khóa")]
    Locked,

    [Display(Name = "Mất")]
    Lost,

    [Display(Name = "Hết hạn")]
    Expired,

    [Display(Name = "Đã thu hồi")]
    Revoked
}

