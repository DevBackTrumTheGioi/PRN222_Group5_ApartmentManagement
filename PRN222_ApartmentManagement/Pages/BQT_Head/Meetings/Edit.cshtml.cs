using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Pages.BQT_Head.Meetings;

[Authorize(Roles = "BQT_Head")]
public class EditModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public EditModel(ApartmentDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public Meeting Meeting { get; set; } = null!;

    public class InputModel
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        public MeetingType MeetingType { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [Required]
        public MeetingStatus Status { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var meeting = await _context.Meetings.FirstOrDefaultAsync(m => m.MeetingId == id);
        if (meeting == null)
        {
            return NotFound();
        }

        Meeting = meeting;
        Input = new InputModel
        {
            Title = meeting.Title,
            Description = meeting.Description,
            MeetingType = meeting.MeetingType,
            Location = meeting.Location,
            ScheduledDate = meeting.ScheduledDate,
            StartTime = meeting.StartTime,
            EndTime = meeting.EndTime,
            Status = meeting.Status
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var meeting = await _context.Meetings.FirstOrDefaultAsync(m => m.MeetingId == id);
        if (meeting == null)
        {
            return NotFound();
        }

        Meeting = meeting;

        if (Input.EndTime.HasValue && Input.StartTime.HasValue && Input.EndTime.Value <= Input.StartTime.Value)
        {
            ModelState.AddModelError(nameof(Input.EndTime), "Giờ kết thúc phải lớn hơn giờ bắt đầu.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        meeting.Title = Input.Title.Trim();
        meeting.Description = Input.Description?.Trim();
        meeting.MeetingType = Input.MeetingType;
        meeting.Location = string.IsNullOrWhiteSpace(Input.Location) ? null : Input.Location.Trim();
        meeting.ScheduledDate = Input.ScheduledDate;
        meeting.StartTime = Input.StartTime;
        meeting.EndTime = Input.EndTime;
        meeting.Status = Input.Status;
        meeting.UpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã cập nhật cuộc họp.";
        return RedirectToPage("Details", new { id = meeting.MeetingId });
    }
}
