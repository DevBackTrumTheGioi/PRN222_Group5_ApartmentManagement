using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Contracts;

[Authorize(Policy = "AdminAndBQLManager")]
public class IndexModel : PageModel
{
    private readonly IContractService _contractService;

    public IndexModel(IContractService contractService)
    {
        _contractService = contractService;
    }

    public List<ContractRowVm> Rows { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public ContractStatus? Status { get; set; }

    [BindProperty(SupportsGet = true)]
    public ContractType? ContractTypeFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public int TotalItems { get; set; }
    public int TotalPages => TotalItems == 0 ? 1 : (int)Math.Ceiling(TotalItems / (double)PageSize);

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        TempData.Remove("StatusMessage");

        var result = await _contractService.GetPagedFilteredAsync(
            SearchTerm,
            Status,
            ContractTypeFilter,
            PageIndex,
            PageSize);

        TotalItems = result.TotalCount;

        Rows = result.Items.Select(c =>
        {
            var owner = c.ContractMembers.FirstOrDefault(cm => cm.MemberRole == MemberRole.ContractOwner);
            return new ContractRowVm
            {
                ContractId = c.ContractId,
                ContractNumber = c.ContractNumber,
                ApartmentDisplay = $"Căn {c.Apartment?.ApartmentNumber}" +
                    (string.IsNullOrWhiteSpace(c.Apartment?.BuildingBlock) ? "" : $", {c.Apartment.BuildingBlock}"),
                TypeLabel = (c.ContractType ?? ContractType.Other) switch
                {
                    ContractType.Rental => "Thuê",
                    ContractType.Purchase => "Mua bán",
                    ContractType.Other => "Khác",
                    _ => "-"
                },
                StartDate = c.StartDate.ToString("dd/MM/yyyy"),
                EndDate = c.EndDate?.ToString("dd/MM/yyyy") ?? "—",
                StatusLabel = c.Status switch
                {
                    ContractStatus.Draft => "Bản nháp",
                    ContractStatus.Active => "Đang hiệu lực",
                    ContractStatus.Expired => "Hết hạn",
                    ContractStatus.Terminated => "Đã chấm dứt",
                    ContractStatus.Cancelled => "Đã hủy",
                    _ => "-"
                },
                StatusCssClass = c.Status switch
                {
                    ContractStatus.Active => "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200",
                    ContractStatus.Draft => "bg-slate-100 text-slate-700 ring-1 ring-slate-200",
                    ContractStatus.Expired => "bg-rose-50 text-rose-700 ring-1 ring-rose-200",
                    ContractStatus.Terminated => "bg-red-50 text-red-700 ring-1 ring-red-200",
                    ContractStatus.Cancelled => "bg-slate-100 text-slate-500 ring-1 ring-slate-200",
                    _ => "bg-slate-100 text-slate-700 ring-1 ring-slate-200"
                },
                OwnerName = c.OwnerFullName ?? (owner?.Resident?.FullName ?? "—"),
                OwnerPhone = !string.IsNullOrWhiteSpace(c.OwnerPhone) ? c.OwnerPhone : (owner?.Resident?.PhoneNumber ?? "—")
            };
        }).ToList();
    }
}

public class ContractRowVm
{
    public int ContractId { get; set; }
    public string ContractNumber { get; set; } = "";
    public string ApartmentDisplay { get; set; } = "";
    public string TypeLabel { get; set; } = "";
    public string StartDate { get; set; } = "";
    public string EndDate { get; set; } = "";
    public string StatusLabel { get; set; } = "";
    public string StatusCssClass { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public string OwnerPhone { get; set; } = "";
}
