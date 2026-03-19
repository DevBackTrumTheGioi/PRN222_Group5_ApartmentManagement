using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Invoices;

[Authorize]
public class PrintModel : PageModel
{
    private readonly IInvoiceManagementService _invoiceManagementService;

    public PrintModel(IInvoiceManagementService invoiceManagementService)
    {
        _invoiceManagementService = invoiceManagementService;
    }

    public Models.Invoice Invoice { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (User.IsInRole("Resident"))
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return RedirectToPage("/Account/Login");
            }

            var residentInvoice = await _invoiceManagementService.GetInvoiceForResidentAsync(id, userId);
            if (residentInvoice == null)
            {
                return NotFound();
            }

            Invoice = residentInvoice;
            return Page();
        }

        if (User.IsInRole("BQL_Manager") || User.IsInRole("BQL_Staff"))
        {
            var invoice = await _invoiceManagementService.GetInvoiceForManagementAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            Invoice = invoice;
            return Page();
        }

        return RedirectToPage("/Account/AccessDenied");
    }
}
