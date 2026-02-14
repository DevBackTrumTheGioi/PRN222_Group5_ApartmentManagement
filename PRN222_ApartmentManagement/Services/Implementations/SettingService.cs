using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Services.Implementations;

public class SettingService : ISettingService
{
    private readonly ApartmentDbContext _context;

    public SettingService(ApartmentDbContext context)
    {
        _context = context;
    }

    public async Task<string> GetSettingAsync(string key, string defaultValue = "")
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == key);
        return setting?.SettingValue ?? defaultValue;
    }

    public async Task<Dictionary<string, string>> GetAllSettingsAsync()
    {
        return await _context.SystemSettings.ToDictionaryAsync(s => s.SettingKey, s => s.SettingValue);
    }

    public async Task UpdateSettingAsync(string key, string value)
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == key);
        if (setting != null)
        {
            setting.SettingValue = value;
            setting.UpdatedAt = DateTime.Now;
        }
        else
        {
            _context.SystemSettings.Add(new SystemSetting
            {
                SettingKey = key,
                SettingValue = value,
                UpdatedAt = DateTime.Now
            });
        }
        await _context.SaveChangesAsync();
    }
}

