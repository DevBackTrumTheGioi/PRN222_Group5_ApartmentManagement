using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Admin.Users;

[Authorize(Policy = "AdminOnly")]
public class CreateModel : PageModel
{
    private readonly ApartmentDbContext _context;
    private readonly IAuthService _authService;

    public CreateModel(ApartmentDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
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
        Apartments = await _context.Apartments
            .Where(a => a.Status != ApartmentStatus.Maintenance)
            .OrderBy(a => a.ApartmentNumber)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Apartments = await _context.Apartments
                .Where(a => a.Status != ApartmentStatus.Maintenance)
                .OrderBy(a => a.ApartmentNumber)
                .ToListAsync();
            return Page();
        }

        // Check if username already exists
        if (await _context.Users.AnyAsync(u => u.Username == Input.Username))
        {
            ModelState.AddModelError("Input.Username", "Tên đăng nhập đã tồn tại.");
            return Page();
        }

        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == Input.Email))
        {
            ModelState.AddModelError("Input.Email", "Email đã được sử dụng.");
            return Page();
        }

        var user = new User
        {
            Username = Input.Username,
            PasswordHash = _authService.HashPassword(Input.Password),
            FullName = Input.FullName,
            Email = Input.Email,
            PhoneNumber = Input.PhoneNumber,
            Role = Input.Role,
            IsActive = Input.IsActive,
            CreatedAt = DateTime.Now,
            // Resident fields
            DateOfBirth = Input.DateOfBirth,
            IdentityCardNumber = Input.IdentityCardNumber,
            ResidentType = Input.ResidentType,
            ResidencyStatus = Input.ResidencyStatus,
            ApartmentId = Input.ApartmentId,
            MoveInDate = Input.MoveInDate,
            MoveOutDate = Input.MoveOutDate,
            Note = Input.Note
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
