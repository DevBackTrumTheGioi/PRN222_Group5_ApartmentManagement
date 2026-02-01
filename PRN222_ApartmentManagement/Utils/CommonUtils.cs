using System.Text.Json;

namespace PRN222_ApartmentManagement.Utils;

/// <summary>
/// Tiện ích xử lý JSON và serialization
/// </summary>
public static class JsonUtils
{
    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Serialize object thành JSON string
    /// </summary>
    public static string Serialize<T>(T obj, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(obj, options ?? DefaultOptions);
    }

    /// <summary>
    /// Deserialize JSON string thành object
    /// </summary>
    public static T? Deserialize<T>(string json, JsonSerializerOptions? options = null)
    {
        if (string.IsNullOrWhiteSpace(json))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(json, options ?? DefaultOptions);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Serialize object thành JSON và lưu vào file
    /// </summary>
    public static async Task SerializeToFileAsync<T>(T obj, string filePath, JsonSerializerOptions? options = null)
    {
        var json = Serialize(obj, options);
        await FileUtils.WriteTextFileAsync(filePath, json);
    }

    /// <summary>
    /// Đọc JSON từ file và deserialize
    /// </summary>
    public static async Task<T?> DeserializeFromFileAsync<T>(string filePath, JsonSerializerOptions? options = null)
    {
        var json = await FileUtils.ReadTextFileAsync(filePath);
        return Deserialize<T>(json, options);
    }

    /// <summary>
    /// Clone object bằng JSON serialization
    /// </summary>
    public static T? Clone<T>(T obj)
    {
        var json = Serialize(obj);
        return Deserialize<T>(json);
    }

    /// <summary>
    /// Kiểm tra JSON có hợp lệ không
    /// </summary>
    public static bool IsValidJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            using var document = JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Format JSON string (prettify)
    /// </summary>
    public static string FormatJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return string.Empty;

        try
        {
            using var document = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(document, DefaultOptions);
        }
        catch
        {
            return json;
        }
    }

    /// <summary>
    /// Minify JSON string (loại bỏ whitespace)
    /// </summary>
    public static string MinifyJson(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return string.Empty;

        try
        {
            using var document = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(document, new JsonSerializerOptions { WriteIndented = false });
        }
        catch
        {
            return json;
        }
    }

    /// <summary>
    /// Merge 2 JSON objects
    /// </summary>
    public static string MergeJson(string json1, string json2)
    {
        try
        {
            using var doc1 = JsonDocument.Parse(json1);
            using var doc2 = JsonDocument.Parse(json2);

            var merged = new Dictionary<string, object?>();

            // Add properties from first JSON
            foreach (var property in doc1.RootElement.EnumerateObject())
            {
                merged[property.Name] = property.Value.GetRawText();
            }

            // Add/Override with properties from second JSON
            foreach (var property in doc2.RootElement.EnumerateObject())
            {
                merged[property.Name] = property.Value.GetRawText();
            }

            return Serialize(merged);
        }
        catch
        {
            return json1;
        }
    }
}

/// <summary>
/// Tiện ích xử lý số và toán học
/// </summary>
public static class NumberUtils
{
    /// <summary>
    /// Làm tròn số tiền (đến hàng nghìn)
    /// </summary>
    public static decimal RoundToThousand(decimal amount)
    {
        return Math.Round(amount / 1000) * 1000;
    }

    /// <summary>
    /// Tính phần trăm
    /// </summary>
    public static decimal CalculatePercentage(decimal value, decimal total)
    {
        if (total == 0) return 0;
        return Math.Round((value / total) * 100, 2);
    }

    /// <summary>
    /// Tính tiền từ phần trăm
    /// </summary>
    public static decimal CalculateAmountFromPercentage(decimal percentage, decimal total)
    {
        return Math.Round((percentage / 100) * total, 2);
    }

    /// <summary>
    /// Tính VAT (thuế GTGT)
    /// </summary>
    public static decimal CalculateVat(decimal amount, decimal vatRate = 10)
    {
        return Math.Round(amount * (vatRate / 100), 2);
    }

    /// <summary>
    /// Tính tổng tiền sau VAT
    /// </summary>
    public static decimal CalculateTotalWithVat(decimal amount, decimal vatRate = 10)
    {
        return amount + CalculateVat(amount, vatRate);
    }

    /// <summary>
    /// Tính trung bình
    /// </summary>
    public static decimal Average(params decimal[] values)
    {
        if (values.Length == 0) return 0;
        return values.Average();
    }

    /// <summary>
    /// Tính tổng
    /// </summary>
    public static decimal Sum(params decimal[] values)
    {
        return values.Sum();
    }

    /// <summary>
    /// Tìm giá trị lớn nhất
    /// </summary>
    public static decimal Max(params decimal[] values)
    {
        if (values.Length == 0) return 0;
        return values.Max();
    }

