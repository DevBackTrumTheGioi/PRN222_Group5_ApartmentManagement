using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Services;

[Authorize(Roles = "BQL_Manager")]
public class CreateModel : PageModel
{
    private readonly IServiceManagementService _serviceManagementService;

    public CreateModel(IServiceManagementService serviceManagementService)
    {
        _serviceManagementService = serviceManagementService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
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
        [Display(Name = "Đơn giá")]
        public decimal UnitPrice { get; set; }

        [MaxLength(500)]
        [Display(Name = "Ghi chú bảng giá")]
        public string? PriceDescription { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool IsActive { get; set; } = true;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var serviceType = new ServiceType
        {
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
}
