using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Examples;

/// <summary>
/// Ví dụ sử dụng các Utils
/// </summary>
public class UtilsExamples
{
    /// <summary>
    /// Demo StringUtils
    /// </summary>
    public void StringUtilsDemo()
    {
        Console.WriteLine("=== STRING UTILS DEMO ===\n");

        // Tạo các mã số
        var invoiceNum = StringUtils.GenerateInvoiceNumber(2026, 2, 1);
        Console.WriteLine($"Invoice Number: {invoiceNum}"); // INV-202602-0001

        var contractNum = StringUtils.GenerateContractNumber(DateTime.Now, 5);
        Console.WriteLine($"Contract Number: {contractNum}"); // CT-20260201-005

        var requestNum = StringUtils.GenerateRequestNumber(DateTime.Now, 10);
        Console.WriteLine($"Request Number: {requestNum}"); // REQ-20260201-010

        // Xử lý chuỗi tiếng Việt
        var slug = StringUtils.ToSlug("Căn hộ cao cấp view biển");
        Console.WriteLine($"Slug: {slug}"); // can-ho-cao-cap-view-bien

        var noTone = StringUtils.RemoveVietnameseTones("Nguyễn Văn A");
        Console.WriteLine($"No tone: {noTone}"); // nguyen van a

        // Validation
        var phoneValid = StringUtils.IsValidVietnamesePhoneNumber("0987654321");
        Console.WriteLine($"Phone valid: {phoneValid}"); // True

        var emailValid = StringUtils.IsValidEmail("test@example.com");
        Console.WriteLine($"Email valid: {emailValid}"); // True

        // Format và mask
        var currency = StringUtils.FormatCurrency(1500000);
        Console.WriteLine($"Currency: {currency}"); // 1,500,000 VNĐ

        var maskedPhone = StringUtils.MaskPhoneNumber("0987654321");
        Console.WriteLine($"Masked Phone: {maskedPhone}"); // 098***4321

        var maskedEmail = StringUtils.MaskEmail("john.doe@example.com");
        Console.WriteLine($"Masked Email: {maskedEmail}"); // joh***@example.com

        // Số sang chữ
        var words = StringUtils.NumberToWords(123456789);
        Console.WriteLine($"Number to words: {words}");
    }

    /// <summary>
    /// Demo DateTimeUtils
    /// </summary>
    public void DateTimeUtilsDemo()
    {
        Console.WriteLine("\n=== DATETIME UTILS DEMO ===\n");

        // Ngày đầu/cuối tháng
        var firstDay = DateTimeUtils.GetFirstDayOfMonth(2026, 2);
        var lastDay = DateTimeUtils.GetLastDayOfMonth(2026, 2);
        Console.WriteLine($"First day: {firstDay:dd/MM/yyyy}");
        Console.WriteLine($"Last day: {lastDay:dd/MM/yyyy}");

        // Tính tuổi
        var age = DateTimeUtils.CalculateAge(new DateTime(1990, 5, 15));
        Console.WriteLine($"Age: {age}");

        // Format tiếng Việt
        var vietDate = DateTimeUtils.FormatVietnameseDate(DateTime.Now);
        Console.WriteLine($"Vietnamese date: {vietDate}");

        var vietDateTime = DateTimeUtils.FormatVietnameseDateTime(DateTime.Now);
        Console.WriteLine($"Vietnamese datetime: {vietDateTime}");

        // Thời gian tương đối
        var relative1 = DateTimeUtils.FormatRelativeTime(DateTime.Now.AddMinutes(-30));
        Console.WriteLine($"30 minutes ago: {relative1}");

        var relative2 = DateTimeUtils.FormatRelativeTime(DateTime.Now.AddDays(-3));
        Console.WriteLine($"3 days ago: {relative2}");

        // Kiểm tra
        var dueDate = DateTime.Now.AddDays(5);
        var isOverdue = DateTimeUtils.IsOverdue(dueDate);
        var isUpcoming = DateTimeUtils.IsUpcoming(dueDate, 7);
        Console.WriteLine($"Due date overdue: {isOverdue}");
        Console.WriteLine($"Due date upcoming: {isUpcoming}");
    }

