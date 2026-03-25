using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Apartments;

[Authorize(Policy = "AdminAndBQLManager")]
public class IndexModel : PageModel
{
    private readonly IApartmentService _apartmentService;

    public IndexModel(IApartmentService apartmentService)
    {
        _apartmentService = apartmentService;
    }

    public List<ApartmentRowVm> Rows { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public ApartmentStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? TypeFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? BlockFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? FloorFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public int TotalItems { get; set; }
    public int TotalPages => TotalItems == 0 ? 1 : (int)Math.Ceiling(TotalItems / (double)PageSize);

    [TempData]
    public string? StatusMessage { get; set; }

    public int Total { get; set; }
    public int AvailableCount { get; set; }
    public int OccupiedCount { get; set; }
    public int ReservedCount { get; set; }

    public List<string> AvailableBlocks { get; set; } = new();
    public List<int> AvailableFloors { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        TempData.Remove("StatusMessage");

        var stats = await _apartmentService.GetStatsAsync();
        Total = stats.Total;
        AvailableCount = stats.Available;
        OccupiedCount = stats.Occupied;
        ReservedCount = stats.Reserved;

        var result = await _apartmentService.GetPagedAsync(
            SearchTerm,
            StatusFilter,
            TypeFilter,
            BlockFilter,
            FloorFilter,
            PageIndex,
            PageSize);

        TotalItems = result.TotalCount;

        Rows = result.Items.Select(a => new ApartmentRowVm
        {
            ApartmentId = a.ApartmentId,
            ApartmentNumber = a.ApartmentNumber,
            BuildingBlock = a.BuildingBlock ?? "—",
            Floor = a.Floor,
            Area = a.Area,
            ApartmentType = a.ApartmentType ?? "—",
            StatusLabel = a.Status switch
            {
                ApartmentStatus.Available => "Trống",
                ApartmentStatus.Occupied => "Đang ở",
                ApartmentStatus.Reserved => "Đã đặt",
                _ => "—"
            },
            StatusCss = a.Status switch
            {
                ApartmentStatus.Available => "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200",
                ApartmentStatus.Occupied => "bg-blue-50 text-blue-700 ring-1 ring-blue-200",
                ApartmentStatus.Reserved => "bg-violet-50 text-violet-700 ring-1 ring-violet-200",
                _ => "bg-slate-100 text-slate-700 ring-1 ring-slate-200"
            },
            Status = a.Status,
            CreatedAt = a.CreatedAt.ToString("dd/MM/yyyy")
        }).ToList();

        // Build filter options from all apartments (unfiltered)
        var allBlocks = await _apartmentService.GetAllAsync();
        AvailableBlocks = allBlocks
            .Select(a => a.BuildingBlock)
            .Where(b => !string.IsNullOrWhiteSpace(b))
            .Distinct()
            .OrderBy(b => b)
            .ToList();
        AvailableFloors = allBlocks
            .Select(a => a.Floor)
            .Distinct()
            .OrderBy(f => f)
            .ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var (success, message) = await _apartmentService.DeleteAsync(id);
        StatusMessage = message;
        return RedirectToPage(new
        {
            pageIndex = PageIndex,
            pageSize = PageSize,
            searchTerm = SearchTerm,
            statusfilter = StatusFilter,
            typeFilter = TypeFilter,
            blockFilter = BlockFilter,
            floorFilter = FloorFilter
        });
    }
}

public class ApartmentRowVm
{
    public int ApartmentId { get; set; }
    public string ApartmentNumber { get; set; } = "";
    public string BuildingBlock { get; set; } = "";
    public int Floor { get; set; }
    public decimal? Area { get; set; }
    public string ApartmentType { get; set; } = "";
    public string StatusLabel { get; set; } = "";
    public string StatusCss { get; set; } = "";
    public ApartmentStatus Status { get; set; }
    public string CreatedAt { get; set; } = "";
}
