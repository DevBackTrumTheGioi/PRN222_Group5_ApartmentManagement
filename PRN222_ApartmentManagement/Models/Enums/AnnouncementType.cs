using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum AnnouncementType
{
    [Display(Name = "Thông báo chung")]
    General,

    [Display(Name = "Hợp tác - Đầu tư")]
    Partnership,

    [Display(Name = "Bảo trì - Sửa chữa")]
    Maintenance,

    [Display(Name = "Sự kiện - Lễ hội")]
    Event,

    [Display(Name = "An ninh - An toàn")]
    Security,

    [Display(Name = "Khẩn cấp")]
    Emergency,

    [Display(Name = "Phí - Hóa đơn")]
    Finance
}

