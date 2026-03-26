using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff.ServiceOrders;

[Authorize(Roles = "BQL_Staff")]
public class AssignedModel : PageModel
{
    private readonly IServiceManagementService _serviceManagementService;

    public AssignedModel(IServiceManagementService serviceManagementService)
    {
        _serviceManagementService = serviceManagementService;
    }

    [BindProperty(SupportsGet = true)]
    public ServiceOrderStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    public IReadOnlyList<ServiceOrder> Orders { get; set; } = [];

    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int ConfirmedCount { get; set; }
    public int InProgressCount { get; set; }
    public int CompletedCount { get; set; }

    public bool HasFilter => StatusFilter.HasValue || !string.IsNullOrWhiteSpace(SearchQuery);

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var allOrders = await _serviceManagementService.GetAssignedOrdersAsync(userId.Value, null, null);
        TotalCount = allOrders.Count;
        PendingCount = allOrders.Count(o => o.Status == ServiceOrderStatus.Pending);
        ConfirmedCount = allOrders.Count(o => o.Status == ServiceOrderStatus.Confirmed);
        InProgressCount = allOrders.Count(o => o.Status == ServiceOrderStatus.InProgress);
        CompletedCount = allOrders.Count(o => o.Status == ServiceOrderStatus.Completed);

        Orders = await _serviceManagementService.GetAssignedOrdersAsync(userId.Value, StatusFilter, SearchQuery);
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int serviceOrderId, ServiceOrderStatus newStatus)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var result = await _serviceManagementService.UpdateAssignedOrderAsync(
            serviceOrderId,
            userId.Value,
            newStatus,
            null,
            null,
            null,
            null);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToPage(new { StatusFilter, SearchQuery });
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
