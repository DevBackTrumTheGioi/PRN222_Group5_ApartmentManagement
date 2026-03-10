using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Requests;

[Authorize(Roles = "Resident")]
public class MyRequestsModel : PageModel
{
    private readonly IRequestService _requestService;

    public MyRequestsModel(IRequestService requestService)
    {
        _requestService = requestService;
    }

    public IReadOnlyList<Request> Requests { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public RequestStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public RequestType? TypeFilter { get; set; }

    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int InProgressCount { get; set; }
    public int CompletedCount { get; set; }
    public int ClosedCount { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdStr == null || !int.TryParse(userIdStr, out var userId))
            return Forbid();

        var all = (await _requestService.GetMyRequestsAsync(userId)).ToList();

        TotalCount = all.Count;
        PendingCount = all.Count(r => r.Status == RequestStatus.Pending);
        InProgressCount = all.Count(r => r.Status == RequestStatus.InProgress);
        CompletedCount = all.Count(r => r.Status == RequestStatus.Completed);
        ClosedCount = all.Count(r => r.Status == RequestStatus.Cancelled || r.Status == RequestStatus.Rejected);

        Requests = all;

        if (StatusFilter.HasValue)
            Requests = Requests.Where(r => r.Status == StatusFilter.Value).ToList();

        if (TypeFilter.HasValue)
            Requests = Requests.Where(r => r.RequestType == TypeFilter.Value).ToList();

        return Page();
    }
}
