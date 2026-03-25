using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Services;

[Authorize(Policy = "ResidentOnly")]
public class IndexModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public IndexModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public class ServiceItem
    {
        public ServiceType ServiceType { get; set; } = null!;
        public decimal? CurrentPrice { get; set; }
    }

    public List<ServiceItem> Services { get; set; } = new();

    public async Task OnGetAsync()
    {
        var types = await _context.ServiceTypes
            .Where(st => st.IsActive && !st.IsDeleted)
            .OrderBy(st => st.ServiceTypeName)
            .ToListAsync();

        var today = DateTime.Today;

        foreach (var t in types)
        {
            var price = await _context.ServicePrices
                .Where(sp => sp.ServiceTypeId == t.ServiceTypeId && sp.EffectiveFrom <= today && (sp.EffectiveTo == null || sp.EffectiveTo >= today))
                .OrderByDescending(sp => sp.EffectiveFrom)
                .FirstOrDefaultAsync();

            Services.Add(new ServiceItem
            {
                ServiceType = t,
                CurrentPrice = price?.UnitPrice
            });
        }
    }
}
