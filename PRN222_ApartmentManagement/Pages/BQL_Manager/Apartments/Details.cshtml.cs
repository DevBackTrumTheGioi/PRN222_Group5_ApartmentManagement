using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Apartments;

[Authorize(Policy = "AdminAndBQLManager")]
public class DetailsModel : PageModel
{
    private readonly IApartmentService _apartmentService;

    public DetailsModel(IApartmentService apartmentService)
    {
        _apartmentService = apartmentService;
    }

    public ApartmentDetailDto? Apartment { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        TempData.Remove("StatusMessage");
        Apartment = await _apartmentService.GetDetailAsync(id);

        if (Apartment == null)
        {
            StatusMessage = "Không tìm thấy căn hộ.";
            return RedirectToPage("Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var (success, message) = await _apartmentService.DeleteAsync(id);
        if (success)
        {
            StatusMessage = message;
            return RedirectToPage("Index");
        }

        Apartment = await _apartmentService.GetDetailAsync(id);
        ErrorMessage = message;
        return Page();
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

    public string CardStatusCss(CardStatus status) => status switch
    {
        CardStatus.Active => "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200",
        CardStatus.Locked => "bg-amber-50 text-amber-700 ring-1 ring-amber-200",
        CardStatus.Lost => "bg-red-50 text-red-700 ring-1 ring-red-200",
        CardStatus.Expired => "bg-slate-100 text-slate-600 ring-1 ring-slate-200",
        _ => "bg-slate-100 text-slate-700 ring-1 ring-slate-200"
    };

    public string CardStatusLabel(CardStatus status) => status switch
    {
        CardStatus.Active => "Hoạt động",
        CardStatus.Locked => "Khóa",
        CardStatus.Lost => "Mất",
        CardStatus.Expired => "Hết hạn",
        _ => "—"
    };

    public string CardTypeLabel(CardType? type) => type switch
    {
        CardType.Resident => "Thẻ căn hộ",
        CardType.Staff => "Thẻ nhân viên",
        CardType.Secondary => "Thẻ phụ",
        CardType.Visitor => "Thẻ khách",
        _ => "—"
    };

    public string CardTypeCss(CardType? type) => type switch
    {
        CardType.Resident => "bg-violet-50 text-violet-700 ring-1 ring-violet-200",
        CardType.Staff => "bg-blue-50 text-blue-700 ring-1 ring-blue-200",
        CardType.Secondary => "bg-amber-50 text-amber-700 ring-1 ring-amber-200",
        CardType.Visitor => "bg-slate-100 text-slate-600 ring-1 ring-slate-200",
        _ => "bg-slate-100 text-slate-700 ring-1 ring-slate-200"
    };
}
