using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Amenities;

[Authorize(Roles = "Resident")]
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
    public int? SelectedAmenityId { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? BookingDate { get; set; }

    public IReadOnlyList<Amenity> Amenities { get; set; } = [];
    public Amenity? SelectedAmenity { get; set; }
    public IReadOnlyList<AmenityAvailabilitySlotDto> Slots { get; set; } = [];
    public bool HasApartment { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = GetUserId();
        if (userId == null)
        {
            return Forbid();
        }

        HasApartment = await _amenityService.ResidentHasApartmentAsync(userId.Value);
        BookingDate ??= DateTime.Now.Date;

        Amenities = await _amenityService.GetResidentAmenitiesAsync(SearchTerm);
        if (!Amenities.Any())
        {
            return Page();
        }

        SelectedAmenity = SelectedAmenityId.HasValue
            ? Amenities.FirstOrDefault(a => a.AmenityId == SelectedAmenityId.Value)
            : Amenities.First();

        SelectedAmenity ??= Amenities.First();
        SelectedAmenityId = SelectedAmenity.AmenityId;

        if (SelectedAmenity.RequiresBooking)
        {
            Slots = await _amenityService.GetAvailabilitySlotsAsync(SelectedAmenity.AmenityId, BookingDate.Value);
        }

        return Page();
    }

    private int? GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, out var userId) ? userId : null;
    }
}
