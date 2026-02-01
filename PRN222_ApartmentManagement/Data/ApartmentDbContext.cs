using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Web.Models;

namespace PRN222_ApartmentManagement.Web.Data
{
    public class ApartmentDbContext : DbContext
    {
        public ApartmentDbContext(DbContextOptions<ApartmentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Apartment> Apartments { get; set; }
    }
}

