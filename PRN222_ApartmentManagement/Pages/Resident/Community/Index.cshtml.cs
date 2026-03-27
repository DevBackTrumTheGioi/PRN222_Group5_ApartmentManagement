using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Resident.Community;

[Authorize(Roles = "Resident")]
public class IndexModel : PageModel
{
    private readonly ICommunityEngagementService _communityEngagementService;

    public IndexModel(ICommunityEngagementService communityEngagementService)
    {
        _communityEngagementService = communityEngagementService;
    }

    public IReadOnlyList<CommunityCampaignListItemDto> Campaigns { get; set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Campaigns = await _communityEngagementService.GetResidentCampaignsAsync(userId, SearchTerm);
    }
}
