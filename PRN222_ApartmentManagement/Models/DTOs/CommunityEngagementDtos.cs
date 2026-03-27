using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Models.DTOs;

public class CommunityCampaignListItemDto
{
    public int CampaignId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string QuestionText { get; set; } = string.Empty;
    public CommunityCampaignType CampaignType { get; set; }
    public CommunityCampaignStatus Status { get; set; }
    public bool AllowMultipleChoices { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public int TotalResponses { get; set; }
    public bool HasResponded { get; set; }
    public bool CanParticipate { get; set; }
}

public class CommunityCampaignOptionDto
{
    public int OptionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public int VoteCount { get; set; }
    public decimal VotePercent { get; set; }
}

public class CommunityCampaignDetailsDto
{
    public int CampaignId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public CommunityCampaignType CampaignType { get; set; }
    public CommunityCampaignStatus Status { get; set; }
    public bool AllowMultipleChoices { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public int TotalResponses { get; set; }
    public bool HasResponded { get; set; }
    public bool CanParticipate { get; set; }
    public string? ExistingComment { get; set; }
    public List<int> SelectedOptionIds { get; set; } = new();
    public List<CommunityCampaignOptionDto> Options { get; set; } = new();
    public List<CommunityCampaignResponseSummaryDto> RecentResponses { get; set; } = new();
}

public class CommunityCampaignResponseSummaryDto
{
    public string ResidentName { get; set; } = string.Empty;
    public string? ApartmentNumber { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string SelectedOptionsLabel { get; set; } = string.Empty;
    public string? Comment { get; set; }
}

public class CommunityCampaignCreateRequestDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public CommunityCampaignType CampaignType { get; set; }
    public bool AllowMultipleChoices { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public List<string> Options { get; set; } = new();
}

public class CommunityParticipationRequestDto
{
    public int CampaignId { get; set; }
    public List<int> SelectedOptionIds { get; set; } = new();
    public string? Comment { get; set; }
}
