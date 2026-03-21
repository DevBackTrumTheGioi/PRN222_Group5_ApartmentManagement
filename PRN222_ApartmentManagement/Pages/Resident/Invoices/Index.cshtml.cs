using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Invoices;

[Authorize(Roles = "Resident")]
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

    public async Task<IActionResult> OnGetAsync()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return RedirectToPage("/Account/Login");
        }

        Invoices = await _invoiceManagementService.GetResidentInvoicesAsync(userId, BillingMonth, BillingYear);
        return Page();
    }
}
