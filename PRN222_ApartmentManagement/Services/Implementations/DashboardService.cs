using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class DashboardService : IDashboardService
{
    private readonly ApartmentDbContext _context;

    public DashboardService(ApartmentDbContext context)
    {
        _context = context;
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
            CompletedTodayRequests = await _context.Requests.CountAsync(r => r.AssignedTo == userId && r.Status == RequestStatus.Completed && r.CreatedAt.Date == today),

            TodayExpectedVisitors = await _context.Visitors.CountAsync(v => v.VisitDate.Date == today),
            TodayCheckedInVisitors = await _context.Visitors.CountAsync(v => v.VisitDate.Date == today && v.Status == VisitorStatus.CheckedOut),

            MyRecentRequests = await _context.Requests
                .Where(r => r.AssignedTo == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync(),

            // Tiện ích hôm nay
            TodayAmenityBookings = await _context.AmenityBookings.CountAsync(b => b.BookingDate == today && b.Status != "Cancelled"),
            TodayCheckedInBookings = await _context.AmenityBookings.CountAsync(b => b.BookingDate == today && b.Status == "CheckedIn"),

            // Danh sách khách hôm nay
            TodayVisitorsList = await _context.Visitors
                .Where(v => v.VisitDate.Date == today)
                .OrderBy(v => v.CreatedAt)
                .ToListAsync()
        };

        return model;
    }

    public async Task<ResidentDashboardViewModel> GetResidentDashboardAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Apartment)
            .FirstOrDefaultAsync(u => u.UserId == userId);
            
        var model = new ResidentDashboardViewModel
        {
            Apartment = user?.Apartment,
            UnpaidInvoices = await _context.Invoices
                .CountAsync(i => user != null && i.ApartmentId == user.ApartmentId && i.Status == InvoiceStatus.Unpaid),
            TotalUnpaidAmount = await _context.Invoices
                .Where(i => user != null && i.ApartmentId == user.ApartmentId && i.Status == InvoiceStatus.Unpaid)
                .SumAsync(i => i.TotalAmount),
                
            RecentRequests = await _context.Requests
                .Where(r => r.ResidentId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync(),
                
            RecentAnnouncements = await _context.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .ToListAsync()
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
                .CountAsync(r => r.RequestType == RequestType.Complaint && r.Status != RequestStatus.Completed),
                
            TotalResidents = await _context.Users.CountAsync(u => !u.IsDeleted && u.Role == UserRole.Resident),
            TotalApartments = await _context.Apartments.CountAsync(),
            OccupiedApartments = await _context.Apartments.CountAsync(a => a.Status == ApartmentStatus.Occupied),
            
            RecentComplaints = await _context.Requests
                .Where(r => r.RequestType == RequestType.Complaint)
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

