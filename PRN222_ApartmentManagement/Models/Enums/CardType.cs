using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum CardType
{
    [Display(Name = "Thẻ cư dân")]
    Resident,

    [Display(Name = "Thẻ nhân viên")]
    Staff,

    [Display(Name = "Thẻ phụ")]
    Secondary,

    [Display(Name = "Thẻ khách")]
    Visitor
}
