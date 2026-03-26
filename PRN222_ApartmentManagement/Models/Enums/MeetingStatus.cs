using System.ComponentModel.DataAnnotations;

namespace PRN222_ApartmentManagement.Models.Enums;

public enum MeetingStatus
{
    [Display(Name = "Đã lên lịch")]
    Scheduled,

    [Display(Name = "Đang diễn ra")]
    InProgress,

    [Display(Name = "Đã hoàn thành")]
    Completed,

    [Display(Name = "Đã hủy")]
    Cancelled
}
