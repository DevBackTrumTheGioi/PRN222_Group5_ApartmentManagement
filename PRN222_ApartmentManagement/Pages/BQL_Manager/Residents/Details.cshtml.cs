using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Residents;

[Authorize(Policy = "AdminAndBQLManager")]
public class DetailsModel : PageModel
{
    private readonly IUserRepository _userRepository;
    private readonly IResidentApartmentRepository _residentApartmentRepository;

    public DetailsModel(IUserRepository userRepository, IResidentApartmentRepository residentApartmentRepository)
    {
        _userRepository = userRepository;
        _residentApartmentRepository = residentApartmentRepository;
    }

    public User? Resident { get; set; }

    /// <summary>
    /// Danh sách lịch sử cư trú theo từng hợp đồng của cư dân.
    /// </summary>
    public List<ResidentApartment> ResidencyRecords { get; set; } = new();

    /// <summary>
    /// Banghi cu tru dang active (neu co).
    /// </summary>
    public ResidentApartment? ActiveResidency => ResidencyRecords.FirstOrDefault(r => r.IsActive);

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        TempData.Remove("StatusMessage");
        Resident = await _userRepository.GetResidentByIdWithDetailsAsync(id);
        if (Resident == null)
        {
            return NotFound();
        }
        ResidencyRecords = await _residentApartmentRepository.GetByUserIdAsync(id);
        return Page();
    }

    // ===== DISPLAY HELPERS =====

    public string ResidentTypeLabel => ActiveResidency?.ResidencyType switch
    {
        ResidencyType.Owner => "Chủ sở hữu",
        ResidencyType.Tenant => "Người thuê",
        ResidencyType.FamilyMember => "Thành viên gia đình",
        ResidencyType.Other => "Khác",
        _ => "—"
    };

    public string ResidentTypeIcon => ActiveResidency?.ResidencyType switch
    {
        ResidencyType.Owner => "account_circle",
        ResidencyType.Tenant => "person_pin",
        ResidencyType.FamilyMember => "group",
        ResidencyType.Other => "person",
        _ => "person"
    };

    public string ResidentTypeCss => ActiveResidency?.ResidencyType switch
    {
        ResidencyType.Owner => "bg-amber-50 text-amber-700 ring-1 ring-amber-200",
        ResidencyType.Tenant => "bg-blue-50 text-blue-700 ring-1 ring-blue-200",
        ResidencyType.FamilyMember => "bg-violet-50 text-violet-700 ring-1 ring-violet-200",
        ResidencyType.Other => "bg-slate-100 text-slate-600 ring-1 ring-slate-200",
        _ => "bg-slate-100 text-slate-600 ring-1 ring-slate-200"
    };

    public string StatusCss => ActiveResidency?.IsActive == true
        ? "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200"
        : "bg-red-50 text-red-600 ring-1 ring-red-200";

    public string StatusLabel => ActiveResidency?.IsActive == true ? "Đang cư trú" : "Đã chuyển đi";

    public int VehicleCount => Resident?.Vehicles?.Count ?? 0;
    public int CardCount => Resident?.ResidentCards?.Count ?? 0;
    public int ActiveContractCount => ResidencyRecords.Count(r => r.IsActive);
    public int RecentRequestCount => Resident?.Requests?.Count ?? 0;

    public string Initials(string? name) =>
        string.IsNullOrWhiteSpace(name) ? "?" :
        string.Join("", name.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2)
            .Select(w => w[0].ToString().ToUpper()));

    public string CardStatusCss(CardStatus status) => status switch
    {
        CardStatus.Active => "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200",
        CardStatus.Locked => "bg-slate-100 text-slate-600 ring-1 ring-slate-200",
        CardStatus.Lost => "bg-red-50 text-red-600 ring-1 ring-red-200",
        CardStatus.Expired => "bg-amber-50 text-amber-700 ring-1 ring-amber-200",
        _ => "bg-slate-100 text-slate-600 ring-1 ring-slate-200"
    };

    public string CardStatusLabel(CardStatus status) => status switch
    {
        CardStatus.Active => "Hoạt động",
        CardStatus.Locked => "Khóa",
        CardStatus.Lost => "Mất",
        CardStatus.Expired => "Hết hạn",
        _ => "-"
    };

    public string CardTypeLabel(CardType? type) => type switch
    {
        CardType.Resident => "Thẻ cư dân",
        CardType.Secondary => "Thẻ phụ",
        CardType.Visitor => "Thẻ khách",
        CardType.Staff => "Thẻ nhân viên",
        _ => "—"
    };

    public string VehicleTypeCss(string? type) => type switch
    {
        "Ô tô" => "bg-blue-50 text-blue-700 ring-1 ring-blue-200",
        "Xe máy" => "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200",
        _ => "bg-slate-100 text-slate-600 ring-1 ring-slate-200"
    };

    public string VehicleTypeLabel(string? type) => type ?? "—";

    public string RequestStatusCss(RequestStatus status) => status switch
    {
        RequestStatus.Pending => "bg-amber-50 text-amber-700 ring-1 ring-amber-200",
        RequestStatus.InProgress => "bg-blue-50 text-blue-700 ring-1 ring-blue-200",
        RequestStatus.Completed => "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200",
        RequestStatus.Rejected => "bg-red-50 text-red-600 ring-1 ring-red-200",
        RequestStatus.Cancelled => "bg-slate-100 text-slate-500 ring-1 ring-slate-200",
        _ => "bg-slate-100 text-slate-600 ring-1 ring-slate-200"
    };

    public string RequestStatusLabel(RequestStatus status) => status switch
    {
        RequestStatus.Pending => "Chờ xử lý",
        RequestStatus.InProgress => "Đang xử lý",
        RequestStatus.Completed => "Đã hoàn thành",
        RequestStatus.Rejected => "Từ chối",
        RequestStatus.Cancelled => "Đã hủy",
        _ => "-"
    };

    public string RequestPriorityCss(RequestPriority priority) => priority switch
    {
        RequestPriority.Low => "bg-slate-100 text-slate-600",
        RequestPriority.Normal => "bg-blue-50 text-blue-700",
        RequestPriority.High => "bg-amber-50 text-amber-700",
        RequestPriority.Emergency => "bg-red-50 text-red-600",
        _ => "bg-slate-100 text-slate-600"
    };

    public string RequestPriorityLabel(RequestPriority priority) => priority switch
    {
        RequestPriority.Low => "Thấp",
        RequestPriority.Normal => "Bình thường",
        RequestPriority.High => "Cao",
        RequestPriority.Emergency => "Khẩn cấp",
        _ => "-"
    };

    public string ContractStatusCss(ContractStatus status) => status switch
    {
        ContractStatus.Active => "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200",
        ContractStatus.Draft => "bg-slate-100 text-slate-600 ring-1 ring-slate-200",
        ContractStatus.Expired => "bg-rose-50 text-rose-700 ring-1 ring-rose-200",
        ContractStatus.Terminated => "bg-red-50 text-red-600 ring-1 ring-red-200",
        ContractStatus.Cancelled => "bg-slate-100 text-slate-500 ring-1 ring-slate-200",
        _ => "bg-slate-100 text-slate-600 ring-1 ring-slate-200"
    };

    public string ContractTypeLabel(ContractType? type) => type switch
    {
        ContractType.Rental => "Thuê",
        ContractType.Purchase => "Mua bán",
        ContractType.Other => "Khác",
        _ => "—"
    };

    public string ContractStatusLabel(ContractStatus status) => status switch
    {
        ContractStatus.Active => "Đang hiệu lực",
        ContractStatus.Draft => "Bản nháp",
        ContractStatus.Expired => "Hết hạn",
        ContractStatus.Terminated => "Đã chấm dứt",
        ContractStatus.Cancelled => "Đã hủy",
        _ => "-"
    };

    public string ResidencyStatusCss(bool isActive) => isActive
        ? "bg-emerald-50 text-emerald-700 ring-1 ring-emerald-200"
        : "bg-slate-100 text-slate-500 ring-1 ring-slate-200";

    public string ResidencyStatusLabel(bool isActive) => isActive ? "Đang cư trú" : "Đã chuyển đi";

    public string ResidencyTypeLabel(ResidencyType type) => type switch
    {
        ResidencyType.Owner => "Chủ sở hữu",
        ResidencyType.Tenant => "Người thuê",
        ResidencyType.FamilyMember => "Thành viên gia đình",
        ResidencyType.Other => "Khác",
        _ => "—"
    };
}
