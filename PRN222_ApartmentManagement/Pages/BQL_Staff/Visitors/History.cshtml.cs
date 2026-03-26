using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff.Visitors;

[Authorize(Roles = "BQL_Staff")]
public class HistoryModel : PageModel
{
    private readonly IVisitorManagementService _visitorManagementService;

    public HistoryModel(IVisitorManagementService visitorManagementService)
    {
        _visitorManagementService = visitorManagementService;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    public IReadOnlyList<Visitor> Visitors { get; set; } = [];

    public int TotalCount { get; set; }
    public int CheckedInCount { get; set; }
    public int CheckedOutCount { get; set; }
    public int RejectedCount { get; set; }
    public int CancelledCount { get; set; }

    public bool HasFilter => FromDate.HasValue || ToDate.HasValue || !string.IsNullOrWhiteSpace(SearchQuery);

    public async Task OnGetAsync()
    {
        var checkedInVisitors = await _visitorManagementService.GetStaffVisitorsAsync(VisitorStatus.CheckedIn, null, SearchQuery);
        var checkedOutVisitors = await _visitorManagementService.GetStaffVisitorsAsync(VisitorStatus.CheckedOut, null, SearchQuery);
        var rejectedVisitors = await _visitorManagementService.GetStaffVisitorsAsync(VisitorStatus.Rejected, null, SearchQuery);
        var cancelledVisitors = await _visitorManagementService.GetStaffVisitorsAsync(VisitorStatus.Cancelled, null, SearchQuery);

        IEnumerable<Visitor> history = checkedInVisitors
            .Concat(checkedOutVisitors)
            .Concat(rejectedVisitors)
            .Concat(cancelledVisitors)
            .GroupBy(v => v.VisitorId)
            .Select(g => g.First());

        if (FromDate.HasValue)
        {
            history = history.Where(v => v.VisitDate >= FromDate.Value.Date);
        }

        if (ToDate.HasValue)
        {
            history = history.Where(v => v.VisitDate <= ToDate.Value.Date);
        }

        var historyList = history
            .OrderByDescending(v => v.VisitDate)
            .ThenByDescending(v => v.CheckOutTime ?? v.CheckInTime ?? v.CreatedAt)
            .ToList();

        Visitors = historyList;
        TotalCount = historyList.Count;
        CheckedInCount = historyList.Count(v => v.Status == VisitorStatus.CheckedIn);
        CheckedOutCount = historyList.Count(v => v.Status == VisitorStatus.CheckedOut);
        RejectedCount = historyList.Count(v => v.Status == VisitorStatus.Rejected);
        CancelledCount = historyList.Count(v => v.Status == VisitorStatus.Cancelled);
    }
}
