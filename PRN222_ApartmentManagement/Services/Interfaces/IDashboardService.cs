using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IDashboardService
{
    // Admin Dashboard
    Task<AdminDashboardViewModel> GetAdminDashboardAsync();
    
    // BQL Manager Dashboard
    Task<BQLManagerDashboardViewModel> GetBQLManagerDashboardAsync();
    
    // BQL Staff Dashboard
    Task<BQLStaffDashboardViewModel> GetBQLStaffDashboardAsync(int userId);
    
    // Resident Dashboard
    Task<ResidentDashboardViewModel> GetResidentDashboardAsync(int userId);
    
    // BQT Head Dashboard
    Task<BQTHeadDashboardViewModel> GetBQTHeadDashboardAsync();
    
    // BQT Member Dashboard
    Task<BQTMemberDashboardViewModel> GetBQTMemberDashboardAsync();
}

// View Models for Dashboards
public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalApartments { get; set; }
    public int OccupiedApartments { get; set; }
    public List<UserRoleCount> UsersByRole { get; set; } = new();
    public List<ActivityLog> RecentActivityLogs { get; set; } = new();
}

public class UserRoleCount
{
    public string Role { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class BQLManagerDashboardViewModel
{
    // Tổng quan căn hộ
    public int TotalApartments { get; set; }
    public int OccupiedApartments { get; set; }
    public int VacantApartments { get; set; }
    
    // Tổng quan cư dân
    public int TotalResidents { get; set; }
    public int NewResidentsThisMonth { get; set; }
    
    // Tài chính
    public decimal TotalRevenueThisMonth { get; set; }
    public decimal TotalOutstandingDebt { get; set; }
    public int PendingInvoices { get; set; }
    public int OverdueInvoices { get; set; }
    
    // Yêu cầu
    public int PendingRequests { get; set; }
    public int InProgressRequests { get; set; }
    public int CompletedRequestsThisMonth { get; set; }
    
    // Dịch vụ
    public int PendingServiceOrders { get; set; }
    public int InProgressServiceOrders { get; set; }
    
    // Tiện ích
    public int TodayAmenityBookings { get; set; }
    
    // Khách thăm
    public int TodayVisitors { get; set; }
    
    // Recent items
    public List<Request> RecentRequests { get; set; } = new();
    public List<Invoice> RecentInvoices { get; set; } = new();
}

public class BQLStaffDashboardViewModel
{
    // Yêu cầu được giao
    public int AssignedRequests { get; set; }
    public int PendingRequests { get; set; }
    public int InProgressRequests { get; set; }
    public int CompletedTodayRequests { get; set; }
    
    // Dịch vụ được giao
    public int AssignedServiceOrders { get; set; }
    public int PendingServiceOrders { get; set; }
    public int InProgressServiceOrders { get; set; }
    
    // Khách thăm hôm nay
    public int TodayExpectedVisitors { get; set; }
    public int TodayCheckedInVisitors { get; set; }
    
    // Tiện ích hôm nay
    public int TodayAmenityBookings { get; set; }
    public int TodayCheckedInBookings { get; set; }
    
    // Recent items
    public List<Request> MyRecentRequests { get; set; } = new();
    public List<ServiceOrder> MyRecentServiceOrders { get; set; } = new();
    public List<Visitor> TodayVisitorsList { get; set; } = new();
}

public class ResidentDashboardViewModel
{
    // Thông tin căn hộ
    public Apartment? Apartment { get; set; }
    public Contract? CurrentContract { get; set; }
    
    // Hóa đơn
    public int UnpaidInvoices { get; set; }
    public decimal TotalUnpaidAmount { get; set; }
    public Invoice? LatestInvoice { get; set; }
    
    // Yêu cầu
    public int PendingRequests { get; set; }
    public int InProgressRequests { get; set; }
    public List<Request> RecentRequests { get; set; } = new();
    
    // Dịch vụ
    public int PendingServiceOrders { get; set; }
    public List<ServiceOrder> RecentServiceOrders { get; set; } = new();
    
    // Tiện ích
    public List<AmenityBooking> UpcomingBookings { get; set; } = new();
    
    // Thông báo
    public List<ResidentAnnouncementPreviewViewModel> RecentAnnouncements { get; set; } = new();
    public int UnreadNotifications { get; set; }

    // Xe & Thẻ
    public int RegisteredVehicles { get; set; }
    public int ActiveCards { get; set; }
}

public class ResidentAnnouncementPreviewViewModel
{
    public int AnnouncementId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}

public class BQTHeadDashboardViewModel
{
    // Tổng quan tài chính
    public decimal TotalRevenueThisMonth { get; set; }
    public decimal TotalRevenueLastMonth { get; set; }
    public decimal RevenueGrowthPercent { get; set; }
    public decimal TotalOutstandingDebt { get; set; }
    
    // Khiếu nại từ cư dân
    public int PendingComplaints { get; set; }
    public int ResolvedComplaintsThisMonth { get; set; }
    public List<Request> RecentComplaints { get; set; } = new();
    
    // Tổng quan vận hành
    public int TotalRequests { get; set; }
    public int ResolvedRequestsThisMonth { get; set; }
    public double RequestResolutionRate { get; set; }
    
    // Cư dân
    public int TotalResidents { get; set; }
    public int TotalApartments { get; set; }
    public int OccupiedApartments { get; set; }
    
    // Thông báo BQT đã đăng
    public List<Announcement> RecentBQTAnnouncements { get; set; } = new();
}

public class BQTMemberDashboardViewModel
{
    // Tổng quan tài chính (chỉ xem)
    public decimal TotalRevenueThisMonth { get; set; }
    public decimal TotalOutstandingDebt { get; set; }
    
    // Khiếu nại từ cư dân (chỉ xem)
    public int PendingComplaints { get; set; }
    public List<Request> RecentComplaints { get; set; } = new();
    
    // Tổng quan vận hành
    public int TotalRequests { get; set; }
    public double RequestResolutionRate { get; set; }
    
    // Cư dân
    public int TotalResidents { get; set; }
    public int TotalApartments { get; set; }
    
    // Thông báo
    public List<Announcement> RecentAnnouncements { get; set; } = new();
}

