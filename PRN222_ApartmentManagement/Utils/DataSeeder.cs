using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PRN222_ApartmentManagement.Data;
using PRN222_ApartmentManagement.Models;
using PRN222_ApartmentManagement.Models.Enums;

namespace PRN222_ApartmentManagement.Utils;

public static class DataSeeder
{
    public static async Task SeedAsync(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApartmentDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        await using var context = new ApartmentDbContext(optionsBuilder.Options);

        if (!await context.Database.CanConnectAsync())
        {
            throw new InvalidOperationException("Không thể kết nối đến cơ sở dữ liệu.");
        }

        await SeedAsync(context);
    }

    public static async Task ResetAndSeedAsync(string connectionString)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApartmentDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        await using (var resetContext = new ApartmentDbContext(optionsBuilder.Options))
        {
            if (await resetContext.Database.CanConnectAsync())
            {
                await resetContext.Database.EnsureDeletedAsync();
            }

            await resetContext.Database.MigrateAsync();
        }

        await using var context = new ApartmentDbContext(optionsBuilder.Options);
        await SeedAsync(context);
    }

    public static async Task SeedAsync(ApartmentDbContext context, ILogger? logger = null)
    {
        var now = DateTime.Now;
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");

        logger?.LogInformation("Bắt đầu seed dữ liệu mẫu thực tế.");

        await EnsureSystemSettingsAsync(context, now);

        var apartments = await EnsureApartmentsAsync(context, now);
        var apartmentByNumber = apartments.ToDictionary(a => a.ApartmentNumber, StringComparer.OrdinalIgnoreCase);

        var users = await EnsureUsersAsync(context, apartmentByNumber, passwordHash, now);
        var adminUser = users.First(u => u.Role == UserRole.Admin);
        var managerUsers = users.Where(u => u.Role == UserRole.BQL_Manager).OrderBy(u => u.UserId).ToList();
        var staffUsers = users.Where(u => u.Role == UserRole.BQL_Staff).OrderBy(u => u.UserId).ToList();
        var residentUsers = users
            .Where(u => u.ApartmentId.HasValue && u.Role is UserRole.Resident or UserRole.BQT_Head or UserRole.BQT_Member)
            .OrderBy(u => u.UserId)
            .ToList();

        var serviceTypes = await EnsureServiceCatalogAsync(context, now);
        var serviceTypeByName = serviceTypes.ToDictionary(s => s.ServiceTypeName, StringComparer.OrdinalIgnoreCase);
        var servicePrices = await context.ServicePrices
            .Include(sp => sp.ServiceType)
            .OrderBy(sp => sp.ServiceTypeId)
            .ThenByDescending(sp => sp.EffectiveFrom)
            .ToListAsync();

        var amenities = await EnsureAmenityCatalogAsync(context, now);
        var amenityByName = amenities.ToDictionary(a => a.AmenityName, StringComparer.OrdinalIgnoreCase);

        var contracts = await EnsureContractsAsync(context, residentUsers, apartmentByNumber, adminUser.UserId, now);
        await EnsureResidentApartmentLinksAsync(context, contracts, residentUsers, now);
        await EnsureResidentCardsAsync(context, residentUsers, now);
        await EnsureVehiclesAsync(context, residentUsers, now);
        await EnsureApartmentServicesAsync(context, apartments, residentUsers, serviceTypeByName, now);
        await EnsureAmenityBookingsAsync(context, residentUsers, amenityByName, now);
        await EnsureServiceOrdersAsync(context, residentUsers, staffUsers, serviceTypeByName, now);
        await EnsureRequestsAsync(context, residentUsers, staffUsers, managerUsers, now);
        await EnsureVisitorsAsync(context, residentUsers, now);
        await EnsureAnnouncementsAsync(context, adminUser.UserId, now);
        await EnsureDocumentsAsync(context, adminUser.UserId, now);
        await EnsureInvoicesAsync(context, adminUser.UserId, apartments, residentUsers, serviceTypeByName, servicePrices, now);
        await EnsureNotificationsAsync(context, residentUsers, now);
        await EnsureAnnouncementAssetsAsync(context, residentUsers, now);
        await EnsureFaceAuthAsync(context, residentUsers, now);
        await EnsureRefreshTokensAsync(context, users, now);
        await EnsureActivityLogsAsync(context, users, now);

        logger?.LogInformation("Hoàn tất seed dữ liệu mẫu thực tế.");
    }

    private static async Task EnsureSystemSettingsAsync(ApartmentDbContext context, DateTime now)
    {
        var settings = new[]
        {
            new SystemSetting { SettingKey = "ApartmentName", SettingValue = "Sunrise Riverside", Description = "Tên chung cư hiển thị trên hệ thống", UpdatedAt = now },
            new SystemSetting { SettingKey = "ApartmentLogo", SettingValue = "apartment", Description = "Biểu tượng hiển thị ở sidebar", UpdatedAt = now },
            new SystemSetting { SettingKey = "ApartmentAddress", SettingValue = "88 Nguyễn Hữu Thọ, Phường Tân Hưng, Quận 7, TP. Hồ Chí Minh", Description = "Địa chỉ chung cư", UpdatedAt = now },
            new SystemSetting { SettingKey = "ApartmentContact", SettingValue = "028 3888 6688", Description = "Số điện thoại lễ tân và ban quản lý", UpdatedAt = now }
        };

        foreach (var setting in settings)
        {
            var existing = await context.SystemSettings.FirstOrDefaultAsync(s => s.SettingKey == setting.SettingKey);
            if (existing == null)
            {
                context.SystemSettings.Add(setting);
                continue;
            }

            existing.SettingValue = setting.SettingValue;
            existing.Description = setting.Description;
            existing.UpdatedAt = now;
        }

        await context.SaveChangesAsync();
    }

    private static async Task<List<Apartment>> EnsureApartmentsAsync(ApartmentDbContext context, DateTime now)
    {
        if (!await context.Apartments.AnyAsync())
        {
            var apartments = new List<Apartment>();
            var apartmentTypes = new[] { ("1PN", 48m), ("2PN", 68m), ("2PN+", 78m), ("3PN", 92m) };

            foreach (var block in new[] { "A", "B" })
            {
                for (int floor = 1; floor <= 10; floor++)
                {
                    for (int unit = 1; unit <= 4; unit++)
                    {
                        var apartmentNumber = $"{block}-{floor:D2}{unit:D2}";
                        var typeSpec = apartmentTypes[(floor + unit) % apartmentTypes.Length];
                        var status = floor switch
                        {
                            <= 2 => unit % 2 == 0 ? ApartmentStatus.Reserved : ApartmentStatus.Available,
                            <= 7 => ApartmentStatus.Occupied,
                            8 => unit == 4 ? ApartmentStatus.Maintenance : ApartmentStatus.Occupied,
                            _ => unit == 1 ? ApartmentStatus.Reserved : ApartmentStatus.Available
                        };

                        apartments.Add(new Apartment
                        {
                            ApartmentNumber = apartmentNumber,
                            Floor = floor,
                            BuildingBlock = block,
                            Area = typeSpec.Item2 + floor,
                            ApartmentType = typeSpec.Item1,
                            Status = status,
                            Description = $"Căn hộ {typeSpec.Item1} tại block {block}, tầng {floor}, phù hợp cho hộ gia đình 2-5 người.",
                            CreatedAt = now.AddMonths(-6).AddDays(-(floor * 2 + unit))
                        });
                    }
                }
            }

            context.Apartments.AddRange(apartments);
            await context.SaveChangesAsync();
        }

        return await context.Apartments
            .OrderBy(a => a.BuildingBlock)
            .ThenBy(a => a.Floor)
            .ThenBy(a => a.ApartmentNumber)
            .ToListAsync();
    }

    private static async Task<List<User>> EnsureUsersAsync(
        ApartmentDbContext context,
        IReadOnlyDictionary<string, Apartment> apartmentByNumber,
        string passwordHash,
        DateTime now)
    {
        var seeds = new[]
        {
            new UserSeed("admin", "Nguyễn Quốc Anh", "admin@sunriseriverside.vn", "0901000001", UserRole.Admin, null, null, null, new DateTime(1987, 4, 12), 0, false, "Quản trị hệ thống"),
            new UserSeed("manager", "Trần Minh Tuấn", "manager@sunriseriverside.vn", "0901000002", UserRole.BQL_Manager, null, null, null, new DateTime(1984, 9, 8), 0, false, "Quản lý vận hành"),
            new UserSeed("staff", "Lê Thị Hồng Nhung", "staff1@sunriseriverside.vn", "0901000003", UserRole.BQL_Staff, null, null, null, new DateTime(1992, 3, 25), 0, false, "Nhân sự lễ tân ca sáng"),
            new UserSeed("staff2", "Phạm Văn Đức", "staff2@sunriseriverside.vn", "0901000004", UserRole.BQL_Staff, null, null, null, new DateTime(1991, 10, 11), 0, false, "Nhân sự kỹ thuật"),
            new UserSeed("staff3", "Võ Thị Mai Lan", "staff3@sunriseriverside.vn", "0901000005", UserRole.BQL_Staff, null, null, null, new DateTime(1994, 7, 2), 0, false, "Nhân sự lễ tân ca tối"),
            new UserSeed("resident", "Nguyễn Hữu Phước", "resident1@sunriseriverside.vn", "0902111111", UserRole.Resident, "A-0301", ResidentType.Owner, "079085000101", new DateTime(1986, 2, 18), 30, true, "Chủ hộ căn A-0301"),
            new UserSeed("resident2", "Lê Thị Bảo Trâm", "resident2@sunriseriverside.vn", "0902111112", UserRole.Resident, "A-0301", ResidentType.FamilyMember, "079092000102", new DateTime(1989, 5, 30), 30, false, "Thành viên gia đình"),
            new UserSeed("resident3", "Phan Minh Khang", "resident3@sunriseriverside.vn", "0902111113", UserRole.Resident, "A-0302", ResidentType.Owner, "079083000103", new DateTime(1983, 8, 19), 28, true, "Chủ hộ căn A-0302"),
            new UserSeed("resident4", "Trần Ngọc Hà", "resident4@sunriseriverside.vn", "0902111114", UserRole.Resident, "A-0401", ResidentType.Tenant, "079094000104", new DateTime(1994, 6, 14), 14, false, "Người thuê căn A-0401"),
            new UserSeed("resident5", "Đặng Gia Hân", "resident5@sunriseriverside.vn", "0902111115", UserRole.Resident, "A-0401", ResidentType.FamilyMember, "079096000105", new DateTime(1996, 1, 22), 14, false, "Người ở cùng hợp đồng thuê"),
            new UserSeed("resident6", "Bùi Quang Khải", "resident6@sunriseriverside.vn", "0902111116", UserRole.Resident, "A-0402", ResidentType.Owner, "079081000106", new DateTime(1981, 12, 3), 40, true, "Chủ hộ căn A-0402"),
            new UserSeed("resident7", "Ngô Thuỳ Dương", "resident7@sunriseriverside.vn", "0902111117", UserRole.Resident, "A-0501", ResidentType.Owner, "079090000107", new DateTime(1990, 9, 9), 22, true, "Chủ hộ căn A-0501"),
            new UserSeed("resident8", "Ngô Gia Bảo", "resident8@sunriseriverside.vn", "0902111118", UserRole.Resident, "A-0501", ResidentType.FamilyMember, "079098000108", new DateTime(2012, 11, 15), 22, false, "Con của chủ hộ"),
            new UserSeed("resident9", "Cao Anh Tuấn", "resident9@sunriseriverside.vn", "0902111119", UserRole.Resident, "B-0301", ResidentType.Owner, "079087000109", new DateTime(1987, 7, 6), 34, true, "Chủ hộ căn B-0301"),
            new UserSeed("resident10", "Hoàng Kim Oanh", "resident10@sunriseriverside.vn", "0902111120", UserRole.Resident, "B-0302", ResidentType.Tenant, "079095000110", new DateTime(1995, 4, 1), 10, false, "Người thuê dài hạn"),
            new UserSeed("resident11", "Dương Thành Đạt", "resident11@sunriseriverside.vn", "0902111121", UserRole.Resident, "B-0302", ResidentType.FamilyMember, "079099000111", new DateTime(1998, 2, 27), 10, false, "Người ở cùng"),
            new UserSeed("resident12", "Lý Thanh Bình", "resident12@sunriseriverside.vn", "0902111122", UserRole.Resident, "B-0401", ResidentType.Owner, "079084000112", new DateTime(1984, 3, 8), 26, true, "Chủ hộ căn B-0401"),
            new UserSeed("resident13", "Vũ Nhật Nam", "resident13@sunriseriverside.vn", "0902111123", UserRole.Resident, "B-0402", ResidentType.Owner, "079088000113", new DateTime(1988, 10, 16), 18, false, "Chủ hộ căn B-0402"),
            new UserSeed("resident14", "Phạm Thuỳ Linh", "resident14@sunriseriverside.vn", "0902111124", UserRole.Resident, "B-0501", ResidentType.Owner, "079093000114", new DateTime(1993, 12, 24), 24, true, "Chủ hộ căn B-0501"),
            new UserSeed("bqt_head", "Lưu Hoàng Sơn", "bqt.head@sunriseriverside.vn", "0902111125", UserRole.BQT_Head, "A-0601", ResidentType.Owner, "079080000115", new DateTime(1980, 5, 17), 36, true, "Trưởng ban quản trị"),
            new UserSeed("bqt_member", "Đặng Minh Ngọc", "bqt.member@sunriseriverside.vn", "0902111126", UserRole.BQT_Member, "B-0601", ResidentType.Owner, "079082000116", new DateTime(1982, 8, 21), 20, false, "Thành viên ban quản trị")
        };

        foreach (var seed in seeds)
        {
            var apartmentId = seed.ApartmentNumber != null
                ? apartmentByNumber[seed.ApartmentNumber].ApartmentId
                : (int?)null;

            var existing = await context.Users.FirstOrDefaultAsync(u => u.Username == seed.Username);
            if (existing == null)
            {
                existing = new User
                {
                    Username = seed.Username,
                    CreatedAt = now.AddMonths(-(seed.MoveInMonthsAgo == 0 ? 6 : seed.MoveInMonthsAgo))
                };

                context.Users.Add(existing);
            }

            existing.PasswordHash = passwordHash;
            existing.FullName = seed.FullName;
            existing.Email = seed.Email;
            existing.PhoneNumber = seed.PhoneNumber;
            existing.Role = seed.Role;
            existing.IsActive = true;
            existing.IsDeleted = false;
            existing.DateOfBirth = seed.DateOfBirth;
            existing.IdentityCardNumber = seed.IdentityCardNumber;
            existing.ResidentType = seed.ResidentType;
            existing.ResidencyStatus = apartmentId.HasValue ? "Thường trú" : null;
            existing.ApartmentId = apartmentId;
            existing.MoveInDate = apartmentId.HasValue ? now.AddMonths(-Math.Max(seed.MoveInMonthsAgo, 1)) : null;
            existing.MoveOutDate = null;
            existing.Note = seed.Note;
            existing.LastLogin = now.AddDays(-(seed.MoveInMonthsAgo % 5 + 1));
            existing.UpdatedAt = now;
            existing.IsFaceRegistered = seed.HasFaceProfile;
            existing.FaceDescriptor = seed.HasFaceProfile ? GenerateFaceDescriptor(seed.Username) : null;
        }

        await context.SaveChangesAsync();

        return await context.Users
            .OrderBy(u => u.Role)
            .ThenBy(u => u.Username)
            .ToListAsync();
    }

    private static async Task<List<ServiceType>> EnsureServiceCatalogAsync(ApartmentDbContext context, DateTime now)
    {
        var currentEffectiveDate = new DateTime(now.Year, 1, 1);
        var previousEffectiveDate = currentEffectiveDate.AddYears(-1);

        var seeds = new[]
        {
            new ServiceSeed("Điện sinh hoạt", new[] { "Electricity" }, "kWh", "Dịch vụ định kỳ theo chỉ số điện tiêu thụ thực tế của căn hộ.", 3800m, 3500m, true),
            new ServiceSeed("Nước sinh hoạt", new[] { "Water" }, "m³", "Dịch vụ định kỳ theo chỉ số nước tiêu thụ thực tế của căn hộ.", 18000m, 16500m, true),
            new ServiceSeed("Phí quản lý", new[] { "Management Fee" }, "m²/tháng", "Dịch vụ định kỳ phục vụ vận hành, vệ sinh và an ninh toàn toà nhà.", 18500m, 17500m, true),
            new ServiceSeed("Phí vệ sinh rác", Array.Empty<string>(), "căn/tháng", "Dịch vụ định kỳ thu gom rác sinh hoạt tại từng căn hộ.", 45000m, null, true),
            new ServiceSeed("Phí gửi xe máy", new[] { "Parking Fee" }, "xe/tháng", "Dịch vụ định kỳ dành cho xe máy gửi tại hầm B1 và B2.", 170000m, 150000m, true),
            new ServiceSeed("Phí gửi ô tô", Array.Empty<string>(), "xe/tháng", "Dịch vụ định kỳ dành cho ô tô gửi tại hầm B2.", 1650000m, 1500000m, true),
            new ServiceSeed("Internet cáp quang", new[] { "Internet" }, "gói/tháng", "Gói Internet cố định 150Mbps do đối tác viễn thông triển khai trong toà nhà.", 240000m, 220000m, true),
            new ServiceSeed("Truyền hình cáp", Array.Empty<string>(), "gói/tháng", "Gói truyền hình mở rộng cho cư dân có nhu cầu đăng ký thêm.", 120000m, null, false),
            new ServiceSeed("Dọn vệ sinh căn hộ", Array.Empty<string>(), "lần", "Dịch vụ theo yêu cầu, nhân viên đến dọn vệ sinh căn hộ theo khung giờ cư dân đặt.", 320000m, null, true),
            new ServiceSeed("Giặt ủi", Array.Empty<string>(), "kg", "Dịch vụ theo yêu cầu, nhận đồ tại sảnh và giao lại trong ngày hoặc hôm sau.", 38000m, null, true),
            new ServiceSeed("Sửa chữa điện nước nhỏ", Array.Empty<string>(), "lần", "Dịch vụ theo yêu cầu dành cho các hạng mục sửa chữa nhỏ trong căn hộ.", 180000m, null, true),
            new ServiceSeed("Vệ sinh máy lạnh", Array.Empty<string>(), "máy/lần", "Dịch vụ theo yêu cầu dành cho vệ sinh dàn lạnh, kiểm tra ga cơ bản.", 250000m, null, true),
            new ServiceSeed("Hỗ trợ chuyển đồ nội khu", Array.Empty<string>(), "lần", "Dịch vụ theo yêu cầu hỗ trợ cư dân vận chuyển đồ đạc trong nội khu chung cư.", 450000m, null, true)
        };

        foreach (var seed in seeds)
        {
            var existing = await context.ServiceTypes.FirstOrDefaultAsync(st =>
                st.ServiceTypeName == seed.Name ||
                seed.LegacyNames.Contains(st.ServiceTypeName));

            if (existing == null)
            {
                existing = new ServiceType
                {
                    ServiceTypeName = seed.Name,
                    CreatedAt = now.AddMonths(-8)
                };

                context.ServiceTypes.Add(existing);
            }

            existing.ServiceTypeName = seed.Name;
            existing.MeasurementUnit = seed.MeasurementUnit;
            existing.Description = seed.Description;
            existing.IsActive = seed.IsActive;
            existing.IsDeleted = false;
            existing.UpdatedAt = now;
        }

        await context.SaveChangesAsync();

        var serviceTypes = await context.ServiceTypes.OrderBy(st => st.ServiceTypeName).ToListAsync();

        foreach (var seed in seeds)
        {
            var serviceType = serviceTypes.First(st => st.ServiceTypeName == seed.Name);

            if (!await context.ServicePrices.AnyAsync(sp => sp.ServiceTypeId == serviceType.ServiceTypeId && sp.EffectiveFrom == currentEffectiveDate))
            {
                context.ServicePrices.Add(new ServicePrice
                {
                    ServiceTypeId = serviceType.ServiceTypeId,
                    UnitPrice = seed.CurrentPrice,
                    EffectiveFrom = currentEffectiveDate,
                    Description = $"Bảng giá đang áp dụng cho {seed.Name}",
                    CreatedAt = now.AddMonths(-3)
                });
            }

            if (seed.PreviousPrice.HasValue &&
                !await context.ServicePrices.AnyAsync(sp => sp.ServiceTypeId == serviceType.ServiceTypeId && sp.EffectiveFrom == previousEffectiveDate))
            {
                context.ServicePrices.Add(new ServicePrice
                {
                    ServiceTypeId = serviceType.ServiceTypeId,
                    UnitPrice = seed.PreviousPrice.Value,
                    EffectiveFrom = previousEffectiveDate,
                    EffectiveTo = currentEffectiveDate.AddDays(-1),
                    Description = $"Bảng giá cũ của {seed.Name}",
                    CreatedAt = now.AddYears(-1)
                });
            }
        }

        await context.SaveChangesAsync();

        return await context.ServiceTypes
            .Include(st => st.ServicePrices)
            .OrderBy(st => st.ServiceTypeName)
            .ToListAsync();
    }

    private static async Task<List<Amenity>> EnsureAmenityCatalogAsync(ApartmentDbContext context, DateTime now)
    {
        var amenityTypeSeeds = new[]
        {
            ("Thể thao", "Nhóm tiện ích cố định phục vụ nhu cầu rèn luyện sức khoẻ hàng ngày."),
            ("Sinh hoạt cộng đồng", "Nhóm tiện ích cố định phục vụ họp mặt, sự kiện cư dân và sinh hoạt cộng đồng."),
            ("Làm việc", "Nhóm tiện ích cố định phục vụ họp nhóm, làm việc hoặc học tập."),
            ("Trẻ em", "Nhóm tiện ích cố định dành cho trẻ em và gia đình.")
        };

        foreach (var (typeName, description) in amenityTypeSeeds)
        {
            var existing = await context.AmenityTypes.FirstOrDefaultAsync(t => t.TypeName == typeName);
            if (existing == null)
            {
                context.AmenityTypes.Add(new AmenityType
                {
                    TypeName = typeName,
                    Description = description,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = now.AddMonths(-12)
                });
                continue;
            }

            existing.Description = description;
            existing.IsActive = true;
            existing.IsDeleted = false;
            existing.UpdatedAt = now;
        }

        await context.SaveChangesAsync();

        var amenityTypes = await context.AmenityTypes.ToDictionaryAsync(t => t.TypeName, StringComparer.OrdinalIgnoreCase);

        var amenitySeeds = new[]
        {
            new AmenitySeed("Phòng Gym", "Thể thao", "Tầng 3 block A", 35, 0m, false, new TimeSpan(5, 30, 0), new TimeSpan(22, 0, 0), 0, "Tiện ích cố định mở cửa hàng ngày, cư dân có thể sử dụng theo khung giờ mở cửa."),
            new AmenitySeed("Hồ bơi bốn mùa", "Thể thao", "Tầng 4 block A", 40, 0m, false, new TimeSpan(6, 0, 0), new TimeSpan(21, 30, 0), 0, "Tiện ích cố định có cứu hộ trực trong giờ cao điểm."),
            new AmenitySeed("Sân Tennis", "Thể thao", "Sân mái block B", 4, 180000m, true, new TimeSpan(6, 0, 0), new TimeSpan(22, 0, 0), 4, "Tiện ích cố định cần đặt lịch trước vì chỉ có 01 sân tiêu chuẩn."),
            new AmenitySeed("Phòng BBQ", "Sinh hoạt cộng đồng", "Vườn mái block A", 20, 550000m, true, new TimeSpan(9, 0, 0), new TimeSpan(22, 0, 0), 6, "Tiện ích cố định phục vụ liên hoan gia đình, sinh nhật và họp mặt cuối tuần."),
            new AmenitySeed("Phòng sinh hoạt cộng đồng", "Sinh hoạt cộng đồng", "Tầng 1 block A", 50, 250000m, true, new TimeSpan(7, 0, 0), new TimeSpan(21, 0, 0), 4, "Tiện ích cố định dùng cho họp dân cư, lớp học cuối tuần hoặc workshop."),
            new AmenitySeed("Phòng họp", "Làm việc", "Tầng 2 block B", 12, 220000m, true, new TimeSpan(8, 0, 0), new TimeSpan(21, 0, 0), 2, "Tiện ích cố định dành cho họp trực tuyến, phỏng vấn hoặc làm việc nhóm."),
            new AmenitySeed("Khu co-working", "Làm việc", "Tầng 2 block B", 18, 0m, false, new TimeSpan(7, 0, 0), new TimeSpan(22, 0, 0), 0, "Tiện ích cố định có wifi, bàn dài và máy in dùng chung."),
            new AmenitySeed("Khu vui chơi trẻ em", "Trẻ em", "Tầng 2 block A", 24, 0m, false, new TimeSpan(7, 0, 0), new TimeSpan(21, 0, 0), 0, "Tiện ích cố định trong nhà dành cho trẻ từ 3 đến 10 tuổi.")
        };

        foreach (var seed in amenitySeeds)
        {
            var existing = await context.Amenities.FirstOrDefaultAsync(a => a.AmenityName == seed.Name);
            if (existing == null)
            {
                existing = new Amenity
                {
                    AmenityName = seed.Name,
                    CreatedAt = now.AddMonths(-10)
                };

                context.Amenities.Add(existing);
            }

            existing.AmenityTypeId = amenityTypes[seed.TypeName].AmenityTypeId;
            existing.Location = seed.Location;
            existing.Capacity = seed.Capacity;
            existing.PricePerHour = seed.PricePerHour;
            existing.RequiresBooking = seed.RequiresBooking;
            existing.OpenTime = seed.OpenTime;
            existing.CloseTime = seed.CloseTime;
            existing.CancellationDeadlineHours = seed.CancellationDeadlineHours;
            existing.IsActive = true;
            existing.IsDeleted = false;
            existing.Description = seed.Description;
        }

        await context.SaveChangesAsync();

        return await context.Amenities
            .Include(a => a.AmenityType)
            .OrderBy(a => a.AmenityName)
            .ToListAsync();
    }

    private static async Task<List<Contract>> EnsureContractsAsync(
        ApartmentDbContext context,
        IReadOnlyList<User> residentUsers,
        IReadOnlyDictionary<string, Apartment> apartmentByNumber,
        int createdByUserId,
        DateTime now)
    {
        if (!await context.Contracts.AnyAsync())
        {
            var groupedResidents = residentUsers
                .Where(u => u.ApartmentId.HasValue)
                .GroupBy(u => u.ApartmentId!.Value)
                .OrderBy(g => g.Key)
                .ToList();

            var contracts = new List<Contract>();

            for (var index = 0; index < groupedResidents.Count; index++)
            {
                var group = groupedResidents[index];
                var primary = group.FirstOrDefault(u => u.ResidentType == ResidentType.Owner) ?? group.First();
                var apartment = apartmentByNumber.Values.First(a => a.ApartmentId == group.Key);
                var contractType = group.Any(u => u.ResidentType == ResidentType.Tenant) ? ContractType.Rental : ContractType.Purchase;
                var signedDate = primary.MoveInDate?.Date ?? now.AddMonths(-(index + 12)).Date;

                contracts.Add(new Contract
                {
                    ContractNumber = $"HD-{signedDate:yyyy}-{index + 1:D3}",
                    ApartmentId = apartment.ApartmentId,
                    ContractType = contractType,
                    StartDate = signedDate,
                    EndDate = contractType == ContractType.Rental ? signedDate.AddYears(1) : null,
                    MonthlyRent = contractType == ContractType.Rental ? 12000000m + (apartment.Area ?? 60m) * 95000m : null,
                    DepositAmount = contractType == ContractType.Rental ? 25000000m : null,
                    PurchasePrice = contractType == ContractType.Purchase ? (apartment.Area ?? 60m) * 49000000m : null,
                    Status = ContractStatus.Active,
                    Terms = "Hợp đồng mẫu được seed theo kịch bản vận hành thực tế tại chung cư Sunrise Riverside.",
                    ContractFile = $"/uploads/contracts/{apartment.ApartmentNumber.ToLowerInvariant().Replace("-", "")}.pdf",
                    SignedDate = signedDate,
                    CreatedBy = createdByUserId,
                    CreatedAt = signedDate,
                    OwnerFullName = primary.FullName,
                    OwnerEmail = primary.Email,
                    OwnerPhone = primary.PhoneNumber,
                    OwnerDateOfBirth = primary.DateOfBirth,
                    OwnerIdentityCard = primary.IdentityCardNumber
                });
            }

            var reservedApartment = apartmentByNumber["A-0202"];
            contracts.Add(new Contract
            {
                ContractNumber = $"HD-{now:yyyy}-999",
                ApartmentId = reservedApartment.ApartmentId,
                ContractType = ContractType.Purchase,
                StartDate = new DateTime(now.Year, now.Month, 1),
                Status = ContractStatus.PendingSignature,
                Terms = "Hợp đồng chờ khách hàng ký xác nhận và hoàn thiện thanh toán đợt 1.",
                CreatedBy = createdByUserId,
                CreatedAt = now.AddDays(-5),
                OwnerFullName = "Khách hàng mẫu Nguyễn Đức Long",
                OwnerEmail = "khachhang.long@example.com",
                OwnerPhone = "0903555777",
                OwnerDateOfBirth = new DateTime(1988, 6, 20),
                OwnerIdentityCard = "079088009999"
            });

            context.Contracts.AddRange(contracts);
            await context.SaveChangesAsync();

            var createdContracts = await context.Contracts
                .Where(c => c.Status == ContractStatus.Active)
                .ToListAsync();

            var contractMembers = new List<ContractMember>();

            foreach (var contract in createdContracts)
            {
                var members = residentUsers.Where(u => u.ApartmentId == contract.ApartmentId).OrderBy(u => u.UserId).ToList();
                var primary = members.FirstOrDefault(u => u.ResidentType == ResidentType.Owner) ?? members.First();

                foreach (var member in members)
                {
                    contractMembers.Add(new ContractMember
                    {
                        ContractId = contract.ContractId,
                        ResidentId = member.UserId,
                        MemberRole = member.UserId == primary.UserId ? MemberRole.ContractOwner : MapMemberRole(member.ResidentType),
                        SignatureStatus = SignatureStatus.Signed,
                        SignedDate = contract.SignedDate,
                        IsActive = true,
                        Notes = member.UserId == primary.UserId ? "Người đại diện ký hợp đồng." : "Thành viên cùng cư trú.",
                        CreatedAt = contract.CreatedAt
                    });
                }
            }

            context.ContractMembers.AddRange(contractMembers);
            await context.SaveChangesAsync();
        }

        return await context.Contracts
            .Include(c => c.ContractMembers)
            .OrderBy(c => c.ApartmentId)
            .ToListAsync();
    }

    private static async Task EnsureResidentApartmentLinksAsync(
        ApartmentDbContext context,
        IReadOnlyList<Contract> contracts,
        IReadOnlyList<User> residentUsers,
        DateTime now)
    {
        if (await context.ResidentApartments.AnyAsync())
        {
            return;
        }

        var contractById = contracts.ToDictionary(c => c.ContractId);
        var activeMembers = await context.ContractMembers.Where(cm => cm.IsActive).ToListAsync();

        var records = activeMembers.Select(member =>
        {
            var contract = contractById[member.ContractId];
            var resident = residentUsers.First(u => u.UserId == member.ResidentId);

            return new ResidentApartment
            {
                UserId = member.ResidentId,
                ApartmentId = contract.ApartmentId,
                ContractId = contract.ContractId,
                ResidencyType = MapResidencyType(resident.ResidentType),
                IsActive = contract.Status == ContractStatus.Active,
                MoveInDate = resident.MoveInDate ?? contract.StartDate,
                MoveOutDate = resident.MoveOutDate,
                CreatedAt = contract.CreatedAt,
                UpdatedAt = now
            };
        }).ToList();

        context.ResidentApartments.AddRange(records);
        await context.SaveChangesAsync();
    }

    private static async Task EnsureResidentCardsAsync(ApartmentDbContext context, IReadOnlyList<User> residentUsers, DateTime now)
    {
        if (await context.ResidentCards.AnyAsync())
        {
            return;
        }

        var cards = residentUsers.Select((resident, index) => new ResidentCard
        {
            CardNumber = $"CARD-{resident.ApartmentId:D4}-{index + 1:D3}",
            CardType = resident.ResidentType == ResidentType.Owner ? CardType.Resident : CardType.Secondary,
            ResidentId = resident.UserId,
            ApartmentId = resident.ApartmentId,
            IssuedDate = (resident.MoveInDate ?? now).Date,
            ExpiryDate = (resident.MoveInDate ?? now).Date.AddYears(5),
            Status = CardStatus.Active,
            Notes = resident.Role is UserRole.BQT_Head or UserRole.BQT_Member ? "Tài khoản cư dân kiêm vai trò ban quản trị." : null,
            CreatedAt = resident.CreatedAt
        }).ToList();

        context.ResidentCards.AddRange(cards);
        await context.SaveChangesAsync();
    }

    private static async Task EnsureVehiclesAsync(ApartmentDbContext context, IReadOnlyList<User> residentUsers, DateTime now)
    {
        if (await context.Vehicles.AnyAsync())
        {
            return;
        }

        var vehicleSeeds = new[]
        {
            ("resident", "Ô tô", "51H-128.66", "Mazda", "CX-5", "Trắng"),
            ("resident2", "Xe máy", "59C2-335.61", "Honda", "SH 160i", "Đen"),
            ("resident3", "Xe máy", "59C2-118.23", "Yamaha", "Grande", "Xanh"),
            ("resident4", "Xe máy", "59S2-909.20", "Honda", "Lead", "Đỏ"),
            ("resident6", "Ô tô", "51K-566.99", "Toyota", "Corolla Cross", "Bạc"),
            ("resident7", "Xe máy", "59X3-225.10", "Honda", "Vision", "Trắng"),
            ("resident9", "Ô tô", "51L-999.88", "Kia", "Carnival", "Xám"),
            ("resident10", "Xe máy", "59P1-668.79", "Yamaha", "Exciter", "Đen đỏ"),
            ("resident12", "Xe máy", "59H2-557.42", "Piaggio", "Liberty", "Bạc"),
            ("bqt_head", "Ô tô", "51A-777.09", "Mercedes", "C200", "Đen")
        };

        var userByUsername = residentUsers.ToDictionary(u => u.Username, StringComparer.OrdinalIgnoreCase);

        context.Vehicles.AddRange(vehicleSeeds.Select((seed, index) => new Vehicle
        {
            ResidentId = userByUsername[seed.Item1].UserId,
            VehicleType = seed.Item2,
            LicensePlate = seed.Item3,
            Brand = seed.Item4,
            Model = seed.Item5,
            Color = seed.Item6,
            RegisteredDate = now.AddYears(-Math.Min(index + 1, 5)).Date,
            Notes = seed.Item2 == "Ô tô" ? "Xe gửi cố định tại hầm B2." : "Xe gửi cố định tại hầm B1.",
            CreatedAt = now.AddMonths(-(index + 2))
        }));

        await context.SaveChangesAsync();
    }

    private static async Task EnsureApartmentServicesAsync(
        ApartmentDbContext context,
        IReadOnlyList<Apartment> apartments,
        IReadOnlyList<User> residentUsers,
        IReadOnlyDictionary<string, ServiceType> serviceTypeByName,
        DateTime now)
    {
        if (await context.ApartmentServices.AnyAsync())
        {
            return;
        }

        var contractApartmentIds = await context.Contracts
            .Where(c => c.Status == ContractStatus.Active)
            .Select(c => c.ApartmentId)
            .Distinct()
            .ToListAsync();

        var vehicles = await context.Vehicles.AsNoTracking().ToListAsync();
        var residentsByApartment = residentUsers
            .Where(u => u.ApartmentId.HasValue)
            .GroupBy(u => u.ApartmentId!.Value)
            .ToDictionary(g => g.Key, g => g.Select(u => u.UserId).ToHashSet());

        var services = new List<ApartmentService>();

        foreach (var apartment in apartments.Where(a => contractApartmentIds.Contains(a.ApartmentId)))
        {
            services.Add(new ApartmentService
            {
                ApartmentId = apartment.ApartmentId,
                ServiceTypeId = serviceTypeByName["Phí quản lý"].ServiceTypeId,
                Quantity = (int)Math.Round(apartment.Area ?? 60m, MidpointRounding.AwayFromZero),
                RegisteredFrom = new DateTime(now.Year - 1, 1, 1),
                IsActive = true,
                Notes = "Tính theo diện tích thông thủy của căn hộ.",
                CreatedAt = now.AddYears(-1)
            });

            services.Add(new ApartmentService
            {
                ApartmentId = apartment.ApartmentId,
                ServiceTypeId = serviceTypeByName["Phí vệ sinh rác"].ServiceTypeId,
                Quantity = 1,
                RegisteredFrom = new DateTime(now.Year - 1, 1, 1),
                IsActive = true,
                Notes = "Thu cố định theo căn hộ.",
                CreatedAt = now.AddYears(-1)
            });

            services.Add(new ApartmentService
            {
                ApartmentId = apartment.ApartmentId,
                ServiceTypeId = serviceTypeByName["Internet cáp quang"].ServiceTypeId,
                Quantity = 1,
                RegisteredFrom = new DateTime(now.Year - 1, 6, 1),
                IsActive = true,
                Notes = "Gói gia đình 150Mbps.",
                CreatedAt = now.AddMonths(-10)
            });

            if (apartment.ApartmentId % 3 == 0)
            {
                services.Add(new ApartmentService
                {
                    ApartmentId = apartment.ApartmentId,
                    ServiceTypeId = serviceTypeByName["Truyền hình cáp"].ServiceTypeId,
                    Quantity = 1,
                    RegisteredFrom = new DateTime(now.Year - 1, 6, 1),
                    IsActive = false,
                    RegisteredTo = new DateTime(now.Year, now.Month, 1).AddDays(-1),
                    Notes = "Đã dừng từ đầu tháng hiện tại để minh hoạ lịch sử dịch vụ.",
                    CreatedAt = now.AddMonths(-10)
                });
            }

            var residentIds = residentsByApartment.GetValueOrDefault(apartment.ApartmentId, new HashSet<int>());
            var apartmentVehicles = vehicles.Where(v => residentIds.Contains(v.ResidentId)).ToList();
            var motorbikeCount = apartmentVehicles.Count(v => string.Equals(v.VehicleType, "Xe máy", StringComparison.OrdinalIgnoreCase));
            var carCount = apartmentVehicles.Count(v => string.Equals(v.VehicleType, "Ô tô", StringComparison.OrdinalIgnoreCase));

            if (motorbikeCount > 0)
            {
                services.Add(new ApartmentService
                {
                    ApartmentId = apartment.ApartmentId,
                    ServiceTypeId = serviceTypeByName["Phí gửi xe máy"].ServiceTypeId,
                    Quantity = motorbikeCount,
                    RegisteredFrom = new DateTime(now.Year - 1, 1, 1),
                    IsActive = true,
                    Notes = "Số lượng xe máy đăng ký gửi cố định.",
                    CreatedAt = now.AddYears(-1)
                });
            }

            if (carCount > 0)
            {
                services.Add(new ApartmentService
                {
                    ApartmentId = apartment.ApartmentId,
                    ServiceTypeId = serviceTypeByName["Phí gửi ô tô"].ServiceTypeId,
                    Quantity = carCount,
                    RegisteredFrom = new DateTime(now.Year - 1, 1, 1),
                    IsActive = true,
                    Notes = "Số lượng ô tô đăng ký gửi cố định.",
                    CreatedAt = now.AddYears(-1)
                });
            }
        }

        context.ApartmentServices.AddRange(services);
        await context.SaveChangesAsync();
    }

    private static async Task EnsureAmenityBookingsAsync(
        ApartmentDbContext context,
        IReadOnlyList<User> residentUsers,
        IReadOnlyDictionary<string, Amenity> amenityByName,
        DateTime now)
    {
        if (await context.AmenityBookings.AnyAsync())
        {
            return;
        }

        var bookingSeeds = new[]
        {
            ("resident", "Sân Tennis", -20, 18, 2, AmenityBookingStatusHelper.Completed, "Đặt sân sau giờ làm."),
            ("resident3", "Phòng BBQ", -12, 17, 3, AmenityBookingStatusHelper.Completed, "Liên hoan sinh nhật gia đình."),
            ("resident6", "Phòng họp", -4, 9, 2, AmenityBookingStatusHelper.Completed, "Họp online cùng đối tác."),
            ("resident7", "Phòng sinh hoạt cộng đồng", 2, 19, 2, AmenityBookingStatusHelper.Confirmed, "Lớp học vẽ cuối tuần."),
            ("resident9", "Sân Tennis", 1, 6, 2, AmenityBookingStatusHelper.Confirmed, "Đánh tennis buổi sáng."),
            ("resident12", "Phòng BBQ", 5, 18, 3, AmenityBookingStatusHelper.Confirmed, "Tiệc họp mặt nhóm bạn."),
            ("resident14", "Phòng họp", 7, 10, 2, AmenityBookingStatusHelper.Confirmed, "Workshop nhóm nhỏ."),
            ("bqt_head", "Phòng sinh hoạt cộng đồng", 3, 18, 2, AmenityBookingStatusHelper.Confirmed, "Họp ban quản trị."),
            ("resident10", "Sân Tennis", -2, 18, 2, AmenityBookingStatusHelper.Cancelled, "Huỷ do thay đổi lịch."),
            ("resident13", "Phòng BBQ", 4, 17, 3, AmenityBookingStatusHelper.Cancelled, "Huỷ do thời tiết xấu.")
        };

        var userByUsername = residentUsers.ToDictionary(u => u.Username, StringComparer.OrdinalIgnoreCase);

        foreach (var seed in bookingSeeds)
        {
            var resident = userByUsername[seed.Item1];
            var amenity = amenityByName[seed.Item2];
            var bookingDate = now.Date.AddDays(seed.Item3);
            var duration = seed.Item5;

            context.AmenityBookings.Add(new AmenityBooking
            {
                AmenityId = amenity.AmenityId,
                ApartmentId = resident.ApartmentId!.Value,
                ResidentId = resident.UserId,
                BookingDate = bookingDate,
                StartTime = new TimeSpan(seed.Item4, 0, 0),
                EndTime = new TimeSpan(seed.Item4 + duration, 0, 0),
                TotalHours = duration,
                TotalAmount = (amenity.PricePerHour ?? 0m) * duration,
                ParticipantCount = Math.Min(6, amenity.Capacity ?? 6),
                Status = seed.Item6,
                Notes = seed.Item7,
                CreatedAt = bookingDate.AddDays(-5)
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureServiceOrdersAsync(
        ApartmentDbContext context,
        IReadOnlyList<User> residentUsers,
        IReadOnlyList<User> staffUsers,
        IReadOnlyDictionary<string, ServiceType> serviceTypeByName,
        DateTime now)
    {
        if (await context.ServiceOrders.AnyAsync())
        {
            return;
        }

        var userByUsername = residentUsers.ToDictionary(u => u.Username, StringComparer.OrdinalIgnoreCase);

        var seeds = new[]
        {
            new ServiceOrderSeed("SO", "resident", "Dọn vệ sinh căn hộ", -40, "Sáng", ServiceOrderStatus.Completed, 0, 340000m, 320000m, "Đã hoàn thành trước giờ cư dân yêu cầu.", 5, "Nhân viên dọn sạch và đúng giờ."),
            new ServiceOrderSeed("SO", "resident3", "Giặt ủi", -18, "Chiều", ServiceOrderStatus.Completed, 1, 228000m, 228000m, "Đã giao lại đồ trong ngày.", 4, "Quần áo gấp gọn, đúng hẹn."),
            new ServiceOrderSeed("SO", "resident4", "Sửa chữa điện nước nhỏ", -12, "Sáng", ServiceOrderStatus.Completed, 1, 250000m, 280000m, "Thay mới phao bồn cầu và siết lại đầu vòi.", 5, "Xử lý triệt để, nhân viên giải thích rõ chi phí."),
            new ServiceOrderSeed("SO", "resident6", "Vệ sinh máy lạnh", -6, "Chiều", ServiceOrderStatus.Completed, 2, 250000m, 250000m, "Đã vệ sinh 01 máy lạnh phòng khách.", 4, "Máy lạnh mát nhanh hơn."),
            new ServiceOrderSeed("SO", "resident7", "Dọn vệ sinh căn hộ", -1, "Sáng", ServiceOrderStatus.Confirmed, 0, 320000m, null, null, null, null),
            new ServiceOrderSeed("SO", "resident9", "Sửa chữa điện nước nhỏ", 0, "Chiều", ServiceOrderStatus.InProgress, 1, 180000m, null, "Đang kiểm tra nguyên nhân rò nước dưới chậu rửa.", null, null),
            new ServiceOrderSeed("SO", "resident10", "Hỗ trợ chuyển đồ nội khu", 1, "Sáng", ServiceOrderStatus.Pending, null, 450000m, null, null, null, null),
            new ServiceOrderSeed("SO", "resident12", "Giặt ủi", 2, "Tối", ServiceOrderStatus.Pending, null, 152000m, null, null, null, null),
            new ServiceOrderSeed("SO", "resident13", "Vệ sinh máy lạnh", 4, "Chiều", ServiceOrderStatus.Confirmed, 2, 500000m, null, null, null, null),
            new ServiceOrderSeed("SO", "resident14", "Dọn vệ sinh căn hộ", -3, "Tối", ServiceOrderStatus.Cancelled, null, 320000m, null, null, null, null),
            new ServiceOrderSeed("SO", "bqt_head", "Sửa chữa điện nước nhỏ", 3, "Sáng", ServiceOrderStatus.Rejected, null, 200000m, null, null, null, null)
        };

        for (var index = 0; index < seeds.Length; index++)
        {
            var seed = seeds[index];
            var resident = userByUsername[seed.Username];
            var requestedDate = now.Date.AddDays(seed.RequestedOffsetDays);
            var assignedStaff = seed.AssignedStaffIndex.HasValue ? staffUsers[seed.AssignedStaffIndex.Value] : null;

            context.ServiceOrders.Add(new ServiceOrder
            {
                OrderNumber = $"{seed.Prefix}-{requestedDate:yyyyMMdd}-{index + 1:D3}",
                ApartmentId = resident.ApartmentId!.Value,
                ResidentId = resident.UserId,
                ServiceTypeId = serviceTypeByName[seed.ServiceTypeName].ServiceTypeId,
                RequestedDate = requestedDate,
                RequestedTimeSlot = seed.TimeSlot,
                Description = $"Cư dân tạo đơn cho dịch vụ {seed.ServiceTypeName.ToLowerInvariant()}.",
                Status = seed.Status,
                AssignedTo = assignedStaff?.UserId,
                AssignedAt = assignedStaff != null ? requestedDate.AddDays(-1).AddHours(17) : null,
                EstimatedPrice = seed.EstimatedPrice,
                ActualPrice = seed.ActualPrice,
                AdditionalCharges = seed.ActualPrice.HasValue && seed.ActualPrice.Value > seed.EstimatedPrice ? seed.ActualPrice.Value - seed.EstimatedPrice : 0m,
                ChargeNotes = seed.ActualPrice.HasValue && seed.ActualPrice.Value > seed.EstimatedPrice ? "Có phát sinh vật tư thay thế." : null,
                CompletedAt = seed.Status == ServiceOrderStatus.Completed ? requestedDate.AddHours(seed.TimeSlot == "Sáng" ? 10 : 16) : null,
                CompletedBy = seed.Status == ServiceOrderStatus.Completed ? assignedStaff?.UserId : null,
                CompletionNotes = seed.CompletionNotes,
                Rating = seed.Rating,
                ReviewComment = seed.ReviewComment,
                ReviewedAt = seed.Rating.HasValue ? requestedDate.AddDays(1) : null,
                CancelledAt = seed.Status == ServiceOrderStatus.Cancelled ? requestedDate.AddDays(-1).AddHours(20) : null,
                CancelReason = seed.Status == ServiceOrderStatus.Cancelled ? "Cư dân thay đổi lịch." : null,
                CreatedAt = requestedDate.AddDays(-2),
                UpdatedAt = now
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureRequestsAsync(
        ApartmentDbContext context,
        IReadOnlyList<User> residentUsers,
        IReadOnlyList<User> staffUsers,
        IReadOnlyList<User> managerUsers,
        DateTime now)
    {
        if (await context.Requests.AnyAsync())
        {
            return;
        }

        var userByUsername = residentUsers.ToDictionary(u => u.Username, StringComparer.OrdinalIgnoreCase);

        var seeds = new[]
        {
            new RequestSeed("resident", "Điều hòa phòng khách chảy nước", RequestType.Repair, RequestPriority.High, RequestStatus.InProgress, 1, null, "Máy lạnh chảy nước xuống sàn sau khi bật khoảng 30 phút."),
            new RequestSeed("resident3", "Đèn hành lang tầng 3 chập chờn", RequestType.Repair, RequestPriority.Normal, RequestStatus.Pending, 0, null, "Đèn khu vực trước căn A-0302 sáng yếu vào buổi tối."),
            new RequestSeed("resident4", "Nước nóng không ổn định", RequestType.Repair, RequestPriority.High, RequestStatus.Completed, 1, null, "Bình nóng lạnh lúc có lúc không, cần kỹ thuật kiểm tra."),
            new RequestSeed("resident6", "Thang máy block A rung mạnh", RequestType.Complaint, RequestPriority.Emergency, RequestStatus.InProgress, null, 0, "Hiện tượng rung mạnh khi đi từ tầng 1 lên tầng 5."),
            new RequestSeed("resident7", "Đề xuất tăng cây xanh tại sân chơi", RequestType.Feedback, RequestPriority.Low, RequestStatus.Completed, null, null, "Cư dân đề xuất trồng thêm cây mát gần khu vui chơi trẻ em."),
            new RequestSeed("resident9", "Tiếng ồn sau 22h từ căn đối diện", RequestType.Complaint, RequestPriority.Normal, RequestStatus.Pending, null, 0, "Tiếng nói chuyện lớn và kéo ghế sau 22h nhiều ngày liên tục."),
            new RequestSeed("resident10", "Khóa cửa phòng ngủ bị kẹt", RequestType.Repair, RequestPriority.High, RequestStatus.Completed, 2, null, "Ổ khoá mở khó, cần thay hoặc tra dầu."),
            new RequestSeed("resident12", "Camera tầng hầm B1 không hoạt động", RequestType.Security, RequestPriority.High, RequestStatus.InProgress, 2, 0, "Camera gần cột B1-17 bị mất tín hiệu từ tối qua."),
            new RequestSeed("resident13", "Wifi sảnh lễ tân chập chờn", RequestType.Feedback, RequestPriority.Normal, RequestStatus.Pending, 0, null, "Kết nối được nhưng tốc độ rất chậm vào buổi tối."),
            new RequestSeed("bqt_head", "Cập nhật nội quy gửi xe giờ cao điểm", RequestType.Other, RequestPriority.Normal, RequestStatus.Completed, 0, 0, "Đề nghị thống nhất nội dung gửi thông báo mới cho cư dân.")
        };

        for (var index = 0; index < seeds.Length; index++)
        {
            var seed = seeds[index];
            var resident = userByUsername[seed.Username];
            var createdAt = now.AddDays(-(index * 2 + 1));
            int? assignedStaffId = seed.AssignedStaffIndex.HasValue ? staffUsers[seed.AssignedStaffIndex.Value].UserId : null;
            int? escalatedToId = seed.EscalatedManagerIndex.HasValue ? managerUsers[seed.EscalatedManagerIndex.Value].UserId : null;

            context.Requests.Add(new Request
            {
                RequestNumber = $"REQ-{createdAt:yyyyMM}-{index + 1:D3}",
                ApartmentId = resident.ApartmentId!.Value,
                ResidentId = resident.UserId,
                RequestType = seed.RequestType,
                Title = seed.Title,
                Description = seed.Description,
                Priority = seed.Priority,
                Status = seed.Status,
                AssignedTo = assignedStaffId,
                EscalatedTo = escalatedToId,
                EscalatedAt = escalatedToId.HasValue ? createdAt.AddHours(2) : null,
                EscalationReason = escalatedToId.HasValue ? "Cần quản lý xác nhận phối hợp với nhà thầu." : null,
                CreatedAt = createdAt,
                UpdatedAt = now.AddDays(-Math.Max(index - 1, 0)),
                ResolvedAt = seed.Status == RequestStatus.Completed ? createdAt.AddDays(2) : null
            });
        }

        await context.SaveChangesAsync();

        if (!await context.RequestComments.AnyAsync())
        {
            var requests = await context.Requests.OrderBy(r => r.RequestId).ToListAsync();
            var comments = new List<RequestComment>();

            foreach (var request in requests.Where(r => r.AssignedTo.HasValue || r.Status == RequestStatus.Completed))
            {
                comments.Add(new RequestComment
                {
                    RequestId = request.RequestId,
                    AuthorId = request.ResidentId,
                    Content = "Cư dân đã bổ sung thêm mô tả chi tiết và mong được hỗ trợ sớm.",
                    CreatedAt = request.CreatedAt.AddHours(1)
                });

                if (request.AssignedTo.HasValue)
                {
                    comments.Add(new RequestComment
                    {
                        RequestId = request.RequestId,
                        AuthorId = request.AssignedTo.Value,
                        Content = request.Status == RequestStatus.Completed
                            ? "Đội kỹ thuật đã xử lý xong và bàn giao lại cho cư dân."
                            : "Nhân viên đã tiếp nhận, đang sắp lịch kiểm tra tại căn hộ.",
                        CreatedAt = request.CreatedAt.AddHours(4)
                    });
                }
            }

            context.RequestComments.AddRange(comments);
            await context.SaveChangesAsync();
        }

        if (!await context.RequestAttachments.AnyAsync())
        {
            var requests = await context.Requests.OrderBy(r => r.RequestId).Take(6).ToListAsync();
            var attachments = requests.Select((request, index) => new RequestAttachment
            {
                RequestId = request.RequestId,
                FileName = $"request-{request.RequestNumber.ToLowerInvariant()}.jpg",
                FilePath = $"/uploads/requests/request-{index + 1}.jpg",
                FileSize = 280_000 + index * 25_000,
                ContentType = "image/jpeg",
                UploadedAt = request.CreatedAt.AddMinutes(20)
            }).ToList();

            context.RequestAttachments.AddRange(attachments);
            await context.SaveChangesAsync();
        }
    }

    private static async Task EnsureVisitorsAsync(
        ApartmentDbContext context,
        IReadOnlyList<User> residentUsers,
        DateTime now)
    {
        if (await context.Visitors.AnyAsync())
        {
            return;
        }

        var residentByUsername = residentUsers.ToDictionary(u => u.Username, StringComparer.OrdinalIgnoreCase);
        var seeds = new[]
        {
            new VisitorSeed("resident", "Nguyễn Thành Vinh", "0903111111", "079094551001", 0, VisitorStatus.Pending, null, null, "Khách đến trao đổi công việc."),
            new VisitorSeed("resident3", "Trần Bảo Ngọc", "0903111112", "079093661002", 1, VisitorStatus.Pending, null, null, "Bạn học đến chơi buổi tối."),
            new VisitorSeed("resident4", "Lê Minh Nhật", "0903111113", "079090771003", 0, VisitorStatus.CheckedIn, now.Date.AddHours(9.5), null, "Thợ giao rèm cửa."),
            new VisitorSeed("resident6", "Phan Gia Bảo", "0903111114", "079089881004", -1, VisitorStatus.CheckedOut, now.Date.AddDays(-1).AddHours(14), now.Date.AddDays(-1).AddHours(16.5), "Người thân lên ăn tối."),
            new VisitorSeed("resident7", "Đỗ Thục Vy", "0903111115", "079087991005", -3, VisitorStatus.CheckedOut, now.Date.AddDays(-3).AddHours(10), now.Date.AddDays(-3).AddHours(12), "Khách giao hồ sơ."),
            new VisitorSeed("resident9", "Bùi Hoàng An", "0903111116", "079086101006", -2, VisitorStatus.Rejected, null, null, "Khách không mang giấy tờ tùy thân."),
            new VisitorSeed("resident10", "Ngô Hữu Tài", "0903111117", "079085201007", 2, VisitorStatus.Pending, null, null, "Khách lên khảo sát sửa nội thất."),
            new VisitorSeed("resident12", "Võ Mỹ Linh", "0903111118", "079084301008", -5, VisitorStatus.Cancelled, null, null, "Cư dân hủy do thay đổi lịch."),
            new VisitorSeed("resident13", "Dương Hồng Sơn", "0903111119", "079083401009", 1, VisitorStatus.Pending, null, null, "Khách đến xem hợp tác cho thuê."),
            new VisitorSeed("bqt_head", "Đặng Tùng Lâm", "0903111120", "079082501010", -7, VisitorStatus.CheckedOut, now.Date.AddDays(-7).AddHours(8.5), now.Date.AddDays(-7).AddHours(11), "Đối tác thi công khảo sát hệ thống nước.")
        };

        for (var index = 0; index < seeds.Length; index++)
        {
            var seed = seeds[index];
            var resident = residentByUsername[seed.RegisteredByUsername];
            var visitDate = now.Date.AddDays(seed.VisitOffsetDays);

            context.Visitors.Add(new Visitor
            {
                VisitorName = seed.VisitorName,
                PhoneNumber = seed.PhoneNumber,
                IdentityCard = seed.IdentityCard,
                ApartmentId = resident.ApartmentId!.Value,
                RegisteredBy = resident.UserId,
                VisitDate = visitDate,
                CheckInTime = seed.CheckInTime,
                CheckOutTime = seed.CheckOutTime,
                QRCode = $"VIS-{visitDate:yyyyMMdd}-{index + 1:D3}",
                Status = seed.Status,
                Notes = seed.Notes,
                CreatedAt = visitDate.AddDays(-1).AddHours(10)
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureAnnouncementsAsync(ApartmentDbContext context, int createdByUserId, DateTime now)
    {
        if (await context.Announcements.AnyAsync())
        {
            return;
        }

        context.Announcements.AddRange(
            new Announcement
            {
                Title = "Thông báo bảo trì máy phát điện dự phòng",
                Content = "Ban quản lý sẽ kiểm tra máy phát điện dự phòng vào sáng thứ Bảy tuần này từ 08:00 đến 11:00. Một số khu vực công cộng có thể nghe tiếng nổ máy ngắn trong thời gian thử tải.",
                AnnouncementType = AnnouncementType.Maintenance,
                Priority = AnnouncementPriority.High,
                PublishedDate = now.AddDays(-3),
                ExpiryDate = now.AddDays(4),
                IsPinned = true,
                IsActive = true,
                Source = "BQL",
                CreatedBy = createdByUserId,
                CreatedAt = now.AddDays(-3)
            },
            new Announcement
            {
                Title = "Lịch họp cư dân quý I",
                Content = "Ban quản trị kính mời cư dân tham gia buổi họp định kỳ quý I vào lúc 19:00 ngày Chủ nhật tại phòng sinh hoạt cộng đồng tầng 1 block A.",
                AnnouncementType = AnnouncementType.General,
                Priority = AnnouncementPriority.Normal,
                PublishedDate = now.AddDays(-5),
                ExpiryDate = now.AddDays(10),
                IsPinned = false,
                IsActive = true,
                Source = "BQT",
                CreatedBy = createdByUserId,
                CreatedAt = now.AddDays(-5)
            },
            new Announcement
            {
                Title = "Cập nhật biểu phí gửi xe từ tháng tới",
                Content = "Từ đầu tháng sau, biểu phí gửi xe được điều chỉnh theo hợp đồng với đơn vị vận hành hầm xe. Cư dân vui lòng xem chi tiết trong phụ lục đính kèm.",
                AnnouncementType = AnnouncementType.Finance,
                Priority = AnnouncementPriority.High,
                PublishedDate = now.AddDays(-10),
                ExpiryDate = now.AddDays(20),
                IsPinned = true,
                IsActive = true,
                Source = "BQL",
                CreatedBy = createdByUserId,
                CreatedAt = now.AddDays(-10)
            },
            new Announcement
            {
                Title = "Diễn tập phòng cháy chữa cháy toàn khu",
                Content = "Chung cư sẽ tổ chức diễn tập phòng cháy chữa cháy vào 09:00 sáng thứ Tư tuần sau tại sân trước block B. Đề nghị cư dân phối hợp theo hướng dẫn của bảo vệ.",
                AnnouncementType = AnnouncementType.Security,
                Priority = AnnouncementPriority.Critical,
                PublishedDate = now.AddDays(-1),
                ExpiryDate = now.AddDays(6),
                IsPinned = true,
                IsActive = true,
                Source = "System",
                CreatedBy = createdByUserId,
                CreatedAt = now.AddDays(-1)
            },
            new Announcement
            {
                Title = "Khai trương khu co-working mới",
                Content = "Khu co-working tại tầng 2 block B đã mở cửa phục vụ cư dân mỗi ngày từ 07:00 đến 22:00. Cư dân vui lòng giữ yên lặng khi sử dụng không gian này.",
                AnnouncementType = AnnouncementType.Event,
                Priority = AnnouncementPriority.Normal,
                PublishedDate = now.AddDays(-7),
                ExpiryDate = now.AddDays(14),
                IsPinned = false,
                IsActive = true,
                Source = "BQL",
                CreatedBy = createdByUserId,
                CreatedAt = now.AddDays(-7)
            });

        await context.SaveChangesAsync();
    }

    private static async Task EnsureDocumentsAsync(ApartmentDbContext context, int uploadedByUserId, DateTime now)
    {
        if (await context.Documents.AnyAsync())
        {
            return;
        }

        context.Documents.AddRange(
            new Document
            {
                Title = "Nội quy cư dân Sunrise Riverside",
                Description = "Bản nội quy chính thức áp dụng cho cư dân và khách ra vào.",
                DocumentType = DocumentType.Regulation,
                FileName = "noi-quy-cu-dan.pdf",
                FilePath = "/uploads/documents/noi-quy-cu-dan.pdf",
                FileSize = 1_250_000,
                UploadedBy = uploadedByUserId,
                UploadedAt = now.AddDays(-45),
                IsActive = true,
                IsDeleted = false
            },
            new Document
            {
                Title = "Cẩm nang sử dụng tiện ích",
                Description = "Hướng dẫn đặt lịch và sử dụng các tiện ích cố định trong toà nhà.",
                DocumentType = DocumentType.Manual,
                FileName = "cam-nang-tien-ich.pdf",
                FilePath = "/uploads/documents/cam-nang-tien-ich.pdf",
                FileSize = 980_000,
                UploadedBy = uploadedByUserId,
                UploadedAt = now.AddDays(-30),
                IsActive = true,
                IsDeleted = false
            },
            new Document
            {
                Title = "Báo cáo thu chi quý gần nhất",
                Description = "Tổng hợp thu phí quản lý, phí gửi xe và chi phí vận hành.",
                DocumentType = DocumentType.Report,
                FileName = "bao-cao-thu-chi-quy.pdf",
                FilePath = "/uploads/documents/bao-cao-thu-chi-quy.pdf",
                FileSize = 1_850_000,
                UploadedBy = uploadedByUserId,
                UploadedAt = now.AddDays(-20),
                IsActive = true,
                IsDeleted = false
            },
            new Document
            {
                Title = "Mẫu hợp đồng mua bán và cho thuê",
                Description = "Bộ hợp đồng mẫu dành cho giao dịch cư dân mới.",
                DocumentType = DocumentType.Legal,
                FileName = "mau-hop-dong.pdf",
                FilePath = "/uploads/documents/mau-hop-dong.pdf",
                FileSize = 2_300_000,
                UploadedBy = uploadedByUserId,
                UploadedAt = now.AddDays(-15),
                IsActive = true,
                IsDeleted = false
            });

        await context.SaveChangesAsync();
    }

    private static async Task EnsureInvoicesAsync(
        ApartmentDbContext context,
        int createdByUserId,
        IReadOnlyList<Apartment> apartments,
        IReadOnlyList<User> residentUsers,
        IReadOnlyDictionary<string, ServiceType> serviceTypeByName,
        IReadOnlyList<ServicePrice> servicePrices,
        DateTime now)
    {
        var billingPeriods = new[]
        {
            new DateTime(now.Year, now.Month, 1).AddMonths(-1),
            new DateTime(now.Year, now.Month, 1)
        };

        if (!await context.Invoices.AnyAsync())
        {
            var activeApartmentIds = await context.Contracts
                .Where(c => c.Status == ContractStatus.Active)
                .Select(c => c.ApartmentId)
                .Distinct()
                .Take(8)
                .ToListAsync();

            foreach (var apartmentId in activeApartmentIds)
            {
                var apartment = apartments.First(a => a.ApartmentId == apartmentId);

                foreach (var period in billingPeriods)
                {
                    var issueDate = new DateTime(period.Year, period.Month, 5);
                    var dueDate = new DateTime(period.Year, period.Month, 20);
                    var isCurrentMonth = period.Month == now.Month && period.Year == now.Year;

                    context.Invoices.Add(new Invoice
                    {
                        InvoiceNumber = $"INV-{period:yyyyMM}-{apartment.ApartmentNumber.Replace("-", string.Empty)}",
                        ApartmentId = apartmentId,
                        BillingMonth = period.Month,
                        BillingYear = period.Year,
                        IssueDate = issueDate,
                        DueDate = dueDate,
                        TotalAmount = 0,
                        PaidAmount = 0,
                        Status = InvoiceStatus.Issued,
                        IsSent = true,
                        SentAt = issueDate.AddDays(1),
                        Notes = $"Hóa đơn tổng hợp dịch vụ tháng {period:MM/yyyy} cho căn {apartment.ApartmentNumber}.",
                        CreatedBy = createdByUserId,
                        CreatedAt = issueDate
                    });
                }
            }

            await context.SaveChangesAsync();
        }

        if (!await context.InvoiceDetails.AnyAsync())
        {
            var invoices = await context.Invoices.Include(i => i.Apartment).OrderBy(i => i.InvoiceId).ToListAsync();
            var apartmentServices = await context.ApartmentServices.ToListAsync();
            var completedOrders = await context.ServiceOrders.Include(so => so.ServiceType).Where(so => so.Status == ServiceOrderStatus.Completed).ToListAsync();
            var residentCountByApartment = residentUsers.Where(u => u.ApartmentId.HasValue).GroupBy(u => u.ApartmentId!.Value).ToDictionary(g => g.Key, g => g.Count());

            foreach (var invoice in invoices)
            {
                var monthStart = new DateTime(invoice.BillingYear, invoice.BillingMonth, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                var details = new List<InvoiceDetail>();

                foreach (var apartmentService in apartmentServices.Where(s => s.ApartmentId == invoice.ApartmentId && s.RegisteredFrom <= monthEnd && (!s.RegisteredTo.HasValue || s.RegisteredTo.Value >= monthStart)))
                {
                    var price = GetEffectivePrice(servicePrices, apartmentService.ServiceTypeId, monthStart);
                    if (price == null) continue;

                    details.Add(new InvoiceDetail
                    {
                        InvoiceId = invoice.InvoiceId,
                        ServiceTypeId = apartmentService.ServiceTypeId,
                        ServicePriceId = price.ServicePriceId,
                        Quantity = apartmentService.Quantity,
                        UnitPrice = price.UnitPrice,
                        Description = $"Dịch vụ định kỳ: {price.ServiceType.ServiceTypeName}"
                    });
                }

                var electricityPrice = GetEffectivePrice(servicePrices, serviceTypeByName["Điện sinh hoạt"].ServiceTypeId, monthStart);
                if (electricityPrice != null)
                {
                    details.Add(new InvoiceDetail
                    {
                        InvoiceId = invoice.InvoiceId,
                        ServiceTypeId = electricityPrice.ServiceTypeId,
                        ServicePriceId = electricityPrice.ServicePriceId,
                        Quantity = Math.Round((invoice.Apartment.Area ?? 60m) * 1.45m + (invoice.BillingMonth * 5), 0),
                        UnitPrice = electricityPrice.UnitPrice,
                        Description = "Điện sinh hoạt theo chỉ số công tơ tháng."
                    });
                }

                var waterPrice = GetEffectivePrice(servicePrices, serviceTypeByName["Nước sinh hoạt"].ServiceTypeId, monthStart);
                if (waterPrice != null)
                {
                    details.Add(new InvoiceDetail
                    {
                        InvoiceId = invoice.InvoiceId,
                        ServiceTypeId = waterPrice.ServiceTypeId,
                        ServicePriceId = waterPrice.ServicePriceId,
                        Quantity = Math.Round(residentCountByApartment.GetValueOrDefault(invoice.ApartmentId, 2) * 4.5m + (invoice.BillingMonth % 3), 0),
                        UnitPrice = waterPrice.UnitPrice,
                        Description = "Nước sinh hoạt theo chỉ số đồng hồ tháng."
                    });
                }

                foreach (var order in completedOrders.Where(so => so.ApartmentId == invoice.ApartmentId && so.CompletedAt.HasValue && so.CompletedAt.Value.Month == invoice.BillingMonth && so.CompletedAt.Value.Year == invoice.BillingYear))
                {
                    var price = GetEffectivePrice(servicePrices, order.ServiceTypeId, monthStart);
                    if (price == null) continue;

                    details.Add(new InvoiceDetail
                    {
                        InvoiceId = invoice.InvoiceId,
                        ServiceTypeId = order.ServiceTypeId,
                        ServicePriceId = price.ServicePriceId,
                        Quantity = 1,
                        UnitPrice = order.ActualPrice ?? order.EstimatedPrice ?? price.UnitPrice,
                        ServiceOrderId = order.ServiceOrderId,
                        Description = $"Dịch vụ theo yêu cầu: {order.ServiceType.ServiceTypeName}"
                    });

                    order.InvoiceId = invoice.InvoiceId;
                    order.UpdatedAt = now;
                }

                if (details.Count == 0) continue;

                context.InvoiceDetails.AddRange(details);
                invoice.TotalAmount = details.Sum(d => d.Quantity * d.UnitPrice);
                invoice.UpdatedAt = now;
            }

            await context.SaveChangesAsync();
        }

        if (!await context.PaymentTransactions.AnyAsync())
        {
            var invoices = await context.Invoices.OrderBy(i => i.BillingYear).ThenBy(i => i.BillingMonth).ThenBy(i => i.ApartmentId).ToListAsync();

            for (var index = 0; index < invoices.Count; index++)
            {
                var invoice = invoices[index];
                var period = new DateTime(invoice.BillingYear, invoice.BillingMonth, 1);
                var isCurrentMonth = period.Month == now.Month && period.Year == now.Year;

                decimal amountToPay = 0;
                string? method = null;

                if (!isCurrentMonth)
                {
                    if (index % 2 == 0)
                    {
                        amountToPay = invoice.TotalAmount;
                        method = "BankTransfer";
                    }
                    else
                    {
                        amountToPay = Math.Round(invoice.TotalAmount * 0.6m, 0);
                        method = "Cash";
                    }
                }
                else if (index == 0)
                {
                    amountToPay = Math.Round(invoice.TotalAmount * 0.5m, 0);
                    method = "VNPay";
                }

                if (amountToPay > 0)
                {
                    var paymentDate = invoice.IssueDate.AddDays(isCurrentMonth ? 10 : 5);
                    context.PaymentTransactions.Add(new PaymentTransaction
                    {
                        InvoiceId = invoice.InvoiceId,
                        TransactionCode = $"PAY-{paymentDate:yyyyMMdd}-{invoice.InvoiceId:D4}",
                        Amount = amountToPay,
                        PaymentMethod = method,
                        PaymentDate = paymentDate,
                        Status = (int)PaymentTransactionStatus.Success,
                        GatewayResponse = $"Seed dữ liệu thanh toán mẫu qua {method}.",
                        CreatedBy = createdByUserId,
                        CreatedAt = paymentDate
                    });

                    invoice.PaidAmount = amountToPay;
                    invoice.PaymentMethod = method;
                    invoice.PaymentDate = paymentDate;
                }

                invoice.Status = DetermineInvoiceStatus(invoice.TotalAmount, invoice.PaidAmount, invoice.DueDate, invoice.IsSent, now);
                invoice.UpdatedAt = now;
            }

            await context.SaveChangesAsync();
        }
    }

    private static async Task EnsureNotificationsAsync(ApartmentDbContext context, IReadOnlyList<User> residentUsers, DateTime now)
    {
        if (await context.Notifications.AnyAsync())
        {
            return;
        }

        var invoices = await context.Invoices.Where(i => i.IsSent).OrderByDescending(i => i.BillingYear).ThenByDescending(i => i.BillingMonth).ToListAsync();
        var requests = await context.Requests.OrderByDescending(r => r.CreatedAt).Take(6).ToListAsync();
        var announcements = await context.Announcements.OrderByDescending(a => a.PublishedDate).Take(4).ToListAsync();
        var visitors = await context.Visitors.Where(v => v.Status == VisitorStatus.Pending).OrderBy(v => v.VisitDate).Take(4).ToListAsync();

        var notifications = new List<Notification>();

        foreach (var resident in residentUsers.Take(8))
        {
            var invoice = invoices.FirstOrDefault(i => i.ApartmentId == resident.ApartmentId);
            if (invoice == null) continue;

            notifications.Add(new Notification
            {
                UserId = resident.UserId,
                Title = $"Hóa đơn tháng {invoice.BillingMonth:D2}/{invoice.BillingYear}",
                Content = "Hóa đơn mới đã được tạo. Vui lòng kiểm tra và thanh toán đúng hạn.",
                NotificationType = NotificationType.Invoice,
                ReferenceType = ReferenceType.Invoice,
                ReferenceId = invoice.InvoiceId,
                IsRead = resident.UserId % 2 == 0,
                ReadAt = resident.UserId % 2 == 0 ? now.AddDays(-1) : null,
                Priority = NotificationPriority.Normal,
                CreatedAt = now.AddDays(-2)
            });
        }

        foreach (var request in requests.Take(4))
        {
            notifications.Add(new Notification
            {
                UserId = request.ResidentId,
                Title = $"Cập nhật yêu cầu {request.RequestNumber}",
                Content = request.Status == RequestStatus.Completed ? "Yêu cầu của bạn đã được xử lý hoàn tất." : "Yêu cầu của bạn đang được ban quản lý tiếp nhận và xử lý.",
                NotificationType = NotificationType.Request,
                ReferenceType = ReferenceType.Request,
                ReferenceId = request.RequestId,
                IsRead = false,
                Priority = request.Priority == RequestPriority.Emergency ? NotificationPriority.Critical : NotificationPriority.Normal,
                CreatedAt = request.CreatedAt.AddHours(4)
            });
        }

        foreach (var announcement in announcements.Take(3))
        {
            notifications.Add(new Notification
            {
                UserId = residentUsers[notifications.Count % residentUsers.Count].UserId,
                Title = announcement.Title,
                Content = "Có thông báo mới từ Ban quản lý/Ban quản trị. Vui lòng đọc để nắm thông tin.",
                NotificationType = NotificationType.Announcement,
                ReferenceType = ReferenceType.Announcement,
                ReferenceId = announcement.AnnouncementId,
                IsRead = false,
                Priority = announcement.Priority == AnnouncementPriority.Critical ? NotificationPriority.Critical : NotificationPriority.High,
                CreatedAt = announcement.PublishedDate
            });
        }

        foreach (var visitor in visitors.Take(2))
        {
            notifications.Add(new Notification
            {
                UserId = visitor.RegisteredBy,
                Title = "Khách đã được đăng ký",
                Content = $"Khách {visitor.VisitorName} đã được lưu trong danh sách ra vào ngày {visitor.VisitDate:dd/MM/yyyy}.",
                NotificationType = NotificationType.Other,
                ReferenceType = ReferenceType.Visitor,
                ReferenceId = visitor.VisitorId,
                IsRead = true,
                ReadAt = now.AddHours(-6),
                Priority = NotificationPriority.Low,
                CreatedAt = visitor.CreatedAt
            });
        }

        context.Notifications.AddRange(notifications);
        await context.SaveChangesAsync();
    }

    private static async Task EnsureAnnouncementAssetsAsync(ApartmentDbContext context, IReadOnlyList<User> residentUsers, DateTime now)
    {
        if (!await context.AnnouncementAttachments.AnyAsync())
        {
            var announcements = await context.Announcements.OrderBy(a => a.AnnouncementId).Take(3).ToListAsync();
            var attachments = announcements.Select((announcement, index) => new AnnouncementAttachment
            {
                AnnouncementId = announcement.AnnouncementId,
                FileName = $"announcement-{announcement.AnnouncementId}.pdf",
                FilePath = $"/uploads/announcements/announcement-{announcement.AnnouncementId}.pdf",
                FileSize = 860_000 + index * 120_000,
                ContentType = "application/pdf",
                UploadedAt = announcement.CreatedAt.AddMinutes(15)
            }).ToList();

            context.AnnouncementAttachments.AddRange(attachments);
            await context.SaveChangesAsync();
        }

        if (!await context.AnnouncementReads.AnyAsync())
        {
            var announcements = await context.Announcements.OrderByDescending(a => a.PublishedDate).Take(3).ToListAsync();
            var reads = new List<AnnouncementRead>();

            foreach (var announcement in announcements)
            {
                foreach (var resident in residentUsers.Take(6))
                {
                    reads.Add(new AnnouncementRead
                    {
                        AnnouncementId = announcement.AnnouncementId,
                        UserId = resident.UserId,
                        ReadAt = announcement.PublishedDate.AddHours((resident.UserId % 5) + 1)
                    });
                }
            }

            context.AnnouncementReads.AddRange(reads);
            await context.SaveChangesAsync();
        }
    }

    private static async Task EnsureFaceAuthAsync(ApartmentDbContext context, IReadOnlyList<User> residentUsers, DateTime now)
    {
        if (await context.FaceAuthHistories.AnyAsync())
        {
            return;
        }

        var histories = new List<FaceAuthHistory>();

        foreach (var resident in residentUsers.Where(u => u.IsFaceRegistered).Take(8))
        {
            histories.Add(new FaceAuthHistory
            {
                ResidentId = resident.UserId,
                AuthTime = now.AddDays(-3).AddHours(resident.UserId % 6 + 6),
                IsSuccess = true,
                ConfidenceScore = 0.93 + (resident.UserId % 4) * 0.01,
                IpAddress = "192.168.10.25",
                DeviceInfo = "Kiosk FaceAuth - Sảnh chính"
            });

            histories.Add(new FaceAuthHistory
            {
                ResidentId = resident.UserId,
                AuthTime = now.AddDays(-1).AddHours(resident.UserId % 5 + 18),
                IsSuccess = resident.UserId % 3 != 0,
                ConfidenceScore = resident.UserId % 3 != 0 ? 0.91 : 0.67,
                IpAddress = "192.168.10.26",
                DeviceInfo = "Kiosk FaceAuth - Tiện ích tầng 3"
            });
        }

        context.FaceAuthHistories.AddRange(histories);
        await context.SaveChangesAsync();
    }

    private static async Task EnsureRefreshTokensAsync(ApartmentDbContext context, IReadOnlyList<User> users, DateTime now)
    {
        if (await context.UserRefreshTokens.AnyAsync())
        {
            return;
        }

        foreach (var user in users.Where(u => u.Username is "admin" or "manager" or "staff" or "resident"))
        {
            context.UserRefreshTokens.Add(new UserRefreshToken
            {
                UserId = user.UserId,
                TokenHash = HashSeedToken($"{user.Username}-{now:yyyyMMdd}"),
                ExpiresAt = now.AddDays(7),
                CreatedAt = now.AddHours(-2),
                CreatedByIp = "127.0.0.1"
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task EnsureActivityLogsAsync(ApartmentDbContext context, IReadOnlyList<User> users, DateTime now)
    {
        if (await context.ActivityLogs.AnyAsync())
        {
            return;
        }

        var userByUsername = users.ToDictionary(u => u.Username, StringComparer.OrdinalIgnoreCase);
        var latestInvoice = await context.Invoices.OrderByDescending(i => i.InvoiceId).FirstOrDefaultAsync();
        var latestRequest = await context.Requests.OrderByDescending(r => r.RequestId).FirstOrDefaultAsync();
        var latestVisitor = await context.Visitors.OrderByDescending(v => v.VisitorId).FirstOrDefaultAsync();
        var latestServiceOrder = await context.ServiceOrders.OrderByDescending(so => so.ServiceOrderId).FirstOrDefaultAsync();
        var latestAmenityBooking = await context.AmenityBookings.OrderByDescending(ab => ab.BookingId).FirstOrDefaultAsync();

        context.ActivityLogs.AddRange(
            new ActivityLog
            {
                UserId = userByUsername["admin"].UserId,
                UserName = userByUsername["admin"].FullName,
                UserRole = userByUsername["admin"].Role?.ToString(),
                Action = "SeedData",
                EntityName = nameof(User),
                Description = "Khởi tạo dữ liệu mẫu toàn hệ thống phục vụ demo và kiểm thử.",
                Timestamp = now,
                IpAddress = "127.0.0.1",
                UserAgent = "Codex Seeder",
                IsSuccess = true
            },
            new ActivityLog
            {
                UserId = userByUsername["manager"].UserId,
                UserName = userByUsername["manager"].FullName,
                UserRole = userByUsername["manager"].Role?.ToString(),
                Action = "ApproveInvoice",
                EntityName = nameof(Invoice),
                EntityId = latestInvoice?.InvoiceId.ToString(),
                Description = latestInvoice == null ? null : $"Duyệt hóa đơn {latestInvoice.InvoiceNumber}.",
                Timestamp = now.AddMinutes(-30),
                IpAddress = "192.168.10.10",
                UserAgent = "Chrome",
                IsSuccess = true
            },
            new ActivityLog
            {
                UserId = userByUsername["staff"].UserId,
                UserName = userByUsername["staff"].FullName,
                UserRole = userByUsername["staff"].Role?.ToString(),
                Action = "UpdateRequest",
                EntityName = nameof(Request),
                EntityId = latestRequest?.RequestId.ToString(),
                Description = latestRequest == null ? null : $"Cập nhật trạng thái yêu cầu {latestRequest.RequestNumber}.",
                Timestamp = now.AddMinutes(-25),
                IpAddress = "192.168.10.20",
                UserAgent = "Edge",
                IsSuccess = true
            },
            new ActivityLog
            {
                UserId = userByUsername["staff"].UserId,
                UserName = userByUsername["staff"].FullName,
                UserRole = userByUsername["staff"].Role?.ToString(),
                Action = "CheckInVisitor",
                EntityName = nameof(Visitor),
                EntityId = latestVisitor?.VisitorId.ToString(),
                Description = latestVisitor == null ? null : $"Ghi nhận khách {latestVisitor.VisitorName} vào sảnh.",
                Timestamp = now.AddMinutes(-20),
                IpAddress = "192.168.10.21",
                UserAgent = "Edge",
                IsSuccess = true
            },
            new ActivityLog
            {
                UserId = userByUsername["staff2"].UserId,
                UserName = userByUsername["staff2"].FullName,
                UserRole = userByUsername["staff2"].Role?.ToString(),
                Action = "CompleteServiceOrder",
                EntityName = nameof(ServiceOrder),
                EntityId = latestServiceOrder?.ServiceOrderId.ToString(),
                Description = latestServiceOrder == null ? null : $"Hoàn thành đơn dịch vụ {latestServiceOrder.OrderNumber}.",
                Timestamp = now.AddMinutes(-15),
                IpAddress = "192.168.10.22",
                UserAgent = "Firefox",
                IsSuccess = true
            },
            new ActivityLog
            {
                UserId = userByUsername["resident"].UserId,
                UserName = userByUsername["resident"].FullName,
                UserRole = userByUsername["resident"].Role?.ToString(),
                Action = "BookAmenity",
                EntityName = nameof(AmenityBooking),
                EntityId = latestAmenityBooking?.BookingId.ToString(),
                Description = latestAmenityBooking == null ? null : "Cư dân đặt tiện ích cố định qua hệ thống.",
                Timestamp = now.AddMinutes(-10),
                IpAddress = "192.168.10.30",
                UserAgent = "Mobile Safari",
                IsSuccess = true
            });

        await context.SaveChangesAsync();
    }

    private static ServicePrice? GetEffectivePrice(IEnumerable<ServicePrice> servicePrices, int serviceTypeId, DateTime targetDate)
    {
        return servicePrices
            .Where(sp => sp.ServiceTypeId == serviceTypeId && sp.EffectiveFrom <= targetDate && (!sp.EffectiveTo.HasValue || sp.EffectiveTo.Value >= targetDate))
            .OrderByDescending(sp => sp.EffectiveFrom)
            .FirstOrDefault();
    }

    private static InvoiceStatus DetermineInvoiceStatus(
        decimal totalAmount,
        decimal paidAmount,
        DateTime dueDate,
        bool isSent,
        DateTime now)
    {
        if (!isSent)
        {
            return InvoiceStatus.Unpaid;
        }

        if (paidAmount >= totalAmount && totalAmount > 0)
        {
            return InvoiceStatus.Paid;
        }

        if (paidAmount > 0)
        {
            return InvoiceStatus.PartiallyPaid;
        }

        return now.Date > dueDate.Date ? InvoiceStatus.Overdue : InvoiceStatus.Unpaid;
    }

    private static MemberRole MapMemberRole(ResidentType? residentType)
    {
        return residentType switch
        {
            ResidentType.FamilyMember => MemberRole.FamilyMember,
            ResidentType.Tenant => MemberRole.CoTenant,
            _ => MemberRole.Other
        };
    }

    private static ResidencyType MapResidencyType(ResidentType? residentType)
    {
        return residentType switch
        {
            ResidentType.Owner => ResidencyType.Owner,
            ResidentType.Tenant => ResidencyType.Tenant,
            ResidentType.FamilyMember => ResidencyType.FamilyMember,
            _ => ResidencyType.Other
        };
    }

    private static string GenerateFaceDescriptor(string seed)
    {
        var baseValue = Math.Abs(seed.GetHashCode());
        var values = Enumerable.Range(0, 16)
            .Select(index => (((baseValue + index * 37) % 1000) / 1000d - 0.5d).ToString("F4", CultureInfo.InvariantCulture));
        return $"[{string.Join(", ", values)}]";
    }

    private static string HashSeedToken(string seed)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(seed)));
    }

    private sealed record UserSeed(
        string Username,
        string FullName,
        string Email,
        string PhoneNumber,
        UserRole Role,
        string? ApartmentNumber,
        ResidentType? ResidentType,
        string? IdentityCardNumber,
        DateTime? DateOfBirth,
        int MoveInMonthsAgo,
        bool HasFaceProfile,
        string? Note);

    private sealed record ServiceSeed(
        string Name,
        string[] LegacyNames,
        string MeasurementUnit,
        string Description,
        decimal CurrentPrice,
        decimal? PreviousPrice,
        bool IsActive);

    private sealed record AmenitySeed(
        string Name,
        string TypeName,
        string Location,
        int Capacity,
        decimal PricePerHour,
        bool RequiresBooking,
        TimeSpan OpenTime,
        TimeSpan CloseTime,
        int CancellationDeadlineHours,
        string Description);

    private sealed record ServiceOrderSeed(
        string Prefix,
        string Username,
        string ServiceTypeName,
        int RequestedOffsetDays,
        string TimeSlot,
        ServiceOrderStatus Status,
        int? AssignedStaffIndex,
        decimal EstimatedPrice,
        decimal? ActualPrice,
        string? CompletionNotes,
        int? Rating,
        string? ReviewComment);

    private sealed record RequestSeed(
        string Username,
        string Title,
        RequestType RequestType,
        RequestPriority Priority,
        RequestStatus Status,
        int? AssignedStaffIndex,
        int? EscalatedManagerIndex,
        string Description);

    private sealed record VisitorSeed(
        string RegisteredByUsername,
        string VisitorName,
        string PhoneNumber,
        string IdentityCard,
        int VisitOffsetDays,
        VisitorStatus Status,
        DateTime? CheckInTime,
        DateTime? CheckOutTime,
        string Notes);
}
