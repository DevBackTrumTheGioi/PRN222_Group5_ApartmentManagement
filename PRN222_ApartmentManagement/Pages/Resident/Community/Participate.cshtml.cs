using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Community;

[Authorize(Roles = "Resident")]
public class ParticipateModel : PageModel
{
    private readonly ICommunityEngagementService _communityEngagementService;

    public ParticipateModel(ICommunityEngagementService communityEngagementService)
    {
        _communityEngagementService = communityEngagementService;
    }

    public CommunityCampaignDetailsDto Campaign { get; set; } = new();

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Display(Name = "Ghi chú thêm")]
        public string? Comment { get; set; }
        public List<int> SelectedOptionIds { get; set; } = new();
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        return await LoadCampaignAsync(id);
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _communityEngagementService.SubmitResponseAsync(userId, new CommunityParticipationRequestDto
        {
            CampaignId = id,
            SelectedOptionIds = Input.SelectedOptionIds,
            Comment = Input.Comment
        });

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.ErrorMessage);
            return await LoadCampaignAsync(id);
        }

        TempData["SuccessMessage"] = "Đã gửi phản hồi thành công.";
        return RedirectToPage("Index");
    }

    private async Task<IActionResult> LoadCampaignAsync(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var campaign = await _communityEngagementService.GetCampaignDetailsAsync(id, userId);
        if (campaign == null)
        {
            return NotFound();
        }

        Campaign = campaign;
        if (Campaign.HasResponded)
        {
            Input.SelectedOptionIds = Campaign.SelectedOptionIds.ToList();
            Input.Comment = Campaign.ExistingComment;
        }
        return Page();
    }
}
