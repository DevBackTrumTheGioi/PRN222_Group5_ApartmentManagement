using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff.Payments;

[Authorize(Roles = "BQL_Staff")]
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

    public async Task OnGetAsync()
    {
        Invoices = await _paymentManagementService.GetCollectableInvoicesAsync(BillingMonth, BillingYear);
        Transactions = await _paymentManagementService.GetManagementTransactionsAsync(BillingMonth, BillingYear, PaymentMethod);
    }

    public async Task<IActionResult> OnPostCollectCashAsync(int invoiceId, decimal amount, string? note)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var result = await _paymentManagementService.RecordCashPaymentAsync(invoiceId, amount, userId.Value, note);
        StatusMessage = result.Message;
        return RedirectToPage(new { BillingMonth, BillingYear, PaymentMethod });
    }

    public async Task<IActionResult> OnPostConfirmTransferAsync(int invoiceId, decimal amount, string referenceCode, string? note)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return RedirectToPage("/Account/Login");
        }

        var result = await _paymentManagementService.ConfirmBankTransferAsync(invoiceId, amount, userId.Value, referenceCode, note);
        StatusMessage = result.Message;
        return RedirectToPage(new { BillingMonth, BillingYear, PaymentMethod });
    }

    private int? GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdString, out var userId) ? userId : null;
    }
}
