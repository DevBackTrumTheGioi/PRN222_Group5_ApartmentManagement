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

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }
    public async Task OnGetAsync()
    {
        var query = _context.Visitors
            .Include(v => v.Apartment)
            .Include(v => v.RegisteredByUser)
            .AsQueryable();

        if (!string.IsNullOrEmpty(SearchTerm))
        {
            var keyword = SearchTerm.ToLower();

            query = query.Where(v =>
                v.VisitorName.ToLower().Contains(keyword) ||
                v.PhoneNumber.Contains(keyword) ||
                (v.IdentityCard != null && v.IdentityCard.Contains(keyword)) ||
                v.Apartment.ApartmentNumber.ToLower().Contains(keyword) ||
                v.RegisteredByUser.FullName.ToLower().Contains(keyword)
            );
        }

        Visitors = await query
            .OrderByDescending(v => v.VisitDate)
            .ThenByDescending(v => v.CreatedAt)
            .ToListAsync();
    }

}
