namespace PRN222_ApartmentManagement.Utils;

/// <summary>
/// Tiện ích xử lý ngày tháng
/// </summary>
public static class DateTimeUtils
{
    /// <summary>
    /// Lấy ngày đầu tháng
    /// </summary>
    public static DateTime GetFirstDayOfMonth(int year, int month)
    {
        return new DateTime(year, month, 1);
    }

    /// <summary>
    /// Lấy ngày cuối tháng
    /// </summary>
    public static DateTime GetLastDayOfMonth(int year, int month)
    {
        return new DateTime(year, month, DateTime.DaysInMonth(year, month));
    }

    /// <summary>
    /// Lấy ngày đầu tháng hiện tại
    /// </summary>
    public static DateTime GetFirstDayOfCurrentMonth()
    {
        var now = DateTime.Now;
        return GetFirstDayOfMonth(now.Year, now.Month);
    }

    /// <summary>
    /// Lấy ngày cuối tháng hiện tại
    /// </summary>
    public static DateTime GetLastDayOfCurrentMonth()
    {
        var now = DateTime.Now;
        return GetLastDayOfMonth(now.Year, now.Month);
    }

    /// <summary>
    /// Kiểm tra ngày có phải cuối tuần không
    /// </summary>
    public static bool IsWeekend(DateTime date)
    {
        return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
    }

    /// <summary>
    /// Tính số ngày giữa 2 ngày
    /// </summary>
    public static int DaysBetween(DateTime startDate, DateTime endDate)
    {
        return (endDate.Date - startDate.Date).Days;
    }

    /// <summary>
    /// Tính số tháng giữa 2 ngày
    /// </summary>
    public static int MonthsBetween(DateTime startDate, DateTime endDate)
    {
        return ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
    }

    /// <summary>
    /// Tính số năm giữa 2 ngày
    /// </summary>
    public static int YearsBetween(DateTime startDate, DateTime endDate)
    {
        var years = endDate.Year - startDate.Year;
        if (endDate.Month < startDate.Month || 
            (endDate.Month == startDate.Month && endDate.Day < startDate.Day))
        {
            years--;
        }
        return years;
    }

    /// <summary>
    /// Tính tuổi
    /// </summary>
    public static int CalculateAge(DateTime birthDate)
    {
        return YearsBetween(birthDate, DateTime.Now);
    }

    /// <summary>
    /// Lấy tên tháng tiếng Việt
    /// </summary>
    public static string GetVietnameseMonthName(int month)
    {
        return $"Tháng {month}";
    }

    /// <summary>
    /// Lấy tên ngày trong tuần tiếng Việt
    /// </summary>
    public static string GetVietnameseDayOfWeek(DayOfWeek dayOfWeek)
    {
        return dayOfWeek switch
        {
            DayOfWeek.Monday => "Thứ Hai",
            DayOfWeek.Tuesday => "Thứ Ba",
            DayOfWeek.Wednesday => "Thứ Tư",
            DayOfWeek.Thursday => "Thứ Năm",
            DayOfWeek.Friday => "Thứ Sáu",
            DayOfWeek.Saturday => "Thứ Bảy",
            DayOfWeek.Sunday => "Chủ Nhật",
            _ => ""
        };
    }

    /// <summary>
    /// Định dạng ngày tháng tiếng Việt: "Thứ Hai, 01/02/2026"
    /// </summary>
    public static string FormatVietnameseDate(DateTime date)
    {
        var dayOfWeek = GetVietnameseDayOfWeek(date.DayOfWeek);
        return $"{dayOfWeek}, {date:dd/MM/yyyy}";
    }

    /// <summary>
    /// Định dạng ngày giờ tiếng Việt: "Thứ Hai, 01/02/2026 14:30"
    /// </summary>
    public static string FormatVietnameseDateTime(DateTime dateTime)
    {
        var dayOfWeek = GetVietnameseDayOfWeek(dateTime.DayOfWeek);
        return $"{dayOfWeek}, {dateTime:dd/MM/yyyy HH:mm}";
    }

