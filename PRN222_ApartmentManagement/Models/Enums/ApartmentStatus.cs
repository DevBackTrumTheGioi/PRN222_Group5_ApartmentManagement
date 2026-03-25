using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum ApartmentStatus
{
    [Display(Name = "Trống")]
    Available,

    [Display(Name = "Đang ở")]
    Occupied,

    [Display(Name = "Đã đặt")]
    Reserved,

    [Display(Name = "Bảo trì")]
    Maintenance
}
