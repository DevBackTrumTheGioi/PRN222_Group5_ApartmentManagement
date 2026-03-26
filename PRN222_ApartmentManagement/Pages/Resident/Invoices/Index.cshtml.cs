using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Invoices;

[Authorize(Roles = "Resident")]
public class IndexModel : PageModel
{
    private readonly IInvoiceManagementService _invoiceManagementService;
    private readonly IPaymentManagementService _paymentManagementService;

    public IndexModel(
        IInvoiceManagementService invoiceManagementService,
        IPaymentManagementService paymentManagementService)
    {
        _invoiceManagementService = invoiceManagementService;
        _paymentManagementService = paymentManagementService;
    }

    public List<Invoice> AllInvoices { get; set; } = new();
    public List<Invoice> PayableInvoices { get; set; } = new();
    public List<PaymentTransaction> Transactions { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? BillingMonth { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? BillingYear { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    private int? GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdString, out var userId) ? userId : null;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToPage("/Account/Login");

        // Handle VNPay return redirect
        if (Request.Query.TryGetValue("vnp_ResponseCode", out var responseCode) &&
            Request.Query.TryGetValue("vnp_TxnRef", out var txnRef))
        {
            var result = await _paymentManagementService.ProcessPaymentCallbackAsync(
                txnRef.ToString(),
                responseCode.ToString(),
                Request.Query.TryGetValue("vnp_TransactionNo", out var txnNo) ? txnNo.ToString() : "",
                Request.Query.TryGetValue("vnp_BankCode", out var bankCode) ? bankCode.ToString() : "",
                Request.Query.TryGetValue("vnp_PayDate", out var payDate) ? payDate.ToString() : "",
                Request.Query.TryGetValue("vnp_SecureHash", out var secureHash) ? secureHash.ToString() : "",
                Request.Query
                    .Where(kv => kv.Key.StartsWith("vnp_"))
                    .ToDictionary(kv => kv.Key, kv => kv.Value.ToString()));

            if (responseCode.ToString() == "00" && Request.Query.TryGetValue("vnp_TransactionStatus", out var txnStatus) && txnStatus.ToString() == "00")
            {
                StatusMessage = "Thanh toán VNPay thành công! Cảm ơn bạn đã hoàn tất nghĩa vụ.";
            }
            else
            {
                StatusMessage = $"Thanh toán không thành công. Mã lỗi: {responseCode}. Vui lòng thử lại hoặc liên hệ ban quản lý.";
            }
        }

        AllInvoices = await _invoiceManagementService.GetResidentInvoicesAsync(userId.Value, BillingMonth, BillingYear);
        PayableInvoices = await _invoiceManagementService.GetResidentPayableInvoicesAsync(userId.Value, BillingMonth, BillingYear);
        Transactions = await _invoiceManagementService.GetResidentTransactionsAsync(userId.Value, BillingMonth, BillingYear);
        return Page();
    }

    public async Task<IActionResult> OnPostPayOnlineAsync(int invoiceId, string gateway)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return RedirectToPage("/Account/Login");

        if (gateway == "VNPay")
        {
            var result = await _paymentManagementService.CreateOnlinePaymentRequestAsync(invoiceId, userId.Value);
            if (!result.Success)
            {
                StatusMessage = result.Message;
                return RedirectToPage(new { BillingMonth, BillingYear });
            }
            return Redirect(result.PaymentUrl);
        }

        // Simulation for Momo and other gateways
        var simResult = await _paymentManagementService.CreateOnlinePaymentAsync(invoiceId, userId.Value, gateway);
        StatusMessage = simResult.Message;
        return RedirectToPage(new { BillingMonth, BillingYear });
    }
}
