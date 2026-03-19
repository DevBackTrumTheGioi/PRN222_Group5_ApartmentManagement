using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Amenities;

[Authorize(Policy = "AdminOnly")]
public class CreateModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public CreateModel(ApartmentDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public List<AmenityType> AmenityTypes { get; set; } = new();

    public class InputModel
    {
        [Required]
        public string AmenityName { get; set; } = string.Empty;

        public int? AmenityTypeId { get; set; }

        public string? Location { get; set; }

        public int? Capacity { get; set; }

        public decimal? PricePerHour { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public async Task OnGetAsync()
    {
        AmenityTypes = await _context.AmenityTypes.Where(t => !t.IsDeleted && t.IsActive).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        var amenity = new Amenity
        {
            AmenityName = Input.AmenityName,
            AmenityTypeId = Input.AmenityTypeId,
            Location = Input.Location,
            Capacity = Input.Capacity,
            PricePerHour = Input.PricePerHour,
            Description = Input.Description,
            IsActive = Input.IsActive,
            CreatedAt = DateTime.Now
        };

        _context.Amenities.Add(amenity);
        await _context.SaveChangesAsync();

        return RedirectToPage("Index");
    }
}
