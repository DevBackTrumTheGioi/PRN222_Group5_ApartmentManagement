namespace PRN222_ApartmentManagement.Models.DTOs;

public class FaceResidentSummaryDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? ApartmentNumber { get; set; }
    public string? BuildingBlock { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public bool IsFaceRegistered { get; set; }
    public DateTime? RegisteredAt { get; set; }
    public DateTime? LastAuthTime { get; set; }
    public int SuccessfulAuthCount { get; set; }
    public int FailedAuthCount { get; set; }
}

public class FaceAuthLogDto
{
    public int HistoryId { get; set; }
    public int ResidentId { get; set; }
    public string ResidentName { get; set; } = string.Empty;
    public string? ApartmentNumber { get; set; }
    public string? BuildingBlock { get; set; }
    public DateTime AuthTime { get; set; }
    public bool IsSuccess { get; set; }
    public double ConfidenceScore { get; set; }
    public string? IpAddress { get; set; }
    public string? DeviceInfo { get; set; }
}

public class FaceAuthDashboardDto
{
    public int TotalResidents { get; set; }
    public int RegisteredResidents { get; set; }
    public int UnregisteredResidents { get; set; }
    public decimal RegistrationRate { get; set; }
    public int RegisteredThisMonth { get; set; }
    public int AttemptsToday { get; set; }
    public int AttemptsInPeriod { get; set; }
    public int SuccessfulAttemptsInPeriod { get; set; }
    public int FailedAttemptsInPeriod { get; set; }
    public decimal SuccessRateInPeriod { get; set; }
    public int RecentDays { get; set; }
    public List<FaceAuthLogDto> RecentLogs { get; set; } = new();
}

public class AmenityFaceAccessResultDto
{
    public DateTime CheckedAt { get; set; } = DateTime.Now;
    public int AmenityId { get; set; }
    public string AmenityName { get; set; } = string.Empty;
    public bool AmenityRequiresBooking { get; set; }
    public bool IsMatchFound { get; set; }
    public bool AccessGranted { get; set; }
    public bool IsManualOverride { get; set; }
    public bool IsWithinOperatingHours { get; set; }
    public bool HasValidBooking { get; set; }
    public int? ResidentId { get; set; }
    public string? ResidentName { get; set; }
    public string? ApartmentNumber { get; set; }
    public string? BuildingBlock { get; set; }
    public double ConfidenceScore { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? BookingWindowLabel { get; set; }
    public string AccessModeLabel { get; set; } = "Quét khuôn mặt";
}
