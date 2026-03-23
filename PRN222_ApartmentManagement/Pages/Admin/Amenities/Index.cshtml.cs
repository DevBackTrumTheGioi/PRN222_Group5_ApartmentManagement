using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Admin.Amenities;

[Authorize(Policy = "AdminOnly")]
public class IndexModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public IndexModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public List<Amenity> Amenities { get; set; } = new();

    // Dashboard stats
    public int TotalAmenities { get; set; }
    public int ActiveAmenities { get; set; }
    public int AmenityTypesCount { get; set; }
    public int TodayBookings { get; set; }

    public async Task OnGetAsync()
    {
        Amenities = await _context.Amenities
            .Include(a => a.AmenityType)
            .Where(a => !a.IsDeleted)
            .OrderBy(a => a.AmenityName)
            .ToListAsync();

        TotalAmenities = await _context.Amenities.CountAsync(a => !a.IsDeleted);
        ActiveAmenities = await _context.Amenities.CountAsync(a => a.IsActive && !a.IsDeleted);
        AmenityTypesCount = await _context.AmenityTypes.CountAsync();

        var today = DateTime.Today;
        TodayBookings = await _context.AmenityBookings.CountAsync(b => b.BookingDate == today && b.Status != "Cancelled");
    }

    public async Task<IActionResult> OnPostToggleActiveAsync(int id)
    {
        var amenity = await _context.Amenities.FindAsync(id);
        if (amenity == null) return NotFound();
        amenity.IsActive = !amenity.IsActive;
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var amenity = await _context.Amenities.FindAsync(id);
        if (amenity == null) return NotFound();
        amenity.IsDeleted = true;
        amenity.IsActive = false;
        await _context.SaveChangesAsync();
        return RedirectToPage();
    }
}
