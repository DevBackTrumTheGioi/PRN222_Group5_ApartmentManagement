using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.DTOs;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IContractService
{
    Task<List<Contract>> GetAllAsync();
    Task<PagedResult<Contract>> GetPagedFilteredAsync(
        string? searchTerm,
        ContractStatus? status,
        ContractType? contractType,
        int pageIndex,
        int pageSize);
    Task<Contract?> GetByIdAsync(int id);
    Task<Contract?> GetByIdWithDetailsAsync(int id);
    Task<Contract> CreateContractAsync(CreateContractDto dto, int creatorId);
    Task<Contract?> ApproveContractAsync(int contractId, int approvedBy);
    Task<Contract?> UpdateContractAsync(int id, UpdateContractDto dto);
    Task<(bool Success, string Message)> TerminateContractAsync(int id, string reason, int terminatedBy);
    Task<(bool Success, string Message)> DeleteContractAsync(int id, int deletedBy);
    Task<string> GenerateContractNumberAsync(ContractType type);
    Task<User?> GetContractOwnerAsync(int contractId);
}
