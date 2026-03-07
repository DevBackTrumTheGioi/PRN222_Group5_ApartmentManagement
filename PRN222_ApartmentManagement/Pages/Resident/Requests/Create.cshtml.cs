using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services.Interfaces;
using PRN222_ApartmentManagement.Utils;

namespace PRN222_ApartmentManagement.Pages.Resident.Requests;

[Authorize(Roles = "Resident")]
public class CreateModel : PageModel
{
    private readonly IRequestService _requestService;
    private readonly IUserRepository _userRepository;

    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg", "image/png", "image/gif", "image/webp",
        "application/pdf"
    ];
    private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    public CreateModel(IRequestService requestService, IUserRepository userRepository)
    {
        _requestService = requestService;
        _userRepository = userRepository;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty]
    public List<IFormFile>? Attachments { get; set; }

    public SelectList RequestTypeOptions { get; set; } = null!;

    public class InputModel
    {
        [Required(ErrorMessage = "Vui lòng nh?p tiêu ??.")]
        [MaxLength(200, ErrorMessage = "Tiêu ?? không v??t quá 200 ký t?.")]
        [Display(Name = "Tiêu ??")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Lo?i yêu c?u")]
        public RequestType? RequestType { get; set; }

        [Display(Name = "Mô t? chi ti?t")]
        public string? Description { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null || user.ApartmentId == null)
        {
            TempData["ErrorMessage"] = "B?n ch?a ???c gán c?n h?. Vui lòng liên h? Ban Qu?n Lý.";
            return RedirectToPage("/Resident/Index");
        }

        LoadSelectLists();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null || user.ApartmentId == null)
        {
            TempData["ErrorMessage"] = "B?n ch?a ???c gán c?n h?. Vui lòng liên h? Ban Qu?n Lý.";
            return RedirectToPage("/Resident/Index");
        }

        ValidateAttachments();

        if (!ModelState.IsValid)
        {
            LoadSelectLists();
            return Page();
        }

        var request = new Request
        {
            ApartmentId = user.ApartmentId.Value,
            ResidentId = userId,
            Title = Input.Title,
            RequestType = Input.RequestType,
            Priority = RequestPriority.Normal,
            Description = Input.Description,
        };

        try
        {
            await _requestService.CreateRequestAsync(request, Attachments);
            TempData["SuccessMessage"] = $"Yêu c?u #{request.RequestNumber} ?ã ???c g?i thành công.";
            return RedirectToPage("MyRequests");
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "?ã x?y ra l?i khi g?i yêu c?u. Vui lòng th? l?i.");
            LoadSelectLists();
            return Page();
        }
    }

    private void ValidateAttachments()
    {
        if (Attachments == null || Attachments.Count == 0) return;

        if (Attachments.Count > 5)
        {
            ModelState.AddModelError(nameof(Attachments), "T?i ?a 5 t?p ?ính kèm.");
            return;
        }

        foreach (var file in Attachments)
        {
            if (file.Length > MaxFileSizeBytes)
            {
                ModelState.AddModelError(nameof(Attachments),
                    $"T?p '{file.FileName}' v??t quá dung l??ng t?i ?a 5MB.");
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                ModelState.AddModelError(nameof(Attachments),
                    $"T?p '{file.FileName}' không h?p l?. Ch? ch?p nh?n ?nh (JPG, PNG, GIF, WEBP) và PDF.");
            }
        }
    }

    private void LoadSelectLists()
    {
        RequestTypeOptions = new SelectList(
            Enum.GetValues<RequestType>().Select(e => new
            {
                Value = e.ToString(),
                Text = e.GetDisplayName()
            }),
            "Value", "Text", Input.RequestType?.ToString());
    }
}
