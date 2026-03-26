using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Apartments;

[Authorize(Policy = "AdminAndBQLManager")]
public class EditModel : PageModel
{
    private readonly IApartmentService _apartmentService;

    public EditModel(IApartmentService apartmentService)
    {
        _apartmentService = apartmentService;
    }

    public Apartment? Apartment { get; set; }

    public bool HasActiveResidents { get; set; }

    [BindProperty]
    public UpdateApartmentDto Input { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        TempData.Remove("SuccessMessage");
        var detail = await _apartmentService.GetDetailAsync(id);

        if (detail == null)
        {
            SuccessMessage = "Không tìm thấy căn hộ.";
            return RedirectToPage("Index");
        }

        Apartment = detail.Apartment;
        HasActiveResidents = detail.ResidentHistory.Any(h => h.IsActive);
        PopulateInput(Apartment!);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var detail = await _apartmentService.GetDetailAsync(id);

        if (detail == null)
        {
            SuccessMessage = "Không tìm thấy căn hộ.";
            return RedirectToPage("Index");
        }

        Apartment = detail.Apartment;
        HasActiveResidents = detail.ResidentHistory.Any(h => h.IsActive);

        var (success, message) = await _apartmentService.UpdateAsync(id, Input);

        if (!success)
        {
            ErrorMessage = message;
            PopulateInput(Apartment!);
            return Page();
        }

        SuccessMessage = message;
        return RedirectToPage("Details", new { id });
    }

    private void PopulateInput(Apartment apt)
    {
        Input = new UpdateApartmentDto
        {
            ApartmentNumber = apt.ApartmentNumber,
            Floor = apt.Floor,
            BuildingBlock = apt.BuildingBlock,
            Area = apt.Area,
            ApartmentType = apt.ApartmentType,
            Description = apt.Description
        };
    }

    public string ApartmentStatusCss(ApartmentStatus status) => status switch
    {
        ApartmentStatus.Available => "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200",
        ApartmentStatus.Occupied => "bg-blue-50 text-blue-700 ring-1 ring-blue-200",
        ApartmentStatus.Reserved => "bg-violet-50 text-violet-700 ring-1 ring-violet-200",
        _ => "bg-slate-100 text-slate-700 ring-1 ring-slate-200"
    };

    public string ApartmentStatusLabel(ApartmentStatus status) => status switch
    {
        ApartmentStatus.Available => "Trống",
        ApartmentStatus.Occupied => "Đang ở",
        ApartmentStatus.Reserved => "Đã đặt",
        _ => "—"
    };
}
