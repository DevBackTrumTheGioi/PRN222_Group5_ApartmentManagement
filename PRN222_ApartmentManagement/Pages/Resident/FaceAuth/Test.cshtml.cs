using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Resident.FaceAuth;

[AllowAnonymous]
[IgnoreAntiforgeryToken]
public class TestModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public TestModel(ApartmentDbContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
        // Default GET handler
    }

    public async Task<JsonResult> OnGetRegisteredFacesAsync()
    {
        var faces = await _context.Users
            .Where(r => r.IsFaceRegistered && r.FaceDescriptor != null && r.Role == UserRole.Resident)
            .Select(r => new { name = r.FullName, descriptor = r.FaceDescriptor })
            .ToListAsync();

        return new JsonResult(faces);
    }

    public async Task<IActionResult> OnPostLogAuthAsync([FromBody] AuthLogRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(r => r.FullName == request.Name && r.Role == UserRole.Resident);
        if (user != null)
        {
            var history = new FaceAuthHistory
            {
                ResidentId = user.UserId,
                AuthTime = DateTime.Now,
                IsSuccess = request.Success,
                ConfidenceScore = request.Score,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                DeviceInfo = Request.Headers["User-Agent"]
            };

            _context.FaceAuthHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        return new JsonResult(new { success = true });
    }

    public class AuthLogRequest
    {
        public string Name { get; set; } = string.Empty;
        public double Score { get; set; }
        public bool Success { get; set; }
    }
}
