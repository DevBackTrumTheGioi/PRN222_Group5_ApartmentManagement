using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Payments;

[Authorize]
public class ReceiptModel : PageModel
{
    private readonly IPaymentManagementService _paymentManagementService;

    public ReceiptModel(IPaymentManagementService paymentManagementService)
    {
        _paymentManagementService = paymentManagementService;
    }

    public PaymentTransaction Transaction { get; set; } = null!;

    public string ReturnUrl { get; set; } = "/Resident/Invoices";

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (User.IsInRole("Resident"))
        {
            ReturnUrl = "/Resident/Invoices";
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out var userId))
            {
                return RedirectToPage("/Account/Login");
            }

            var transaction = await _paymentManagementService.GetReceiptForResidentAsync(id, userId);
            if (transaction == null)
            {
                return NotFound();
            }

            Transaction = transaction;
            return Page();
        }

        if (User.IsInRole("BQL_Manager"))
        {
            ReturnUrl = "/BQL_Manager/Invoices";
            var transaction = await _paymentManagementService.GetReceiptForManagementAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            Transaction = transaction;
            return Page();
        }

        if (User.IsInRole("BQL_Staff"))
        {
            ReturnUrl = "/BQL_Staff/Invoices";
            var transaction = await _paymentManagementService.GetReceiptForManagementAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }

            Transaction = transaction;
            return Page();
        }

        return RedirectToPage("/Account/AccessDenied");
    }
}
