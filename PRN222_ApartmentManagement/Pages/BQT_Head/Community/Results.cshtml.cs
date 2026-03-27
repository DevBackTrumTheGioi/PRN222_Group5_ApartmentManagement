using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQT_Head.Community;

[Authorize(Roles = "BQT_Head")]
public class ResultsModel : PageModel
{
    private readonly ICommunityEngagementService _communityEngagementService;

    public ResultsModel(ICommunityEngagementService communityEngagementService)
    {
        _communityEngagementService = communityEngagementService;
    }

    public CommunityCampaignDetailsDto Campaign { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var campaign = await _communityEngagementService.GetCampaignDetailsAsync(id, null, includeResponses: true);
        if (campaign == null)
        {
            return NotFound();
        }

        Campaign = campaign;
        return Page();
    }
}
