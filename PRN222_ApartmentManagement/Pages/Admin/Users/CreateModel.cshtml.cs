using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Admin.Users;

[Authorize(Policy = "AdminOnly")]
public class CreateModel : PageModel
{
    private readonly IUserManagementService _userManagementService;

    public CreateModel(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        public UserRole Role { get; set; }

        public bool IsActive { get; set; } = true;

        // Resident specific fields
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(20)]
        public string? IdentityCardNumber { get; set; }

        public ResidentType? ResidentType { get; set; }

        [StringLength(20)]
        public string? ResidencyStatus { get; set; }

        public int? ApartmentId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? MoveInDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? MoveOutDate { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }
    }

    public List<Apartment> Apartments { get; set; } = new();

    public async Task OnGetAsync()
    {
        Apartments = await _userManagementService.GetAssignableApartmentsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Apartments = await _userManagementService.GetAssignableApartmentsAsync();
            return Page();
        }

        var request = new UserUpsertRequest
        {
            Username = Input.Username,
            Password = Input.Password,
            FullName = Input.FullName,
            Email = Input.Email,
            PhoneNumber = Input.PhoneNumber,
            Role = Input.Role,
            IsActive = Input.IsActive,
            DateOfBirth = Input.DateOfBirth,
            IdentityCardNumber = Input.IdentityCardNumber,
            ResidentType = Input.ResidentType,
            ResidencyStatus = Input.ResidencyStatus,
            ApartmentId = Input.ApartmentId,
            MoveInDate = Input.MoveInDate,
            MoveOutDate = Input.MoveOutDate,
            Note = Input.Note
        };

        var (success, errorField, errorMessage) = await _userManagementService.CreateUserAsync(request);
        if (!success)
        {
            Apartments = await _userManagementService.GetAssignableApartmentsAsync();
            ModelState.AddModelError(errorField ?? string.Empty, errorMessage ?? "Không thể tạo người dùng.");
            return Page();
        }

        return RedirectToPage("./Index");
    }
}
