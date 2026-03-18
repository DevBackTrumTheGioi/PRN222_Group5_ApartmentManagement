using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Visitors;

[Authorize(Roles = "BQL_Staff,BQL_Manager,Admin")]
public class IndexModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public IndexModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public List<Visitor> Visitors { get; set; } = new();

    public async Task OnGetAsync()
    {
        Visitors = await _context.Visitors
            .Include(v => v.Apartment)
            .Include(v => v.RegisteredByUser)
            .OrderByDescending(v => v.VisitDate)
            .ThenByDescending(v => v.CreatedAt)
            .ToListAsync();
    }
}
