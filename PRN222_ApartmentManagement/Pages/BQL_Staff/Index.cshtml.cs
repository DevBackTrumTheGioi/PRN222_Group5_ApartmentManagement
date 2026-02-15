using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace PRN222_ApartmentManagement.Pages.BQL_Staff;

[Authorize(Roles = "BQL_Staff")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}

