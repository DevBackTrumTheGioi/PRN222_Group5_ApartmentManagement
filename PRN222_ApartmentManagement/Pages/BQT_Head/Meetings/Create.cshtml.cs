using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQT_Head.Meetings;

[Authorize(Roles = "BQT_Head")]
public class CreateModel : PageModel
{
    private readonly ApartmentDbContext _context;
    private readonly INotificationService _notificationService;

    public CreateModel(ApartmentDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
        [MaxLength(200, ErrorMessage = "Tiêu đề không quá 200 ký tự.")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000, ErrorMessage = "Mô tả không quá 2000 ký tự.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại cuộc họp.")]
        public MeetingType MeetingType { get; set; } = MeetingType.BQT;

        [MaxLength(200, ErrorMessage = "Địa điểm không quá 200 ký tự.")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thời gian họp.")]
        public DateTime ScheduledDate { get; set; } = DateTime.Now.AddDays(7);

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [Required]
        public MeetingStatus Status { get; set; } = MeetingStatus.Scheduled;

        public bool SendNotificationToResidents { get; set; } = true;
    }

    public void OnGet()
    {
        Input.ScheduledDate = DateTime.Now.AddDays(7);
        Input.Status = MeetingStatus.Scheduled;
        Input.MeetingType = MeetingType.BQT;
        Input.SendNotificationToResidents = true;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Input.EndTime.HasValue && Input.StartTime.HasValue && Input.EndTime.Value <= Input.StartTime.Value)
        {
            ModelState.AddModelError(nameof(Input.EndTime), "Giờ kết thúc phải lớn hơn giờ bắt đầu.");
        }

        if (Input.ScheduledDate == default)
        {
            ModelState.AddModelError(nameof(Input.ScheduledDate), "Thời gian họp không hợp lệ.");
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var meeting = new Meeting
        {
            Title = Input.Title.Trim(),
            Description = Input.Description?.Trim(),
            MeetingType = Input.MeetingType,
            Location = string.IsNullOrWhiteSpace(Input.Location) ? null : Input.Location.Trim(),
            ScheduledDate = Input.ScheduledDate,
            StartTime = Input.StartTime,
            EndTime = Input.EndTime,
            Status = Input.Status,
            CreatedBy = userId.Value,
            CreatedAt = DateTime.Now
        };

        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        if (Input.SendNotificationToResidents)
        {
            var recipientQuery = _context.Users
                .Where(u => u.IsActive && !u.IsDeleted);

            recipientQuery = Input.MeetingType switch
            {
                MeetingType.Resident => recipientQuery.Where(u => u.Role == UserRole.Resident),
                MeetingType.BQT => recipientQuery.Where(u => u.Role == UserRole.BQT_Head || u.Role == UserRole.BQT_Member),
                MeetingType.Emergency => recipientQuery.Where(u => u.Role == UserRole.Resident || u.Role == UserRole.BQT_Head || u.Role == UserRole.BQT_Member),
                _ => recipientQuery.Where(u => u.Role == UserRole.Resident)
            };

            var recipientIds = await recipientQuery.Select(u => u.UserId).ToListAsync();

            if (recipientIds.Count > 0)
            {
                var (title, content) = Utils.NotificationUtils.CreateMeetingNotification(
                    meeting.Title,
                    meeting.ScheduledDate,
                    meeting.Location);

                await _notificationService.CreateBulkNotificationsAsync(
                    recipientIds,
                    title,
                    content,
                    NotificationType.Meeting,
                    ReferenceType.Meeting,
                    meeting.MeetingId,
                    meeting.Status == MeetingStatus.Cancelled ? NotificationPriority.High : NotificationPriority.Normal);
            }
        }

        TempData["SuccessMessage"] = "Đã tạo cuộc họp mới.";
        return RedirectToPage("Details", new { id = meeting.MeetingId });
    }

    private int? GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var userId) ? userId : null;
    }
}
