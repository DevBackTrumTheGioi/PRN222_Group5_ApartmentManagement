using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace PRN222_ApartmentManagement.Pages.BQL_Manager;

[Authorize(Roles = "BQL_Manager")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}

