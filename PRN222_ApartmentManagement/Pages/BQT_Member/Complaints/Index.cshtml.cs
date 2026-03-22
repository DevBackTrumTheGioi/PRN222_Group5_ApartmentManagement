using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQT_Member.Complaints;

[Authorize(Roles = "BQT_Member")]
public class IndexModel : PageModel
{
    private readonly IRequestService _requestService;

    public IndexModel(IRequestService requestService)
    {
        _requestService = requestService;
    }

    [BindProperty(SupportsGet = true)]
    public RequestStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    [StringLength(100, ErrorMessage = "Từ khóa tìm kiếm tối đa 100 kí tự.")]
    public string? SearchQuery { get; set; }

    public IReadOnlyList<Request> Complaints { get; set; } = [];

    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int InProgressCount { get; set; }
    public int ClosedCount { get; set; }

    public bool HasFilter => StatusFilter.HasValue || !string.IsNullOrWhiteSpace(SearchQuery);

    public async Task<IActionResult> OnGetAsync()
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Bộ lọc không hợp lệ, vui lòng thử lại";
            StatusFilter = null;
            SearchQuery = null;
        }

        if (StatusFilter.HasValue && !Enum.IsDefined(StatusFilter.Value))
        {
            TempData["ErrorMessage"] = "Trạng thái lọc không hợp lệ";
            StatusFilter = null;
        }

        var allComplaints = (await _requestService.GetComplaintsAsync()).ToList();

        TotalCount = allComplaints.Count;
        PendingCount = allComplaints.Count(r => r.Status == RequestStatus.Pending);
        InProgressCount = allComplaints.Count(r => r.Status == RequestStatus.InProgress);
        ClosedCount = allComplaints.Count(r => r.Status is RequestStatus.Completed or RequestStatus.Cancelled or RequestStatus.Rejected);

        IEnumerable<Request> filtered = allComplaints;

        if (StatusFilter.HasValue)
            filtered = filtered.Where(r => r.Status == StatusFilter.Value);

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var keyword = SearchQuery.Trim().ToLowerInvariant();
            filtered = filtered.Where(r =>
                r.RequestNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                r.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                (r.Resident?.FullName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (r.Apartment?.ApartmentNumber?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        Complaints = filtered
            .OrderBy(r => r.Status switch
            {
                RequestStatus.Pending => 0,
                RequestStatus.InProgress => 1,
                RequestStatus.Completed => 2,
                RequestStatus.Cancelled => 3,
                RequestStatus.Rejected => 4,
                _ => 5
            })
            .ThenByDescending(r => (int)r.Priority)
            .ThenBy(r => r.CreatedAt)
            .ToList();

        return Page();
    }
}
