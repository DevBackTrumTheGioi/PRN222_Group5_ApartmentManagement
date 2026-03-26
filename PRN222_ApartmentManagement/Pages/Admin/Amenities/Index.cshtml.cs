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

    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public int TotalItems { get; set; }
    public int TotalPages { get; set; }

    public async Task OnGetAsync()
    {
        var q = _context.Amenities
            .Include(a => a.AmenityType)
            .Where(a => !a.IsDeleted)
            .OrderBy(a => a.AmenityName)
            .AsQueryable();

        TotalItems = await q.CountAsync();
        if (PageIndex < 1) PageIndex = 1;
        TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
        if (PageIndex > TotalPages && TotalPages > 0) PageIndex = TotalPages;

        Amenities = await q.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToListAsync();

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
        return RedirectToPage(new { PageIndex, PageSize });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var amenity = await _context.Amenities.FindAsync(id);
        if (amenity == null) return NotFound();
        amenity.IsDeleted = true;
        amenity.IsActive = false;
        await _context.SaveChangesAsync();
        return RedirectToPage(new { PageIndex, PageSize });
    }
}
