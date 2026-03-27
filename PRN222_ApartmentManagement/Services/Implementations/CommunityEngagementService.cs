using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class CommunityEngagementService : ICommunityEngagementService
{
    private readonly ApartmentDbContext _context;
    private readonly INotificationService _notificationService;

    public CommunityEngagementService(ApartmentDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<IReadOnlyList<CommunityCampaignListItemDto>> GetManagementCampaignsAsync(string? searchTerm, CommunityCampaignType? typeFilter)
    {
        IQueryable<CommunityCampaign> query = _context.CommunityCampaigns
            .AsNoTracking()
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var keyword = searchTerm.Trim();
            query = query.Where(c => c.Title.Contains(keyword) || c.QuestionText.Contains(keyword));
        }

        if (typeFilter.HasValue)
        {
            query = query.Where(c => c.CampaignType == typeFilter.Value);
        }

        var campaigns = await query
            .Include(c => c.Creator)
            .Include(c => c.Responses)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return campaigns.Select(c => MapListItem(c, null)).ToList();
    }

    public async Task<IReadOnlyList<CommunityCampaignListItemDto>> GetResidentCampaignsAsync(int userId, string? searchTerm)
    {
        IQueryable<CommunityCampaign> query = _context.CommunityCampaigns
            .AsNoTracking()
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var keyword = searchTerm.Trim();
            query = query.Where(c => c.Title.Contains(keyword) || c.QuestionText.Contains(keyword));
        }

        var campaigns = await query
            .Include(c => c.Creator)
            .Include(c => c.Responses)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();

        return campaigns.Select(c => MapListItem(c, userId)).ToList();
    }

    public async Task<CommunityCampaignDetailsDto?> GetCampaignDetailsAsync(int campaignId, int? userId = null, bool includeResponses = false)
    {
        var campaign = await _context.CommunityCampaigns
            .AsNoTracking()
            .Where(c => c.CampaignId == campaignId && !c.IsDeleted)
            .Include(c => c.Creator)
            .Include(c => c.Options.OrderBy(o => o.DisplayOrder))
            .Include(c => c.Responses)
                .ThenInclude(r => r.SelectedOptions)
                    .ThenInclude(ro => ro.Option)
            .Include(c => c.Responses)
                .ThenInclude(r => r.Respondent)
                    .ThenInclude(u => u.Apartment)
            .FirstOrDefaultAsync();

        if (campaign == null)
        {
            return null;
        }

        var details = new CommunityCampaignDetailsDto
        {
            CampaignId = campaign.CampaignId,
            Title = campaign.Title,
            Description = campaign.Description,
            QuestionText = campaign.QuestionText,
            CampaignType = campaign.CampaignType,
            Status = GetEffectiveStatus(campaign),
            AllowMultipleChoices = campaign.AllowMultipleChoices,
            StartsAt = campaign.StartsAt,
            EndsAt = campaign.EndsAt,
            CreatedAt = campaign.CreatedAt,
            CreatorName = campaign.Creator.FullName,
            TotalResponses = campaign.Responses.Count,
            Options = campaign.Options
                .Select(option => new CommunityCampaignOptionDto
                {
                    OptionId = option.OptionId,
                    OptionText = option.OptionText,
                    DisplayOrder = option.DisplayOrder,
                    VoteCount = campaign.Responses.Count(r => r.SelectedOptions.Any(ro => ro.OptionId == option.OptionId)),
                    VotePercent = campaign.Responses.Count == 0
                        ? 0
                        : Math.Round(campaign.Responses.Count(r => r.SelectedOptions.Any(ro => ro.OptionId == option.OptionId)) * 100m / campaign.Responses.Count, 1)
                })
                .OrderBy(o => o.DisplayOrder)
                .ToList()
        };

        var existingResponse = userId.HasValue
            ? campaign.Responses.FirstOrDefault(r => r.UserId == userId.Value)
            : null;

        details.HasResponded = existingResponse != null;
        details.CanParticipate = CanParticipate(campaign, existingResponse != null);
        details.ExistingComment = existingResponse?.Comment;
        details.SelectedOptionIds = existingResponse?.SelectedOptions.Select(o => o.OptionId).ToList() ?? [];

        if (includeResponses)
        {
            details.RecentResponses = campaign.Responses
                .OrderByDescending(r => r.SubmittedAt)
                .Take(10)
                .Select(r => new CommunityCampaignResponseSummaryDto
                {
                    ResidentName = r.Respondent.FullName,
                    ApartmentNumber = r.Respondent.Apartment?.ApartmentNumber,
                    SubmittedAt = r.SubmittedAt,
                    SelectedOptionsLabel = string.Join(", ", r.SelectedOptions
                        .OrderBy(o => o.Option.DisplayOrder)
                        .Select(o => o.Option.OptionText)),
                    Comment = r.Comment
                })
                .ToList();
        }

        return details;
    }

    public async Task<(bool Success, string ErrorMessage, int CampaignId)> CreateCampaignAsync(CommunityCampaignCreateRequestDto request, int creatorUserId)
    {
        var sanitizedOptions = request.Options
            .Where(option => !string.IsNullOrWhiteSpace(option))
            .Select(option => option.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return (false, "Tiêu đề là bắt buộc.", 0);
        }

        if (string.IsNullOrWhiteSpace(request.QuestionText))
        {
            return (false, "Câu hỏi là bắt buộc.", 0);
        }

        if (request.EndsAt <= request.StartsAt)
        {
            return (false, "Thời gian kết thúc phải sau thời gian bắt đầu.", 0);
        }

        if (sanitizedOptions.Count < 2)
        {
            return (false, "Cần ít nhất 2 lựa chọn hợp lệ.", 0);
        }

        var campaign = new CommunityCampaign
        {
            Title = request.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            QuestionText = request.QuestionText.Trim(),
            CampaignType = request.CampaignType,
            Status = CommunityCampaignStatus.Published,
            AllowMultipleChoices = request.CampaignType == CommunityCampaignType.Survey && request.AllowMultipleChoices,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            CreatedBy = creatorUserId,
            CreatedAt = DateTime.Now
        };

        campaign.Options = sanitizedOptions
            .Select((option, index) => new CommunityCampaignOption
            {
                OptionText = option,
                DisplayOrder = index + 1
            })
            .ToList();

        _context.CommunityCampaigns.Add(campaign);
        await _context.SaveChangesAsync();

        var residentIds = await _context.Users
            .Where(u => u.Role == UserRole.Resident && u.IsActive && !u.IsDeleted)
            .Select(u => u.UserId)
            .ToListAsync();

        if (residentIds.Count > 0)
        {
            var (title, content) = NotificationUtils.CreateCommunityCampaignNotification(campaign.Title, campaign.CampaignType.ToString());
            await _notificationService.CreateBulkNotificationsAsync(
                residentIds,
                title,
                content,
                NotificationType.Community,
                ReferenceType.CommunityCampaign,
                campaign.CampaignId,
                NotificationPriority.Normal);
        }

        return (true, string.Empty, campaign.CampaignId);
    }

    public async Task<(bool Success, string ErrorMessage)> SubmitResponseAsync(int userId, CommunityParticipationRequestDto request)
    {
        var campaign = await _context.CommunityCampaigns
            .Include(c => c.Options)
            .Include(c => c.Responses)
            .FirstOrDefaultAsync(c => c.CampaignId == request.CampaignId && !c.IsDeleted);

        if (campaign == null)
        {
            return (false, "Khảo sát hoặc cuộc bỏ phiếu không tồn tại.");
        }

        if (!CanParticipate(campaign, campaign.Responses.Any(r => r.UserId == userId)))
        {
            return (false, "Chiến dịch này hiện không còn nhận phản hồi.");
        }

        var selectedOptionIds = request.SelectedOptionIds.Distinct().ToList();
        if (selectedOptionIds.Count == 0)
        {
            return (false, "Vui lòng chọn ít nhất một đáp án.");
        }

        if (!campaign.AllowMultipleChoices && selectedOptionIds.Count > 1)
        {
            return (false, "Chiến dịch này chỉ cho phép chọn một đáp án.");
        }

        var validOptionIds = campaign.Options.Select(o => o.OptionId).ToHashSet();
        if (selectedOptionIds.Any(optionId => !validOptionIds.Contains(optionId)))
        {
            return (false, "Có lựa chọn không hợp lệ.");
        }

        var response = new CommunityCampaignResponse
        {
            CampaignId = campaign.CampaignId,
            UserId = userId,
            Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
            SubmittedAt = DateTime.Now,
            SelectedOptions = selectedOptionIds.Select(optionId => new CommunityCampaignResponseOption
            {
                OptionId = optionId
            }).ToList()
        };

        _context.CommunityCampaignResponses.Add(response);
        await _context.SaveChangesAsync();

        return (true, string.Empty);
    }

    public async Task<(bool Success, string ErrorMessage)> CloseCampaignAsync(int campaignId, int actorUserId)
    {
        var campaign = await _context.CommunityCampaigns
            .FirstOrDefaultAsync(c => c.CampaignId == campaignId && !c.IsDeleted);

        if (campaign == null)
        {
            return (false, "Không tìm thấy chiến dịch.");
        }

        if (campaign.CreatedBy != actorUserId)
        {
            return (false, "Bạn không có quyền đóng chiến dịch này.");
        }

        campaign.Status = CommunityCampaignStatus.Closed;
        campaign.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return (true, string.Empty);
    }

    private static CommunityCampaignListItemDto MapListItem(CommunityCampaign campaign, int? userId)
    {
        var hasResponded = userId.HasValue && campaign.Responses.Any(r => r.UserId == userId.Value);
        return new CommunityCampaignListItemDto
        {
            CampaignId = campaign.CampaignId,
            Title = campaign.Title,
            QuestionText = campaign.QuestionText,
            CampaignType = campaign.CampaignType,
            Status = GetEffectiveStatus(campaign),
            AllowMultipleChoices = campaign.AllowMultipleChoices,
            StartsAt = campaign.StartsAt,
            EndsAt = campaign.EndsAt,
            CreatedAt = campaign.CreatedAt,
            CreatorName = campaign.Creator.FullName,
            TotalResponses = campaign.Responses.Count,
            HasResponded = hasResponded,
            CanParticipate = CanParticipate(campaign, hasResponded)
        };
    }

    private static CommunityCampaignStatus GetEffectiveStatus(CommunityCampaign campaign)
    {
        if (campaign.Status == CommunityCampaignStatus.Closed || campaign.EndsAt < DateTime.Now)
        {
            return CommunityCampaignStatus.Closed;
        }

        return CommunityCampaignStatus.Published;
    }

    private static bool CanParticipate(CommunityCampaign campaign, bool hasResponded)
    {
        var now = DateTime.Now;
        return !hasResponded
               && campaign.Status == CommunityCampaignStatus.Published
               && campaign.StartsAt <= now
               && campaign.EndsAt >= now;
    }
}
