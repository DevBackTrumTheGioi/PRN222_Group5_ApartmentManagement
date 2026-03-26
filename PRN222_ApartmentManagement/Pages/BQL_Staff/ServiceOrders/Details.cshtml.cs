using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff.ServiceOrders;

[Authorize(Roles = "BQL_Staff")]
public class DetailsModel : PageModel
{
    private readonly IServiceManagementService _serviceManagementService;

    public DetailsModel(IServiceManagementService serviceManagementService)
    {
        _serviceManagementService = serviceManagementService;
    }

    public ServiceOrder Order { get; set; } = null!;

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Chi phí thực tế không hợp lệ.")]
        [Display(Name = "Tổng chi phí thực tế")]
        public decimal? ActualPrice { get; set; }

        [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Chi phí phát sinh không hợp lệ.")]
        [Display(Name = "Chi phí phát sinh")]
        public decimal? AdditionalCharges { get; set; }

        [MaxLength(500)]
        [Display(Name = "Ghi chú chi phí")]
        public string? ChargeNotes { get; set; }

        [MaxLength(2000)]
        [Display(Name = "Ghi chú hoàn thành")]
        public string? CompletionNotes { get; set; }
    }

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

        if (order.AssignedTo != userId.Value)
        {
            return Forbid();
        }

        Order = order;
        Input = new InputModel
        {
            ActualPrice = order.ActualPrice,
            AdditionalCharges = order.AdditionalCharges,
            ChargeNotes = order.ChargeNotes,
            CompletionNotes = order.CompletionNotes
        };

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int id, ServiceOrderStatus newStatus)
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

        if (order.AssignedTo != userId.Value)
        {
            return Forbid();
        }

        Order = order;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _serviceManagementService.UpdateAssignedOrderAsync(
            id,
            userId.Value,
            newStatus,
            Input.ActualPrice,
            Input.AdditionalCharges,
            Input.ChargeNotes,
            Input.CompletionNotes);

        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
        return RedirectToPage(new { id });
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
