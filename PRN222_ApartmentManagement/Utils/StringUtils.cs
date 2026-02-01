using System.Text;
using System.Text.RegularExpressions;

namespace PRN222_ApartmentManagement.Utils;

/// <summary>
/// Tiện ích xử lý chuỗi
/// </summary>
public static class StringUtils
{
    /// <summary>
    /// Tạo mã ngẫu nhiên với tiền tố
    /// </summary>
    public static string GenerateCode(string prefix, int length = 8)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var code = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        return $"{prefix}{code}";
    }

    /// <summary>
    /// Tạo mã hóa đơn theo định dạng: INV-YYYYMM-XXXX
    /// </summary>
    public static string GenerateInvoiceNumber(int year, int month, int sequence)
    {
        return $"INV-{year:0000}{month:00}-{sequence:0000}";
    }

    /// <summary>
    /// Tạo mã hợp đồng theo định dạng: CT-YYYYMMDD-XXX
    /// </summary>
    public static string GenerateContractNumber(DateTime date, int sequence)
    {
        return $"CT-{date:yyyyMMdd}-{sequence:000}";
    }

    /// <summary>
    /// Tạo mã yêu cầu theo định dạng: REQ-YYYYMMDD-XXX
    /// </summary>
    public static string GenerateRequestNumber(DateTime date, int sequence)
    {
        return $"REQ-{date:yyyyMMdd}-{sequence:000}";
    }

    /// <summary>
    /// Tạo mã giao dịch thanh toán
    /// </summary>
    public static string GenerateTransactionCode()
    {
        return $"TXN-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper()}";
    }

    /// <summary>
    /// Chuẩn hóa số điện thoại (loại bỏ khoảng trắng, dấu gạch)
    /// </summary>
    public static string NormalizePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return string.Empty;
        return Regex.Replace(phoneNumber, @"[\s\-\(\)]", "");
    }

    /// <summary>
    /// Kiểm tra số điện thoại Việt Nam hợp lệ
    /// </summary>
    public static bool IsValidVietnamesePhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return false;
        var normalized = NormalizePhoneNumber(phoneNumber);
        return Regex.IsMatch(normalized, @"^(0|\+84)(3|5|7|8|9)\d{8}$");
    }

    /// <summary>
    /// Kiểm tra email hợp lệ
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    /// <summary>
    /// Kiểm tra CMND/CCCD hợp lệ (9 hoặc 12 số)
    /// </summary>
    public static bool IsValidIdentityCard(string identityCard)
    {
        if (string.IsNullOrWhiteSpace(identityCard)) return false;
        var normalized = Regex.Replace(identityCard, @"\s", "");
        return Regex.IsMatch(normalized, @"^\d{9}$|^\d{12}$");
    }

    /// <summary>
    /// Tạo slug từ chuỗi tiếng Việt
    /// </summary>
    public static string ToSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        text = text.ToLowerInvariant();
        text = RemoveVietnameseTones(text);
        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        text = Regex.Replace(text, @"\s+", "-");
        text = Regex.Replace(text, @"-+", "-");
        return text.Trim('-');
    }

    /// <summary>
    /// Loại bỏ dấu tiếng Việt
    /// </summary>
    public static string RemoveVietnameseTones(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;

        var arr1 = new[] { "á", "à", "ả", "ã", "ạ", "â", "ấ", "ầ", "ẩ", "ẫ", "ậ", "ă", "ắ", "ằ", "ẳ", "ẵ", "ặ" };
        var arr2 = new[] { "đ" };
        var arr3 = new[] { "é", "è", "ẻ", "ẽ", "ẹ", "ê", "ế", "ề", "ể", "ễ", "ệ" };
        var arr4 = new[] { "í", "ì", "ỉ", "ĩ", "ị" };
        var arr5 = new[] { "ó", "ò", "ỏ", "õ", "ọ", "ô", "ố", "ồ", "ổ", "ỗ", "ộ", "ơ", "ớ", "ờ", "ở", "ỡ", "ợ" };
        var arr6 = new[] { "ú", "ù", "ủ", "ũ", "ụ", "ư", "ứ", "ừ", "ử", "ữ", "ự" };
        var arr7 = new[] { "ý", "ỳ", "ỷ", "ỹ", "ỵ" };

        foreach (var c in arr1) text = text.Replace(c, "a");
        foreach (var c in arr2) text = text.Replace(c, "d");
        foreach (var c in arr3) text = text.Replace(c, "e");
        foreach (var c in arr4) text = text.Replace(c, "i");
        foreach (var c in arr5) text = text.Replace(c, "o");
        foreach (var c in arr6) text = text.Replace(c, "u");
        foreach (var c in arr7) text = text.Replace(c, "y");

        return text;
    }

    /// <summary>
    /// Cắt chuỗi và thêm "..."
    /// </summary>
    public static string Truncate(string text, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        if (text.Length <= maxLength) return text;
        return text.Substring(0, maxLength - suffix.Length) + suffix;
    }

    /// <summary>
    /// Che giấu email: abc***@gmail.com
    /// </summary>
    public static string MaskEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@")) return email;
        
        var parts = email.Split('@');
        var username = parts[0];
        var domain = parts[1];
        
        if (username.Length <= 3)
            return $"{username[0]}***@{domain}";
        
        return $"{username.Substring(0, 3)}***@{domain}";
    }

    /// <summary>
    /// Che giấu số điện thoại: 098***1234
    /// </summary>
    public static string MaskPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) return string.Empty;
        var normalized = NormalizePhoneNumber(phoneNumber);
        
        if (normalized.Length < 7) return phoneNumber;
        
        var prefix = normalized.Substring(0, 3);
        var suffix = normalized.Substring(normalized.Length - 4);
        return $"{prefix}***{suffix}";
    }

    /// <summary>
    /// Định dạng số tiền VND
    /// </summary>
    public static string FormatCurrency(decimal amount)
    {
        return $"{amount:N0} VNĐ";
    }

    /// <summary>
    /// Chuyển đổi số thành chữ (tiếng Việt)
    /// </summary>
    public static string NumberToWords(long number)
    {
        if (number == 0) return "Không";

        if (number < 0) return "Âm " + NumberToWords(Math.Abs(number));

        string[] units = { "", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        string[] scales = { "", "nghìn", "triệu", "tỷ" };

        if (number < 10) return units[number];

        var result = new StringBuilder();
        var groupIndex = 0;

        while (number > 0)
        {
            var group = (int)(number % 1000);
            if (group > 0)
            {
                var groupWords = ConvertGroupToWords(group, units);
                if (groupIndex > 0)
                    groupWords += " " + scales[groupIndex];
                
                if (result.Length > 0)
                    result.Insert(0, groupWords + " ");
                else
                    result.Insert(0, groupWords);
            }

            number /= 1000;
            groupIndex++;
        }

        return CapitalizeFirstLetter(result.ToString().Trim());
    }

    private static string ConvertGroupToWords(int number, string[] units)
    {
        var result = new StringBuilder();
        var hundred = number / 100;
        var ten = (number % 100) / 10;
        var unit = number % 10;

        if (hundred > 0)
        {
            result.Append(units[hundred] + " trăm");
        }

        if (ten > 1)
        {
            result.Append((result.Length > 0 ? " " : "") + units[ten] + " mươi");
        }
        else if (ten == 1)
        {
            result.Append((result.Length > 0 ? " " : "") + "mười");
        }

        if (unit > 0)
        {
            if (ten > 1 && unit == 1)
                result.Append(" mốt");
            else if (ten > 0 && unit == 5)
                result.Append(" lăm");
            else
                result.Append((result.Length > 0 ? " " : "") + units[unit]);
        }

        return result.ToString();
    }

    private static string CapitalizeFirstLetter(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return text;
        return char.ToUpper(text[0]) + text.Substring(1);
    }
}

