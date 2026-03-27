using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQT_Member.Complaints;

[Authorize(Roles = "BQT_Member")]
public class StatsModel : PageModel
{
    private readonly IRequestService _requestService;

    public StatsModel(IRequestService requestService)
    {
        _requestService = requestService;
    }

    public int TotalComplaints { get; set; }
    public int ResolvedThisMonth { get; set; }
    public int OpenComplaints { get; set; }
    public int EscalatedComplaints { get; set; }
    public int UnassignedComplaints { get; set; }
    public int HighPriorityComplaints { get; set; }
    public string AverageResolutionHoursText { get; set; } = "0 giờ";
    public string ResolutionRateText { get; set; } = "0%";
    public List<ComplaintStatusStatItem> StatusItems { get; set; } = [];

    public async Task OnGetAsync()
    {
        var complaints = (await _requestService.GetComplaintsAsync()).ToList();
        var now = DateTime.Now;
        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

        TotalComplaints = complaints.Count;
        ResolvedThisMonth = complaints.Count(r => r.Status == RequestStatus.Completed && r.ResolvedAt >= firstDayOfMonth);
        OpenComplaints = complaints.Count(r => r.Status is RequestStatus.Pending or RequestStatus.InProgress);
        EscalatedComplaints = complaints.Count(r => r.EscalatedAt.HasValue || r.EscalatedTo.HasValue);
        UnassignedComplaints = complaints.Count(r => !r.AssignedTo.HasValue);
        HighPriorityComplaints = complaints.Count(r => r.Priority is RequestPriority.High or RequestPriority.Emergency);

        var resolvedComplaints = complaints
            .Where(r => r.Status == RequestStatus.Completed && r.ResolvedAt.HasValue)
            .ToList();

        var averageResolutionHours = resolvedComplaints.Count == 0
            ? 0
            : resolvedComplaints.Average(r => (r.ResolvedAt!.Value - r.CreatedAt).TotalHours);

        AverageResolutionHoursText = averageResolutionHours <= 0
            ? "0 giờ"
            : $"{averageResolutionHours:0.#} giờ";

        var resolvedCount = complaints.Count(r => r.Status == RequestStatus.Completed);
        ResolutionRateText = TotalComplaints == 0
            ? "0%"
            : $"{resolvedCount * 100.0 / TotalComplaints:0.#}%";

        StatusItems =
        [
            CreateStatusItem("Chờ xử lý", complaints.Count(r => r.Status == RequestStatus.Pending), "bg-amber-400"),
            CreateStatusItem("Đang xử lý", complaints.Count(r => r.Status == RequestStatus.InProgress), "bg-blue-500"),
            CreateStatusItem("Hoàn thành", complaints.Count(r => r.Status == RequestStatus.Completed), "bg-emerald-500"),
            CreateStatusItem("Đóng/Từ chối", complaints.Count(r => r.Status is RequestStatus.Cancelled or RequestStatus.Rejected), "bg-slate-500")
        ];
    }

    private ComplaintStatusStatItem CreateStatusItem(string label, int count, string barClass)
    {
        return new ComplaintStatusStatItem
        {
            Label = label,
            Count = count,
            Percent = TotalComplaints == 0 ? 0 : count * 100.0 / TotalComplaints,
            BarClass = barClass
        };
    }

    public sealed class ComplaintStatusStatItem
    {
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percent { get; set; }
        public string BarClass { get; set; } = string.Empty;
    }
}
