using System.ComponentModel.DataAnnotations;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Models.DTOs;

public class FamilyMemberCreateDto
{
    [Required(ErrorMessage = "Họ tên không được để trống.")]
    [MaxLength(200, ErrorMessage = "Họ tên không vượt quá 200 ký tự.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số CCCD không được để trống.")]
    [MaxLength(20, ErrorMessage = "Số CCCD không vượt quá 20 ký tự.")]
    public string IdentityCardNumber { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [MaxLength(100, ErrorMessage = "Email không vượt quá 100 ký tự.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Số điện thoại không được để trống.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [MaxLength(20, ErrorMessage = "Số điện thoại không vượt quá 20 ký tự.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày sinh không được để trống.")]
    public DateTime? DateOfBirth { get; set; }
}
