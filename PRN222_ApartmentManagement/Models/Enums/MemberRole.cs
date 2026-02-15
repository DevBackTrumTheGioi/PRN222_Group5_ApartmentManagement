using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum MemberRole
{
    [Display(Name = "Chủ hợp đồng")]
    ContractOwner,

    [Display(Name = "Thành viên gia đình")]
    FamilyMember,

    [Display(Name = "Người đồng sở hữu")]
    CoOwner,

    [Display(Name = "Người thuê chung")]
    CoTenant,

    [Display(Name = "Người phụ thuộc")]
    Dependent,

    [Display(Name = "Khác")]
    Other
}

