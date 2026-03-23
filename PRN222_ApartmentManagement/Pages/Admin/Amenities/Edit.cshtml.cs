using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Admin.Amenities;

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

    public IReadOnlyList<SelectListItem> AmenityTypeList { get; set; } = new List<SelectListItem>();

    public class InputModel
    {
        public int AmenityId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tiện ích.")]
        public string AmenityName { get; set; } = string.Empty;

        public int? AmenityTypeId { get; set; }

        public string? Location { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Sức chứa phải lớn hơn 0.")]
        public int? Capacity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0.")]
        public decimal? PricePerHour { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var amenity = await _context.Amenities.FindAsync(id);
        if (amenity == null) return NotFound();

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

        var types = await _context.AmenityTypes.OrderBy(t => t.TypeName).ToListAsync();
        AmenityTypeList = types.Select(t => new SelectListItem(t.TypeName, t.AmenityTypeId.ToString())).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync(Input.AmenityId);
            return Page();
        }

        var amenity = await _context.Amenities.FindAsync(Input.AmenityId);
        if (amenity == null) return NotFound();

        amenity.AmenityName = Input.AmenityName;
        amenity.AmenityTypeId = Input.AmenityTypeId;
        amenity.Location = Input.Location;
        amenity.Capacity = Input.Capacity;
        amenity.PricePerHour = Input.PricePerHour;
        amenity.Description = Input.Description;
        amenity.IsActive = Input.IsActive;

        await _context.SaveChangesAsync();

        return RedirectToPage("/Admin/Amenities/Index");
    }
}
