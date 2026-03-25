using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Services;

[Authorize(Roles = "BQL_Manager,BQL_Staff,Admin")]
public class DashboardModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public DashboardModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public ServiceDashboardViewModel Data { get; set; } = new();
    public List<ServiceOrder> RecentOrders { get; set; } = new();

    public async Task OnGetAsync()
    {
        Data.TotalOrders = await _context.ServiceOrders.CountAsync();
        Data.Pending = await _context.ServiceOrders.CountAsync(so => so.Status == PRN222_ApartmentManagement.Models.Enums.ServiceOrderStatus.Pending);
        Data.InProgress = await _context.ServiceOrders.CountAsync(so => so.Status == PRN222_ApartmentManagement.Models.Enums.ServiceOrderStatus.InProgress);
        Data.Completed = await _context.ServiceOrders.CountAsync(so => so.Status == PRN222_ApartmentManagement.Models.Enums.ServiceOrderStatus.Completed);

        RecentOrders = await _context.ServiceOrders
            .Include(so => so.ServiceType)
            .OrderByDescending(so => so.CreatedAt)
            .Take(10)
            .ToListAsync();
    }
}

public class ServiceDashboardViewModel
{
    public int TotalOrders { get; set; }
    public int Pending { get; set; }
    public int InProgress { get; set; }
    public int Completed { get; set; }
}
