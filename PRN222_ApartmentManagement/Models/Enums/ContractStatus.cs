using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum ContractStatus
{
    [Display(Name = "Bản nháp")]
    Draft,

    [Display(Name = "Chờ ký")]
    PendingSignature,

    [Display(Name = "Có hiệu lực")]
    Active,

    [Display(Name = "Hết hạn")]
    Expired,

    [Display(Name = "Đã chấm dứt")]
    Terminated,

    [Display(Name = "Đã hủy")]
    Cancelled
}

