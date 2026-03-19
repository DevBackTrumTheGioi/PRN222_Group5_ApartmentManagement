using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Admin.Users;

[Authorize(Policy = "AdminOnly")]
public class IndexModel : PageModel
{
    private readonly IUserManagementService _userManagementService;

    public IndexModel(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    public List<User> Users { get; set; } = new();

    public List<object> TableRows { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? RoleFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public int TotalItems { get; set; }
    public int TotalPages { get; set; }

    public async Task OnGetAsync()
    {
        var result = await _userManagementService.GetPagedUsersAsync(SearchTerm, RoleFilter, PageIndex, PageSize);
        Users = result.Users;
        TotalItems = result.TotalItems;
        TotalPages = result.TotalPages;

        if (PageIndex < 1) PageIndex = 1;
        if (PageIndex > TotalPages && TotalPages > 0) PageIndex = TotalPages;

        TableRows = Users.Select(u => (object)new {
            PrimaryId = u.UserId,
            User = new {
                Name = u.FullName,
                Subtitle = "@@" + u.Username,
                Avatar = string.Empty // Can add logic for initials-based avatar if needed
            },
            Contact = u.Email + "\n" + u.PhoneNumber,
            Role = u.Role?.ToString(),
            IsActive = u.IsActive,
            StatusText = u.IsActive ? "Hoạt động" : "Khóa",
            RoleBadgeClass = u.Role switch {
                UserRole.Admin => "bg-red-100 text-red-800",
                UserRole.Resident => "bg-blue-100 text-blue-800",
                _ => "bg-emerald-100 text-emerald-800"
            }
        }).ToList();
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(int id)
    {
        await _userManagementService.ToggleStatusAsync(id);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        await _userManagementService.SoftDeleteAsync(id);
        return RedirectToPage();
    }
}
