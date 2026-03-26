using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Services;

[Authorize(Roles = "Resident")]
public class DetailsModel : PageModel
{
    private readonly IServiceManagementService _serviceManagementService;

    public DetailsModel(IServiceManagementService serviceManagementService)
    {
        _serviceManagementService = serviceManagementService;
    }

    public ServiceOrder Order { get; set; } = null!;

    [BindProperty]
    [Range(1, 5, ErrorMessage = "Vui lòng chọn số sao từ 1 đến 5.")]
    public int? Rating { get; set; }

    [BindProperty]
    [StringLength(500, ErrorMessage = "Nhận xét không vượt quá 500 ký tự.")]
    public string? ReviewComment { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var order = await _serviceManagementService.GetServiceOrderAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        if (order.ResidentId != userId.Value)
        {
            return Forbid();
        }

        Order = order;
        return Page();
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var order = await _serviceManagementService.GetServiceOrderAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        if (order.ResidentId != userId.Value)
        {
            return Forbid();
        }

        var result = await _serviceManagementService.CancelOrderAsync(id, userId.Value, null);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostReviewAsync(int id)
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var order = await _serviceManagementService.GetServiceOrderAsync(id);
        if (order == null)
        {
            return NotFound();
        }

        if (order.ResidentId != userId.Value)
        {
            return Forbid();
        }

        Order = order;

        if (!ModelState.IsValid || !Rating.HasValue)
        {
            if (!Rating.HasValue)
            {
                ModelState.AddModelError(nameof(Rating), "Vui lòng chọn số sao đánh giá.");
            }

            return Page();
        }

        var result = await _serviceManagementService.SubmitReviewAsync(id, userId.Value, Rating.Value, ReviewComment);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToPage(new { id });
    }

    public string GetStatusNote()
    {
        return Order.Status switch
        {
            ServiceOrderStatus.Pending => "Đơn đang chờ Ban Quản lý phân công nhân viên phụ trách.",
            ServiceOrderStatus.Confirmed => "Nhân viên đã tiếp nhận đơn và sẽ sắp xếp thực hiện.",
            ServiceOrderStatus.InProgress => "Dịch vụ đang được thực hiện.",
            ServiceOrderStatus.Completed => "Dịch vụ đã hoàn tất và sẵn sàng đưa vào hóa đơn.",
            ServiceOrderStatus.Cancelled => "Đơn đã được hủy.",
            ServiceOrderStatus.Rejected => "Đơn đã bị từ chối.",
            _ => string.Empty
        };
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
