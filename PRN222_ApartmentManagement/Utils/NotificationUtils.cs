namespace PRN222_ApartmentManagement.Utils;

/// <summary>
/// Tiện ích xử lý thông báo và notification
/// </summary>
public static class NotificationUtils
{
    /// <summary>
    /// Tạo nội dung thông báo hóa đơn mới
    /// </summary>
    public static (string Title, string Content) CreateInvoiceNotification(
        string invoiceNumber, 
        decimal amount, 
        DateTime dueDate)
    {
        var title = "Hóa đơn mới";
        var content = $"Bạn có hóa đơn mới {invoiceNumber} với số tiền {StringUtils.FormatCurrency(amount)}. " +
                     $"Hạn thanh toán: {dueDate:dd/MM/yyyy}";
        return (title, content);
    }

    /// <summary>
    /// Tạo nội dung thông báo thanh toán thành công
    /// </summary>
    public static (string Title, string Content) CreatePaymentSuccessNotification(
        string invoiceNumber, 
        decimal amount)
    {
        var title = "Thanh toán thành công";
        var content = $"Thanh toán hóa đơn {invoiceNumber} thành công với số tiền {StringUtils.FormatCurrency(amount)}. " +
                     "Cảm ơn bạn đã thanh toán đúng hạn!";
        return (title, content);
    }

    /// <summary>
    /// Tạo nội dung thông báo hóa đơn quá hạn
    /// </summary>
    public static (string Title, string Content) CreateOverdueInvoiceNotification(
        string invoiceNumber, 
        decimal amount, 
        int daysOverdue)
    {
        var title = "Hóa đơn quá hạn";
        var content = $"Hóa đơn {invoiceNumber} đã quá hạn {daysOverdue} ngày với số tiền {StringUtils.FormatCurrency(amount)}. " +
                     "Vui lòng thanh toán sớm để tránh bị phạt chậm!";
        return (title, content);
    }

    /// <summary>
    /// Tạo nội dung thông báo yêu cầu mới
    /// </summary>
    public static (string Title, string Content) CreateRequestNotification(
        string requestNumber, 
        string requestType, 
        string title)
    {
        var notifTitle = "Yêu cầu mới";
        var content = $"Bạn có yêu cầu mới {requestNumber} - {requestType}: {title}";
        return (notifTitle, content);
    }

    /// <summary>
    /// Tạo nội dung thông báo yêu cầu được xử lý
    /// </summary>
    public static (string Title, string Content) CreateRequestProcessedNotification(
        string requestNumber, 
        string status)
    {
        var title = "Yêu cầu được cập nhật";
        var content = $"Yêu cầu {requestNumber} đã được cập nhật trạng thái: {status}";
        return (title, content);
    }

    /// <summary>
    /// Tạo nội dung thông báo khách đến thăm
    /// </summary>
    public static (string Title, string Content) CreateVisitorNotification(
        string visitorName, 
        DateTime visitDate)
    {
        var title = "Thông báo khách đến thăm";
        var content = $"Bạn có khách đến thăm: {visitorName} vào ngày {visitDate:dd/MM/yyyy HH:mm}";
        return (title, content);
    }

    /// <summary>
    /// Tạo nội dung thông báo bưu phẩm
    /// </summary>
    public static (string Title, string Content) CreateParcelNotification(
        string? trackingNumber, 
        string? sender)
    {
        var title = "Thông báo nhận bưu phẩm";
        var content = $"Bạn có bưu phẩm mới";
        if (!string.IsNullOrEmpty(trackingNumber))
            content += $" (Mã: {trackingNumber})";
        if (!string.IsNullOrEmpty(sender))
            content += $" từ {sender}";
        content += ". Vui lòng xuống quầy lễ tân để nhận.";
        return (title, content);
    }

    /// <summary>
    /// Tạo nội dung thông báo thông báo chung
    /// </summary>
    public static (string Title, string Content) CreateAnnouncementNotification(
        string announcementTitle, 
        string priority)
    {
        var title = priority == "Urgent" ? "⚠️ Thông báo khẩn cấp" : "Thông báo chung";
        var content = $"Bạn có thông báo mới: {announcementTitle}";
        return (title, content);
    }