    /// <summary>
    /// Demo SecurityUtils
    /// </summary>
    public void SecurityUtilsDemo()
    {
        Console.WriteLine("\n=== SECURITY UTILS DEMO ===\n");

        // Hash password
        var password = "MySecurePassword123!";
        var hashed = SecurityUtils.HashPassword(password);
        Console.WriteLine($"Hashed password: {hashed[..20]}...");

        // Verify password
        var isValid = SecurityUtils.VerifyPassword(password, hashed);
        Console.WriteLine($"Password valid: {isValid}");

        // Generate OTP
        var otp = SecurityUtils.GenerateOtp(6);
        Console.WriteLine($"OTP: {otp}");

        // Generate token
        var token = SecurityUtils.GenerateToken(32);
        Console.WriteLine($"Token: {token}");

        // Check password strength
        var strength1 = SecurityUtils.CheckPasswordStrength("123456");
        var strength2 = SecurityUtils.CheckPasswordStrength("MySecure123!");
        Console.WriteLine($"'123456' strength: {strength1}");
        Console.WriteLine($"'MySecure123!' strength: {strength2}");

        // Generate random password
        var randomPass = SecurityUtils.GenerateRandomPassword(12, true);
        Console.WriteLine($"Random password: {randomPass}");

        // Generate QR code data
        var qrCode = SecurityUtils.GenerateVisitorQrCode(123, DateTime.Now);
        Console.WriteLine($"QR Code data: {qrCode[..30]}...");
    }

    /// <summary>
    /// Demo FileUtils
    /// </summary>
    public void FileUtilsDemo()
    {
        Console.WriteLine("\n=== FILE UTILS DEMO ===\n");

        // MIME type
        var mimeType = FileUtils.GetMimeType("document.pdf");
        Console.WriteLine($"PDF MIME type: {mimeType}");

        // File type checks
        var isImage = FileUtils.IsImageFile("photo.jpg");
        var isDoc = FileUtils.IsDocumentFile("report.pdf");
        Console.WriteLine($"photo.jpg is image: {isImage}");
        Console.WriteLine($"report.pdf is document: {isDoc}");

        // Format file size
        var size1 = FileUtils.FormatFileSize(1024);
        var size2 = FileUtils.FormatFileSize(1536000);
        var size3 = FileUtils.FormatFileSize(10737418240);
        Console.WriteLine($"1024 bytes: {size1}");
        Console.WriteLine($"1536000 bytes: {size2}");
        Console.WriteLine($"10737418240 bytes: {size3}");

        // Generate unique filename
        var uniqueName = FileUtils.GenerateUniqueFileName("my-document.pdf");
        Console.WriteLine($"Unique filename: {uniqueName}");

        // Upload path
        var uploadPath = FileUtils.GetUploadPath("invoice", 123);
        Console.WriteLine($"Upload path: {uploadPath}");

        // Sanitize filename
        var sanitized = FileUtils.SanitizeFileName("Invalid<>|Name?.pdf");
        Console.WriteLine($"Sanitized: {sanitized}");
    }

    /// <summary>
    /// Demo ValidationUtils
    /// </summary>
    public void ValidationUtilsDemo()
    {
        Console.WriteLine("\n=== VALIDATION UTILS DEMO ===\n");

        // Validate apartment
        var (valid1, errors1) = ValidationUtils.ValidateApartment("A101", 5, 75.5m);
        Console.WriteLine($"Apartment valid: {valid1}");
        if (!valid1)
        {
            Console.WriteLine($"Errors: {string.Join(", ", errors1)}");
        }

        // Validate resident
        var (valid2, errors2) = ValidationUtils.ValidateResident(
            "Nguyễn Văn A",
            new DateTime(1990, 1, 1),
            "0987654321",
            "test@example.com"
        );
        Console.WriteLine($"Resident valid: {valid2}");
        if (!valid2)
        {
            Console.WriteLine($"Errors: {string.Join(", ", errors2)}");
        }

        // Validate invoice
        var (valid3, errors3) = ValidationUtils.ValidateInvoice(
            2, 2026, 1500000, DateTime.Now.AddDays(7)
        );
        Console.WriteLine($"Invoice valid: {valid3}");

        // License plate validation
        var licensePlateValid = ValidationUtils.IsValidLicensePlate("30A-12345");
        Console.WriteLine($"License plate '30A-12345' valid: {licensePlateValid}");

        // File upload validation
        var (valid4, errors4) = ValidationUtils.ValidateFileUpload(
            "document.pdf",
            5242880, // 5MB
            new[] { ".pdf", ".doc", ".docx" },
            10
        );
        Console.WriteLine($"File upload valid: {valid4}");
    }

    /// <summary>
    /// Demo NumberUtils
    /// </summary>
    public void NumberUtilsDemo()
    {
        Console.WriteLine("\n=== NUMBER UTILS DEMO ===\n");

        // Round to thousand
        var rounded = NumberUtils.RoundToThousand(1234567.89m);
        Console.WriteLine($"Rounded to thousand: {rounded:N0}");

        // Calculate percentage
        var percent = NumberUtils.CalculatePercentage(300000, 1000000);
        Console.WriteLine($"300,000 / 1,000,000 = {percent}%");

        // VAT calculation
        var amount = 1000000m;
        var vat = NumberUtils.CalculateVat(amount, 10);
        var totalWithVat = NumberUtils.CalculateTotalWithVat(amount, 10);
        Console.WriteLine($"Amount: {amount:N0}");
        Console.WriteLine($"VAT (10%): {vat:N0}");
        Console.WriteLine($"Total: {totalWithVat:N0}");

        // Monthly payment
        var loanAmount = 10000000m;
        var annualRate = 12m;
        var months = 24;
        var monthlyPayment = NumberUtils.CalculateMonthlyPayment(loanAmount, annualRate, months);
        Console.WriteLine($"\nLoan: {loanAmount:N0} VNĐ");
        Console.WriteLine($"Rate: {annualRate}%/year, Term: {months} months");
        Console.WriteLine($"Monthly payment: {monthlyPayment:N0} VNĐ");

        // Currency format
        var currency = NumberUtils.ToCurrency(1500000, "VNĐ");
        Console.WriteLine($"Currency: {currency}");
    }

