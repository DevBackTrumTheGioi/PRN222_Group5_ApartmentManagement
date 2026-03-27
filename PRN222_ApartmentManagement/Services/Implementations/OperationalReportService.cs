using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class OperationalReportService : IOperationalReportService
{
    private readonly ApartmentDbContext _context;

    public OperationalReportService(ApartmentDbContext context)
    {
        _context = context;
    }

    public async Task<OperationalReportDto> GetOperationalReportAsync(OperationalReportFilterDto filter)
    {
        var (currentStart, currentEndExclusive, currentLabel) = ResolvePeriod(filter);
        var (previousStart, previousEndExclusive, previousLabel) = ResolvePreviousPeriod(filter, currentStart);

        var currentRequests = _context.Requests.Where(r => r.CreatedAt >= currentStart && r.CreatedAt < currentEndExclusive);
        var previousRequests = _context.Requests.Where(r => r.CreatedAt >= previousStart && r.CreatedAt < previousEndExclusive);

        var currentServiceOrders = _context.ServiceOrders.Where(o => o.CreatedAt >= currentStart && o.CreatedAt < currentEndExclusive);
        var previousServiceOrders = _context.ServiceOrders.Where(o => o.CreatedAt >= previousStart && o.CreatedAt < previousEndExclusive);

        var currentVisitors = _context.Visitors.Where(v => v.CheckInTime.HasValue && v.CheckInTime.Value >= currentStart && v.CheckInTime.Value < currentEndExclusive);
        var previousVisitors = _context.Visitors.Where(v => v.CheckInTime.HasValue && v.CheckInTime.Value >= previousStart && v.CheckInTime.Value < previousEndExclusive);

        var currentAmenityBookings = _context.AmenityBookings.Where(b => b.CreatedAt >= currentStart && b.CreatedAt < currentEndExclusive);
        var previousAmenityBookings = _context.AmenityBookings.Where(b => b.CreatedAt >= previousStart && b.CreatedAt < previousEndExclusive);

        var currentRequestList = await currentRequests.ToListAsync();
        var previousRequestList = await previousRequests.ToListAsync();
        var currentServiceOrderList = await currentServiceOrders.ToListAsync();
        var previousServiceOrderList = await previousServiceOrders.ToListAsync();

        var currentAvgResolutionHours = CalculateAverageResolutionHours(currentRequestList);
        var previousAvgResolutionHours = CalculateAverageResolutionHours(previousRequestList);

        var currentResolutionRate = CalculateResolutionRate(currentRequestList);
        var previousResolutionRate = CalculateResolutionRate(previousRequestList);

        var report = new OperationalReportDto
        {
            CurrentPeriodLabel = currentLabel,
            PreviousPeriodLabel = previousLabel,
            CurrentPeriodStart = currentStart,
            CurrentPeriodEnd = currentEndExclusive.AddTicks(-1),
            PreviousPeriodStart = previousStart,
            PreviousPeriodEnd = previousEndExclusive.AddTicks(-1),
            Metrics =
            [
                CreateMetric("Yêu cầu cư dân", "assignment", currentRequestList.Count, previousRequestList.Count),
                CreateMetric("Khiếu nại / sự cố", "report_problem",
                    currentRequestList.Count(r => r.RequestType == RequestType.Complaint),
                    previousRequestList.Count(r => r.RequestType == RequestType.Complaint)),
                CreateMetric("Tỷ lệ xử lý", "task_alt", currentResolutionRate, previousResolutionRate, "%"),
                CreateMetric("TG xử lý TB", "schedule", currentAvgResolutionHours, previousAvgResolutionHours, " giờ"),
                CreateMetric("Đơn dịch vụ", "handyman", currentServiceOrderList.Count, previousServiceOrderList.Count),
                CreateMetric("Khách check-in", "groups",
                    await currentVisitors.CountAsync(),
                    await previousVisitors.CountAsync()),
                CreateMetric("Booking tiện ích", "event_available",
                    await currentAmenityBookings.CountAsync(),
                    await previousAmenityBookings.CountAsync())
            ],
            RequestStatusBreakdown =
            [
                CreateBreakdown("Chờ xử lý",
                    currentRequestList.Count(r => r.Status == RequestStatus.Pending),
                    previousRequestList.Count(r => r.Status == RequestStatus.Pending)),
                CreateBreakdown("Đang xử lý",
                    currentRequestList.Count(r => r.Status == RequestStatus.InProgress),
                    previousRequestList.Count(r => r.Status == RequestStatus.InProgress)),
                CreateBreakdown("Hoàn thành",
                    currentRequestList.Count(r => r.Status == RequestStatus.Completed),
                    previousRequestList.Count(r => r.Status == RequestStatus.Completed)),
                CreateBreakdown("Đóng/Từ chối",
                    currentRequestList.Count(r => r.Status is RequestStatus.Cancelled or RequestStatus.Rejected),
                    previousRequestList.Count(r => r.Status is RequestStatus.Cancelled or RequestStatus.Rejected))
            ],
            ServiceOrderBreakdown =
            [
                CreateBreakdown("Chờ xác nhận",
                    currentServiceOrderList.Count(o => o.Status == ServiceOrderStatus.Pending),
                    previousServiceOrderList.Count(o => o.Status == ServiceOrderStatus.Pending)),
                CreateBreakdown("Đang xử lý",
                    currentServiceOrderList.Count(o => o.Status == ServiceOrderStatus.InProgress),
                    previousServiceOrderList.Count(o => o.Status == ServiceOrderStatus.InProgress)),
                CreateBreakdown("Hoàn thành",
                    currentServiceOrderList.Count(o => o.Status == ServiceOrderStatus.Completed),
                    previousServiceOrderList.Count(o => o.Status == ServiceOrderStatus.Completed)),
                CreateBreakdown("Đã hủy",
                    currentServiceOrderList.Count(o => o.Status == ServiceOrderStatus.Cancelled),
                    previousServiceOrderList.Count(o => o.Status == ServiceOrderStatus.Cancelled))
            ]
        };

        return report;
    }

    private static (DateTime Start, DateTime EndExclusive, string Label) ResolvePeriod(OperationalReportFilterDto filter)
    {
        return filter.PeriodType switch
        {
            ReportingPeriodType.Month =>
                (new DateTime(filter.Year, filter.Month, 1),
                new DateTime(filter.Year, filter.Month, 1).AddMonths(1),
                $"Tháng {filter.Month:00}/{filter.Year}"),
            ReportingPeriodType.Quarter =>
                (new DateTime(filter.Year, ((filter.Quarter - 1) * 3) + 1, 1),
                new DateTime(filter.Year, ((filter.Quarter - 1) * 3) + 1, 1).AddMonths(3),
                $"Quý {filter.Quarter}/{filter.Year}"),
            _ =>
                (new DateTime(filter.Year, 1, 1),
                new DateTime(filter.Year, 1, 1).AddYears(1),
                $"Năm {filter.Year}")
        };
    }

    private static (DateTime Start, DateTime EndExclusive, string Label) ResolvePreviousPeriod(OperationalReportFilterDto filter, DateTime currentStart)
    {
        return filter.PeriodType switch
        {
            ReportingPeriodType.Month =>
                (currentStart.AddMonths(-1), currentStart, $"Tháng {currentStart.AddMonths(-1):MM/yyyy}"),
            ReportingPeriodType.Quarter =>
                (currentStart.AddMonths(-3), currentStart,
                    $"Quý {(((currentStart.AddMonths(-3).Month - 1) / 3) + 1)}/{currentStart.AddMonths(-3).Year}"),
            _ =>
                (currentStart.AddYears(-1), currentStart, $"Năm {currentStart.AddYears(-1).Year}")
        };
    }

    private static decimal CalculateAverageResolutionHours(IEnumerable<Models.Request> requests)
    {
        var resolved = requests
            .Where(r => r.Status == RequestStatus.Completed && r.ResolvedAt.HasValue)
            .ToList();

        if (resolved.Count == 0)
        {
            return 0;
        }

        return (decimal)resolved.Average(r => (r.ResolvedAt!.Value - r.CreatedAt).TotalHours);
    }

    private static decimal CalculateResolutionRate(IReadOnlyCollection<Models.Request> requests)
    {
        if (requests.Count == 0)
        {
            return 0;
        }

        var completedCount = requests.Count(r => r.Status == RequestStatus.Completed);
        return Math.Round(completedCount * 100m / requests.Count, 1);
    }

    private static OperationalMetricComparisonDto CreateMetric(string label, string icon, decimal currentValue, decimal previousValue, string unitSuffix = "")
    {
        var deltaValue = currentValue - previousValue;
        var deltaPercent = previousValue == 0
            ? (currentValue == 0 ? 0 : 100)
            : Math.Round(deltaValue * 100 / previousValue, 1);

        return new OperationalMetricComparisonDto
        {
            Label = label,
            Icon = icon,
            UnitSuffix = unitSuffix,
            CurrentValue = currentValue,
            PreviousValue = previousValue,
            DeltaValue = deltaValue,
            DeltaPercent = deltaPercent
        };
    }

    private static OperationalBreakdownItemDto CreateBreakdown(string label, int currentCount, int previousCount)
    {
        return new OperationalBreakdownItemDto
        {
            Label = label,
            CurrentCount = currentCount,
            PreviousCount = previousCount
        };
    }
}
