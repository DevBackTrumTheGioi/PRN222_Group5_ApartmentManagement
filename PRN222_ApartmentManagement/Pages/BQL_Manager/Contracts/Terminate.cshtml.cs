using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Contracts;

[Authorize(Policy = "AdminAndBQLManager")]
public class TerminateModel : PageModel
{
    private readonly IContractService _contractService;

    public TerminateModel(IContractService contractService)
    {
        _contractService = contractService;
    }

    public Models.Contract? Contract { get; set; }

    [BindProperty]
    public string TerminationReason { get; set; } = string.Empty;

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        TempData.Remove("StatusMessage");
        Contract = await _contractService.GetByIdWithDetailsAsync(id);

        if (Contract == null)
        {
            TempData["StatusMessage"] = "Khong tim thay hop dong.";
            return RedirectToPage("Index");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (string.IsNullOrWhiteSpace(TerminationReason))
        {
            ErrorMessage = "Vui lòng nhập lý do kết thúc hợp đồng";
            Contract = await _contractService.GetByIdWithDetailsAsync(id);
            return Page();
        }

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var userId))
        {
            ErrorMessage = "Khong the xac dinh nguoi dung hien tai.";
            Contract = await _contractService.GetByIdWithDetailsAsync(id);
            return Page();
        }

        var (success, message) = await _contractService.TerminateContractAsync(id, TerminationReason, userId);
        TempData["StatusMessage"] = message;

        if (success)
            return RedirectToPage("Index");

        ErrorMessage = message;
        Contract = await _contractService.GetByIdWithDetailsAsync(id);
        return Page();
    }
}
