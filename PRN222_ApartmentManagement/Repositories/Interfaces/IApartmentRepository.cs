namespace PRN222_ApartmentManagement.Web.Repositories.Interfaces;

using System.Threading.Tasks;
using System.Collections.Generic;
using PRN222_ApartmentManagement.Web.Models;

public interface IApartmentRepository
{
    Task DeleteAsync(int id);
    Task UpdateAsync(Apartment apartment);
    Task AddAsync(Apartment apartment);
    Task<Apartment?> GetByIdAsync(int id);
    Task<IEnumerable<Apartment>> GetAllAsync();
}