using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly ApartmentDbContext _context;
    private readonly IResidentApartmentAccessService _residentApartmentAccessService;

    public DashboardService(
        ApartmentDbContext context,
        IResidentApartmentAccessService residentApartmentAccessService)
    {
        _context = context;
        _residentApartmentAccessService = residentApartmentAccessService;
    }

    public async Task<AdminDashboardViewModel> GetAdminDashboardAsync()
    {
        var model = new AdminDashboardViewModel
        {
            TotalUsers = await _context.Users.CountAsync(u => !u.IsDeleted),
            ActiveUsers = await _context.Users.CountAsync(u => !u.IsDeleted && u.IsActive),
            TotalApartments = await _context.Apartments.CountAsync(),
            OccupiedApartments = await _context.Apartments.CountAsync(a => a.Status == ApartmentStatus.Occupied),
            UsersByRole = await _context.Users
                .Where(u => !u.IsDeleted && u.Role.HasValue)
                .GroupBy(u => u.Role)
                .Select(g => new UserRoleCount
                {
                    Role = g.Key.ToString()!,
                    Count = g.Count()
                }).ToListAsync()
        };

        return model;
    }

    public async Task<BQLManagerDashboardViewModel> GetBQLManagerDashboardAsync()
    {
        var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0, DateTimeKind.Local);
        
        var model = new BQLManagerDashboardViewModel
        {
            TotalApartments = await _context.Apartments.CountAsync(),
            OccupiedApartments = await _context.Apartments.CountAsync(a => a.Status == ApartmentStatus.Occupied),
            VacantApartments = await _context.Apartments.CountAsync(a => a.Status == ApartmentStatus.Available),
            
            TotalResidents = await _context.Users.CountAsync(u => !u.IsDeleted && u.Role == UserRole.Resident),
            NewResidentsThisMonth = await _context.Users.CountAsync(u => !u.IsDeleted && u.Role == UserRole.Resident && u.CreatedAt >= firstDayOfMonth),
            
            TotalRevenueThisMonth = await _context.PaymentTransactions
                .Where(t => t.PaymentDate >= firstDayOfMonth && t.Status == 1) // Assuming 1 is Success if no enum
                .SumAsync(t => t.Amount),
            
            TotalOutstandingDebt = await _context.Invoices
                .Where(i => i.Status == InvoiceStatus.Unpaid)
                .SumAsync(i => i.TotalAmount),
                
            PendingInvoices = await _context.Invoices.CountAsync(i => i.Status == InvoiceStatus.Unpaid),
            
            PendingRequests = await _context.Requests.CountAsync(r => r.Status == RequestStatus.Pending),
            InProgressRequests = await _context.Requests.CountAsync(r => r.Status == RequestStatus.InProgress),
            
            RecentRequests = await _context.Requests
                .Include(r => r.Resident)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync(),
                
            RecentInvoices = await _context.Invoices
                .Include(i => i.Apartment)
                .OrderByDescending(i => i.CreatedAt)
                .Take(5)
                .ToListAsync()
        };

        return model;
    }

    public async Task<BQLStaffDashboardViewModel> GetBQLStaffDashboardAsync(int userId)
    {
        var today = DateTime.Today;
        
        var model = new BQLStaffDashboardViewModel
        {
            AssignedRequests = await _context.Requests.CountAsync(r => r.AssignedTo == userId && r.Status != RequestStatus.Completed),
            PendingRequests = await _context.Requests.CountAsync(r => r.AssignedTo == userId && r.Status == RequestStatus.Pending),
            InProgressRequests = await _context.Requests.CountAsync(r => r.AssignedTo == userId && r.Status == RequestStatus.InProgress),
            
            TodayExpectedVisitors = await _context.Visitors.CountAsync(v => v.VisitDate.Date == today),
            
            MyRecentRequests = (await _context.Requests
                .Where(r => r.AssignedTo == userId)
                .Include(r => r.Apartment)
                .ToListAsync())
                .OrderBy(r => r.Status is RequestStatus.Completed or RequestStatus.Cancelled or RequestStatus.Rejected ? 1 : 0)
                .ThenByDescending(r => (int)r.Priority)
                .ThenBy(r => r.CreatedAt)
                .Take(5)
                .ToList()
        };

        return model;
    }

    public async Task<ResidentDashboardViewModel> GetResidentDashboardAsync(int userId)
    {
        var preferredApartment = await _residentApartmentAccessService.GetPreferredApartmentAsync(userId);
        var apartmentIds = await _residentApartmentAccessService.GetActiveApartmentIdsAsync(userId);
        var now = DateTime.Now;

        var recentAnnouncements = await _context.Announcements
            .Where(a => !a.IsDeleted
                        && a.IsActive
                        && a.PublishedDate <= now
                        && (!a.ExpiryDate.HasValue || a.ExpiryDate >= now))
            .OrderByDescending(a => a.IsPinned)
            .ThenByDescending(a => a.CreatedAt)
            .Take(5)
            .Select(a => new ResidentAnnouncementPreviewViewModel
            {
                AnnouncementId = a.AnnouncementId,
                Title = a.Title,
                Content = a.Content,
                Source = a.Source,
                CreatedAt = a.CreatedAt,
                IsPinned = a.IsPinned,
                IsRead = a.AnnouncementReads.Any(ar => ar.UserId == userId)
            })
            .ToListAsync();

        var model = new ResidentDashboardViewModel
        {
            Apartment = preferredApartment,
            UnpaidInvoices = apartmentIds.Count == 0
                ? 0
                : await _context.Invoices
                    .CountAsync(i => apartmentIds.Contains(i.ApartmentId) && i.Status == InvoiceStatus.Unpaid),
            TotalUnpaidAmount = apartmentIds.Count == 0
                ? 0
                : await _context.Invoices
                    .Where(i => apartmentIds.Contains(i.ApartmentId) && i.Status == InvoiceStatus.Unpaid)
                    .SumAsync(i => i.TotalAmount),
                
            RecentRequests = await _context.Requests
                .Where(r => r.ResidentId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync(),
                
            RecentAnnouncements = recentAnnouncements
        };

        return model;
    }

    public async Task<BQTHeadDashboardViewModel> GetBQTHeadDashboardAsync()
    {
        var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0, DateTimeKind.Local);
        
        var model = new BQTHeadDashboardViewModel
        {
            TotalRevenueThisMonth = await _context.PaymentTransactions
                .Where(t => t.PaymentDate >= firstDayOfMonth && t.Status == 1)
                .SumAsync(t => t.Amount),
                
            TotalOutstandingDebt = await _context.Invoices
                .Where(i => i.Status == InvoiceStatus.Unpaid)
                .SumAsync(i => i.TotalAmount),
                
            PendingComplaints = await _context.Requests
                .CountAsync(r => r.RequestType == RequestType.Complaint && r.Status == RequestStatus.Pending),
                
            TotalResidents = await _context.Users.CountAsync(u => !u.IsDeleted && u.Role == UserRole.Resident),
            TotalApartments = await _context.Apartments.CountAsync(),
            OccupiedApartments = await _context.Apartments.CountAsync(a => a.Status == ApartmentStatus.Occupied),
            
            RecentComplaints = await _context.Requests
                .Where(r => r.RequestType == RequestType.Complaint)
                .Include(r => r.Resident)
                .Include(r => r.Apartment)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync()
        };

        return model;
    }

    public async Task<BQTMemberDashboardViewModel> GetBQTMemberDashboardAsync()
    {
        var model = new BQTMemberDashboardViewModel
        {
            TotalOutstandingDebt = await _context.Invoices
                .Where(i => i.Status == InvoiceStatus.Unpaid)
                .SumAsync(i => i.TotalAmount),
                
            PendingComplaints = await _context.Requests
                .CountAsync(r => r.RequestType == RequestType.Complaint && r.Status != RequestStatus.Completed),
                
            TotalResidents = await _context.Users.CountAsync(u => !u.IsDeleted && u.Role == UserRole.Resident),
            TotalApartments = await _context.Apartments.CountAsync()
        };

        return model;
    }
}

