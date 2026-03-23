using Newtonsoft.Json.Linq;
using PRN222_ApartmentManagement.Models.Enums;
using UniSdk;

namespace PRN222_ApartmentManagement.Utils;

public class UniMatrixHelper
{
    private readonly UniClient _client;

    public UniMatrixHelper(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        var config = configuration.GetSection("UniMatrix");
        var accessKeyId = config["AccessKeyId"] ?? throw new InvalidOperationException("UniMatrix AccessKeyId is not configured.");
        var accessKeySecret = config["AccessKeySecret"] ?? "";

        _client = string.IsNullOrEmpty(accessKeySecret)
            ? new UniClient(accessKeyId)
            : new UniClient(accessKeyId, accessKeySecret);
    }

    public async Task<UniMatrixSmsResult> SendSmsAsync(string phoneNumber, string message)
    {
        var phoneE164 = NormalizeToE164(phoneNumber);

        try
        {
            var resp = await _client.Messages.SendAsync(new
            {
                to = phoneE164,
                text = message
            });

            if (resp.Code == "0")
            {
                var messages = resp.Data?["messages"] as JArray;
                var firstMsg = messages?.FirstOrDefault() as JObject;
                return new UniMatrixSmsResult
                {
                    Success = true,
                    MessageId = firstMsg?["id"]?.ToString() ?? "",
                    Phone = firstMsg?["to"]?.ToString() ?? phoneE164
                };
            }

            return new UniMatrixSmsResult
            {
                Success = false,
                Error = resp.Message ?? "Unknown error"
            };
        }
        catch (Exception ex)
        {
            return new UniMatrixSmsResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public async Task<UniMatrixOtpSendResult> SendOtpAsync(string phoneNumber, OtpIntent intent)
    {
        var phoneE164 = NormalizeToE164(phoneNumber);

        try
        {
            var resp = await _client.Otp.SendAsync(new { to = phoneE164 });

            if (resp.Code == "0")
            {
                return new UniMatrixOtpSendResult
                {
                    Success = true,
                    MessageId = resp.Data?["messageId"]?.ToString() ?? "",
                    Phone = phoneE164
                };
            }

            return new UniMatrixOtpSendResult
            {
                Success = false,
                Error = resp.Message ?? "Unknown error"
            };
        }
        catch (Exception ex)
        {
            return new UniMatrixOtpSendResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public async Task<UniMatrixOtpVerifyResult> VerifyOtpAsync(string phoneNumber, string code, OtpIntent intent)
    {
        var phoneE164 = NormalizeToE164(phoneNumber);

        try
        {
            var resp = await _client.Otp.VerifyAsync(new { to = phoneE164, code = code });

            if (resp.Code == "0")
            {
                return new UniMatrixOtpVerifyResult
                {
                    Valid = resp.Data?["valid"]?.Value<bool>() ?? false
                };
            }

            return new UniMatrixOtpVerifyResult
            {
                Valid = false,
                Error = resp.Message ?? "Unknown error"
            };
        }
        catch (Exception ex)
        {
            return new UniMatrixOtpVerifyResult
            {
                Valid = false,
                Error = ex.Message
            };
        }
    }

    private static string NormalizeToE164(string phoneNumber)
    {
        var normalized = StringUtils.NormalizePhoneNumber(phoneNumber);

        if (normalized.StartsWith("+84"))
            return normalized;

        if (normalized.StartsWith("84"))
            return "+" + normalized;

        if (normalized.StartsWith("0"))
            return "+84" + normalized[1..];

        return "+" + normalized;
    }
}

public class UniMatrixOtpSendResult
{
    public bool Success { get; set; }
    public string? MessageId { get; set; }
    public string? Phone { get; set; }
    public string? Error { get; set; }
}

public class UniMatrixOtpVerifyResult
{
    public bool Valid { get; set; }
    public string? Error { get; set; }
}

public class UniMatrixSmsResult
{
    public bool Success { get; set; }
    public string? MessageId { get; set; }
    public string? Phone { get; set; }
    public string? Error { get; set; }
}
