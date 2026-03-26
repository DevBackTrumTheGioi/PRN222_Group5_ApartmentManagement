using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Visitors;

[Authorize(Roles = "Resident")]
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

    public IReadOnlyList<Visitor> Visitors { get; set; } = [];

    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int TodayCount { get; set; }
    public int ClosedCount { get; set; }

    public bool HasFilter => StatusFilter.HasValue || VisitDateFilter.HasValue;

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var allVisitors = await _visitorManagementService.GetResidentVisitorsAsync(userId.Value, null, null);
        var today = DateTime.Now.Date;

        TotalCount = allVisitors.Count;
        PendingCount = allVisitors.Count(v => v.Status == VisitorStatus.Pending);
        TodayCount = allVisitors.Count(v => v.VisitDate == today);
        ClosedCount = allVisitors.Count(v => v.Status is VisitorStatus.CheckedOut or VisitorStatus.Cancelled or VisitorStatus.Rejected);

        Visitors = await _visitorManagementService.GetResidentVisitorsAsync(userId.Value, StatusFilter, VisitDateFilter);
        return Page();
    }

    public async Task<IActionResult> OnPostCancelAsync(int visitorId)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var result = await _visitorManagementService.CancelResidentVisitorAsync(visitorId, userId.Value);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToPage(new { StatusFilter, VisitDateFilter });
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
