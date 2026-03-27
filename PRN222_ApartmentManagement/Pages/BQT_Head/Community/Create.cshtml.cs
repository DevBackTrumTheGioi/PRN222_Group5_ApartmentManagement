using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQT_Head.Community;

[Authorize(Roles = "BQT_Head")]
public class CreateModel : PageModel
{
    private readonly ICommunityEngagementService _communityEngagementService;

    public CreateModel(ICommunityEngagementService communityEngagementService)
    {
        _communityEngagementService = communityEngagementService;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [Display(Name = "Tiêu đề chiến dịch")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Câu hỏi là bắt buộc")]
        [Display(Name = "Câu hỏi chính")]
        public string QuestionText { get; set; } = string.Empty;

        [Display(Name = "Loại chiến dịch")]
        public CommunityCampaignType CampaignType { get; set; } = CommunityCampaignType.Survey;

        [Display(Name = "Cho phép chọn nhiều đáp án")]
        public bool AllowMultipleChoices { get; set; }

        [Display(Name = "Bắt đầu")]
        public DateTime StartsAt { get; set; } = DateTime.Now.AddHours(1);

        [Display(Name = "Kết thúc")]
        public DateTime EndsAt { get; set; } = DateTime.Now.AddDays(7);

        [Required(ErrorMessage = "Vui lòng nhập ít nhất 2 lựa chọn, mỗi lựa chọn trên một dòng")]
        [Display(Name = "Các lựa chọn")]
        public string OptionLines { get; set; } = string.Empty;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var request = new CommunityCampaignCreateRequestDto
        {
            Title = Input.Title,
            Description = Input.Description,
            QuestionText = Input.QuestionText,
            CampaignType = Input.CampaignType,
            AllowMultipleChoices = Input.AllowMultipleChoices,
            StartsAt = Input.StartsAt,
            EndsAt = Input.EndsAt,
            Options = Input.OptionLines
                .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList()
        };

        var result = await _communityEngagementService.CreateCampaignAsync(request, userId.Value);
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return Page();
        }

        TempData["SuccessMessage"] = "Đã tạo chiến dịch thành công.";
        return RedirectToPage("Results", new { id = result.CampaignId });
    }

    private int? GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var userId) ? userId : null;
    }
}
