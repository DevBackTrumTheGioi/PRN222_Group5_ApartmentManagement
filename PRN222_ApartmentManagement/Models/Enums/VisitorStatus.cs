using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum VisitorStatus
{
    [Display(Name = "Chờ đến")]
    Pending,

    [Display(Name = "Đã vào")]
    CheckedIn,

    [Display(Name = "Đã ra")]
    CheckedOut,

    [Display(Name = "Đã hủy")]
    Cancelled,

    [Display(Name = "Từ chối vào")]
    Rejected
}

