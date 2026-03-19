using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Amenities;

[Authorize(Policy = "AdminOnly")]
public class EditModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public EditModel(ApartmentDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<AmenityType> AmenityTypes { get; set; } = new();

    public class InputModel
    {
        public int AmenityId { get; set; }

        [Required]
        public string AmenityName { get; set; } = string.Empty;

        public int? AmenityTypeId { get; set; }

        public string? Location { get; set; }

        public int? Capacity { get; set; }

        public decimal? PricePerHour { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        AmenityTypes = await _context.AmenityTypes.Where(t => !t.IsDeleted && t.IsActive).ToListAsync();

        var amenity = await _context.Amenities.FindAsync(id);
        if (amenity == null || amenity.IsDeleted) return NotFound();

        Input = new InputModel
        {
            AmenityId = amenity.AmenityId,
            AmenityName = amenity.AmenityName,
            AmenityTypeId = amenity.AmenityTypeId,
            Location = amenity.Location,
            Capacity = amenity.Capacity,
            PricePerHour = amenity.PricePerHour,
            Description = amenity.Description,
            IsActive = amenity.IsActive
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            AmenityTypes = await _context.AmenityTypes.Where(t => !t.IsDeleted && t.IsActive).ToListAsync();
            return Page();
        }

        var amenity = await _context.Amenities.FindAsync(Input.AmenityId);
        if (amenity == null || amenity.IsDeleted) return NotFound();

        amenity.AmenityName = Input.AmenityName;
        amenity.AmenityTypeId = Input.AmenityTypeId;
        amenity.Location = Input.Location;
        amenity.Capacity = Input.Capacity;
        amenity.PricePerHour = Input.PricePerHour;
        amenity.Description = Input.Description;
        amenity.IsActive = Input.IsActive;

        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
