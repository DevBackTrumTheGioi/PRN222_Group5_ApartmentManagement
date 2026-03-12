using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff.Requests;

[Authorize(Roles = "BQL_Staff")]
public class AssignedModel : PageModel
{
    private readonly IRequestService _requestService;

    public AssignedModel(IRequestService requestService)
    {
        _requestService = requestService;
    }

    // --- Filter ---
    [BindProperty(SupportsGet = true)]
    public RequestStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public RequestPriority? PriorityFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    // --- Data ---
    public IReadOnlyList<Request> Requests { get; set; } = [];

    // --- Stats ---
    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int InProgressCount { get; set; }
    public int CompletedCount { get; set; }

    public bool HasFilter => StatusFilter.HasValue || PriorityFilter.HasValue || !string.IsNullOrWhiteSpace(SearchQuery);

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetUserId();
        if (userId == null) return Forbid();

        var all = (await _requestService.GetAssignedRequestsAsync(userId.Value)).ToList();

        TotalCount = all.Count;
        PendingCount = all.Count(r => r.Status == RequestStatus.Pending);
        InProgressCount = all.Count(r => r.Status == RequestStatus.InProgress);
        CompletedCount = all.Count(r => r.Status == RequestStatus.Completed);

        // Apply filters
        IEnumerable<Request> filtered = all;

        if (StatusFilter.HasValue)
            filtered = filtered.Where(r => r.Status == StatusFilter.Value);

        if (PriorityFilter.HasValue)
            filtered = filtered.Where(r => r.Priority == PriorityFilter.Value);

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var keyword = SearchQuery.Trim().ToLower();
            filtered = filtered.Where(r =>
                r.RequestNumber.ToLower().Contains(keyword) ||
                r.Title.ToLower().Contains(keyword) ||
                (r.Apartment?.ApartmentNumber?.ToLower().Contains(keyword) ?? false) ||
                (r.Resident?.FullName?.ToLower().Contains(keyword) ?? false));
        }

        Requests = filtered.ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int requestId, RequestStatus newStatus)
    {
        if (requestId <= 0) return NotFound();

        var userId = GetUserId();
        if (userId == null) return Forbid();

        // Verify ownership: staff chỉ được cập nhật yêu cầu được giao cho mình
        var request = await _requestService.GetRequestDetailAsync(requestId);
        if (request == null) return NotFound();
        if (request.AssignedTo != userId.Value) return Forbid();

        // Staff chỉ được chuyển sang InProgress hoặc Completed
        var allowed = new[] { RequestStatus.InProgress, RequestStatus.Completed };
        if (!allowed.Contains(newStatus))
        {
            TempData["ErrorMessage"] = "Trạng thái không hợp lệ.";
            return RedirectToPage(new { StatusFilter, PriorityFilter, SearchQuery });
        }

        try
        {
            await _requestService.UpdateStatusAsync(requestId, newStatus);
            TempData["SuccessMessage"] = newStatus == RequestStatus.Completed
                ? "Đã đánh dấu hoàn thành yêu cầu."
                : "Đã chuyển yêu cầu sang Đang xử lý.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { StatusFilter, PriorityFilter, SearchQuery });
    }

    private int? GetUserId()
    {
        var str = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return str != null && int.TryParse(str, out var id) ? id : null;
    }
}
