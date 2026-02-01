namespace PRN222_ApartmentManagement.Utils;

/// <summary>
/// Tiện ích xử lý file và upload
/// </summary>
public static class FileUtils
{
    private static readonly Dictionary<string, string> MimeTypes = new()
    {
        { ".txt", "text/plain" },
        { ".pdf", "application/pdf" },
        { ".doc", "application/vnd.ms-word" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
        { ".xls", "application/vnd.ms-excel" },
        { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
        { ".png", "image/png" },
        { ".jpg", "image/jpeg" },
        { ".jpeg", "image/jpeg" },
        { ".gif", "image/gif" },
        { ".csv", "text/csv" },
        { ".zip", "application/zip" },
        { ".rar", "application/x-rar-compressed" }
    };

    private static readonly HashSet<string> AllowedImageExtensions = new()
    {
        ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
    };

    private static readonly HashSet<string> AllowedDocumentExtensions = new()
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".txt", ".csv"
    };

    /// <summary>
    /// Lấy MIME type từ extension
    /// </summary>
    public static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return MimeTypes.TryGetValue(extension, out var mimeType) ? mimeType : "application/octet-stream";
    }

    /// <summary>
    /// Kiểm tra file có phải ảnh không
    /// </summary>
    public static bool IsImageFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedImageExtensions.Contains(extension);
    }

    /// <summary>
    /// Kiểm tra file có phải document không
    /// </summary>
    public static bool IsDocumentFile(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return AllowedDocumentExtensions.Contains(extension);
    }

    /// <summary>
    /// Kiểm tra extension có hợp lệ không
    /// </summary>
    public static bool IsValidExtension(string fileName, string[] allowedExtensions)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }

    /// <summary>
    /// Tạo tên file unique với timestamp
    /// </summary>
    public static string GenerateUniqueFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(originalFileName);
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var randomString = Guid.NewGuid().ToString("N").Substring(0, 8);
        
        return $"{fileNameWithoutExt}_{timestamp}_{randomString}{extension}";
    }

    /// <summary>
    /// Định dạng kích thước file
    /// </summary>
    public static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    /// <summary>
    /// Kiểm tra kích thước file có hợp lệ không (MB)
    /// </summary>
    public static bool IsValidFileSize(long fileSizeInBytes, int maxSizeInMB)
    {
        var maxSizeInBytes = maxSizeInMB * 1024 * 1024;
        return fileSizeInBytes <= maxSizeInBytes;
    }

    /// <summary>
    /// Lấy đường dẫn upload theo loại file
    /// </summary>
    public static string GetUploadPath(string fileType, int? relatedId = null)
    {
        var basePath = Path.Combine("wwwroot", "uploads");
        var yearMonth = DateTime.Now.ToString("yyyy-MM");
        
        var path = fileType.ToLower() switch
        {
            "avatar" => Path.Combine(basePath, "avatars", yearMonth),
            "document" => Path.Combine(basePath, "documents", yearMonth),
            "contract" => Path.Combine(basePath, "contracts", yearMonth),
            "invoice" => Path.Combine(basePath, "invoices", yearMonth),
            "request" => Path.Combine(basePath, "requests", yearMonth),
            "announcement" => Path.Combine(basePath, "announcements", yearMonth),
            "message" => Path.Combine(basePath, "messages", yearMonth),
            _ => Path.Combine(basePath, "others", yearMonth)
        };

        return relatedId.HasValue ? Path.Combine(path, relatedId.Value.ToString()) : path;
    }

    /// <summary>
    /// Tạo thư mục nếu chưa tồn tại
    /// </summary>
    public static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// Lưu file từ stream
    /// </summary>
    public static async Task<string> SaveFileAsync(Stream fileStream, string fileName, string uploadPath)
    {
        EnsureDirectoryExists(uploadPath);
        
        var uniqueFileName = GenerateUniqueFileName(fileName);
        var fullPath = Path.Combine(uploadPath, uniqueFileName);

        using (var fileStreamOutput = new FileStream(fullPath, FileMode.Create))
        {
            await fileStream.CopyToAsync(fileStreamOutput);
        }

        return uniqueFileName;
    }

    /// <summary>
    /// Xóa file
    /// </summary>
    public static bool DeleteFile(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Xóa thư mục và toàn bộ nội dung
    /// </summary>
    public static bool DeleteDirectory(string directoryPath, bool recursive = true)
    {
        try
        {
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, recursive);
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Đọc file text
    /// </summary>
    public static async Task<string> ReadTextFileAsync(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("File not found", filePath);

        return await File.ReadAllTextAsync(filePath);
    }

    /// <summary>
    /// Ghi file text
    /// </summary>
    public static async Task WriteTextFileAsync(string filePath, string content)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            EnsureDirectoryExists(directory);
        }

        await File.WriteAllTextAsync(filePath, content);
    }

    /// <summary>
    /// Copy file
    /// </summary>
    public static bool CopyFile(string sourceFilePath, string destinationFilePath, bool overwrite = false)
    {
        try
        {
            if (!File.Exists(sourceFilePath))
                return false;

            var directory = Path.GetDirectoryName(destinationFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                EnsureDirectoryExists(directory);
            }

            File.Copy(sourceFilePath, destinationFilePath, overwrite);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Move file
    /// </summary>
    public static bool MoveFile(string sourceFilePath, string destinationFilePath, bool overwrite = false)
    {
        try
        {
            if (!File.Exists(sourceFilePath))
                return false;

            var directory = Path.GetDirectoryName(destinationFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                EnsureDirectoryExists(directory);
            }

            if (overwrite && File.Exists(destinationFilePath))
            {
                File.Delete(destinationFilePath);
            }

            File.Move(sourceFilePath, destinationFilePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Lấy thông tin file
    /// </summary>
    public static FileInfo? GetFileInfo(string filePath)
    {
        return File.Exists(filePath) ? new FileInfo(filePath) : null;
    }

    /// <summary>
    /// Lấy danh sách file trong thư mục
    /// </summary>
    public static List<string> GetFilesInDirectory(string directoryPath, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        if (!Directory.Exists(directoryPath))
            return new List<string>();

        return Directory.GetFiles(directoryPath, searchPattern, searchOption).ToList();
    }

    /// <summary>
    /// Dọn dẹp file cũ (xóa file quá X ngày)
    /// </summary>
    public static int CleanupOldFiles(string directoryPath, int daysOld)
    {
        if (!Directory.Exists(directoryPath))
            return 0;

        var deletedCount = 0;
        var cutoffDate = DateTime.Now.AddDays(-daysOld);

        var files = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            if (fileInfo.LastWriteTime < cutoffDate)
            {
                try
                {
                    File.Delete(file);
                    deletedCount++;
                }
                catch
                {
                    // Ignore errors
                }
            }
        }

        return deletedCount;
    }

    /// <summary>
    /// Sanitize tên file (loại bỏ ký tự không hợp lệ)
    /// </summary>
    public static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars));
        return sanitized;
    }

    /// <summary>
    /// Tạo đường dẫn URL từ đường dẫn vật lý
    /// </summary>
    public static string ToWebPath(string physicalPath)
    {
        if (string.IsNullOrEmpty(physicalPath))
            return string.Empty;

        var wwwrootIndex = physicalPath.IndexOf("wwwroot", StringComparison.OrdinalIgnoreCase);
        if (wwwrootIndex == -1)
            return physicalPath;

        var relativePath = physicalPath.Substring(wwwrootIndex + 8);
        return "/" + relativePath.Replace("\\", "/").TrimStart('/');
    }
}

