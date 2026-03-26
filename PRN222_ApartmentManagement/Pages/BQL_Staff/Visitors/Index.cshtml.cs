using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff.Visitors;

[Authorize(Roles = "BQL_Staff")]
public class IndexModel : PageModel
{
    private readonly IVisitorManagementService _visitorManagementService;

    public IndexModel(IVisitorManagementService visitorManagementService)
    {
        _visitorManagementService = visitorManagementService;
    }

    [BindProperty(SupportsGet = true)]
    public VisitorStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? VisitDateFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    public IReadOnlyList<Visitor> Visitors { get; set; } = [];

    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int CheckedInCount { get; set; }
    public int UpcomingCount { get; set; }
    public int ClosedCount { get; set; }

    public bool HasFilter => StatusFilter.HasValue || VisitDateFilter.HasValue || !string.IsNullOrWhiteSpace(SearchQuery);

    public async Task<IActionResult> OnGetAsync()
    {
        var defaultQueue = (await _visitorManagementService.GetStaffVisitorsAsync(null, null, null))
            .Where(IsOperationalWindow)
            .ToList();
        var today = DateTime.Now.Date;
        var tomorrow = today.AddDays(1);

        TotalCount = defaultQueue.Count;
        PendingCount = defaultQueue.Count(v => v.Status == VisitorStatus.Pending);
        CheckedInCount = defaultQueue.Count(v => v.Status == VisitorStatus.CheckedIn);
        UpcomingCount = defaultQueue.Count(v => v.VisitDate == today || v.VisitDate == tomorrow);
        ClosedCount = defaultQueue.Count(v => v.Status is VisitorStatus.CheckedOut or VisitorStatus.Cancelled or VisitorStatus.Rejected);

        var filteredVisitors = await _visitorManagementService.GetStaffVisitorsAsync(StatusFilter, null, SearchQuery);
        Visitors = filteredVisitors
            .Where(IsOperationalWindow)
            .Where(v => !VisitDateFilter.HasValue || v.VisitDate == VisitDateFilter.Value.Date)
            .ToList();
        return Page();
    }

    public async Task<IActionResult> OnPostCheckInAsync(int visitorId)
    {
        var result = await _visitorManagementService.CheckInVisitorAsync(visitorId);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToCurrentFilters();
    }

    public async Task<IActionResult> OnPostCheckOutAsync(int visitorId)
    {
        var result = await _visitorManagementService.CheckOutVisitorAsync(visitorId);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToCurrentFilters();
    }

    public async Task<IActionResult> OnPostRejectAsync(int visitorId)
    {
        var result = await _visitorManagementService.RejectVisitorAsync(visitorId);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToCurrentFilters();
    }

    private RedirectToPageResult RedirectToCurrentFilters()
    {
        return RedirectToPage(new
        {
            StatusFilter,
            VisitDateFilter,
            SearchQuery
        });
    }

    private static bool IsOperationalWindow(Visitor visitor)
    {
        var today = DateTime.Now.Date;
        var tomorrow = today.AddDays(1);
        return visitor.VisitDate == today || visitor.VisitDate == tomorrow;
    }
}
