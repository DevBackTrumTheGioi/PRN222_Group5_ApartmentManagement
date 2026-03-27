using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQT_Head.Community;

[Authorize(Roles = "BQT_Head")]
public class IndexModel : PageModel
{
    private readonly ICommunityEngagementService _communityEngagementService;

    public IndexModel(ICommunityEngagementService communityEngagementService)
    {
        _communityEngagementService = communityEngagementService;
    }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public CommunityCampaignType? TypeFilter { get; set; }

    public IReadOnlyList<CommunityCampaignListItemDto> Campaigns { get; set; } = [];

    public async Task OnGetAsync()
    {
        Campaigns = await _communityEngagementService.GetManagementCampaignsAsync(SearchTerm, TypeFilter);
    }

    public async Task<IActionResult> OnPostCloseAsync(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Forbid();
        }

        var result = await _communityEngagementService.CloseCampaignAsync(id, userId.Value);
        TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success
            ? "Đã đóng chiến dịch."
            : result.ErrorMessage;

        return RedirectToPage(new { SearchTerm, TypeFilter });
    }

    private int? GetCurrentUserId()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var userId) ? userId : null;
    }
}