    /// <summary>
    /// Tạo nội dung thông báo đặt tiện ích
    /// </summary>
    public static (string Title, string Content) CreateAmenityBookingNotification(
        string amenityName, 
        DateTime bookingDate, 
        string startTime, 
        string endTime,
        string status)
    {
        var title = status == "Confirmed" ? "Đặt tiện ích thành công" : "Cập nhật đặt tiện ích";
        var content = $"Đặt {amenityName} vào {bookingDate:dd/MM/yyyy} từ {startTime} đến {endTime}. " +
                     $"Trạng thái: {status}";
        return (title, content);
    }

    /// <summary>
    /// Tạo nội dung thông báo hợp đồng
    /// </summary>
    public static (string Title, string Content) CreateContractNotification(
        string contractNumber, 
        string action, 
        DateTime? startDate = null)
    {
        var title = action switch
        {
            "Created" => "Hợp đồng mới",
            "Signed" => "Hợp đồng đã ký",
            "Expiring" => "Hợp đồng sắp hết hạn",
            "Expired" => "Hợp đồng đã hết hạn",
            _ => "Cập nhật hợp đồng"
        };

        var content = action switch
        {
            "Created" => $"Hợp đồng {contractNumber} đã được tạo. Vui lòng kiểm tra và ký.",
            "Signed" => $"Hợp đồng {contractNumber} đã được ký thành công.",
            "Expiring" => $"Hợp đồng {contractNumber} sắp hết hạn. Vui lòng liên hệ để gia hạn.",
            "Expired" => $"Hợp đồng {contractNumber} đã hết hạn.",
            _ => $"Hợp đồng {contractNumber} đã được cập nhật."
        };

        if (startDate.HasValue && action == "Created")
        {
            content += $" Ngày hiệu lực: {startDate.Value:dd/MM/yyyy}";
        }

        return (title, content);
    }

    /// <summary>
    /// Tạo nội dung tin nhắn hệ thống
    /// </summary>
    public static string CreateSystemMessage(string action, string details)
    {
        return $"[Hệ thống] {action}: {details}";
    }

    /// <summary>
    /// Lấy icon cho loại thông báo
    /// </summary>
    public static string GetNotificationIcon(string notificationType)
    {
        return notificationType switch
        {
            "Invoice" => "💰",
            "Request" => "📝",
            "Announcement" => "📢",
            "Message" => "💬",
            "Visitor" => "👤",
            "Parcel" => "📦",
            "Contract" => "📄",
            "Amenity" => "🏊",
            "System" => "⚙️",
            _ => "🔔"
        };
    }

    /// <summary>
    /// Lấy màu cho mức độ ưu tiên
    /// </summary>
    public static string GetPriorityColor(int priority)
    {
        return priority switch
        {
            3 => "danger",  // Urgent
            2 => "warning", // Important
            _ => "info"     // Normal
        };
    }

    /// <summary>
    /// Lấy badge class cho trạng thái
    /// </summary>
    public static string GetStatusBadgeClass(string status)
    {
        return status?.ToLower() switch
        {
            "pending" or "new" => "badge-warning",
            "inprogress" or "confirmed" => "badge-info",
            "completed" or "paid" or "resolved" or "active" => "badge-success",
            "cancelled" or "rejected" or "expired" => "badge-danger",
            "overdue" => "badge-danger",
            _ => "badge-secondary"
        };
    }

    /// <summary>
    /// Lấy URL điều hướng cho thông báo
    /// </summary>
    public static string GetNotificationRedirectUrl(string notificationType, string referenceType, int? referenceId, string? recipientRole = null)
    {
        if (!referenceId.HasValue || referenceId.Value <= 0)
        {
            return "/Notifications";
        }

        return referenceType switch
        {
            "Request" => GetRequestRedirectUrl(recipientRole, referenceId.Value),
            "Announcement" => $"/Announcements/Details/{referenceId.Value}",
            "Invoice" => $"/Invoices/Index?highlight={referenceId.Value}",
            "Contract" => $"/Contracts/Details/{referenceId.Value}",
            "Amenity" => $"/Amenities/Bookings/{referenceId.Value}",
            _ => "/Notifications"
        };
    }

