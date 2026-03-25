using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Cards;

[Authorize(Policy = "AdminAndBQLManager")]
public class CreateModel : PageModel
{
    private readonly IResidentCardService _residentCardService;

    public CreateModel(IResidentCardService residentCardService)
    {
        _residentCardService = residentCardService;
    }

    [BindProperty]
    public CreateResidentCardDto Input { get; set; } = new();

    public string CardNumberPreview { get; set; } = "";

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        TempData.Remove("StatusMessage");
        CardNumberPreview = await _residentCardService.GenerateCardNumberAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        CardNumberPreview = await _residentCardService.GenerateCardNumberAsync();

        if (Input.ResidentId == 0)
        {
            ErrorMessage = "Vui lòng tra cứu và chọn cư dân trước.";
            return Page();
        }

        var creatorId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
        var (success, message) = await _residentCardService.CreateAsync(Input, creatorId);

        if (!success)
        {
            ErrorMessage = message;
            return Page();
        }

        StatusMessage = message;
        return RedirectToPage("./Index");
    }

    public async Task<IActionResult> OnGetSearchUserAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return new JsonResult(new { found = false, message = "Username không được để trống." });

        var result = await _residentCardService.SearchUserAsync(username.Trim());
        if (result == null)
            return new JsonResult(new { found = false, message = $"Không tìm thấy người dùng với username \"{username}\"." });

        return new JsonResult(new
        {
            found = true,
            result.UserId,
            result.Username,
            result.FullName,
            result.Phone,
            result.IsStaff,
            result.HasApartments,
            result.IsActive,
            result.IsDeleted
        });
    }

    public async Task<IActionResult> OnGetLookupApartmentsAsync(int userId)
    {
        if (userId == 0)
            return new JsonResult(new List<ApartmentWithType>());

        var apartments = await _residentCardService.GetApartmentsByUserAsync(userId);
        return new JsonResult(apartments);
    }

    public async Task<IActionResult> OnGetPreviewCardNumberAsync()
    {
        var num = await _residentCardService.GenerateCardNumberAsync();
        return new JsonResult(new { cardNumber = num });
    }
}
