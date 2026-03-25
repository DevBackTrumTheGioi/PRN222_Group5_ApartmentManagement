using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Cards;

[Authorize(Roles = "BQL_Manager")]
public class IndexModel : PageModel
{
    private readonly IResidentCardService _residentCardService;

    public IndexModel(IResidentCardService residentCardService)
    {
        _residentCardService = residentCardService;
    }

    public List<CardRowVm> Rows { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public CardStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public int TotalItems { get; set; }
    public int TotalPages => TotalItems == 0 ? 1 : (int)Math.Ceiling(TotalItems / (double)PageSize);

    [TempData]
    public string? StatusMessage { get; set; }

    public int TotalCards { get; set; }
    public int ActiveCards { get; set; }
    public int LockedCards { get; set; }
    public int LostCards { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        TempData.Remove("StatusMessage");

        var stats = await _residentCardService.GetStatsAsync();
        TotalCards = stats.Total;
        ActiveCards = stats.Active;
        LockedCards = stats.Locked;
        LostCards = stats.Lost;

        var result = await _residentCardService.GetPagedAsync(
            SearchTerm,
            StatusFilter,
            null,
            PageIndex,
            PageSize);

        TotalItems = result.TotalCount;

        Rows = result.Items.Select(c => new CardRowVm
        {
            CardId = c.ResidentCardId,
            CardNumber = c.CardNumber,
            ResidentName = c.Resident?.FullName ?? "—",
            ResidentPhone = c.Resident?.PhoneNumber ?? "—",
            ApartmentDisplay = c.Apartment != null
                ? $"Căn {c.Apartment.ApartmentNumber}" + (string.IsNullOrWhiteSpace(c.Apartment.BuildingBlock) ? "" : $", Block {c.Apartment.BuildingBlock}")
                : "—",
            StatusLabel = c.Status switch
            {
                CardStatus.Active => "Hoạt động",
                CardStatus.Locked => "Khóa",
                CardStatus.Lost => "Mất",
                CardStatus.Expired => "Hết hạn",
                _ => "—"
            },
            StatusCss = c.Status switch
            {
                CardStatus.Active => "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200",
                CardStatus.Locked => "bg-amber-50 text-amber-700 ring-1 ring-amber-200",
                CardStatus.Lost => "bg-red-50 text-red-700 ring-1 ring-red-200",
                CardStatus.Expired => "bg-slate-100 text-slate-600 ring-1 ring-slate-200",
                _ => "bg-slate-100 text-slate-700 ring-1 ring-slate-200"
            },
            Status = c.Status,
            IssuedDate = c.IssuedDate.ToString("dd/MM/yyyy"),
            ExpiryDate = c.ExpiryDate?.ToString("dd/MM/yyyy") ?? "—"
        }).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int id, CardStatus status)
    {
        var (success, message) = await _residentCardService.UpdateStatusAsync(id, status);
        StatusMessage = message;
        return RedirectToPage(new
        {
            pageIndex = PageIndex,
            pageSize = PageSize,
            searchTerm = SearchTerm,
            statusfilter = StatusFilter
        });
    }
}

public class CardRowVm
{
    public int CardId { get; set; }
    public string CardNumber { get; set; } = "";
    public string ResidentName { get; set; } = "";
    public string ResidentPhone { get; set; } = "";
    public string ApartmentDisplay { get; set; } = "";
    public string StatusLabel { get; set; } = "";
    public string StatusCss { get; set; } = "";
    public CardStatus Status { get; set; }
    public string IssuedDate { get; set; } = "";
    public string ExpiryDate { get; set; } = "";
}
