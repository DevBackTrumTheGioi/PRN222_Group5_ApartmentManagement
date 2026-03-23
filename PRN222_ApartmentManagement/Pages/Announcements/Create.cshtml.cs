using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Pages.Announcements;

[Authorize(Roles = "BQL_Manager,BQT_Head")]
public class CreateModel : PageModel
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

    public CreateModel(ApartmentDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public List<IFormFile>? Attachments { get; set; }

    public string AnnouncementActionLabel { get; private set; } = string.Empty;
    public string SourceCode { get; private set; } = string.Empty;

    public class InputModel
    {
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
        public DateTime PublishedDate { get; set; } = DateTime.Now;

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsPinned { get; set; } = false;
    }

    public IActionResult OnGet()
    {
        if (!TryResolveSource(out _, out _))
        {
            return Forbid();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!TryResolveSource(out var source, out _))
        {
            return Forbid();
        }

        if (Input.PublishedDate == default)
        {
            ModelState.AddModelError(nameof(Input.PublishedDate), "Ngày đăng không hợp lệ.");
        }

        if (Input.ExpiryDate.HasValue && Input.ExpiryDate.Value < Input.PublishedDate)
        {
            ModelState.AddModelError(nameof(Input.ExpiryDate), "Ngày hết hạn phải lớn hơn hoặc bằng ngày đăng.");
        }

        ValidateAttachments();

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var entity = new Announcement
        {
            Title = Input.Title.Trim(),
            Content = Input.Content.Trim(),
            AnnouncementType = Input.AnnouncementType,
            Priority = Input.Priority,
            PublishedDate = Input.PublishedDate,
            ExpiryDate = Input.ExpiryDate,
            IsActive = Input.IsActive,
            IsPinned = Input.IsPinned,
            Source = source,
            CreatedBy = userId.Value,
            CreatedAt = DateTime.Now,
            IsDeleted = false
        };

        _context.Announcements.Add(entity);
        await _context.SaveChangesAsync();

        await SaveAttachmentsAsync(entity.AnnouncementId);

        TempData["SuccessMessage"] = "Tạo thông báo thành công.";
        return RedirectToPage("Index");
    }

    private void ValidateAttachments()
    {
        if (Attachments == null || Attachments.Count == 0) return;

        if (Attachments.Count > 10)
        {
            ModelState.AddModelError(nameof(Attachments), "Tối đa 10 tệp đính kèm.");
            return;
        }

        foreach (var file in Attachments)
        {
            if (file.Length > MaxFileSizeBytes)
            {
                ModelState.AddModelError(nameof(Attachments), $"Tệp '{file.FileName}' vượt quá dung lượng tối đa 20MB.");
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                ModelState.AddModelError(nameof(Attachments), $"Tệp '{file.FileName}' không hợp lệ.");
            }
        }
    }

    private async Task SaveAttachmentsAsync(int announcementId)
    {
        if (Attachments == null || Attachments.Count == 0) return;

        var uploadPath = Path.Combine(_environment.WebRootPath, "uploads", "announcements", announcementId.ToString());
        Directory.CreateDirectory(uploadPath);

        foreach (var file in Attachments)
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

        await _context.SaveChangesAsync();
    }

    private bool TryResolveSource(out string source, out string actionLabel)
    {
        if (User.IsInRole("BQL_Manager"))
        {
            source = "BQL";
            actionLabel = "Đăng thông báo vận hành";
            SourceCode = source;
            AnnouncementActionLabel = actionLabel;
            return true;
        }

        if (User.IsInRole("BQT_Head"))
        {
            source = "BQT";
            actionLabel = "Đăng thông báo chính sách";
            SourceCode = source;
            AnnouncementActionLabel = actionLabel;
            return true;
        }

        source = string.Empty;
        actionLabel = string.Empty;
        return false;
    }

    private int? GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var userId) ? userId : null;
    }
}
