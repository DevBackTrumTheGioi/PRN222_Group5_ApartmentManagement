using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Cards;

[Authorize(Roles = "BQL_Manager")]
public class DetailsModel : PageModel
{
    private readonly IResidentCardService _residentCardService;

    public DetailsModel(IResidentCardService residentCardService)
    {
        _residentCardService = residentCardService;
    }

    public ResidentCard? Card { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        TempData.Remove("StatusMessage");
        Card = await _residentCardService.GetByIdWithDetailsAsync(id);

        if (Card == null)
        {
            StatusMessage = "Không tìm thấy thẻ.";
            return RedirectToPage("Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int id, CardStatus status)
    {
        Card = await _residentCardService.GetByIdWithDetailsAsync(id);

        if (Card == null)
        {
            StatusMessage = "Không tìm thấy thẻ.";
            return RedirectToPage("Index");
        }

        var (success, message) = await _residentCardService.UpdateStatusAsync(id, status);
        if (!success)
        {
            ErrorMessage = message;
        }
        else
        {
            StatusMessage = message;
        }

        return await OnGetAsync(id);
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var (success, message) = await _residentCardService.DeleteAsync(id);
        if (success)
        {
            StatusMessage = message;
            return RedirectToPage("Index");
        }

        Card = await _residentCardService.GetByIdWithDetailsAsync(id);
        ErrorMessage = message;
        return Page();
    }

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