    /// <summary>
    /// Tìm giá trị nhỏ nhất
    /// </summary>
    public static decimal Min(params decimal[] values)
    {
        if (values.Length == 0) return 0;
        return values.Min();
    }

    /// <summary>
    /// Kiểm tra số có phải số nguyên không
    /// </summary>
    public static bool IsInteger(decimal value)
    {
        return value % 1 == 0;
    }

    /// <summary>
    /// Chuyển đổi số sang định dạng tiền tệ
    /// </summary>
    public static string ToCurrency(decimal amount, string currencySymbol = "VNĐ")
    {
        return $"{amount:N0} {currencySymbol}";
    }

    /// <summary>
    /// Tính lãi suất đơn giản
    /// </summary>
    public static decimal CalculateSimpleInterest(decimal principal, decimal rate, int months)
    {
        return principal * (rate / 100) * (months / 12m);
    }

    /// <summary>
    /// Tính lãi suất kép
    /// </summary>
    public static decimal CalculateCompoundInterest(decimal principal, decimal annualRate, int months)
    {
        var monthlyRate = annualRate / 100 / 12;
        return principal * (decimal)Math.Pow((double)(1 + monthlyRate), months) - principal;
    }

    /// <summary>
    /// Tính số tiền trả góp hàng tháng
    /// </summary>
    public static decimal CalculateMonthlyPayment(decimal loanAmount, decimal annualRate, int months)
    {
        if (annualRate == 0) return loanAmount / months;
        
        var monthlyRate = annualRate / 100 / 12;
        return loanAmount * monthlyRate * (decimal)Math.Pow((double)(1 + monthlyRate), months) /
               ((decimal)Math.Pow((double)(1 + monthlyRate), months) - 1);
    }

    /// <summary>
    /// Làm tròn lên
    /// </summary>
    public static decimal RoundUp(decimal value, int decimals = 0)
    {
        var multiplier = (decimal)Math.Pow(10, decimals);
        return Math.Ceiling(value * multiplier) / multiplier;
    }

    /// <summary>
    /// Làm tròn xuống
    /// </summary>
    public static decimal RoundDown(decimal value, int decimals = 0)
    {
        var multiplier = (decimal)Math.Pow(10, decimals);
        return Math.Floor(value * multiplier) / multiplier;
    }

    /// <summary>
    /// Kiểm tra số có trong khoảng không (bao gồm biên)
    /// </summary>
    public static bool IsBetween(decimal value, decimal min, decimal max)
    {
        return value >= min && value <= max;
    }

    /// <summary>
    /// Clamp số trong khoảng
    /// </summary>
    public static decimal Clamp(decimal value, decimal min, decimal max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}

/// <summary>
/// Tiện ích xử lý Collection
/// </summary>
public static class CollectionUtils
{
    /// <summary>
    /// Phân trang danh sách
    /// </summary>
    public static IEnumerable<T> Paginate<T>(IEnumerable<T> source, int pageNumber, int pageSize)
    {
        return source.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    /// <summary>
    /// Tính tổng số trang
    /// </summary>
    public static int CalculateTotalPages(int totalItems, int pageSize)
    {
        return (int)Math.Ceiling(totalItems / (double)pageSize);
    }

    /// <summary>
    /// Chia danh sách thành các batch
    /// </summary>
    public static IEnumerable<IEnumerable<T>> Batch<T>(IEnumerable<T> source, int batchSize)
    {
        var batch = new List<T>(batchSize);
        foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }
        if (batch.Count > 0)
            yield return batch;
    }

    /// <summary>
    /// Shuffle danh sách (xáo trộn ngẫu nhiên)
    /// </summary>
    public static IEnumerable<T> Shuffle<T>(IEnumerable<T> source)
    {
        var random = new Random();
        return source.OrderBy(_ => random.Next());
    }

    /// <summary>
    /// Distinct by property
    /// </summary>
    public static IEnumerable<T> DistinctBy<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        var seenKeys = new HashSet<TKey>();
        foreach (var item in source)
        {
            if (seenKeys.Add(keySelector(item)))
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Kiểm tra collection có null hoặc rỗng không
    /// </summary>
    public static bool IsNullOrEmpty<T>(IEnumerable<T>? collection)
    {
        return collection == null || !collection.Any();
    }

    /// <summary>
    /// Safe FirstOrDefault
    /// </summary>
    public static T? SafeFirstOrDefault<T>(IEnumerable<T>? collection) where T : class
    {
        return collection?.FirstOrDefault();
    }

    /// <summary>
    /// Safe Count
    /// </summary>
    public static int SafeCount<T>(IEnumerable<T>? collection)
    {
        return collection?.Count() ?? 0;
    }
}

