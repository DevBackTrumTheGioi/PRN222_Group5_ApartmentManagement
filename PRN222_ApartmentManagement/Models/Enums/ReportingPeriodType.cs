using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum ReportingPeriodType
{
    [Display(Name = "Tháng")]
    Month,

    [Display(Name = "Quý")]
    Quarter,

    [Display(Name = "Năm")]
    Year
}
