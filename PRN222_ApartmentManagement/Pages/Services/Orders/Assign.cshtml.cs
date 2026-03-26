using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace PRN222_ApartmentManagement.Pages.Orders;

[Authorize(Roles = "BQL_Manager")]
public class AssignModel : PageModel
{
    private readonly IServiceOrderRepository _orderRepo;
    private readonly IUserRepository _userRepo;

    public AssignModel(IServiceOrderRepository orderRepo, IUserRepository userRepo)
    {
        _orderRepo = orderRepo;
        _userRepo = userRepo;
    }

    public ServiceOrder? Order { get; set; }
    public IReadOnlyList<User> StaffOptions { get; set; } = new List<User>();

    [BindProperty]
    public int SelectedStaffId { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id <= 0) return NotFound();
        Order = await _orderRepo.GetWithDetailsAsync(id);
        if (Order == null) return NotFound();

        // Only allow assign when order has been accepted by staff (Confirmed) and not already assigned
        if (Order.Status != ServiceOrderStatus.Confirmed || Order.AssignedTo != null)
        {
            TempData["Error"] = "Đơn chưa ở trạng thái 'Confirmed' hoặc đã được phân công.";
            return RedirectToPage("/Services/Orders/Details", new { id });
        }

        var all = await _userRepo.GetAllAsync();
        StaffOptions = all.Where(u => u.Role == UserRole.BQL_Staff && u.IsActive && !u.IsDeleted).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (SelectedStaffId <= 0) return BadRequest();

        var order = await _orderRepo.GetWithDetailsAsync(id);
        if (order == null) return NotFound();

        // Validate state again on post
        if (order.Status != ServiceOrderStatus.Confirmed || order.AssignedTo != null)
        {
            TempData["Error"] = "Không thể phân công. Đơn phải ở trạng thái 'Confirmed' và chưa được phân công.";
            return RedirectToPage("/Services/Orders/Details", new { id });
        }

        order.AssignedTo = SelectedStaffId;
        order.AssignedAt = DateTime.Now;
        order.Status = ServiceOrderStatus.InProgress;
        order.UpdatedAt = DateTime.Now;

        await _orderRepo.UpdateAsync(order);

        TempData["Success"] = "Phân công thành công.";
        return RedirectToPage("/Services/Orders/Details", new { id });
    }

    // AJAX handler: return staff info and potential schedule conflicts for this order's requested date/time slot
    public async Task<IActionResult> OnGetStaffInfoAsync(int id, int staffId)
    {
        if (id <= 0) return BadRequest("Thiếu id đơn.");
        if (staffId <= 0) return BadRequest("Thiếu id nhân viên.");

        var order = await _orderRepo.GetWithDetailsAsync(id);
        if (order == null) return NotFound("Đơn không tồn tại.");

        var staff = await _userRepo.GetByIdAsync(staffId);
        if (staff == null) return NotFound("Nhân viên không tồn tại.");

        // get assigned orders for the staff and check conflicts with requested date/time slot
        var assigned = (await _orderRepo.GetByAssignedStaffAsync(staffId)).ToList();

        var conflicts = assigned
            .Where(a => a.ServiceOrderId != id // ignore current order
                        && a.RequestedDate.Date == order.RequestedDate.Date
                        && string.Equals(a.RequestedTimeSlot ?? string.Empty, order.RequestedTimeSlot ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                        && a.Status != ServiceOrderStatus.Completed
                        && a.Status != ServiceOrderStatus.Cancelled)
            .Select(a => new
            {
                a.ServiceOrderId,
                a.OrderNumber,
                RequestedDate = a.RequestedDate.ToString("yyyy-MM-dd"),
                TimeSlot = a.RequestedTimeSlot,
                Status = a.Status.ToString()
            })
            .ToList();

        var result = new
        {
            Staff = new
            {
                staff.UserId,
                staff.FullName,
                staff.Email,
                staff.PhoneNumber,
                staff.IsActive
            },
            RequestedDate = order.RequestedDate.ToString("yyyy-MM-dd"),
            RequestedTimeSlot = order.RequestedTimeSlot,
            Conflicts = conflicts,
            CanAssign = !conflicts.Any()
        };

        return new JsonResult(result);
    }
}
