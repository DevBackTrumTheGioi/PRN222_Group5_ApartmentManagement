using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum SignatureStatus
{
    [Display(Name = "Chờ ký")]
    Pending,

    [Display(Name = "Đã ký")]
    Signed,

    [Display(Name = "Từ chối")]
    Rejected,

    [Display(Name = "Đã hủy")]
    Cancelled
}

