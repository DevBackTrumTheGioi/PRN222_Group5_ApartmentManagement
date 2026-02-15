using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum ReferenceType
{
    [Display(Name = "Không xác định")]
    None,

    [Display(Name = "Hóa đơn")]
    Invoice,

    [Display(Name = "Yêu cầu")]
    Request,

    [Display(Name = "Thông báo")]
    Announcement,

    [Display(Name = "Hợp đồng")]
    Contract,

    [Display(Name = "Tiện ích")]
    Amenity,

    [Display(Name = "Khách viếng thăm")]
    Visitor,

    [Display(Name = "Phương tiện")]
    Vehicle
}

