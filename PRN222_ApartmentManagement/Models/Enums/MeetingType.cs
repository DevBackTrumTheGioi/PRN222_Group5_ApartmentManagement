using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum MeetingType
{
    [Display(Name = "BQT")]
    BQT,

    [Display(Name = "Cư dân")]
    Resident,

    [Display(Name = "Khẩn cấp")]
    Emergency
}
