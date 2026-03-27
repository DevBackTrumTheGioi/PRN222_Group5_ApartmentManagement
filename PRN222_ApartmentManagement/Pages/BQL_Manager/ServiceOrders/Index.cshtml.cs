using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.ServiceOrders;

[Authorize(Policy = "AdminAndBQLManager")]
public class IndexModel : PageModel
{
    private readonly IServiceManagementService _serviceManagementService;

    public IndexModel(IServiceManagementService serviceManagementService)
    {
        _serviceManagementService = serviceManagementService;
    }

    [BindProperty(SupportsGet = true)]
    public ServiceOrderStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? ServiceTypeIdFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    public IReadOnlyList<ServiceOrder> Orders { get; set; } = [];
    public IReadOnlyList<ServiceType> ServiceTypes { get; set; } = [];
    public IReadOnlyList<User> StaffList { get; set; } = [];

    public int TotalCount { get; set; }
    public int PendingCount { get; set; }
    public int ConfirmedCount { get; set; }
    public int InProgressCount { get; set; }
    public int CompletedCount { get; set; }

    public bool HasFilter => StatusFilter.HasValue || ServiceTypeIdFilter.HasValue || !string.IsNullOrWhiteSpace(SearchQuery);

    public async Task OnGetAsync()
    {
        var allOrders = await _serviceManagementService.GetManagerOrdersAsync(null, null, null);
        TotalCount = allOrders.Count;
        PendingCount = allOrders.Count(o => o.Status == ServiceOrderStatus.Pending);
        ConfirmedCount = allOrders.Count(o => o.Status == ServiceOrderStatus.Confirmed);
        InProgressCount = allOrders.Count(o => o.Status == ServiceOrderStatus.InProgress);
        CompletedCount = allOrders.Count(o => o.Status == ServiceOrderStatus.Completed);

        Orders = await _serviceManagementService.GetManagerOrdersAsync(StatusFilter, ServiceTypeIdFilter, SearchQuery);
        ServiceTypes = await _serviceManagementService.GetManagerServiceTypesAsync(null, null);
        StaffList = await _serviceManagementService.GetAvailableStaffAsync();
    }

    public async Task<IActionResult> OnPostAssignAsync(int serviceOrderId, int staffId)
    {
        var result = await _serviceManagementService.AssignOrderAsync(serviceOrderId, staffId);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToPage(new { StatusFilter, ServiceTypeIdFilter, SearchQuery });
    }
}
