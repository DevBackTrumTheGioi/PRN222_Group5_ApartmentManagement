using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Cards;

[Authorize(Roles = "BQL_Manager")]
public class EditModel : PageModel
{
    private readonly IResidentCardService _residentCardService;

    public EditModel(IResidentCardService residentCardService)
    {
        _residentCardService = residentCardService;
    }

    public ResidentCard? Card { get; set; }

    [BindProperty]
    public UpdateResidentCardDto Input { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        TempData.Remove("SuccessMessage");
        Card = await _residentCardService.GetByIdWithDetailsAsync(id);

        if (Card == null)
        {
            SuccessMessage = "Không tìm thấy thẻ.";
            return RedirectToPage("Index");
        }

        if (Card.IsDeleted)
        {
            ErrorMessage = "Thẻ đang ở trạng thái Mất. Không thể chỉnh sửa thông tin.";
        }

        PopulateInput(Card);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Card = await _residentCardService.GetByIdWithDetailsAsync(id);

        if (Card == null)
        {
            SuccessMessage = "Không tìm thấy thẻ.";
            return RedirectToPage("Index");
        }

        if (Card.Status == CardStatus.Lost)
        {
            ErrorMessage = "Thẻ đang ở trạng thái Mất. Không thể chỉnh sửa thông tin.";
            PopulateInput(Card);
            return Page();
        }

        var (success, message) = await _residentCardService.UpdateAsync(id, Input);

        if (!success)
        {
            ErrorMessage = message;
            PopulateInput(Card);
            return Page();
        }

        SuccessMessage = message;
        return RedirectToPage("Details", new { id });
    }

    private void PopulateInput(ResidentCard card)
    {
        Input = new UpdateResidentCardDto
        {
            ExpiryDate = card.ExpiryDate,
            Notes = card.Notes
        };
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

    public string CardTypeCss(CardType? type) => type switch
    {
        CardType.Resident => "bg-violet-50 text-violet-700 ring-1 ring-violet-200",
        CardType.Staff => "bg-blue-50 text-blue-700 ring-1 ring-blue-200",
        CardType.Secondary => "bg-amber-50 text-amber-700 ring-1 ring-amber-200",
        CardType.Visitor => "bg-slate-100 text-slate-600 ring-1 ring-slate-200",
        _ => "bg-slate-100 text-slate-700 ring-1 ring-slate-200"
    };

    public string CardTypeLabel(CardType? type) => type switch
    {
        CardType.Resident => "Thẻ căn hộ",
        CardType.Staff => "Thẻ nhân viên",
        CardType.Secondary => "Thẻ phụ",
        CardType.Visitor => "Thẻ khách",
        _ => "—"
    };
}
