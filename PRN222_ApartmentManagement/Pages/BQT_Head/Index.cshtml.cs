using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace PRN222_ApartmentManagement.Pages.BQT_Head;

[Authorize(Roles = "BQT_Head")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}

