using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IInvoiceManagementService
{
    Task<(bool Success, string Message, int CreatedCount)> GenerateInvoicesAsync(int billingMonth, int billingYear, int dueDay, int createdByUserId);
    Task<List<Invoice>> GetInvoicesAsync(int? billingMonth, int? billingYear, InvoiceStatus? status, InvoiceApprovalStatus? approvalStatus);
    Task<List<Invoice>> GetResidentInvoicesAsync(int residentUserId, int? billingMonth, int? billingYear);
    Task<Invoice?> GetInvoiceForManagementAsync(int invoiceId);
    Task<Invoice?> GetInvoiceForResidentAsync(int invoiceId, int residentUserId);
    Task<(bool Success, string Message)> ApproveInvoiceAsync(int invoiceId, int approverUserId);
    Task<(bool Success, string Message)> SendInvoiceAsync(int invoiceId);
}
