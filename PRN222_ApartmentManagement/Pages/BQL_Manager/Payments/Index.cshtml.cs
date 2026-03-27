using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Payments;

[Authorize(Policy = "AdminAndBQLManager")]
public class IndexModel : PageModel
{
    private readonly IPaymentManagementService _paymentManagementService;

    public IndexModel(IPaymentManagementService paymentManagementService)
    {
        _paymentManagementService = paymentManagementService;
    }

    public List<PaymentTransaction> Transactions { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? BillingMonth { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? BillingYear { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? PaymentMethod { get; set; }

    public async Task OnGetAsync()
    {
        Transactions = await _paymentManagementService.GetManagementTransactionsAsync(BillingMonth, BillingYear, PaymentMethod);
    }
}
