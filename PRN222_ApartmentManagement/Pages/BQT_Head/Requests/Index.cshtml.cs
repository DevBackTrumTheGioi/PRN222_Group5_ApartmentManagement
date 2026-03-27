using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQT_Head.Requests;

[Authorize(Roles = "BQT_Head")]
public class IndexModel : PageModel
{
    private readonly IRequestService _requestService;

    public IndexModel(IRequestService requestService)
    {
        _requestService = requestService;
    }

    [BindProperty(SupportsGet = true)]
    public RequestType? TypeFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public RequestPriority? PriorityFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    [StringLength(100, ErrorMessage = "Từ khóa tìm kiếm tối đa 100 kí tự.")]
    public string? SearchQuery { get; set; }

    public IReadOnlyList<Request> Requests { get; set; } = [];

    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int InProgressCount { get; set; }
    public int HighPriorityCount { get; set; }

    public bool HasFilter => TypeFilter.HasValue || PriorityFilter.HasValue || !string.IsNullOrWhiteSpace(SearchQuery);

    public async Task<IActionResult> OnGetAsync()
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Bộ lọc không hợp lệ, vui lòng thử lại.";
            TypeFilter = null;
            PriorityFilter = null;
            SearchQuery = null;
        }

        if (TypeFilter == RequestType.Complaint)
        {
            TempData["ErrorMessage"] = "Khiếu nại được theo dõi ở trang riêng.";
            TypeFilter = null;
        }

        var allRequests = (await _requestService.GetOutstandingNonComplaintRequestsAsync()).ToList();

        TotalCount = allRequests.Count;
        PendingCount = allRequests.Count(r => r.Status == RequestStatus.Pending);
        InProgressCount = allRequests.Count(r => r.Status == RequestStatus.InProgress);
        HighPriorityCount = allRequests.Count(r => r.Priority is RequestPriority.High or RequestPriority.Emergency);

        IEnumerable<Request> filtered = allRequests;

        if (TypeFilter.HasValue)
        {
            filtered = filtered.Where(r => r.RequestType == TypeFilter.Value);
        }

        if (PriorityFilter.HasValue)
        {
            filtered = filtered.Where(r => r.Priority == PriorityFilter.Value);
        }

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var keyword = SearchQuery.Trim();
            filtered = filtered.Where(r =>
                r.RequestNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                r.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                (r.Resident?.FullName?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (r.Apartment?.ApartmentNumber?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        Requests = filtered.ToList();
        return Page();
    }
}
