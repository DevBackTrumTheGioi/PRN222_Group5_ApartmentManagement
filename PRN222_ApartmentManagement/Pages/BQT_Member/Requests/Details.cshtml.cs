using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.BQT_Member.Requests;

[Authorize(Roles = "BQT_Member")]
public class DetailsModel : PageModel
{
    private readonly IRequestService _requestService;

    public DetailsModel(IRequestService requestService)
    {
        _requestService = requestService;
    }

    public Request Request { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        return await LoadRequestAsync(id);
    }

    public async Task<IActionResult> OnPostUpdatePriorityAsync(int id, RequestPriority newPriority)
    {
        var request = await _requestService.GetRequestDetailAsync(id);
        if (request == null)
        {
            return NotFound();
        }

        if (request.RequestType == RequestType.Complaint)
        {
            TempData["ErrorMessage"] = "Khiếu nại được theo dõi ở trang riêng.";
            return RedirectToPage("/BQT_Member/Complaints/Index");
        }

        if (request.Status is not (RequestStatus.Pending or RequestStatus.InProgress))
        {
            TempData["ErrorMessage"] = "Chỉ có thể gán ưu tiên cho yêu cầu còn tồn đọng.";
            return RedirectToPage(new { id });
        }

        if (!Enum.IsDefined(typeof(RequestPriority), newPriority))
        {
            TempData["ErrorMessage"] = "Mức độ ưu tiên không hợp lệ.";
            return RedirectToPage(new { id });
        }

        try
        {
            await _requestService.UpdatePriorityAsync(id, newPriority);
            TempData["SuccessMessage"] = "Đã cập nhật mức độ ưu tiên.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }

        return RedirectToPage(new { id });
    }

    private async Task<IActionResult> LoadRequestAsync(int id)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        var request = await _requestService.GetRequestDetailAsync(id);
        if (request == null)
        {
            return NotFound();
        }

        if (request.RequestType == RequestType.Complaint)
        {
            TempData["ErrorMessage"] = "Khiếu nại được theo dõi ở trang riêng.";
            return RedirectToPage("/BQT_Member/Complaints/Index");
        }

        if (request.Status is not (RequestStatus.Pending or RequestStatus.InProgress))
        {
            TempData["ErrorMessage"] = "Yêu cầu này không còn nằm trong danh sách tồn đọng.";
            return RedirectToPage("Index");
        }

        Request = request;
        return Page();
    }
}
