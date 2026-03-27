using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Services;

[Authorize(Policy = "AdminAndBQLManager")]
public class EditModel : PageModel
{
    private readonly IServiceManagementService _serviceManagementService;

    public EditModel(IServiceManagementService serviceManagementService)
    {
        _serviceManagementService = serviceManagementService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        public int ServiceTypeId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ.")]
        [MaxLength(100)]
        [Display(Name = "Tên dịch vụ")]
        public string ServiceTypeName { get; set; } = string.Empty;

        [MaxLength(50)]
        [Display(Name = "Đơn vị tính")]
        public string? MeasurementUnit { get; set; }

        [MaxLength(500)]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Đơn giá không hợp lệ.")]
        [Display(Name = "Đơn giá hiện hành")]
        public decimal UnitPrice { get; set; }

        [MaxLength(500)]
        [Display(Name = "Ghi chú bảng giá")]
        public string? PriceDescription { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool IsActive { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var serviceType = await _serviceManagementService.GetServiceTypeWithPricesAsync(id);
        if (serviceType == null)
        {
            return NotFound();
        }

        var currentPrice = GetCurrentPrice(serviceType);
        Input = new InputModel
        {
            ServiceTypeId = serviceType.ServiceTypeId,
            ServiceTypeName = serviceType.ServiceTypeName,
            MeasurementUnit = serviceType.MeasurementUnit,
            Description = serviceType.Description,
            UnitPrice = currentPrice?.UnitPrice ?? 0,
            PriceDescription = currentPrice?.Description,
            IsActive = serviceType.IsActive
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var serviceType = new ServiceType
        {
            ServiceTypeId = Input.ServiceTypeId,
            ServiceTypeName = Input.ServiceTypeName,
            MeasurementUnit = Input.MeasurementUnit,
            Description = Input.Description,
            IsActive = Input.IsActive
        };

        var result = await _serviceManagementService.SaveServiceTypeAsync(
            serviceType,
            Input.UnitPrice,
            Input.PriceDescription);

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Message);
            return Page();
        }

        TempData["SuccessMessage"] = result.Message;
        return RedirectToPage("Index");
    }

    private static ServicePrice? GetCurrentPrice(ServiceType serviceType)
    {
        var today = DateTime.Now.Date;
        return serviceType.ServicePrices
            .Where(sp => sp.EffectiveFrom <= today && (!sp.EffectiveTo.HasValue || sp.EffectiveTo.Value >= today))
            .OrderByDescending(sp => sp.EffectiveFrom)
            .ThenByDescending(sp => sp.ServicePriceId)
            .FirstOrDefault();
    }
}
