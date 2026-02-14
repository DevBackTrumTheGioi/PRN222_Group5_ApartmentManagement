namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface ISettingService
{
    Task<string> GetSettingAsync(string key, string defaultValue = "");
    Task<Dictionary<string, string>> GetAllSettingsAsync();
    Task UpdateSettingAsync(string key, string value);
}

