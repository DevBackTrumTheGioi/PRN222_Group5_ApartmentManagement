using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Repositories.Interfaces;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

public class RequestRepository : GenericRepository<Request>, IRequestRepository
{
    public RequestRepository(ApartmentDbContext context) : base(context)
    {
    }
}

