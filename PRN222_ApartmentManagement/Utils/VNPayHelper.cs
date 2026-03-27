using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PRN222_ApartmentManagement.Utils;

public class VNPayHelper
{
    private readonly string _tmnCode;
    private readonly string _hashSecret;
    private readonly string _baseUrl;
    private readonly string _ipnUrl;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public VNPayHelper(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        var vnpay = configuration.GetSection("VNPay");
        _tmnCode = vnpay["TmnCode"]
            ?? throw new InvalidOperationException("VNPay TmnCode is not configured.");
        _hashSecret = vnpay["HashSecret"]
            ?? throw new InvalidOperationException("VNPay HashSecret is not configured.");
        _baseUrl = vnpay["BaseUrl"]
            ?? throw new InvalidOperationException("VNPay BaseUrl is not configured.");
        _ipnUrl = vnpay["IpnUrl"] ?? "";
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Trả về base URL động (http://host:port) từ request hiện tại.
    /// </summary>
    private string GetBaseUrl()
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null)
            return "http://localhost:5001";
        return $"{request.Scheme}://{request.Host}";
    }

    public string CreatePaymentUrl(int invoiceId, decimal amountInVnd, string invoiceNumber, string txnRef)
    {
        var now = DateTime.Now;
        var vnpCreateDate = now.ToString("yyyyMMddHHmmss");
        var vnpExpireDate = now.AddMinutes(15).ToString("yyyyMMddHHmmss");
        var returnUrl = $"{GetBaseUrl()}/Resident/Invoices";

        var requestData = new SortedList<string, string>(new InvariantCultureOrdinalComparer())
        {
            ["vnp_Version"] = "2.1.0",
            ["vnp_Command"] = "pay",
            ["vnp_TmnCode"] = _tmnCode,
            ["vnp_Amount"] = ((long)(amountInVnd * 100)).ToString(),
            ["vnp_CurrCode"] = "VND",
            ["vnp_TxnRef"] = txnRef,
            ["vnp_OrderInfo"] = $"Thanh toán hóa đơn {invoiceNumber}",
            ["vnp_OrderType"] = "billpayment",
            ["vnp_Locale"] = "vn",
            ["vnp_ReturnUrl"] = returnUrl,
            ["vnp_IpAddr"] = "127.0.0.1",
            ["vnp_CreateDate"] = vnpCreateDate,
            ["vnp_ExpireDate"] = vnpExpireDate
        };

        var queryString = BuildQueryString(requestData);
        var signature = ComputeHmacSha512(queryString, _hashSecret);

        return $"{_baseUrl}?{queryString}&vnp_SecureHash={Uri.EscapeDataString(signature)}";
    }

    /// <summary>
    /// Verify chữ ký từ VNPay callback (IPN & Return).
    /// </summary>
    public bool VerifySignature(Dictionary<string, string> parameters)
    {
        if (!parameters.TryGetValue("vnp_SecureHash", out var receivedHash) || string.IsNullOrEmpty(receivedHash))
            return false;

        var responseData = parameters
            .Where(kv => kv.Key.StartsWith("vnp_")
                && kv.Key != "vnp_SecureHash"
                && kv.Key != "vnp_SecureHashType")
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        var queryString = BuildQueryStringForResponse(responseData);
        var expectedHash = ComputeHmacSha512(queryString, _hashSecret);

        return string.Equals(expectedHash, receivedHash, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Parse kết quả từ VNPay trả về.
    /// </summary>
    public VNPayResult ParseReturn(Dictionary<string, string> parameters)
    {
        return new VNPayResult
        {
            TxnRef = parameters.GetValueOrDefault("vnp_TxnRef", ""),
            VnpAmount = long.TryParse(parameters.GetValueOrDefault("vnp_Amount", "0"), out var amt) ? amt : 0,
            VnpResponseCode = parameters.GetValueOrDefault("vnp_ResponseCode", ""),
            VnpTransactionStatus = parameters.GetValueOrDefault("vnp_TransactionStatus", ""),
            VnpTransactionNo = parameters.GetValueOrDefault("vnp_TransactionNo", ""),
            VnpBankCode = parameters.GetValueOrDefault("vnp_BankCode", ""),
            VnpPayDate = parameters.GetValueOrDefault("vnp_PayDate", ""),
            VnpOrderInfo = parameters.GetValueOrDefault("vnp_OrderInfo", ""),
            IsSuccess = parameters.GetValueOrDefault("vnp_ResponseCode", "") == "00"
                        && parameters.GetValueOrDefault("vnp_TransactionStatus", "") == "00"
        };
    }

    /// <summary>
    /// Build query string cho hash: key=value&key=value (cả key và value đều WebUtility.UrlEncode).
    /// </summary>
    private static string BuildQueryString(SortedList<string, string> requestData)
    {
        var sb = new StringBuilder();
        foreach (var kv in requestData.Where(kv => !string.IsNullOrEmpty(kv.Value)))
        {
            sb.Append(WebUtility.UrlEncode(kv.Key));
            sb.Append('=');
            sb.Append(WebUtility.UrlEncode(kv.Value));
            sb.Append('&');
        }
        if (sb.Length > 0)
            sb.Length--;
        return sb.ToString();
    }

    /// <summary>
    /// Build query string cho response verification (cả key và value đều WebUtility.UrlEncode).
    /// </summary>
    private static string BuildQueryStringForResponse(Dictionary<string, string> responseData)
    {
        var sorted = new SortedList<string, string>(responseData, new InvariantCultureOrdinalComparer());
        var sb = new StringBuilder();
        foreach (var kv in sorted.Where(kv => !string.IsNullOrEmpty(kv.Value)))
        {
            sb.Append(WebUtility.UrlEncode(kv.Key));
            sb.Append('=');
            sb.Append(WebUtility.UrlEncode(kv.Value));
            sb.Append('&');
        }
        if (sb.Length > 0)
            sb.Length--;
        return sb.ToString();
    }

    /// <summary>
    /// HMAC-SHA512 hash, format hex không có dấu '-'.
    /// </summary>
    private static string ComputeHmacSha512(string data, string key)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "");
    }
}

public class VNPayResult
{
    public string TxnRef { get; set; } = "";
    public long VnpAmount { get; set; }
    public string VnpResponseCode { get; set; } = "";
    public string VnpTransactionStatus { get; set; } = "";
    public string VnpTransactionNo { get; set; } = "";
    public string VnpBankCode { get; set; } = "";
    public string VnpPayDate { get; set; } = "";
    public string VnpOrderInfo { get; set; } = "";
    public bool IsSuccess { get; set; }
}

/// <summary>
/// So sánh string theo culture-invariant ordinal (giống hệt VNPay.NET library).
/// </summary>
internal sealed class InvariantCultureOrdinalComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        return CompareInfo.GetCompareInfo("en-US").Compare(x, y, CompareOptions.Ordinal);
    }
}
