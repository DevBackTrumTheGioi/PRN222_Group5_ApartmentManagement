using Microsoft.EntityFrameworkCore;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;

namespace PRN222_ApartmentManagement.Utils;

public class DataSeeder
{
    public static async Task SeedAsync(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApartmentDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        using var context = new ApartmentDbContext(optionsBuilder.Options);
        
        Console.WriteLine("Checking database connection...");
        if (!await context.Database.CanConnectAsync())
        {
            Console.WriteLine("Cannot connect to database. Please check your connection string.");
            return;
        }

        Console.WriteLine("Seeding users...");
        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");

        var roleFullNames = new Dictionary<UserRole, string>
        {
            { UserRole.Admin, "Quản trị viên hệ thống" },
            { UserRole.BQL_Manager, "Trưởng ban quản lý" },
            { UserRole.BQL_Staff, "Nhân viên ban quản lý" },
            { UserRole.BQT_Head, "Trưởng ban quản trị" },
            { UserRole.BQT_Member, "Thành viên ban quản trị" },
            { UserRole.Resident, "Cư dân Nguyễn Văn A" }
        };

        var roles = Enum.GetValues<UserRole>();
        foreach (var role in roles)
        {
            var username = role.ToString().ToLower();
            if (!await context.Users.AnyAsync(u => u.Username == username))
            {
                var fullName = roleFullNames.ContainsKey(role) ? roleFullNames[role] : $"Người dùng {role}";
                var user = new User
                {
                    Username = username,
                    PasswordHash = passwordHash,
                    FullName = fullName,
                    Email = $"{username}@apartment.com",
                    Role = role,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                context.Users.Add(user);
                Console.WriteLine($"Added user: {username} with role {role}");
            }
        }

        Console.WriteLine("Seeding service types...");
        var serviceTypes = new[]
        {
            new ServiceType { ServiceTypeName = "Tiền điện", MeasurementUnit = "kWh", IsActive = true },
            new ServiceType { ServiceTypeName = "Tiền nước", MeasurementUnit = "m³", IsActive = true },
            new ServiceType { ServiceTypeName = "Phí quản lý", MeasurementUnit = "tháng", IsActive = true },
            new ServiceType { ServiceTypeName = "Phí gửi xe", MeasurementUnit = "xe", IsActive = true },
            new ServiceType { ServiceTypeName = "Tiền Internet", MeasurementUnit = "tháng", IsActive = true }
        };

        foreach (var st in serviceTypes)
        {
            if (!await context.ServiceTypes.AnyAsync(s => s.ServiceTypeName == st.ServiceTypeName))
            {
                context.ServiceTypes.Add(st);
                Console.WriteLine($"Added service type: {st.ServiceTypeName}");
            }
        }

        await context.SaveChangesAsync();
        Console.WriteLine("Seeding completed successfully!");
    }
}

