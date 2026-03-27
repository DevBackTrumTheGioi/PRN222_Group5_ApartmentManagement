using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum CommunityCampaignStatus
{
    [Display(Name = "Đang mở")]
    Published,

    [Display(Name = "Đã đóng")]
    Closed
}
