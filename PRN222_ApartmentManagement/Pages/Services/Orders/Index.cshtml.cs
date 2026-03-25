using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;

namespace PRN222_ApartmentManagement.Pages.Orders;

[Authorize(Roles = "BQL_Manager,BQL_Staff")]
public class IndexModel : PageModel
{
    private readonly IServiceOrderRepository _orderRepo;

    public IndexModel(IServiceOrderRepository orderRepo)
    {
        _orderRepo = orderRepo;
    }

    [BindProperty(SupportsGet = true)]
    public ServiceOrderStatus? StatusFilter { get; set; }

    public List<ServiceOrder> Orders { get; set; } = new();

    public async Task OnGetAsync()
    {
        if (StatusFilter.HasValue)
        {
            Orders = (await _orderRepo.GetByStatusAsync(StatusFilter.Value)).ToList();
        }
        else
        {
            // load recent orders with related details so navigation properties are populated
            Orders = (await _orderRepo.GetAllWithDetailsAsync()).OrderByDescending(o => o.CreatedAt).Take(200).ToList();
        }
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int id, ServiceOrderStatus newStatus)
    {
        if (id <= 0) return BadRequest();

        var order = await _orderRepo.GetWithDetailsAsync(id);
        if (order == null) return NotFound();

        // Role checks
        bool isStaff = User.IsInRole("BQL_Staff");
        bool isManager = User.IsInRole("BQL_Manager");

        // Staff allowed transitions: Pending -> Confirmed, Confirmed -> InProgress, InProgress -> Completed
        // Manager allowed to move to InProgress or Completed as well

        var allowedForStaff = new[] { ServiceOrderStatus.Confirmed, ServiceOrderStatus.InProgress, ServiceOrderStatus.Completed };
        var allowedForManager = new[] { ServiceOrderStatus.InProgress, ServiceOrderStatus.Completed, ServiceOrderStatus.Cancelled };

        if (isStaff && !allowedForStaff.Contains(newStatus)) return Forbid();
        if (isManager && !allowedForManager.Contains(newStatus) && !isStaff) return Forbid();

        order.Status = newStatus;
        if (newStatus == ServiceOrderStatus.InProgress)
        {
            order.AssignedAt ??= DateTime.Now;
        }
        if (newStatus == ServiceOrderStatus.Completed)
        {
            order.CompletedAt = DateTime.Now;
        }

        order.UpdatedAt = DateTime.Now;

        await _orderRepo.UpdateAsync(order);

        TempData["SuccessMessage"] = "Đã cập nhật trạng thái.";
        return RedirectToPage(new { StatusFilter });
    }
}
