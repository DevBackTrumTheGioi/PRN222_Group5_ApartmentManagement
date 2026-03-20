using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum InvoiceApprovalStatus
{
    [Display(Name = "Bản nháp")]
    Draft,

    [Display(Name = "Chờ duyệt")]
    PendingApproval,

    [Display(Name = "Đã duyệt")]
    Approved,

    [Display(Name = "Từ chối")]
    Rejected
}
