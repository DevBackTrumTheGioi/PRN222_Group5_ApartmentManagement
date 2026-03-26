namespace PRN222_ApartmentManagement.Models.DTOs;

public class AmenityAvailabilitySlotDto
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
}
