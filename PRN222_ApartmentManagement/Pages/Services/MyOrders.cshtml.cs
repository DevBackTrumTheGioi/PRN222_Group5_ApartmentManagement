using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Pages.Services;

[Authorize(Policy = "ResidentOnly")]
public class MyOrdersModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public MyOrdersModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public List<ServiceOrder> Orders { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            Orders = new();
            return;
        }

        Orders = await _context.ServiceOrders
            .Include(o => o.ServiceType)
            .Where(o => o.ResidentId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Forbid();
        }

        var order = await _context.ServiceOrders.FindAsync(id);
        if (order == null || order.ResidentId != userId) return NotFound();

        if (order.Status != ServiceOrderStatus.Pending)
        {
            TempData["Error"] = "Đơn không thể hủy ở trạng thái hiện tại.";
            return RedirectToPage();
        }

        order.Status = ServiceOrderStatus.Cancelled;
        order.CancelledAt = DateTime.Now;
        order.CancelReason = "Cancelled by resident";
        order.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã hủy đơn.";
        return RedirectToPage();
    }

    // Resident rating handler: only allowed when order is Completed
    public async Task<IActionResult> OnPostRateAsync(int id, int rating, string? comment)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return Forbid();
        }

        if (rating < 1 || rating > 5)
        {
            TempData["Error"] = "Đánh giá không hợp lệ.";
            return RedirectToPage();
        }

        var order = await _context.ServiceOrders.FindAsync(id);
        if (order == null || order.ResidentId != userId) return NotFound();

        if (order.Status != ServiceOrderStatus.Completed)
        {
            TempData["Error"] = "Chỉ có thể đánh giá khi đơn đã hoàn thành.";
            return RedirectToPage();
        }

        // prevent re-rating if already rated - update allowed? We'll block repeated rating
        if (order.Rating.HasValue)
        {
            TempData["Error"] = "Bạn đã đánh giá đơn này.";
            return RedirectToPage();
        }

        order.Rating = rating;
        order.ReviewComment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        order.ReviewedAt = DateTime.Now;
        order.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Cảm ơn bạn đã đánh giá.";
        return RedirectToPage();
    }

    // AJAX handler: return completed orders for current resident (history)
    public async Task<IActionResult> OnGetHistoryAsync()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            return new JsonResult(new { success = false, message = "Không xác thực người dùng." }) { StatusCode = 403 };
        }

        var completed = await _context.ServiceOrders
            .AsNoTracking()
            .Include(o => o.ServiceType)
            .Where(o => o.ResidentId == userId && o.Status == ServiceOrderStatus.Completed)
            .OrderByDescending(o => o.CompletedAt)
            .Select(o => new
            {
                o.ServiceOrderId,
                o.OrderNumber,
                ServiceType = o.ServiceType != null ? o.ServiceType.ServiceTypeName : string.Empty,
                RequestedDate = o.RequestedDate.ToString("yyyy-MM-dd"),
                RequestedTimeSlot = o.RequestedTimeSlot,
                CompletedAt = o.CompletedAt.HasValue ? o.CompletedAt.Value.ToString("yyyy-MM-dd HH:mm") : null,
                Rating = o.Rating,
                ReviewComment = o.ReviewComment
            })
            .ToListAsync();

        return new JsonResult(new { success = true, items = completed });
    }
}
