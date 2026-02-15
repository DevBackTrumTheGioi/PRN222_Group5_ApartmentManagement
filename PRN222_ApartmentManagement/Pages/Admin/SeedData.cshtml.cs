using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.Admin;

public class SeedDataModel : PageModel
{
    private readonly IConfiguration _configuration;

    public SeedDataModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string? Message { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("Connection string not found.");
            
            // await DataSeeder.SeedAsync(connectionString);
            Message = "Dữ liệu mẫu đã được khởi tạo thành công!";
        }
        catch (Exception ex)
        {
            Message = $"Lỗi: {ex.Message}";
        }

        return Page();
    }
}
