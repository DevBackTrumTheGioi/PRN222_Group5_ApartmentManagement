using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Contracts;

[Authorize(Roles = "BQL_Manager")]
public class CreateModel : PageModel
{
    private readonly IContractService _contractService;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<CreateModel> _logger;

    public CreateModel(
        IContractService contractService,
        IApartmentRepository apartmentRepository,
        IUserRepository userRepository,
        ILogger<CreateModel> logger)
    {
        _contractService = contractService;
        _apartmentRepository = apartmentRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public List<Apartment> AvailableApartments { get; set; } = new();

    [BindProperty]
    public CreateContractDto Input { get; set; } = new();

    public string ContractNumberPreview { get; set; } = string.Empty;

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        TempData.Remove("StatusMessage");
        AvailableApartments = (await _apartmentRepository.GetAvailableApartmentsAsync()).ToList();
        ContractNumberPreview = await _contractService.GenerateContractNumberAsync(ContractType.Purchase);
        Input.StartDate = DateTime.Today;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var apartments = (await _apartmentRepository.GetAvailableApartmentsAsync()).ToList();
        AvailableApartments = apartments;
        ContractNumberPreview = await _contractService.GenerateContractNumberAsync(Input.ContractType ?? ContractType.Purchase);

        // Chủ cũ: bỏ qua validation client/implicit trên các field chủ mới
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
        else
        {
            if (string.IsNullOrWhiteSpace(Input.OwnerFullName))
            {
                ModelState.AddModelError("Input.OwnerFullName", "Vui lòng nhập họ và tên.");
            }
            if (string.IsNullOrWhiteSpace(Input.OwnerPhone))
            {
                ModelState.AddModelError("Input.OwnerPhone", "Vui lòng nhập số điện thoại.");
            }
            if (string.IsNullOrWhiteSpace(Input.OwnerIdentityCard))
            {
                ModelState.AddModelError("Input.OwnerIdentityCard", "CCCD là bắt buộc.");
            }

            if (string.IsNullOrWhiteSpace(ErrorMessage))
            {
                var emailToCheck = Input.OwnerEmail?.Trim() ?? "";
                var phoneToCheck = Input.OwnerPhone?.Trim() ?? "";
                var cccdToCheck = Input.OwnerIdentityCard?.Trim() ?? "";

                if (!string.IsNullOrWhiteSpace(emailToCheck) && await _userRepository.EmailExistsAsync(emailToCheck))
                {
                    var dup = await _userRepository.FindByIdentityCardAsync(cccdToCheck);
                    if (dup != null)
                        ErrorMessage = $"Email \"{emailToCheck}\" đã được gắn với chủ hộ mang CCCD \"{dup.IdentityCardNumber}\".";
                    else
                        ErrorMessage = $"Email \"{emailToCheck}\" đã được sử dụng bởi tài khoản khác.";
                    return Page();
                }

                if (!string.IsNullOrWhiteSpace(phoneToCheck) && await _userRepository.PhoneExistsAsync(phoneToCheck))
                {
                    var dup = await _userRepository.FindByPhoneAsync(phoneToCheck);
                    ErrorMessage = $"Số điện thoại \"{phoneToCheck}\" đã được gắn với chủ hộ mang CCCD \"{dup?.IdentityCardNumber}\".";
                    return Page();
                }

                if (!string.IsNullOrWhiteSpace(cccdToCheck) && await _userRepository.IdentityCardExistsAsync(cccdToCheck))
                {
                    ErrorMessage = $"CCCD \"{cccdToCheck}\" đã được gắn với chủ hộ khác trong hệ thống.";
                    return Page();
                }
            }
        }

        if (!ModelState.IsValid)
            return Page();

        if (!Input.IsExistingOwner)
        {
            if (!StringUtils.IsValidVietnamesePhoneNumber(Input.OwnerPhone!))
            {
                ModelState.AddModelError("Input.OwnerPhone", "So dien thoai khong hop le.");
                return Page();
            }
        }

        if (!string.IsNullOrWhiteSpace(Input.OwnerEmail) && !StringUtils.IsValidEmail(Input.OwnerEmail))
        {
            ModelState.AddModelError("Input.OwnerEmail", "Email khong hop le.");
            return Page();
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var creatorId))
        {
            ErrorMessage = "Không thể xác định người dùng hiện tại";
            return Page();
        }

        try
        {
            var contract = await _contractService.CreateContractAsync(Input, creatorId);
            TempData["StatusMessage"] = $"Đã tạo hợp đồng {contract.ContractNumber} thành công ở trạng thái bản nháp";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LỖI TẠO HỢP ĐỒNG - Message: {Message} | StackTrace: {StackTrace} | Inner: {Inner}",
                ex.Message, ex.StackTrace, ex.InnerException?.Message);
            ErrorMessage = $"Loi khi tao hop dong: {ex.Message}\nStack: {ex.StackTrace}\nInner: {ex.InnerException?.Message}";
            return Page();
        }
    }

    public async Task<IActionResult> OnGetPreviewContractNumber(string type)
    {
        if (!Enum.TryParse<ContractType>(type, out var contractType))
            return new JsonResult(new { contractNumber = "" });

        var number = await _contractService.GenerateContractNumberAsync(contractType);
        return new JsonResult(new { contractNumber = number });
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
