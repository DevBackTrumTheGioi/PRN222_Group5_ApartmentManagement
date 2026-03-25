using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Pages.Visitors;

[Authorize(Policy = "ResidentOnly")]
public class RegisterModel : PageModel
{
    private readonly ApartmentDbContext _context;

    public RegisterModel(ApartmentDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên khách.")]
        [Display(Name = "Tên khách")]
        public string VisitorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập CMND/CCCD của khách.")]
        [Display(Name = "CMND/CCCD")]
        public string? IdentityCard { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày đến")]
        public DateTime VisitDate { get; set; } = DateTime.Today;

        [Display(Name = "Giờ đến (ước tính) - tuỳ chọn")]
        [DataType(DataType.Time)]
        public string? ExpectedArrivalTime { get; set; }

        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }
    }

    public async Task OnGetAsync()
    {
       
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Prevent booking visit in the past
        if (Input.VisitDate.Date < DateTime.Today)
        {
            ModelState.AddModelError("Input.VisitDate", "Ngày đến phải là hôm nay hoặc ngày trong tương lai.");
            return Page();
        }

        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username)) return Forbid();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return Forbid();

        if (!user.ApartmentId.HasValue)
        {
            ModelState.AddModelError(string.Empty, "Bạn cần liên kết căn hộ để đăng ký khách.");
            return Page();
        }

        DateTime? expectedDateTime = null;

        if (!string.IsNullOrEmpty(Input.ExpectedArrivalTime))
        {
            if (TimeSpan.TryParse(Input.ExpectedArrivalTime, out var ts))
            {
                expectedDateTime = Input.VisitDate.Date.Add(ts);
            }
        }

        var visitor = new Visitor
        {
            VisitorName = Input.VisitorName,
            PhoneNumber = Input.PhoneNumber,
            IdentityCard = Input.IdentityCard,
            ApartmentId = user.ApartmentId.Value,
            RegisteredBy = user.UserId,
            VisitDate = Input.VisitDate.Date,
            Notes = Input.Notes,
            Status = PRN222_ApartmentManagement.Models.Enums.VisitorStatus.Pending,
            CreatedAt = DateTime.Now,

            CheckInTime = expectedDateTime
        };

        _context.Visitors.Add(visitor);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Đăng ký khách thành công.";
        return RedirectToPage("MyVisitors");
    }
}