    private static string GetRequestRedirectUrl(string? recipientRole, int requestId)
    {
        if (string.Equals(recipientRole, "BQT_Head", StringComparison.OrdinalIgnoreCase))
        {
            return $"/BQT_Head/Complaints/Response/{requestId}";
        }

        if (string.Equals(recipientRole, "BQL_Staff", StringComparison.OrdinalIgnoreCase))
        {
            return $"/BQL_Staff/Requests/Details/{requestId}";
        }

        if (string.Equals(recipientRole, "BQL_Manager", StringComparison.OrdinalIgnoreCase))
        {
            return $"/BQL_Manager/Requests/Details/{requestId}";
        }

        if (string.Equals(recipientRole, "Resident", StringComparison.OrdinalIgnoreCase))
        {
            return $"/Resident/Requests/Details/{requestId}";
        }

        return $"/Notifications?highlightRequestId={requestId}";
    }
}

/// <summary>
/// Tiện ích xử lý Email
/// </summary>
public static class EmailUtils
{
    /// <summary>
    /// Tạo subject email cho hóa đơn
    /// </summary>
    public static string CreateInvoiceEmailSubject(string invoiceNumber, string apartmentNumber)
    {
        return $"[Apartment Management] Hóa đơn {invoiceNumber} - Căn hộ {apartmentNumber}";
    }

    /// <summary>
    /// Tạo nội dung email hóa đơn
    /// </summary>
    public static string CreateInvoiceEmailBody(
        string residentName,
        string invoiceNumber,
        string apartmentNumber,
        decimal totalAmount,
        DateTime dueDate,
        string invoiceDetailsHtml)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
        .amount {{ font-size: 24px; font-weight: bold; color: #4CAF50; }}
        .warning {{ color: #f44336; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>HÓA ĐƠN THANH TOÁN</h2>
        </div>
        <div class='content'>
            <p>Kính gửi: <strong>{residentName}</strong></p>
            <p>Chúng tôi xin gửi đến bạn hóa đơn thanh toán cho căn hộ <strong>{apartmentNumber}</strong></p>
            
            <h3>Thông tin hóa đơn:</h3>
            <ul>
                <li>Mã hóa đơn: <strong>{invoiceNumber}</strong></li>
                <li>Tổng tiền: <span class='amount'>{StringUtils.FormatCurrency(totalAmount)}</span></li>
                <li>Hạn thanh toán: <strong>{dueDate:dd/MM/yyyy}</strong></li>
            </ul>

            <h3>Chi tiết:</h3>
            {invoiceDetailsHtml}

            <p class='warning'>Lưu ý: Vui lòng thanh toán trước ngày {dueDate:dd/MM/yyyy} để tránh phí phạt chậm.</p>
        </div>
        <div class='footer'>
            <p>Email này được gửi tự động từ hệ thống quản lý chung cư.</p>
            <p>Nếu có thắc mắc, vui lòng liên hệ ban quản lý.</p>
        </div>
    </div>
</body>
</html>";
    }

    /// <summary>
    /// Tạo subject email cho yêu cầu
    /// </summary>
    public static string CreateRequestEmailSubject(string requestNumber, string requestType)
    {
        return $"[Apartment Management] Yêu cầu {requestType} - {requestNumber}";
    }

    /// <summary>
    /// Tạo nội dung email thông báo chung
    /// </summary>
    public static string CreateAnnouncementEmailBody(string title, string content, string author)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background-color: #f9f9f9; }}
        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>THÔNG BÁO CHUNG CƯ</h2>
        </div>
        <div class='content'>
            <h3>{title}</h3>
            <div>{content}</div>
            <p style='margin-top: 20px;'><em>Người đăng: {author}</em></p>
        </div>
        <div class='footer'>
            <p>Email này được gửi tự động từ hệ thống quản lý chung cư.</p>
        </div>
    </div>
</body>
</html>";
    }
}

