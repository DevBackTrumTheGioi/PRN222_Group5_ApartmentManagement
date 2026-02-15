using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum CardType
{
    [Display(Name = "Thẻ chính")]
    Primary,

    [Display(Name = "Thẻ phụ")]
    Secondary,

    [Display(Name = "Thẻ khách")]
    Visitor,

    [Display(Name = "Thẻ nhân viên")]
    Staff
}

