using PRN222_ApartmentManagement.Models.DTOs;

namespace PRN222_ApartmentManagement.Services.Interfaces;

public interface IOperationalReportService
{
    Task<OperationalReportDto> GetOperationalReportAsync(OperationalReportFilterDto filter);
}
