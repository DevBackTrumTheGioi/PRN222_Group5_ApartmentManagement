using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.Account;

[Authorize]
public class ProfileModel : PageModel
{
    private readonly ApartmentDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public ProfileModel(
        ApartmentDbContext context,
        IUserRepository userRepository,
        IAuthService authService)
    {
        _context = context;
        _userRepository = userRepository;
        _authService = authService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public User CurrentUser { get; set; } = new();
    public string RoleDisplayName { get; set; } = string.Empty;
    public string? ApartmentDisplay { get; set; }
    public string? SuccessMessage { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        [StringLength(200, ErrorMessage = "Họ và tên tối đa 200 ký tự")]
        [Display(Name = "Họ và tên")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(20, ErrorMessage = "CCCD/CMND tối đa 20 ký tự")]
        [Display(Name = "CCCD/CMND")]
        public string? IdentityCardNumber { get; set; }

        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await LoadCurrentUserAsync();
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        PopulateInput(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await LoadCurrentUserAsync();
        if (user == null)
        {
            return RedirectToPage("/Account/Login");
        }

        NormalizeInput();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!string.IsNullOrWhiteSpace(Input.Email) &&
            await _userRepository.EmailExistsAsync(Input.Email, user.UserId))
        {
            ModelState.AddModelError("Input.Email", "Email này đã được sử dụng.");
        }

        if (!string.IsNullOrWhiteSpace(Input.PhoneNumber) &&
            await _userRepository.PhoneExistsAsync(Input.PhoneNumber, user.UserId))
        {
            ModelState.AddModelError("Input.PhoneNumber", "Số điện thoại này đã được sử dụng.");
        }

        if (!string.IsNullOrWhiteSpace(Input.IdentityCardNumber) &&
            await _userRepository.IdentityCardExistsAsync(Input.IdentityCardNumber, user.UserId))
        {
            ModelState.AddModelError("Input.IdentityCardNumber", "CCCD/CMND này đã được sử dụng.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        user.FullName = Input.FullName.Trim();
        user.Email = Input.Email;
        user.PhoneNumber = Input.PhoneNumber;
        user.DateOfBirth = Input.DateOfBirth;
        user.IdentityCardNumber = Input.IdentityCardNumber;
        user.Note = Input.Note;
        user.UpdatedAt = DateTime.Now;

        await _userRepository.UpdateAsync(user);

        var refreshedTokens = await _authService.GenerateTokensForUserAsync(user.UserId, HttpContext.Connection.RemoteIpAddress?.ToString());
        if (refreshedTokens != null)
        {
            AuthCookieHelper.SetAuthCookies(HttpContext, refreshedTokens);
        }

        CurrentUser = user;
        RoleDisplayName = user.Role?.ToString() ?? "Chưa phân quyền";
        ApartmentDisplay = user.Apartment?.ApartmentNumber;
        SuccessMessage = "Thông tin hồ sơ đã được cập nhật.";

        return Page();
    }

    private async Task<User?> LoadCurrentUserAsync()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId))
        {
            return null;
        }

        CurrentUser = await _context.Users
            .Include(u => u.Apartment)
            .FirstOrDefaultAsync(u => u.UserId == userId && !u.IsDeleted) ?? new User();

        if (CurrentUser.UserId == 0)
        {
            return null;
        }

        RoleDisplayName = CurrentUser.Role?.ToString() ?? "Chưa phân quyền";
        ApartmentDisplay = CurrentUser.Apartment?.ApartmentNumber;

        return CurrentUser;
    }

    private void PopulateInput(User user)
    {
        Input = new InputModel
        {
            FullName = user.FullName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            DateOfBirth = user.DateOfBirth,
            IdentityCardNumber = user.IdentityCardNumber,
            Note = user.Note
        };
    }

    private void NormalizeInput()
    {
        Input.FullName = Input.FullName.Trim();
        Input.Email = string.IsNullOrWhiteSpace(Input.Email) ? null : Input.Email.Trim();
        Input.PhoneNumber = string.IsNullOrWhiteSpace(Input.PhoneNumber) ? null : Input.PhoneNumber.Trim();
        Input.IdentityCardNumber = string.IsNullOrWhiteSpace(Input.IdentityCardNumber) ? null : Input.IdentityCardNumber.Trim();
        Input.Note = string.IsNullOrWhiteSpace(Input.Note) ? null : Input.Note.Trim();
    }
}
