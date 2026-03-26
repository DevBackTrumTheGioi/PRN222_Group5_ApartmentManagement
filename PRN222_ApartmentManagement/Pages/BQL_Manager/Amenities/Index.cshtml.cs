using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager.Amenities;

[Authorize(Policy = "AdminAndBQLManager")]
public class IndexModel : PageModel
{
    private readonly IAmenityService _amenityService;

    public IndexModel(IAmenityService amenityService)
    {
        _amenityService = amenityService;
    }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? ActiveFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? RequiresBookingFilter { get; set; }

    public IReadOnlyList<Amenity> Amenities { get; set; } = [];

    public int TotalCount { get; set; }
    public int ActiveCount { get; set; }
    public int BookingCount { get; set; }
    public int OpenAccessCount { get; set; }

    public bool HasFilter =>
        !string.IsNullOrWhiteSpace(SearchTerm) ||
        ActiveFilter.HasValue ||
        RequiresBookingFilter.HasValue;

    public async Task OnGetAsync()
    {
        var allAmenities = await _amenityService.GetManagerAmenitiesAsync(null, null, null);
        TotalCount = allAmenities.Count;
        ActiveCount = allAmenities.Count(a => a.IsActive);
        BookingCount = allAmenities.Count(a => a.RequiresBooking);
        OpenAccessCount = allAmenities.Count(a => !a.RequiresBooking);

        Amenities = await _amenityService.GetManagerAmenitiesAsync(SearchTerm, ActiveFilter, RequiresBookingFilter);
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var result = await _amenityService.DeleteAmenityAsync(id);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

        return RedirectToPage(new
        {
            SearchTerm,
            ActiveFilter,
            RequiresBookingFilter
        });
    }
}
