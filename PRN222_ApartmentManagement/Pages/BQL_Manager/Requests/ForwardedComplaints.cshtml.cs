using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Requests;

[Authorize(Roles = "BQL_Manager")]
public class ForwardedComplaintsModel : PageModel
{
    private readonly IRequestService _requestService;

    public ForwardedComplaintsModel(IRequestService requestService)
    {
        _requestService = requestService;
    }

    [BindProperty(SupportsGet = true)]
    public RequestStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    public IReadOnlyList<Request> Requests { get; set; } = [];

    public int TotalCount { get; set; }
    public int OpenCount { get; set; }
    public int ClosedCount { get; set; }

    public bool HasFilter => StatusFilter.HasValue || !string.IsNullOrWhiteSpace(SearchQuery);

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetUserId();
        if (userId == null) return Forbid();

        var all = (await _requestService.GetForwardedComplaintsAsync(userId.Value)).ToList();

        TotalCount = all.Count;
        OpenCount = all.Count(r => r.Status is not (RequestStatus.Completed or RequestStatus.Cancelled or RequestStatus.Rejected));
        ClosedCount = all.Count(r => r.Status is RequestStatus.Completed or RequestStatus.Cancelled or RequestStatus.Rejected);

        IEnumerable<Request> filtered = all;

        if (StatusFilter.HasValue)
            filtered = filtered.Where(r => r.Status == StatusFilter.Value);

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var keyword = SearchQuery.Trim().ToLower();
            filtered = filtered.Where(r =>
                r.RequestNumber.ToLower().Contains(keyword) ||
                r.Title.ToLower().Contains(keyword) ||
                (r.Resident?.FullName?.ToLower().Contains(keyword) ?? false) ||
                (r.Apartment?.ApartmentNumber?.ToLower().Contains(keyword) ?? false) ||
                (r.EscalationReason?.ToLower().Contains(keyword) ?? false));
        }

        Requests = filtered.ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int requestId, RequestStatus newStatus)
    {
        if (requestId <= 0) return NotFound();

        var userId = GetUserId();
        if (userId == null) return Forbid();

        var request = await _requestService.GetRequestDetailAsync(requestId);
        if (request == null) return NotFound();
        if (request.RequestType != RequestType.Complaint || request.EscalatedTo != userId.Value) return Forbid();

        if (request.Status is RequestStatus.Completed or RequestStatus.Cancelled or RequestStatus.Rejected)
        {
            TempData["ErrorMessage"] = "Khiếu nại này đã được đóng, không thể cập nhật trạng thái.";
            return RedirectToPage(new { StatusFilter, SearchQuery });
        }

        var allowed = new[] {
            RequestStatus.InProgress,
            RequestStatus.Completed,
            RequestStatus.Cancelled,
            RequestStatus.Rejected
        };

        if (!allowed.Contains(newStatus))
        {
            TempData["ErrorMessage"] = "Trạng thái không hợp lệ.";
            return RedirectToPage(new { StatusFilter, SearchQuery });
        }

        try
        {
            await _requestService.UpdateStatusAsync(requestId, newStatus);
            TempData["SuccessMessage"] = "Đã cập nhật trạng thái khiếu nại.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { StatusFilter, SearchQuery });
    }

    private int? GetUserId()
    {
        var str = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return str != null && int.TryParse(str, out var id) ? id : null;
    }
}
