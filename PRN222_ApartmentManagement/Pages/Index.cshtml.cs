using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN222_ApartmentManagement.Web.Models;
using PRN222_ApartmentManagement.Web.Repositories.Interfaces;

namespace PRN222_ApartmentManagement.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IApartmentRepository _apartmentRepository;

        public IEnumerable<Apartment> Apartments { get; set; } = new List<Apartment>();

        public IndexModel(IApartmentRepository apartmentRepository)
        {
            _apartmentRepository = apartmentRepository;
        }

        public async Task OnGetAsync()
        {
            Apartments = await _apartmentRepository.GetAllAsync();
        }
    }
}
