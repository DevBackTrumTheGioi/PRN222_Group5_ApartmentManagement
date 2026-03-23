using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Repositories.Interfaces;
public interface IContractRepository : IGenericRepository<Contract>
{
    Task<PagedResult<Contract>> GetPagedFilteredAsync(
        string? searchTerm,
        ContractStatus? status,
        ContractType? contractType,
        int pageIndex,
        int pageSize);
}




