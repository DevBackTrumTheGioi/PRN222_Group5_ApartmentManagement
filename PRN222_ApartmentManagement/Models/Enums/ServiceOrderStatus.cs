using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum ServiceOrderStatus
{
    [Display(Name = "Chờ xử lý")]
    Pending,

    [Display(Name = "Đã xác nhận")]
    Confirmed,

    [Display(Name = "Đang thực hiện")]
    InProgress,

    [Display(Name = "Đã hoàn thành")]
    Completed,

    [Display(Name = "Đã hủy")]
    Cancelled,

    [Display(Name = "Đã từ chối")]
    Rejected
}

