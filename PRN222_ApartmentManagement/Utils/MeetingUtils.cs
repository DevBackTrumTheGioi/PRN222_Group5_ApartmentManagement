using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Utils;

public static class MeetingUtils
{
    public static string GetMeetingTime(Meeting meeting)
    {
        var start = meeting.StartTime.HasValue ? meeting.StartTime.Value.ToString(@"hh\:mm") : "--:--";
        var end = meeting.EndTime.HasValue ? meeting.EndTime.Value.ToString(@"hh\:mm") : "--:--";
        return $"{start} - {end}";
    }
}
