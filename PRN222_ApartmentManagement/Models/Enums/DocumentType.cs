using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum DocumentType
{
    [Display(Name = "Nội quy - Quy định")]
    Regulation,

    [Display(Name = "Thông báo - Công văn")]
    Notice,

    [Display(Name = "Báo cáo - Tài chính")]
    Report,

    [Display(Name = "Hợp đồng - Pháp lý")]
    Legal,

    [Display(Name = "Hướng dẫn - Cẩm nang")]
    Manual,

    [Display(Name = "Khác")]
    Other
}

