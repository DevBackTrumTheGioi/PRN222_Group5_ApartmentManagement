using System.Security.Cryptography;
using System.Text;

namespace PRN222_ApartmentManagement.Utils;

/// <summary>
/// Tiện ích bảo mật và mã hóa
/// </summary>
public static class SecurityUtils
{
    /// <summary>
    /// Hash mật khẩu sử dụng SHA256
    /// </summary>
    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Hash mật khẩu với salt
    /// </summary>
    public static string HashPasswordWithSalt(string password, string salt)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        var saltedPassword = password + salt;
        return HashPassword(saltedPassword);
    }

    /// <summary>
    /// Tạo salt ngẫu nhiên
    /// </summary>
    public static string GenerateSalt(int length = 32)
    {
        var randomBytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// So sánh mật khẩu với hash
    /// </summary>
    public static bool VerifyPassword(string password, string hash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == hash;
    }

    /// <summary>
    /// So sánh mật khẩu với hash và salt
    /// </summary>
    public static bool VerifyPasswordWithSalt(string password, string hash, string salt)
    {
        var hashedPassword = HashPasswordWithSalt(password, salt);
        return hashedPassword == hash;
    }

    /// <summary>
    /// Tạo token ngẫu nhiên
    /// </summary>
    public static string GenerateToken(int length = 32)
    {
        var randomBytes = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "")
            .Substring(0, length);
    }

    /// <summary>
    /// Tạo OTP (One-Time Password) 6 số
    /// </summary>
    public static string GenerateOtp(int length = 6)
    {
        var randomNumber = new byte[4];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        
        var value = BitConverter.ToInt32(randomNumber, 0);
        value = Math.Abs(value);
        
        var otp = (value % (int)Math.Pow(10, length)).ToString();
        return otp.PadLeft(length, '0');
    }

    /// <summary>
    /// Mã hóa chuỗi đơn giản (Base64)
    /// </summary>
    public static string Encode(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        var bytes = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Giải mã chuỗi đơn giản (Base64)
    /// </summary>
    public static string Decode(string encodedText)
    {
        if (string.IsNullOrEmpty(encodedText)) return string.Empty;
        try
        {
            var bytes = Convert.FromBase64String(encodedText);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Tạo QR Code data cho check-in khách
    /// </summary>
    public static string GenerateVisitorQrCode(int visitorId, DateTime visitDate)
    {
        var data = $"VISITOR:{visitorId}:{visitDate:yyyyMMdd}:{GenerateToken(8)}";
        return Encode(data);
    }

    /// <summary>
    /// Kiểm tra độ mạnh mật khẩu
    /// </summary>
    public static PasswordStrength CheckPasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return PasswordStrength.VeryWeak;

        var score = 0;

        // Độ dài
        if (password.Length >= 8) score++;
        if (password.Length >= 12) score++;

        // Có chữ thường
        if (password.Any(char.IsLower)) score++;

        // Có chữ hoa
        if (password.Any(char.IsUpper)) score++;

        // Có số
        if (password.Any(char.IsDigit)) score++;

        // Có ký tự đặc biệt
        if (password.Any(c => !char.IsLetterOrDigit(c))) score++;

        return score switch
        {
            <= 2 => PasswordStrength.VeryWeak,
            3 => PasswordStrength.Weak,
            4 => PasswordStrength.Medium,
            5 => PasswordStrength.Strong,
            _ => PasswordStrength.VeryStrong
        };
    }

    /// <summary>
    /// Tạo mật khẩu ngẫu nhiên
    /// </summary>
    public static string GenerateRandomPassword(int length = 12, bool includeSpecialChars = true)
    {
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string special = "!@#$%^&*";

        var chars = lowercase + uppercase + digits;
        if (includeSpecialChars)
            chars += special;

        var random = new Random();
        var password = new StringBuilder();

        // Đảm bảo có ít nhất 1 ký tự mỗi loại
        password.Append(lowercase[random.Next(lowercase.Length)]);
        password.Append(uppercase[random.Next(uppercase.Length)]);
        password.Append(digits[random.Next(digits.Length)]);

        if (includeSpecialChars)
            password.Append(special[random.Next(special.Length)]);

        // Điền đầy các ký tự còn lại
        for (int i = password.Length; i < length; i++)
        {
            password.Append(chars[random.Next(chars.Length)]);
        }

        // Xáo trộn
        return new string(password.ToString().OrderBy(_ => random.Next()).ToArray());
    }

    /// <summary>
    /// Tạo checksum MD5 cho file
    /// </summary>
    public static string ComputeFileChecksum(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found", filePath);

        using var md5 = MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// Tạo checksum MD5 cho stream
    /// </summary>
    public static string ComputeStreamChecksum(Stream stream)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}

/// <summary>
/// Độ mạnh mật khẩu
/// </summary>
public enum PasswordStrength
{
    VeryWeak = 0,
    Weak = 1,
    Medium = 2,
    Strong = 3,
    VeryStrong = 4
}

