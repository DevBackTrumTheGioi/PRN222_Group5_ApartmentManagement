using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Payments;

[Authorize(Roles = "Resident")]
public class IndexModel : PageModel
{
    private readonly IPaymentManagementService _paymentManagementService;

    public IndexModel(IPaymentManagementService paymentManagementService)
    {
        _paymentManagementService = paymentManagementService;
    }

    public List<Invoice> Invoices { get; set; } = new();
    public List<PaymentTransaction> Transactions { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? BillingMonth { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? BillingYear { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? PaymentMethod { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        Invoices = await _paymentManagementService.GetResidentPayableInvoicesAsync(userId.Value, BillingMonth, BillingYear);
        Transactions = await _paymentManagementService.GetResidentTransactionsAsync(userId.Value, BillingMonth, BillingYear, PaymentMethod);
        return Page();
    }

    public async Task<IActionResult> OnPostPayOnlineAsync(int invoiceId, string gateway)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var result = await _paymentManagementService.CreateOnlinePaymentAsync(invoiceId, userId.Value, gateway);
        StatusMessage = result.Message;
        return RedirectToPage(new { BillingMonth, BillingYear, PaymentMethod });
    }

    private int? GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdString, out var userId) ? userId : null;
    }
}
