using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Contracts;

[Authorize(Policy = "AdminAndBQLManager")]
public class EditModel : PageModel
{
    private readonly IContractService _contractService;
    private readonly IUserRepository _userRepository;

    public EditModel(IContractService contractService, IUserRepository userRepository)
    {
        _contractService = contractService;
        _userRepository = userRepository;
    }

    public Contract? Contract { get; set; }

    [BindProperty]
    public EditContractInput Input { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        TempData.Remove("StatusMessage");
        Contract = await _contractService.GetByIdWithDetailsAsync(id);
        if (Contract == null)
        {
            TempData["StatusMessage"] = "Không tìm thấy hợp đồng";
            return RedirectToPage("Index");
        }

        if (Contract.Status != ContractStatus.Draft)
        {
            TempData["StatusMessage"] = "Chỉ có thể chỉnh sửa hợp đồng ở trạng thái bản nháp";
            return RedirectToPage("Details", new { id });
        }

        await PopulateInputAsync(Contract);

        return Page();
    }

    private int? _currentOwnerUserId;
    private int? GetCurrentOwnerUserId(Contract contract)
    {
        if (_currentOwnerUserId.HasValue) return _currentOwnerUserId;
        var owner = contract.ContractMembers?
            .FirstOrDefault(m => m.MemberRole == MemberRole.ContractOwner);
        _currentOwnerUserId = owner?.ResidentId;
        return _currentOwnerUserId;
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        Contract = await _contractService.GetByIdWithDetailsAsync(id);
        if (Contract == null)
        {
            TempData["StatusMessage"] = "Khong tim thay hop dong.";
            return RedirectToPage("Index");
        }

        if (Contract.Status != ContractStatus.Draft)
        {
            TempData["StatusMessage"] = "Chỉ có thể chỉnh sửa hợp đồng ở trạng thái bản nháp";
            return RedirectToPage("Details", new { id });
        }

        // Chủ cũ: bỏ implicit validation trên field chủ mới + điền từ User (bản nháp thường chưa có ContractMember)
        if (Input.IsExistingOwner)
        {
            ModelState.Remove("Input.OwnerFullName");
            ModelState.Remove("Input.OwnerPhone");
            ModelState.Remove("Input.OwnerEmail");
            ModelState.Remove("Input.OwnerIdentityCard");
            ModelState.Remove("Input.OwnerDateOfBirth");

            if (!Input.ExistingOwnerId.HasValue || Input.ExistingOwnerId.Value == 0)
            {
                ErrorMessage = "Vui lòng tra cứu CCCD để chọn chủ hộ đã tồn tại.";
                return Page();
            }

            var existing = await _userRepository.GetActiveByIdAsync(Input.ExistingOwnerId.Value);
            if (existing == null || string.IsNullOrWhiteSpace(existing.IdentityCardNumber))
            {
                ErrorMessage = "Không tìm thấy cư dân hoặc thiếu CCCD trong hồ sơ.";
                return Page();
            }

            if (!string.Equals(existing.IdentityCardNumber.Trim(), Input.ExistingOwnerCccd?.Trim(), StringComparison.Ordinal))
            {
                ErrorMessage = "Thông tin chủ hộ không khớp. Vui lòng tra cứu lại.";
                return Page();
            }

            Input.OwnerFullName = existing.FullName;
            Input.OwnerPhone = existing.PhoneNumber ?? "";
            Input.OwnerEmail = existing.Email;
            Input.OwnerDateOfBirth = existing.DateOfBirth;
            Input.OwnerIdentityCard = existing.IdentityCardNumber;
            Input.ExistingOwnerCccd = existing.IdentityCardNumber;
        }

        var excludeUserId = GetCurrentOwnerUserId(Contract)
            ?? (Input.IsExistingOwner ? Input.ExistingOwnerId : null);
        var emailToCheck = Input.OwnerEmail?.Trim() ?? "";
        var phoneToCheck = Input.OwnerPhone?.Trim() ?? "";
        var cccdToCheck = Input.OwnerIdentityCard?.Trim() ?? "";

        if (!string.IsNullOrWhiteSpace(emailToCheck) && await _userRepository.EmailExistsAsync(emailToCheck, excludeUserId))
        {
            var dup = await _userRepository.GetByEmailAsync(emailToCheck);
            if (dup != null && dup.UserId != excludeUserId)
                ErrorMessage = $"Email \"{emailToCheck}\" đã được sử dụng bởi tài khoản mang CCCD \"{dup.IdentityCardNumber}\".";
            else
                ErrorMessage = $"Email \"{emailToCheck}\" đã được sử dụng bởi tài khoản khác.";
            return Page();
        }

        if (!string.IsNullOrWhiteSpace(phoneToCheck) && await _userRepository.PhoneExistsAsync(phoneToCheck, excludeUserId))
        {
            var dup = await _userRepository.FindByPhoneAsync(phoneToCheck);
            if (dup != null && dup.UserId != excludeUserId)
                ErrorMessage = $"Số điện thoại \"{phoneToCheck}\" đã được gắn với chủ hộ mang CCCD \"{dup.IdentityCardNumber}\".";
            else
                ErrorMessage = $"Số điện thoại \"{phoneToCheck}\" đã được sử dụng bởi tài khoản khác.";
            return Page();
        }

        if (!string.IsNullOrWhiteSpace(cccdToCheck) && await _userRepository.IdentityCardExistsAsync(cccdToCheck, excludeUserId))
        {
            ErrorMessage = $"CCCD \"{cccdToCheck}\" đã được gắn với chủ hộ khác trong hệ thống.";
            return Page();
        }

        if (!ModelState.IsValid)
        {
            ErrorMessage = "Vui lòng kiểm tra lại thông tin";
            return Page();
        }

        try
        {
            var dto = BuildDto();
            var updated = await _contractService.UpdateContractAsync(id, dto);
            if (updated == null)
            {
                ErrorMessage = "Không tể cập nhật hợp đồng";
                return Page();
            }

            TempData["StatusMessage"] = "Đã cập nhật hợp đồng thành công";
            return RedirectToPage("Details", new { id });
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Loi: {ex.Message}";
            return Page();
        }
    }