    /// <summary>
    /// Demo CollectionUtils
    /// </summary>
    public void CollectionUtilsDemo()
    {
        Console.WriteLine("\n=== COLLECTION UTILS DEMO ===\n");

        var numbers = Enumerable.Range(1, 100).ToList();

        // Pagination
        var page1 = CollectionUtils.Paginate(numbers, 1, 10).ToList();
        var page2 = CollectionUtils.Paginate(numbers, 2, 10).ToList();
        Console.WriteLine($"Page 1 (10 items): {string.Join(", ", page1.Take(5))}...");
        Console.WriteLine($"Page 2 (10 items): {string.Join(", ", page2.Take(5))}...");

        // Total pages
        var totalPages = CollectionUtils.CalculateTotalPages(100, 10);
        Console.WriteLine($"Total pages: {totalPages}");

        // Batch
        var batches = CollectionUtils.Batch(numbers.Take(25), 5).ToList();
        Console.WriteLine($"Number of batches (25 items, size 5): {batches.Count}");

        // Shuffle
        var shuffled = CollectionUtils.Shuffle(Enumerable.Range(1, 10)).ToList();
        Console.WriteLine($"Shuffled 1-10: {string.Join(", ", shuffled)}");
    }

    /// <summary>
    /// Demo NotificationUtils
    /// </summary>
    public void NotificationUtilsDemo()
    {
        Console.WriteLine("\n=== NOTIFICATION UTILS DEMO ===\n");

        // Invoice notification
        var (title1, content1) = NotificationUtils.CreateInvoiceNotification(
            "INV-202602-0001",
            1500000,
            DateTime.Now.AddDays(7)
        );
        Console.WriteLine($"Invoice Notification:\nTitle: {title1}\nContent: {content1}\n");

        // Payment success notification
        var (title2, content2) = NotificationUtils.CreatePaymentSuccessNotification(
            "INV-202602-0001",
            1500000
        );
        Console.WriteLine($"Payment Success:\nTitle: {title2}\nContent: {content2}\n");

        // Visitor notification
        var (title3, content3) = NotificationUtils.CreateVisitorNotification(
            "Nguyễn Văn B",
            DateTime.Now.AddHours(2)
        );
        Console.WriteLine($"Visitor Notification:\nTitle: {title3}\nContent: {content3}\n");

        // Icons and badges
        var icon = NotificationUtils.GetNotificationIcon("Invoice");
        var color = NotificationUtils.GetPriorityColor(3);
        var badge = NotificationUtils.GetStatusBadgeClass("paid");
        Console.WriteLine($"Invoice icon: {icon}");
        Console.WriteLine($"Priority 3 color: {color}");
        Console.WriteLine($"'paid' badge class: {badge}");
    }

    /// <summary>
    /// Demo JsonUtils
    /// </summary>
    public void JsonUtilsDemo()
    {
        Console.WriteLine("\n=== JSON UTILS DEMO ===\n");

        var person = new
        {
            Name = "Nguyễn Văn A",
            Age = 30,
            Email = "test@example.com",
            Address = new
            {
                Street = "123 Main St",
                City = "Hà Nội"
            }
        };

        // Serialize
        var json = JsonUtils.Serialize(person);
        Console.WriteLine("Serialized JSON:");
        Console.WriteLine(json);

        // Minify
        var minified = JsonUtils.MinifyJson(json);
        Console.WriteLine($"\nMinified: {minified[..50]}...");

        // Validate
        var isValid = JsonUtils.IsValidJson(json);
        Console.WriteLine($"\nJSON valid: {isValid}");
    }

    /// <summary>
    /// Run all demos
    /// </summary>
    public void RunAllDemos()
    {
        StringUtilsDemo();
        DateTimeUtilsDemo();
        SecurityUtilsDemo();
        FileUtilsDemo();
        ValidationUtilsDemo();
        NumberUtilsDemo();
        CollectionUtilsDemo();
        NotificationUtilsDemo();
        JsonUtilsDemo();

        Console.WriteLine("\n=== ALL DEMOS COMPLETED ===");
    }
}

