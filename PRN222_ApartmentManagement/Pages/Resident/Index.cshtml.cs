using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace PRN222_ApartmentManagement.Pages.Resident;

[Authorize(Roles = "Resident")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}

