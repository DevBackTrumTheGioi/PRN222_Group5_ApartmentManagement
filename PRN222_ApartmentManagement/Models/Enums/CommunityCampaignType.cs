using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum CommunityCampaignType
{
    [Display(Name = "Khảo sát")]
    Survey,

    [Display(Name = "Bỏ phiếu")]
    Vote
}
