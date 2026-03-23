using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Residents;

[Authorize(Roles = "BQL_Manager")]
public class IndexModel : PageModel
{
    private readonly IUserRepository _userRepository;

    public IndexModel(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public List<ResidentRowVm> Rows { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public int TotalItems { get; set; }
    public int TotalPages => TotalItems == 0 ? 1 : (int)Math.Ceiling(TotalItems / (double)PageSize);

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        TempData.Remove("StatusMessage");

        var result = await _userRepository.GetPagedResidentsAsync(
            SearchTerm,
            StatusFilter,
            PageIndex,
            PageSize);

        TotalItems = result.TotalCount;

        Rows = result.Items.Select(u => new ResidentRowVm
        {
            UserId = u.UserId,
            Username = u.Username,
            FullName = u.FullName,
            Email = u.Email ?? "—",
            PhoneNumber = u.PhoneNumber ?? "—",
            IdentityCardNumber = u.IdentityCardNumber ?? "—",
            IsActive = u.IsActive,
            IsActiveLabel = u.IsActive ? "Hoạt động" : "Chưa xác thực",
            IsActiveCss = u.IsActive
                ? "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200"
                : "bg-red-50 text-red-600 ring-1 ring-red-200",
            IsFaceRegistered = u.IsFaceRegistered,
            CreatedAt = u.CreatedAt.ToString("dd/MM/yyyy")
        }).ToList();
    }
}

public class ResidentRowVm
{
    public int UserId { get; set; }
    public string Username { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string IdentityCardNumber { get; set; } = "";
    public bool IsActive { get; set; }
    public string IsActiveLabel { get; set; } = "";
    public string IsActiveCss { get; set; } = "";
    public bool IsFaceRegistered { get; set; }
    public string CreatedAt { get; set; } = "";
}
