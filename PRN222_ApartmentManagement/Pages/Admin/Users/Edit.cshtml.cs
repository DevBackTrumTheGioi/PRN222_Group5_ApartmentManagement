using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Admin.Users;

[Authorize(Policy = "AdminOnly")]
public class EditModel : PageModel
{
    private readonly ApartmentDbContext _context;
    private readonly IAuthService _authService;

    public EditModel(ApartmentDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public string? Username { get; set; }

    public class InputModel
    {
        public int UserId { get; set; }

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

        public bool IsActive { get; set; }

        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

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

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null || user.IsDeleted)
        {
            return NotFound();
        }

        Apartments = await _context.Apartments
            .Where(a => a.Status != "Maintenance")
            .OrderBy(a => a.ApartmentNumber)
            .ToListAsync();

        Username = user.Username;
        Input = new InputModel
        {
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role ?? UserRole.Resident,
            IsActive = user.IsActive,
            // Resident fields
            DateOfBirth = user.DateOfBirth,
            IdentityCardNumber = user.IdentityCardNumber,
            ResidentType = user.ResidentType,
            ResidencyStatus = user.ResidencyStatus,
            ApartmentId = user.ApartmentId,
            MoveInDate = user.MoveInDate,
            MoveOutDate = user.MoveOutDate,
            Note = user.Note
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            Apartments = await _context.Apartments
                .Where(a => a.Status != "Maintenance")
                .OrderBy(a => a.ApartmentNumber)
                .ToListAsync();
            return Page();
        }

        var user = await _context.Users.FindAsync(Input.UserId);

        if (user == null || user.IsDeleted)
        {
            return NotFound();
        }

        // Check email uniqueness if changed
        if (Input.Email != user.Email && await _context.Users.AnyAsync(u => u.Email == Input.Email))
        {
            ModelState.AddModelError("Input.Email", "Email đã được sử dụng bởi người dùng khác.");
            return Page();
        }

        user.FullName = Input.FullName;
        user.Email = Input.Email;
        user.PhoneNumber = Input.PhoneNumber;
        user.Role = Input.Role;
        user.IsActive = Input.IsActive;
        user.UpdatedAt = DateTime.Now;

        // Resident fields
        user.DateOfBirth = Input.DateOfBirth;
        user.IdentityCardNumber = Input.IdentityCardNumber;
        user.ResidentType = Input.ResidentType;
        user.ResidencyStatus = Input.ResidencyStatus;
        user.ApartmentId = Input.ApartmentId;
        user.MoveInDate = Input.MoveInDate;
        user.MoveOutDate = Input.MoveOutDate;
        user.Note = Input.Note;

        if (!string.IsNullOrEmpty(Input.NewPassword))
        {
            user.PasswordHash = _authService.HashPassword(Input.NewPassword);
        }

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
