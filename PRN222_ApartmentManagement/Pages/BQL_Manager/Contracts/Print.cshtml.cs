using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Contracts;

[Authorize(Roles = "BQL_Manager")]
public class PrintModel : PageModel
{
    private readonly IContractService _contractService;

    public PrintModel(IContractService contractService)
    {
        _contractService = contractService;
    }

    public Models.Contract? Contract { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Contract = await _contractService.GetByIdWithDetailsAsync(id);

        if (Contract == null)
        {
            TempData["StatusMessage"] = "Khong tim thay hop dong.";
            return RedirectToPage("Index");
        }

        return Page();
    }
}
