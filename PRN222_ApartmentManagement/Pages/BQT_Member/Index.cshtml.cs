using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace PRN222_ApartmentManagement.Pages.BQT_Member;

[Authorize(Roles = "BQT_Member")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}

