using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Pages.Visitors;

[Authorize(Roles = "BQL_Staff,BQL_Manager,Admin")]
public class CheckInModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public CheckInModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public Visitor? Visitor { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        Visitor = await _context.Visitors.Include(v => v.Apartment).Include(v => v.RegisteredByUser).FirstOrDefaultAsync(v => v.VisitorId == id);
        if (Visitor == null) return NotFound();
        if (Visitor.Status != VisitorStatus.Pending) return BadRequest();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var visitor = await _context.Visitors.FindAsync(id);
        if (visitor == null) return NotFound();
        if (visitor.Status != VisitorStatus.Pending) return BadRequest();

        visitor.CheckInTime = DateTime.Now;
        visitor.Status = VisitorStatus.CheckedIn;

        await _context.SaveChangesAsync();

        TempData["Success"] = "Đã ghi nhận khách vào.";
        return RedirectToPage("Index");
    }
}
