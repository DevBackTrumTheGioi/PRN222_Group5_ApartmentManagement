using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Requests;

[Authorize(Roles = "BQL_Manager")]
public class IndexModel : PageModel
{
    private readonly IRequestService _requestService;
    private readonly IUserRepository _userRepository;

    public IndexModel(IRequestService requestService, IUserRepository userRepository)
    {
        _requestService = requestService;
        _userRepository = userRepository;
    }

    // --- Filter params ---
    [BindProperty(SupportsGet = true)]
    public RequestStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public RequestType? TypeFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public RequestPriority? PriorityFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    // --- Data ---
    public IReadOnlyList<Request> Requests { get; set; } = [];
    public IReadOnlyList<User> StaffList { get; set; } = [];

    // --- Stats ---
    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int InProgressCount { get; set; }
    public int CompletedCount { get; set; }
    public int ClosedCount { get; set; }

    public bool HasFilter => StatusFilter.HasValue || TypeFilter.HasValue || PriorityFilter.HasValue || !string.IsNullOrWhiteSpace(SearchQuery);

    public async Task<IActionResult> OnGetAsync()
    {
        // Load stats t? toàn b? (tr??c filter)
        var all = (await _requestService.GetAllRequestsAsync()).ToList();
        TotalCount = all.Count;
        PendingCount = all.Count(r => r.Status == RequestStatus.Pending);
        InProgressCount = all.Count(r => r.Status == RequestStatus.InProgress);
        CompletedCount = all.Count(r => r.Status == RequestStatus.Completed);
        ClosedCount = all.Count(r => r.Status is RequestStatus.Cancelled or RequestStatus.Rejected);

        // Load filtered list
        var filtered = await _requestService.GetAllRequestsAsync(StatusFilter, TypeFilter, PriorityFilter, SearchQuery);
        Requests = filtered.ToList();

        // Load danh sách Staff ?? phân công
        var allUsers = await _userRepository.GetAllAsync();
        StaffList = allUsers
            .Where(u => u.Role == UserRole.BQL_Staff && u.IsActive && !u.IsDeleted)
            .OrderBy(u => u.FullName)
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAssignAsync(int requestId, int staffId)
    {
        if (requestId <= 0 || staffId <= 0)
        {
            TempData["ErrorMessage"] = "Thông tin phân công không h?p l?.";
            return RedirectToPage();
        }

        try
        {
            await _requestService.AssignRequestAsync(requestId, staffId);
            TempData["SuccessMessage"] = "Phân công yêu c?u thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { StatusFilter, TypeFilter, PriorityFilter, SearchQuery });
    }

    public async Task<IActionResult> OnPostUpdatePriorityAsync(int requestId, RequestPriority newPriority)
    {
        if (requestId <= 0 || !Enum.IsDefined(typeof(RequestPriority), newPriority))
        {
            TempData["ErrorMessage"] = "Thông tin không h?p l?.";
            return RedirectToPage(new { StatusFilter, TypeFilter, PriorityFilter, SearchQuery });
        }

        try
        {
            await _requestService.UpdatePriorityAsync(requestId, newPriority);
            TempData["SuccessMessage"] = "?ã c?p nh?t m?c ?? ?u tiên.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { StatusFilter, TypeFilter, PriorityFilter, SearchQuery });
    }
}