    /// <summary>
    /// Bản nháp thường chưa có ContractMember (chỉ tạo khi kích hoạt).
    /// Nhận diện chủ cũ: CCCD trên hợp đồng trùng user trong DB (hoặc đã có ContractOwner).
    /// </summary>
    private async Task PopulateInputAsync(Contract contract)
    {
        var contractOwner = contract.ContractMembers?
            .FirstOrDefault(m => m.MemberRole == MemberRole.ContractOwner);

        var cccd = (contractOwner?.Resident?.IdentityCardNumber ?? contract.OwnerIdentityCard)?.Trim();
        User? userByCccd = null;
        if (!string.IsNullOrWhiteSpace(cccd))
        {
            var matches = await _userRepository.FindAsync(u =>
                u.IdentityCardNumber == cccd && !u.IsDeleted);
            userByCccd = matches.FirstOrDefault();
        }

        var isExistingOwner = (contractOwner != null && contractOwner.ResidentId > 0) || userByCccd != null;

        if (isExistingOwner)
        {
            Input.IsExistingOwner = true;
            Input.ExistingOwnerId = contractOwner?.ResidentId > 0
                ? contractOwner.ResidentId
                : userByCccd?.UserId;
            Input.ExistingOwnerCccd = cccd;
            Input.OwnerFullName = "";
            Input.OwnerPhone = "";
            Input.OwnerEmail = null;
            Input.OwnerDateOfBirth = null;
            Input.OwnerIdentityCard = null;
        }
        else
        {
            Input.IsExistingOwner = false;
            Input.ExistingOwnerId = null;
            Input.ExistingOwnerCccd = null;
            Input.OwnerFullName = contract.OwnerFullName ?? "";
            Input.OwnerEmail = contract.OwnerEmail;
            Input.OwnerPhone = contract.OwnerPhone ?? "";
            Input.OwnerDateOfBirth = contract.OwnerDateOfBirth;
            Input.OwnerIdentityCard = contract.OwnerIdentityCard;
        }

        Input.ContractType = contract.ContractType;
        Input.StartDate = contract.StartDate;
        Input.EndDate = contract.EndDate;
        Input.MonthlyRent = contract.MonthlyRent.HasValue
            ? decimal.Round(contract.MonthlyRent.Value, 0, MidpointRounding.AwayFromZero)
            : null;
        Input.DepositAmount = contract.DepositAmount.HasValue
            ? decimal.Round(contract.DepositAmount.Value, 0, MidpointRounding.AwayFromZero)
            : null;
        Input.PurchasePrice = contract.PurchasePrice.HasValue
            ? decimal.Round(contract.PurchasePrice.Value, 0, MidpointRounding.AwayFromZero)
            : null;
        Input.Terms = contract.Terms;
    }

