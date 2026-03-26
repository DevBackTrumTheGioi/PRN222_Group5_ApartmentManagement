using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.BQT_Head.Meetings;

[Authorize(Roles = "BQT_Head")]
public class MinutesModel : PageModel
{
    private readonly ApartmentDbContext _context;
    private readonly IWebHostEnvironment _environment;

    private static readonly string[] AllowedExtensions = [".pdf", ".doc", ".docx", ".txt", ".png", ".jpg", ".jpeg"];

    public MinutesModel(ApartmentDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public IFormFile? MinutesFile { get; set; }

    public Meeting Meeting { get; set; } = null!;

    public class InputModel
    {
        [MaxLength(5000)]
        public string? MinutesContent { get; set; }

        [MaxLength(2000)]
        public string? Attendees { get; set; }

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
            MinutesContent = meeting.MinutesContent,
            Attendees = meeting.Attendees,
            Status = meeting.Status == MeetingStatus.Scheduled ? MeetingStatus.Completed : meeting.Status
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

        if (MinutesFile != null && !IsAllowedFile(MinutesFile.FileName))
        {
            ModelState.AddModelError(nameof(MinutesFile), "Chỉ chấp nhận file PDF, Word, TXT hoặc ảnh.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        meeting.MinutesContent = string.IsNullOrWhiteSpace(Input.MinutesContent) ? null : Input.MinutesContent.Trim();
        meeting.Attendees = string.IsNullOrWhiteSpace(Input.Attendees) ? null : Input.Attendees.Trim();
        meeting.Status = Input.Status;
        meeting.UpdatedAt = DateTime.Now;

        if (MinutesFile != null && MinutesFile.Length > 0)
        {
            var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "meetings", meeting.MeetingId.ToString());
            FileUtils.EnsureDirectoryExists(uploadFolder);

            if (!string.IsNullOrWhiteSpace(meeting.MinutesFilePath))
            {
                DeletePhysicalFile(meeting.MinutesFilePath);
            }

            var uniqueFileName = FileUtils.GenerateUniqueFileName(MinutesFile.FileName);
            var physicalPath = Path.Combine(uploadFolder, uniqueFileName);

            await using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await MinutesFile.CopyToAsync(stream);
            }

            meeting.MinutesFileName = MinutesFile.FileName;
            meeting.MinutesFilePath = $"/uploads/meetings/{meeting.MeetingId}/{uniqueFileName}";
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Đã lưu biên bản họp.";
        return RedirectToPage("Details", new { id = meeting.MeetingId });
    }

    private static bool IsAllowedFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedExtensions.Contains(extension);
    }

    private void DeletePhysicalFile(string webPath)
    {
        var relativePath = webPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var physicalPath = Path.Combine(_environment.WebRootPath, relativePath);
        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }
    }
}
