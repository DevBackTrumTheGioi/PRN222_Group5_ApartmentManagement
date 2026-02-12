using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

public class MeterReadingRepository : GenericRepository<MeterReading>, IMeterReadingRepository
{
    public MeterReadingRepository(ApartmentDbContext context, IActivityLogService activityLog) : base(context, activityLog)
    {
    }
}

