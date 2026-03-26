using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Contracts;

[Authorize(Policy = "AdminAndBQLManager")]
public class DetailsModel : PageModel
{
    private readonly IContractService _contractService;

    public DetailsModel(IContractService contractService)
    {
        _contractService = contractService;
    }

    public Models.Contract? Contract { get; set; }
    public Models.User? Owner { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        TempData.Remove("StatusMessage");
        Contract = await _contractService.GetByIdWithDetailsAsync(id);

        if (Contract == null)
        {
            TempData["StatusMessage"] = "Khong tim thay hop dong.";
            return RedirectToPage("Index");
        }

        Owner = await _contractService.GetContractOwnerAsync(id);

        return Page();
    }

    public async Task<IActionResult> OnPostActivateAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userId, out var approverId))
        {
            ErrorMessage = "Khong the xac dinh nguoi dung hien tai.";
            return await OnGetAsync(id);
        }

        try
        {
            var contract = await _contractService.ApproveContractAsync(id, approverId);
            SuccessMessage = $"Đã kích hoạt hợp đồng {contract?.ContractNumber} - Tài khoản chủ hộ đã được tạo";
            return await OnGetAsync(id);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return await OnGetAsync(id);
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdClaim, out var deletedBy))
        {
            ErrorMessage = "Khong the xac dinh nguoi dung hien tai.";
            return await OnGetAsync(id);
        }

        var (success, message) = await _contractService.DeleteContractAsync(id, deletedBy);
        if (success)
        {
            TempData["StatusMessage"] = message;
            return RedirectToPage("Index");
        }

        ErrorMessage = message;
        return await OnGetAsync(id);
    }
}
