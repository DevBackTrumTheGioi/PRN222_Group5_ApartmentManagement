using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum InvoiceStatus
{
    [Display(Name = "Chưa thanh toán")]
    Unpaid,

    [Display(Name = "Đã thanh toán")]
    Paid,

    [Display(Name = "Thanh toán một phần")]
    PartiallyPaid,

    [Display(Name = "Quá hạn")]
    Overdue,

    [Display(Name = "Đã hủy")]
    Cancelled,

    [Display(Name = "Chờ xác nhận")]
    Pending
}

