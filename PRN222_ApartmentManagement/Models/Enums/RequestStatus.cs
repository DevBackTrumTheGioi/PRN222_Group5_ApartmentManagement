using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum RequestStatus
{
    [Display(Name = "Chờ xử lý")]
    Pending,

    [Display(Name = "Đang xử lý")]
    InProgress,

    [Display(Name = "Đã hoàn thành")]
    Completed,

    [Display(Name = "Đã hủy")]
    Cancelled,

    [Display(Name = "Đã từ chối")]
    Rejected
}

