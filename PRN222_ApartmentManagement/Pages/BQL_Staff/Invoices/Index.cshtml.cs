using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff.Invoices;

[Authorize(Roles = "BQL_Staff")]
public class IndexModel : PageModel
{
    private readonly IInvoiceManagementService _invoiceManagementService;

    public IndexModel(IInvoiceManagementService invoiceManagementService)
    {
        _invoiceManagementService = invoiceManagementService;
    }

    public List<Invoice> Invoices { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int? BillingMonth { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? BillingYear { get; set; }

    [BindProperty(SupportsGet = true)]
    public InvoiceStatus? Status { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Invoices = await _invoiceManagementService.GetInvoicesAsync(BillingMonth, BillingYear, Status);
    }

    public async Task<IActionResult> OnPostGenerateAsync(int billingMonth, int billingYear, int dueDay = 10)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return RedirectToPage("/Account/Login");
        }

        var result = await _invoiceManagementService.GenerateInvoicesAsync(billingMonth, billingYear, dueDay, userId);
        StatusMessage = result.Message;
        return RedirectToPage(new { BillingMonth = billingMonth, BillingYear = billingYear });
    }
}
