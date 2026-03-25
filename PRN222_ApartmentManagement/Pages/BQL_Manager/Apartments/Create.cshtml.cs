using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Apartments;

[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly IApartmentService _apartmentService;

    public CreateModel(IApartmentService apartmentService)
    {
        _apartmentService = apartmentService;
    }

    [BindProperty]
    public CreateApartmentDto Input { get; set; } = new();

    [TempData]
    public string? ErrorMessage { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public void OnGet()
    {
        TempData.Remove("StatusMessage");
        Input = new CreateApartmentDto();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            ErrorMessage = "Vui lòng kiểm tra lại thông tin.";
            return Page();
        }

        var creatorId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;
        var (success, message) = await _apartmentService.CreateAsync(Input, creatorId);

        if (!success)
        {
            ErrorMessage = message;
            return Page();
        }

        StatusMessage = message;
        return RedirectToPage("./Index");
    }
}