    /// <summary>
    /// Định dạng thời gian tương đối: "2 giờ trước", "3 ngày trước"
    /// </summary>
    public static string FormatRelativeTime(DateTime dateTime)
    {
        var timeSpan = DateTime.Now - dateTime;

        if (timeSpan.TotalSeconds < 60)
            return "Vừa xong";

        if (timeSpan.TotalMinutes < 60)
            return $"{(int)timeSpan.TotalMinutes} phút trước";

        if (timeSpan.TotalHours < 24)
            return $"{(int)timeSpan.TotalHours} giờ trước";

        if (timeSpan.TotalDays < 7)
            return $"{(int)timeSpan.TotalDays} ngày trước";

        if (timeSpan.TotalDays < 30)
            return $"{(int)(timeSpan.TotalDays / 7)} tuần trước";

        if (timeSpan.TotalDays < 365)
            return $"{(int)(timeSpan.TotalDays / 30)} tháng trước";

        return $"{(int)(timeSpan.TotalDays / 365)} năm trước";
    }

    /// <summary>
    /// Kiểm tra ngày có quá hạn không
    /// </summary>
    public static bool IsOverdue(DateTime dueDate)
    {
        return DateTime.Now.Date > dueDate.Date;
    }

    /// <summary>
    /// Kiểm tra ngày có sắp đến hạn không (trong vòng X ngày)
    /// </summary>
    public static bool IsUpcoming(DateTime dueDate, int withinDays = 7)
    {
        var daysUntilDue = (dueDate.Date - DateTime.Now.Date).Days;
        return daysUntilDue >= 0 && daysUntilDue <= withinDays;
    }

    /// <summary>
    /// Lấy danh sách ngày trong tháng
    /// </summary>
    public static List<DateTime> GetDaysInMonth(int year, int month)
    {
        var days = new List<DateTime>();
        var daysInMonth = DateTime.DaysInMonth(year, month);

        for (int day = 1; day <= daysInMonth; day++)
        {
            days.Add(new DateTime(year, month, day));
        }

        return days;
    }

    /// <summary>
    /// Lấy danh sách ngày làm việc trong tháng (trừ cuối tuần)
    /// </summary>
    public static List<DateTime> GetWorkingDaysInMonth(int year, int month)
    {
        return GetDaysInMonth(year, month)
            .Where(d => !IsWeekend(d))
            .ToList();
    }

    /// <summary>
    /// Thêm tháng vào ngày (xử lý ngày cuối tháng)
    /// </summary>
    public static DateTime AddMonthsSafe(DateTime date, int months)
    {
        var targetMonth = date.AddMonths(months);
        var daysInTargetMonth = DateTime.DaysInMonth(targetMonth.Year, targetMonth.Month);
        
        if (date.Day > daysInTargetMonth)
            return new DateTime(targetMonth.Year, targetMonth.Month, daysInTargetMonth);
        
        return targetMonth;
    }

    /// <summary>
    /// Tạo khoảng thời gian (range)
    /// </summary>
    public static List<DateTime> GetDateRange(DateTime startDate, DateTime endDate)
    {
        var dates = new List<DateTime>();
        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
        {
            dates.Add(date);
        }
        return dates;
    }

    /// <summary>
    /// Kiểm tra 2 khoảng thời gian có trùng nhau không
    /// </summary>
    public static bool IsOverlapping(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
    {
        return start1 <= end2 && start2 <= end1;
    }

    /// <summary>
    /// Làm tròn xuống đầu giờ
    /// </summary>
    public static DateTime RoundDownToHour(DateTime dateTime)
    {
        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
    }

    /// <summary>
    /// Làm tròn lên đầu giờ tiếp theo
    /// </summary>
    public static DateTime RoundUpToHour(DateTime dateTime)
    {
        var rounded = RoundDownToHour(dateTime);
        if (rounded != dateTime)
            rounded = rounded.AddHours(1);
        return rounded;
    }
}