    private UpdateContractDto BuildDto()
    {
        return new UpdateContractDto
        {
            ContractType = Input.ContractType,
            StartDate = Input.StartDate,
            EndDate = Input.ContractType == ContractType.Rental ? Input.EndDate : null,
            MonthlyRent = Input.ContractType == ContractType.Rental ? Input.MonthlyRent : null,
            DepositAmount = Input.ContractType == ContractType.Rental ? Input.DepositAmount : null,
            PurchasePrice = Input.ContractType == ContractType.Purchase ? Input.PurchasePrice : null,
            Terms = Input.Terms,
            Owner = new OwnerUpdateDto
            {
                FullName = Input.OwnerFullName,
                Email = Input.OwnerEmail,
                PhoneNumber = Input.OwnerPhone,
                DateOfBirth = Input.OwnerDateOfBirth,
                IdentityCardNumber = Input.OwnerIdentityCard
            },
            IsExistingOwner = Input.IsExistingOwner,
            ExistingOwnerId = Input.ExistingOwnerId,
            ExistingOwnerCccd = Input.ExistingOwnerCccd
        };
    }

    public async Task<IActionResult> OnGetLookupOwner(string cccd)
    {
        if (string.IsNullOrWhiteSpace(cccd))
            return new JsonResult(new { found = false, message = "CCCD không được để trống." });

        var user = await _userRepository.FindAsync(u =>
            u.IdentityCardNumber == cccd && !u.IsDeleted);

        var existingUser = user.FirstOrDefault();
        if (existingUser == null)
            return new JsonResult(new { found = false, message = "Không tìm thấy cư dân với CCCD này." });

        return new JsonResult(new
        {
            found = true,
            userId = existingUser.UserId,
            fullName = existingUser.FullName,
            phone = existingUser.PhoneNumber ?? "",
            email = existingUser.Email ?? "",
            dateOfBirth = existingUser.DateOfBirth?.ToString("yyyy-MM-dd") ?? "",
            identityCard = existingUser.IdentityCardNumber ?? ""
        });
    }
}

public class EditContractInput
{
    public ContractType? ContractType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? MonthlyRent { get; set; }
    public decimal? DepositAmount { get; set; }
    public decimal? PurchasePrice { get; set; }
    public string? Terms { get; set; }

    // Chủ hộ
    public string OwnerFullName { get; set; } = "";
    public string? OwnerEmail { get; set; }
    public string OwnerPhone { get; set; } = "";
    public DateTime? OwnerDateOfBirth { get; set; }
    public string? OwnerIdentityCard { get; set; }

    // Chủ cũ
    public bool IsExistingOwner { get; set; }
    public int? ExistingOwnerId { get; set; }
    public string? ExistingOwnerCccd { get; set; }
}
