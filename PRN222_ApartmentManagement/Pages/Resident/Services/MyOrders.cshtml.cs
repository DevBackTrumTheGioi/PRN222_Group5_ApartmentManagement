using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Services;

[Authorize(Roles = "Resident")]
public class MyOrdersModel : PageModel
{
    private readonly IServiceManagementService _serviceManagementService;

    public MyOrdersModel(IServiceManagementService serviceManagementService)
    {
        _serviceManagementService = serviceManagementService;
    }

    [BindProperty(SupportsGet = true)]
    public ServiceOrderStatus? StatusFilter { get; set; }

    public IReadOnlyList<ServiceOrder> Orders { get; set; } = [];

    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int InProgressCount { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var allOrders = await _serviceManagementService.GetResidentOrdersAsync(userId.Value, null);
        TotalCount = allOrders.Count;
        PendingCount = allOrders.Count(o => o.Status == ServiceOrderStatus.Pending);
        InProgressCount = allOrders.Count(o => o.Status is ServiceOrderStatus.Confirmed or ServiceOrderStatus.InProgress);
        CompletedCount = allOrders.Count(o => o.Status == ServiceOrderStatus.Completed);
        CancelledCount = allOrders.Count(o => o.Status is ServiceOrderStatus.Cancelled or ServiceOrderStatus.Rejected);

        Orders = StatusFilter.HasValue
            ? allOrders.Where(o => o.Status == StatusFilter.Value).ToList()
            : allOrders;

        return Page();
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
