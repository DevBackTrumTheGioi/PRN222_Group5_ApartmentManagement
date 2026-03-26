using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Services;

[Authorize(Roles = "Resident")]
public class IndexModel : PageModel
{
    private readonly IServiceManagementService _serviceManagementService;

    public IndexModel(IServiceManagementService serviceManagementService)
    {
        _serviceManagementService = serviceManagementService;
    }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public IReadOnlyList<ServiceType> Services { get; set; } = [];

    public async Task OnGetAsync()
    {
        Services = await _serviceManagementService.GetResidentActiveServiceTypesAsync(SearchTerm);
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
