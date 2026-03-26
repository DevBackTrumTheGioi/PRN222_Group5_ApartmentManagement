using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Services;

[Authorize(Roles = "Resident")]
public class CreateModel : PageModel
{
    private readonly IServiceManagementService _serviceManagementService;

    private static readonly string[] TimeSlots = ["Sáng", "Chiều", "Tối"];

    public CreateModel(IServiceManagementService serviceManagementService)
    {
        _serviceManagementService = serviceManagementService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new()
    {
        RequestedDate = DateTime.Now.Date,
        RequestedTimeSlot = "Sáng"
    };

    public SelectList ServiceOptions { get; set; } = null!;
    public SelectList TimeSlotOptions { get; set; } = null!;
    public IReadOnlyList<ServiceType> ActiveServices { get; set; } = [];
    public ServiceType? SelectedService { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Vui lòng chọn dịch vụ.")]
        [Display(Name = "Loại dịch vụ")]
        public int? ServiceTypeId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày thực hiện.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày yêu cầu")]
        public DateTime RequestedDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn khung giờ.")]
        [Display(Name = "Khung giờ")]
        public string? RequestedTimeSlot { get; set; }

        [MaxLength(2000)]
        [Display(Name = "Ghi chú")]
        public string? Description { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int? serviceTypeId)
    {
        Input.ServiceTypeId = serviceTypeId;
        await LoadReferenceDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadReferenceDataAsync();

        ValidateRequestedTimeSlot();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var result = await _serviceManagementService.CreateOrderAsync(
            userId.Value,
            Input.ServiceTypeId!.Value,
            Input.RequestedDate,
            Input.RequestedTimeSlot,
            Input.Description);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return Page();
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToPage("MyOrders");
    }

    public ServicePrice? GetCurrentPrice(ServiceType? serviceType)
    {
        if (serviceType == null)
        {
            return null;
        }

        var today = DateTime.Now.Date;
        return serviceType.ServicePrices
            .Where(sp => sp.EffectiveFrom <= today && (!sp.EffectiveTo.HasValue || sp.EffectiveTo.Value >= today))
            .OrderByDescending(sp => sp.EffectiveFrom)
            .ThenByDescending(sp => sp.ServicePriceId)
            .FirstOrDefault();
    }

    private async Task LoadReferenceDataAsync()
    {
        ActiveServices = await _serviceManagementService.GetResidentActiveServiceTypesAsync(null);

        if (!Input.ServiceTypeId.HasValue && ActiveServices.Any())
        {
            Input.ServiceTypeId = ActiveServices.First().ServiceTypeId;
        }

        SelectedService = ActiveServices.FirstOrDefault(st => st.ServiceTypeId == Input.ServiceTypeId);

        ServiceOptions = new SelectList(
            ActiveServices.Select(st => new
            {
                Value = st.ServiceTypeId,
                Text = st.ServiceTypeName
            }),
            "Value",
            "Text",
            Input.ServiceTypeId);

        TimeSlotOptions = new SelectList(
            TimeSlots.Select(slot => new { Value = slot, Text = slot }),
            "Value",
            "Text",
            Input.RequestedTimeSlot);
    }

    private void ValidateRequestedTimeSlot()
    {
        if (string.IsNullOrWhiteSpace(Input.RequestedTimeSlot))
        {
            ModelState.AddModelError($"{nameof(Input)}.{nameof(Input.RequestedTimeSlot)}", "Vui lòng chọn khung giờ.");
            return;
        }

        var isValid = TimeSlots.Contains(Input.RequestedTimeSlot.Trim(), StringComparer.OrdinalIgnoreCase);
        if (!isValid)
        {
            ModelState.AddModelError($"{nameof(Input)}.{nameof(Input.RequestedTimeSlot)}", "Khung giờ không hợp lệ.");
        }
    }

    private int? GetUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : null;
    }
}
