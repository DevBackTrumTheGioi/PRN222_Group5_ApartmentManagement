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
public class CreateModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public CreateModel(ApartmentDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public IReadOnlyList<SelectListItem> AmenityTypeList { get; set; } = new List<SelectListItem>();

    public class InputModel
    {
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

        public bool IsActive { get; set; } = true;
    }

    public async Task OnGetAsync()
    {
        var types = await _context.AmenityTypes.OrderBy(t => t.TypeName).ToListAsync();
        AmenityTypeList = types.Select(t => new SelectListItem(t.TypeName, t.AmenityTypeId.ToString())).ToList();
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

        return RedirectToPage("/Admin/Amenities/Index");
    }
}
