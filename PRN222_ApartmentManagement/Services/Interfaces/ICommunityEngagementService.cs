using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface ICommunityEngagementService
{
    Task<IReadOnlyList<CommunityCampaignListItemDto>> GetManagementCampaignsAsync(string? searchTerm, CommunityCampaignType? typeFilter);
    Task<IReadOnlyList<CommunityCampaignListItemDto>> GetResidentCampaignsAsync(int userId, string? searchTerm);
    Task<CommunityCampaignDetailsDto?> GetCampaignDetailsAsync(int campaignId, int? userId = null, bool includeResponses = false);
    Task<(bool Success, string ErrorMessage, int CampaignId)> CreateCampaignAsync(CommunityCampaignCreateRequestDto request, int creatorUserId);
    Task<(bool Success, string ErrorMessage)> SubmitResponseAsync(int userId, CommunityParticipationRequestDto request);
    Task<(bool Success, string ErrorMessage)> CloseCampaignAsync(int campaignId, int actorUserId);
}
