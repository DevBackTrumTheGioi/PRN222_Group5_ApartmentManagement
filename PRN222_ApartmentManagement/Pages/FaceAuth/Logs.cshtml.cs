using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Services.Interfaces;

namespace PRN222_ApartmentManagement.Pages.FaceAuth;

[Authorize(Roles = "BQL_Staff,BQL_Manager")]
public class LogsModel : PageModel
{
    private readonly IFaceAuthService _faceAuthService;

    public LogsModel(IFaceAuthService faceAuthService)
    {
        _faceAuthService = faceAuthService;
    }

    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsSuccess { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    public List<FaceAuthLogDto> Logs { get; set; } = new();

    public async Task OnGetAsync()
    {
        Logs = await _faceAuthService.GetManagementLogsAsync(SearchQuery, IsSuccess, FromDate, ToDate);
    }
}
