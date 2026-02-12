using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services;

namespace PRN222_ApartmentManagement.Pages.Admin;

public class ActivityLogsModel : PageModel
{
    private readonly IActivityLogService _activityLogService;

    public ActivityLogsModel(IActivityLogService activityLogService)
    {
        _activityLogService = activityLogService;
    }

    public List<ActivityLog> Logs { get; set; } = new();
    
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? Action { get; set; }
    public string? EntityName { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public int TotalPages { get; set; }

    public async Task OnGetAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? action = null,
        string? entityName = null,
        int pageNumber = 1)
    {
        FromDate = fromDate;
        ToDate = toDate;
        Action = action;
        EntityName = entityName;
        PageNumber = pageNumber;

        Logs = await _activityLogService.GetLogsAsync(
            fromDate: fromDate,
            toDate: toDate,
            action: action,
            entityName: entityName,
            pageNumber: pageNumber,
            pageSize: PageSize
        );

        // Calculate total pages (simplified - in production, get total count from service)
        TotalPages = Logs.Count < PageSize ? PageNumber : PageNumber + 1;
    }
}

