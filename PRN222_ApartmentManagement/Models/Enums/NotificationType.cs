using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum NotificationType
{
    [Display(Name = "Hệ thống")]
    System,

    [Display(Name = "Hóa đơn")]
    Invoice,

    [Display(Name = "Yêu cầu")]
    Request,

    [Display(Name = "Thông báo chung")]
    Announcement,

    [Display(Name = "Hợp đồng")]
    Contract,

    [Display(Name = "Tiện ích")]
    Amenity,

    [Display(Name = "Khác")]
    Other
}

