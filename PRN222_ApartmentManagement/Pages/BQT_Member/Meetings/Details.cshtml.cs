using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.BQT_Member.Meetings;

[Authorize(Roles = "BQT_Member")]
public class DetailsModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public DetailsModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public Meeting Meeting { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var meeting = await _context.Meetings
            .AsNoTracking()
            .Include(m => m.Creator)
            .FirstOrDefaultAsync(m => m.MeetingId == id);

        if (meeting == null)
        {
            return NotFound();
        }

        Meeting = meeting;
        return Page();
    }
}
