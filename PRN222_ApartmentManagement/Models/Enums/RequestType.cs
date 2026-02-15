using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum RequestType
{
    [Display(Name = "Sửa chữa")]
    Repair,

    [Display(Name = "Khiếu nại")]
    Complaint,

    [Display(Name = "Góp ý")]
    Feedback,

    [Display(Name = "Dịch vụ")]
    Service,

    [Display(Name = "An ninh")]
    Security,

    [Display(Name = "Khác")]
    Other
}

