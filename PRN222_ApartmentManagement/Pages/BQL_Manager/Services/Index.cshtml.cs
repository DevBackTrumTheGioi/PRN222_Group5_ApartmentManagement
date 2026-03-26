using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Services;

[Authorize(Policy = "AdminAndBQLManager")]
public class IndexModel : PageModel
{
    private readonly IServiceManagementService _serviceManagementService;

    public IndexModel(IServiceManagementService serviceManagementService)
    {
        _serviceManagementService = serviceManagementService;
    }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? ActiveFilter { get; set; }

    public IReadOnlyList<ServiceType> ServiceTypes { get; set; } = [];

    public int TotalCount { get; set; }
    public int ActiveCount { get; set; }
    public int InactiveCount { get; set; }

    public bool HasFilter => !string.IsNullOrWhiteSpace(SearchTerm) || ActiveFilter.HasValue;

    public async Task OnGetAsync()
    {
        var allServiceTypes = await _serviceManagementService.GetManagerServiceTypesAsync(null, null);
        TotalCount = allServiceTypes.Count;
        ActiveCount = allServiceTypes.Count(st => st.IsActive);
        InactiveCount = allServiceTypes.Count(st => !st.IsActive);

        ServiceTypes = await _serviceManagementService.GetManagerServiceTypesAsync(SearchTerm, ActiveFilter);
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(int serviceTypeId)
    {
        if (!User.IsInRole("BQL_Manager"))
        {
            return Forbid();
        }

        var result = await _serviceManagementService.ToggleServiceTypeStatusAsync(serviceTypeId);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToPage(new { SearchTerm, ActiveFilter });
    }

    public ServicePrice? GetCurrentPrice(ServiceType serviceType)
    {
        var today = DateTime.Now.Date;
        return serviceType.ServicePrices
            .Where(sp => sp.EffectiveFrom <= today && (!sp.EffectiveTo.HasValue || sp.EffectiveTo.Value >= today))
            .OrderByDescending(sp => sp.EffectiveFrom)
            .ThenByDescending(sp => sp.ServicePriceId)
            .FirstOrDefault();
    }
}
