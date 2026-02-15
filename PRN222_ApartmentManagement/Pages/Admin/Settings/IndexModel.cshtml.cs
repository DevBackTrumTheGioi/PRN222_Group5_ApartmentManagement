using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.Admin.Settings;

[Authorize(Policy = "AdminOnly")]
public class IndexModel : PageModel
{
    private readonly ISettingService _settingService;
    private readonly IWebHostEnvironment _environment;

    public IndexModel(ISettingService settingService, IWebHostEnvironment environment)
    {
        _settingService = settingService;
        _environment = environment;
    }

    public Dictionary<string, string> Settings { get; set; } = new();

    public async Task OnGetAsync()
    {
        Settings = await _settingService.GetAllSettingsAsync();
    }

    public async Task<IActionResult> OnPostAsync(Dictionary<string, string> settings, IFormFile? logoFile)
    {
        if (logoFile != null)
        {
            try
            {
                var uploadsDir = Path.Combine(_environment.WebRootPath, "uploads", "system");
                if (!Directory.Exists(uploadsDir)) Directory.CreateDirectory(uploadsDir);

                var fileName = "logo" + Path.GetExtension(logoFile.FileName);
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await logoFile.CopyToAsync(stream);
                }

                settings["ApartmentLogo"] = "/uploads/system/" + fileName;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Lỗi khi tải ảnh: " + ex.Message);
            }
        }

        foreach (var setting in settings)
        {
            await _settingService.UpdateSettingAsync(setting.Key, setting.Value);
        }

        TempData["SuccessMessage"] = "Cấu hình hệ thống đã được cập nhật thành công.";
        return RedirectToPage();
    }
}

