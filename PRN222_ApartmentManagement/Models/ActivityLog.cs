using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN222_ApartmentManagement.Models;

/// <summary>
/// Ghi log tất cả các hoạt động CRUD trong hệ thống
/// </summary>
[Table("ActivityLogs")]
public class ActivityLog
{
    [Key]
    public int LogId { get; set; }

    /// <summary>
    /// User thực hiện hành động
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Tên đầy đủ của user (để lưu lịch sử khi user bị xóa)
    /// </summary>
    [MaxLength(200)]
    public string? UserName { get; set; }

    /// <summary>
    /// Role của user tại thời điểm thực hiện
    /// </summary>
    [MaxLength(50)]
    public string? UserRole { get; set; }

    /// <summary>
    /// Loại hành động: Create, Read, Update, Delete, Login, Logout
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Tên bảng/Entity bị tác động: Apartments, Residents, Invoices, etc.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// ID của record bị tác động
    /// </summary>
    [MaxLength(50)]
    public string? EntityId { get; set; }

    /// <summary>
    /// Mô tả chi tiết hành động
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Dữ liệu cũ (JSON) - dùng cho Update/Delete
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? OldValues { get; set; }

    /// <summary>
    /// Dữ liệu mới (JSON) - dùng cho Create/Update
    /// </summary>
    [Column(TypeName = "nvarchar(max)")]
    public string? NewValues { get; set; }

    /// <summary>
    /// IP Address của client
    /// </summary>
    [MaxLength(50)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// User Agent (Browser info)
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    /// <summary>
    /// Thời gian thực hiện
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.Now;

    /// <summary>
    /// Có thành công không (để log cả lỗi)
    /// </summary>
    public bool IsSuccess { get; set; } = true;

    /// <summary>
    /// Error message nếu có
    /// </summary>
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }

    // Navigation property
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}

