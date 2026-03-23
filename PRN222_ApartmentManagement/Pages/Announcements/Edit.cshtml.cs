using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Pages.Announcements;

[Authorize(Roles = "BQL_Manager,BQT_Head")]
public class EditModel : PageModel
{
    private readonly ApartmentDbContext _context;
    private readonly IWebHostEnvironment _environment;

    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg", "image/png", "image/gif", "image/webp",
        "video/mp4", "video/webm", "video/quicktime",
        "application/pdf", "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "text/plain", "application/zip", "application/x-zip-compressed"
    ];

    private const long MaxFileSizeBytes = 20 * 1024 * 1024;

    public EditModel(ApartmentDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public List<IFormFile>? NewAttachments { get; set; }

    [BindProperty]
    public List<int> RemoveAttachmentIds { get; set; } = [];

    public List<AnnouncementAttachment> ExistingAttachments { get; set; } = [];

    public string AnnouncementActionLabel { get; private set; } = string.Empty;
    public string SourceCode { get; private set; } = string.Empty;

    public class InputModel
    {
        public int AnnouncementId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề.")]
        [MaxLength(200, ErrorMessage = "Tiêu đề không vượt quá 200 ký tự.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Tiêu đề không được chỉ chứa khoảng trắng.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập nội dung.")]
        [MaxLength(4000, ErrorMessage = "Nội dung không vượt quá 4000 ký tự.")]
        [RegularExpression(@".*\S.*", ErrorMessage = "Nội dung không được chỉ chứa khoảng trắng.")]
        public string Content { get; set; } = string.Empty;

        public AnnouncementType? AnnouncementType { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn mức độ ưu tiên.")]
        public AnnouncementPriority Priority { get; set; } = AnnouncementPriority.Normal;

        [Required(ErrorMessage = "Vui lòng chọn thời điểm đăng.")]
        public DateTime PublishedDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; }

        public bool IsPinned { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        if (!TryResolveSource(out var source, out _))
        {
            return Forbid();
        }

        var announcement = await _context.Announcements
            .Include(a => a.Attachments)
            .FirstOrDefaultAsync(a => a.AnnouncementId == id && !a.IsDeleted && a.Source == source);

        if (announcement == null)
        {
            return NotFound();
        }

        Input = new InputModel
        {
            AnnouncementId = announcement.AnnouncementId,
            Title = announcement.Title,
            Content = announcement.Content,
            AnnouncementType = announcement.AnnouncementType,
            Priority = announcement.Priority,
            PublishedDate = announcement.PublishedDate,
            ExpiryDate = announcement.ExpiryDate,
            IsActive = announcement.IsActive,
            IsPinned = announcement.IsPinned
        };

        ExistingAttachments = announcement.Attachments.OrderByDescending(x => x.UploadedAt).ToList();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!TryResolveSource(out var source, out _))
        {
            return Forbid();
        }

        if (Input.AnnouncementId <= 0)
        {
            return NotFound();
        }

        if (Input.PublishedDate == default)
        {
            ModelState.AddModelError(nameof(Input.PublishedDate), "Ngày đăng không hợp lệ.");
        }

        if (Input.ExpiryDate.HasValue && Input.ExpiryDate.Value < Input.PublishedDate)
        {
            ModelState.AddModelError(nameof(Input.ExpiryDate), "Ngày hết hạn phải lớn hơn hoặc bằng ngày đăng.");
        }

        ValidateNewAttachments();

        var announcement = await _context.Announcements
            .Include(a => a.Attachments)
            .FirstOrDefaultAsync(a => a.AnnouncementId == Input.AnnouncementId && !a.IsDeleted && a.Source == source);

        if (announcement == null)
        {
            return NotFound();
        }

        ExistingAttachments = announcement.Attachments.OrderByDescending(x => x.UploadedAt).ToList();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        announcement.Title = Input.Title.Trim();
        announcement.Content = Input.Content.Trim();
        announcement.AnnouncementType = Input.AnnouncementType;
        announcement.Priority = Input.Priority;
        announcement.PublishedDate = Input.PublishedDate;
        announcement.ExpiryDate = Input.ExpiryDate;
        announcement.IsActive = Input.IsActive;
        announcement.IsPinned = Input.IsPinned;
        announcement.UpdatedAt = DateTime.Now;

        await RemoveAttachmentsAsync(announcement);
        await AddNewAttachmentsAsync(announcement.AnnouncementId);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Cập nhật thông báo thành công.";
        return RedirectToPage("Index");
    }

    private void ValidateNewAttachments()
    {
        if (NewAttachments == null || NewAttachments.Count == 0) return;

        if (NewAttachments.Count > 10)
        {
            ModelState.AddModelError(nameof(NewAttachments), "Tối đa 10 tệp mới mỗi lần cập nhật.");
            return;
        }

        foreach (var file in NewAttachments)
        {
            if (file.Length > MaxFileSizeBytes)
            {
                ModelState.AddModelError(nameof(NewAttachments), $"Tệp '{file.FileName}' vượt quá dung lượng tối đa 20MB.");
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                ModelState.AddModelError(nameof(NewAttachments), $"Tệp '{file.FileName}' không hợp lệ.");
            }
        }
    }

    private async Task RemoveAttachmentsAsync(Announcement announcement)
    {
        if (RemoveAttachmentIds == null || RemoveAttachmentIds.Count == 0) return;

        var toRemove = announcement.Attachments
            .Where(a => RemoveAttachmentIds.Contains(a.AttachmentId))
            .ToList();

        foreach (var attachment in toRemove)
        {
            var physicalPath = Path.Combine(_environment.WebRootPath, attachment.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            _context.AnnouncementAttachments.Remove(attachment);
        }

        await Task.CompletedTask;
    }

    private async Task AddNewAttachmentsAsync(int announcementId)
    {
        if (NewAttachments == null || NewAttachments.Count == 0) return;

        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "announcements", announcementId.ToString());
        Directory.CreateDirectory(uploadPath);

        foreach (var file in NewAttachments)
        {
            if (file.Length == 0) continue;

            var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var physicalPath = Path.Combine(uploadPath, storedFileName);

            await using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _context.AnnouncementAttachments.Add(new AnnouncementAttachment
            {
                AnnouncementId = announcementId,
                FileName = file.FileName,
                FilePath = $"/uploads/announcements/{announcementId}/{storedFileName}",
                FileSize = file.Length,
                ContentType = file.ContentType,
                UploadedAt = DateTime.Now
            });
        }
    }

    private bool TryResolveSource(out string source, out string actionLabel)
    {
        if (User.IsInRole("BQL_Manager"))
        {
            source = "BQL";
            actionLabel = "Đăng thông báo vận hành (Source = 'BQL')";
            SourceCode = source;
            AnnouncementActionLabel = actionLabel;
            return true;
        }

        if (User.IsInRole("BQT_Head"))
        {
            source = "BQT";
            actionLabel = "Đăng thông báo chính sách (Source = 'BQT')";
            SourceCode = source;
            AnnouncementActionLabel = actionLabel;
            return true;
        }

        source = string.Empty;
        actionLabel = string.Empty;
        return false;
    }
}
