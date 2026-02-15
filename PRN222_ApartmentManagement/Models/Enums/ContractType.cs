using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum ContractType
{
    [Display(Name = "Mua bán")]
    Purchase,

    [Display(Name = "Cho thuê")]
    Rental,

    [Display(Name = "Khác")]
    Other
}

