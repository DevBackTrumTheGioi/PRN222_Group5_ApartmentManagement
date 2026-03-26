using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff.Visitors;

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

    [BindProperty(SupportsGet = true)]
    public int PageIndex { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public int TotalItems { get; set; }
    public int TotalPages { get; set; }

    [BindProperty(SupportsGet = true)]
    public PRN222_ApartmentManagement.Models.Enums.VisitorStatus? StatusFilter { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool ShowHistory { get; set; } = false;

    public async Task OnGetAsync()
    {
        var q = _context.Visitors
            .Include(v => v.Apartment)
            .Include(v => v.RegisteredByUser)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            var t = SearchTerm.Trim();
            q = q.Where(v => v.VisitorName.Contains(t) || v.PhoneNumber.Contains(t) || (v.IdentityCard != null && v.IdentityCard.Contains(t)) || (v.Apartment != null && v.Apartment.ApartmentNumber.Contains(t)));
        }

        if (ShowHistory)
        {
            // history: only arrived visitors
            q = q.Where(v => v.Status == PRN222_ApartmentManagement.Models.Enums.VisitorStatus.CheckedIn || v.Status == PRN222_ApartmentManagement.Models.Enums.VisitorStatus.CheckedOut);

            if (FromDate.HasValue)
            {
                var f = FromDate.Value.Date;
                q = q.Where(v => v.VisitDate >= f);
            }
            if (ToDate.HasValue)
            {
                var t = ToDate.Value.Date.AddDays(1).AddTicks(-1);
                q = q.Where(v => v.VisitDate <= t);
            }

            q = q.OrderByDescending(v => v.VisitDate).ThenByDescending(v => v.CheckInTime ?? v.CreatedAt);
        }
        else
        {
            // current registrations: allow filtering by status
            if (StatusFilter.HasValue)
            {
                q = q.Where(v => v.Status == StatusFilter.Value);
            }

            q = q.OrderByDescending(v => v.VisitDate).ThenByDescending(v => v.CreatedAt);
        }

        TotalItems = await q.CountAsync();
        if (PageIndex < 1) PageIndex = 1;
        TotalPages = (int)Math.Ceiling(TotalItems / (double)PageSize);
        if (PageIndex > TotalPages && TotalPages > 0) PageIndex = TotalPages;

        Visitors = await q.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToListAsync();
    }
}