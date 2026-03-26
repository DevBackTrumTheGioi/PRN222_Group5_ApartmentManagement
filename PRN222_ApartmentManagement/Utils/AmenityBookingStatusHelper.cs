namespace PRN222_ApartmentManagement.Utils;

public static class AmenityBookingStatusHelper
{
    public const string Confirmed = "Confirmed";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";

    public static bool BlocksSlot(string? status)
    {
        return !string.Equals(status, Cancelled, StringComparison.OrdinalIgnoreCase);
    }

    public static string GetDisplayName(string? status)
    {
        return status switch
        {
            Confirmed => "Đã xác nhận",
            Completed => "Hoàn tất",
            Cancelled => "Đã hủy",
            _ => "Không xác định"
        };
    }

    public static string GetBadgeCss(string? status)
    {
        return status switch
        {
            Confirmed => "bg-blue-50 text-blue-700 border-blue-200",
            Completed => "bg-slate-100 text-slate-700 border-slate-200",
            Cancelled => "bg-red-50 text-red-700 border-red-200",
            _ => "bg-slate-50 text-slate-600 border-slate-200"
        };
    }

    public static string GetIcon(string? status)
    {
        return status switch
        {
            Confirmed => "event_available",
            Completed => "task_alt",
            Cancelled => "cancel",
            _ => "help"
        };
    }
}
