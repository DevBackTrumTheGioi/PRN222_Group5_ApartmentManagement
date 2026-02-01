namespace PRN222_ApartmentManagement.Utils;

/// <summary>
/// Tiện ích validate dữ liệu
/// </summary>
public static class ValidationUtils
{
    /// <summary>
    /// Kiểm tra chuỗi có null hoặc rỗng không
    /// </summary>
    public static bool IsNullOrEmpty(string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    /// <summary>
    /// Kiểm tra email hợp lệ
    /// </summary>
    public static bool IsValidEmail(string email)
    {
        return StringUtils.IsValidEmail(email);
    }

    /// <summary>
    /// Kiểm tra số điện thoại Việt Nam hợp lệ
    /// </summary>
    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        return StringUtils.IsValidVietnamesePhoneNumber(phoneNumber);
    }

    /// <summary>
    /// Kiểm tra CMND/CCCD hợp lệ
    /// </summary>
    public static bool IsValidIdentityCard(string identityCard)
    {
        return StringUtils.IsValidIdentityCard(identityCard);
    }

    /// <summary>
    /// Kiểm tra số có trong khoảng không
    /// </summary>
    public static bool IsInRange(decimal value, decimal min, decimal max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Kiểm tra số có dương không
    /// </summary>
    public static bool IsPositive(decimal value)
    {
        return value > 0;
    }

    /// <summary>
    /// Kiểm tra số có không âm không
    /// </summary>
    public static bool IsNonNegative(decimal value)
    {
        return value >= 0;
    }

    /// <summary>
    /// Kiểm tra ngày có hợp lệ không (không trong tương lai)
    /// </summary>
    public static bool IsValidPastDate(DateTime date)
    {
        return date.Date <= DateTime.Now.Date;
    }

    /// <summary>
    /// Kiểm tra ngày có hợp lệ không (không trong quá khứ)
    /// </summary>
    public static bool IsValidFutureDate(DateTime date)
    {
        return date.Date >= DateTime.Now.Date;
    }

    /// <summary>
    /// Kiểm tra ngày có trong khoảng không
    /// </summary>
    public static bool IsDateInRange(DateTime date, DateTime startDate, DateTime endDate)
    {
        return date >= startDate && date <= endDate;
    }

    /// <summary>
    /// Kiểm tra tuổi có hợp lệ không
    /// </summary>
    public static bool IsValidAge(DateTime birthDate, int minAge = 0, int maxAge = 150)
    {
        var age = DateTimeUtils.CalculateAge(birthDate);
        return age >= minAge && age <= maxAge;
    }

    /// <summary>
    /// Kiểm tra độ dài chuỗi
    /// </summary>
    public static bool IsValidLength(string value, int minLength, int maxLength)
    {
        if (string.IsNullOrEmpty(value))
            return minLength == 0;
        
        return value.Length >= minLength && value.Length <= maxLength;
    }

    /// <summary>
    /// Kiểm tra mật khẩu có đủ mạnh không
    /// </summary>
    public static bool IsStrongPassword(string password, PasswordStrength minimumStrength = PasswordStrength.Medium)
    {
        var strength = SecurityUtils.CheckPasswordStrength(password);
        return strength >= minimumStrength;
    }

    /// <summary>
    /// Kiểm tra URL hợp lệ
    /// </summary>
    public static bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    /// <summary>
    /// Kiểm tra số nhà/tầng hợp lệ
    /// </summary>
    public static bool IsValidFloorNumber(int floor, int minFloor = 1, int maxFloor = 100)
    {
        return floor >= minFloor && floor <= maxFloor;
    }

    /// <summary>
    /// Kiểm tra diện tích căn hộ hợp lệ
    /// </summary>
    public static bool IsValidApartmentArea(decimal area, decimal minArea = 20, decimal maxArea = 500)
    {
        return area >= minArea && area <= maxArea;
    }

    /// <summary>
    /// Kiểm tra số tiền hợp lệ
    /// </summary>
    public static bool IsValidAmount(decimal amount, decimal minAmount = 0)
    {
        return amount >= minAmount;
    }

    /// <summary>
    /// Kiểm tra tháng hợp lệ
    /// </summary>
    public static bool IsValidMonth(int month)
    {
        return month >= 1 && month <= 12;
    }

    /// <summary>
    /// Kiểm tra năm hợp lệ
    /// </summary>
    public static bool IsValidYear(int year, int minYear = 2000)
    {
        var currentYear = DateTime.Now.Year;
        return year >= minYear && year <= currentYear + 10;
    }

    /// <summary>
    /// Kiểm tra biển số xe hợp lệ (Việt Nam)
    /// </summary>
    public static bool IsValidLicensePlate(string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            return false;

        // Format: 30A-12345 hoặc 30A12345
        var pattern = @"^\d{2}[A-Z]{1,2}[-\s]?\d{4,5}$";
        return System.Text.RegularExpressions.Regex.IsMatch(licensePlate.ToUpper(), pattern);
    }

    /// <summary>
    /// Kiểm tra mã tracking bưu phẩm
    /// </summary>
    public static bool IsValidTrackingNumber(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            return false;

        // Chấp nhận chữ và số, độ dài 6-30 ký tự
        return trackingNumber.Length >= 6 && trackingNumber.Length <= 30
               && System.Text.RegularExpressions.Regex.IsMatch(trackingNumber, @"^[A-Z0-9]+$");
    }

    /// <summary>
    /// Kiểm tra danh sách có rỗng không
    /// </summary>
    public static bool IsEmptyList<T>(IEnumerable<T>? list)
    {
        return list == null || !list.Any();
    }

    /// <summary>
    /// Kiểm tra collection có phần tử trùng lặp không
    /// </summary>
    public static bool HasDuplicates<T>(IEnumerable<T> collection)
    {
        var set = new HashSet<T>();
        return collection.Any(item => !set.Add(item));
    }

    /// <summary>
    /// Tạo danh sách lỗi validation
    /// </summary>
    public static List<string> GetValidationErrors(params (bool isValid, string errorMessage)[] validations)
    {
        var errors = new List<string>();
        
        foreach (var (isValid, errorMessage) in validations)
        {
            if (!isValid)
            {
                errors.Add(errorMessage);
            }
        }

        return errors;
    }

    /// <summary>
    /// Validate thông tin căn hộ
    /// </summary>
    public static (bool IsValid, List<string> Errors) ValidateApartment(
        string apartmentNumber, 
        int floor, 
        decimal? area)
    {
        var validations = new List<(bool, string)>
        {
            (!IsNullOrEmpty(apartmentNumber), "Số căn hộ không được để trống"),
            (IsValidFloorNumber(floor), $"Tầng phải từ 1 đến 100"),
        };

        if (area.HasValue)
        {
            validations.Add((IsValidApartmentArea(area.Value), "Diện tích căn hộ không hợp lệ (20-500 m²)"));
        }

        var errors = GetValidationErrors(validations.ToArray());
        return (errors.Count == 0, errors);
    }

    /// <summary>
    /// Validate thông tin cư dân
    /// </summary>
    public static (bool IsValid, List<string> Errors) ValidateResident(
        string fullName, 
        DateTime? dateOfBirth, 
        string? phoneNumber, 
        string? email)
    {
        var validations = new List<(bool, string)>
        {
            (!IsNullOrEmpty(fullName), "Họ tên không được để trống"),
            (IsValidLength(fullName, 2, 200), "Họ tên phải từ 2-200 ký tự"),
        };

        if (dateOfBirth.HasValue)
        {
            validations.Add((IsValidAge(dateOfBirth.Value, 0, 150), "Ngày sinh không hợp lệ"));
        }

        if (!IsNullOrEmpty(phoneNumber))
        {
            validations.Add((IsValidPhoneNumber(phoneNumber!), "Số điện thoại không hợp lệ"));
        }

        if (!IsNullOrEmpty(email))
        {
            validations.Add((IsValidEmail(email!), "Email không hợp lệ"));
        }

        var errors = GetValidationErrors(validations.ToArray());
        return (errors.Count == 0, errors);
    }

    /// <summary>
    /// Validate thông tin hóa đơn
    /// </summary>
    public static (bool IsValid, List<string> Errors) ValidateInvoice(
        int billingMonth,
        int billingYear,
        decimal totalAmount,
        DateTime dueDate)
    {
        var validations = new List<(bool, string)>
        {
            (IsValidMonth(billingMonth), "Tháng thanh toán không hợp lệ"),
            (IsValidYear(billingYear), "Năm thanh toán không hợp lệ"),
            (IsValidAmount(totalAmount), "Tổng tiền phải lớn hơn 0"),
            (IsValidFutureDate(dueDate) || dueDate.Date == DateTime.Now.Date, "Ngày đến hạn không được trong quá khứ"),
        };

        var errors = GetValidationErrors(validations.ToArray());
        return (errors.Count == 0, errors);
    }

    /// <summary>
    /// Validate file upload
    /// </summary>
    public static (bool IsValid, List<string> Errors) ValidateFileUpload(
        string fileName,
        long fileSize,
        string[] allowedExtensions,
        int maxSizeInMB = 10)
    {
        var validations = new List<(bool, string)>
        {
            (!IsNullOrEmpty(fileName), "Tên file không được để trống"),
            (FileUtils.IsValidExtension(fileName, allowedExtensions), 
                $"File không hợp lệ. Chỉ chấp nhận: {string.Join(", ", allowedExtensions)}"),
            (FileUtils.IsValidFileSize(fileSize, maxSizeInMB), 
                $"File quá lớn. Kích thước tối đa: {maxSizeInMB}MB"),
        };

        var errors = GetValidationErrors(validations.ToArray());
        return (errors.Count == 0, errors);
    }
}

