using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Repositories.Interfaces;
using PRN222_ApartmentManagement.Services;

namespace PRN222_ApartmentManagement.Repositories.Implementations;

public class AmenityBookingRepository : GenericRepository<AmenityBooking>, IAmenityBookingRepository
{
    public AmenityBookingRepository(ApartmentDbContext context, IActivityLogService activityLog) : base(context, activityLog)
    {
    }
}

