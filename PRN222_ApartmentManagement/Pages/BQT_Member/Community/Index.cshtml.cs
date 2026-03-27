using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQT_Member.Community;

[Authorize(Roles = "BQT_Member")]
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
}
