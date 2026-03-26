using Microsoft.EntityFrameworkCore;
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

        using var context = new ApartmentDbContext(optionsBuilder.Options);

        if (!await context.Database.CanConnectAsync())
        {
            throw new InvalidOperationException("Không thể kết nối đến cơ sở dữ liệu.");
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword("123456");
        var now = DateTime.Now;

        // ========== 1. USERS (Quản lý) ==========
        if (!await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
        {
            var managers = new List<User>
            {
                new User { Username = "admin", FullName = "Nguyễn Quốc Anh", Email = "admin@sunriseapartment.vn", PhoneNumber = "0901000001", Role = UserRole.Admin, PasswordHash = passwordHash, IsActive = true, CreatedAt = now },
                new User { Username = "manager", FullName = "Trần Minh Tuấn", Email = "tuan.manager@sunriseapartment.vn", PhoneNumber = "0901000002", Role = UserRole.BQL_Manager, PasswordHash = passwordHash, IsActive = true, CreatedAt = now },
                new User { Username = "staff1", FullName = "Lê Thị Hồng Nhung", Email = "nhung.staff@sunriseapartment.vn", PhoneNumber = "0901000003", Role = UserRole.BQL_Staff, PasswordHash = passwordHash, IsActive = true, CreatedAt = now },
                new User { Username = "staff2", FullName = "Phạm Văn Đức", Email = "duc.staff@sunriseapartment.vn", PhoneNumber = "0901000004", Role = UserRole.BQL_Staff, PasswordHash = passwordHash, IsActive = true, CreatedAt = now },
                new User { Username = "staff3", FullName = "Võ Thị Mai Lan", Email = "lan.staff@sunriseapartment.vn", PhoneNumber = "0901000005", Role = UserRole.BQL_Staff, PasswordHash = passwordHash, IsActive = true, CreatedAt = now },
            };
            context.Users.AddRange(managers);
            await context.SaveChangesAsync();
        }

        // ========== 2. APARTMENTS ==========
        if (!await context.Apartments.AnyAsync())
        {
            var apartments = new List<Apartment>();
            var apartmentTypes = new[] { "Studio", "1PN", "2PN", "3PN", "Penthouse" };
            var areas = new[] { 35m, 55m, 75m, 95m, 150m };

            foreach (var block in new[] { "A", "B" })
            {
                for (int floor = 1; floor <= 10; floor++)
                {
                    for (int unit = 1; unit <= 4; unit++)
                    {
                        var typeIndex = (floor + unit) % 5;
                        var status = (floor * unit) % 10 == 0 ? ApartmentStatus.Available :
                                     (floor * unit) % 7 == 0 ? ApartmentStatus.Reserved : ApartmentStatus.Occupied;
                        
                        apartments.Add(new Apartment
                        {
                            ApartmentNumber = $"{block}-{floor:D2}{unit:D2}",
                            Floor = floor,
                            BuildingBlock = block,
                            Area = areas[typeIndex] + (floor * 2),
                            ApartmentType = apartmentTypes[typeIndex],
                            Status = status,
                            Description = $"Căn hộ {apartmentTypes[typeIndex]} tầng {floor} block {block}",
                            CreatedAt = now
                        });
                    }
                }
            }
            context.Apartments.AddRange(apartments);
            await context.SaveChangesAsync();
        }

        var allApartments = await context.Apartments.ToListAsync();
        var occupiedApartments = allApartments.Where(a => a.Status == ApartmentStatus.Occupied).ToList();
        var targetApts = occupiedApartments.Any() ? occupiedApartments : allApartments;

        if (!targetApts.Any())
        {
            throw new InvalidOperationException("Không tìm thấy căn hộ nào để thực hiện seeding.");
        }

        // ========== 3. USERS (Cư dân) ==========
        if (!await context.Users.AnyAsync(u => u.Role == UserRole.Resident))
        {
            var residentNames = new[]
            {
                ("Nguyễn Văn Hùng", "079085012345", new DateTime(1985, 3, 15), "0901234567", ResidentType.Owner),
                ("Trần Thị Lan", "079090123456", new DateTime(1990, 7, 22), "0912345678", ResidentType.FamilyMember),
                ("Lê Hoàng Nam", "079088234567", new DateTime(1988, 11, 10), "0923456789", ResidentType.Owner),
                ("Phạm Thị Hoa", "079092345678", new DateTime(1992, 5, 5), "0934567890", ResidentType.FamilyMember),
                ("Lê Minh Khoa", "079075456789", new DateTime(1975, 1, 28), "0945678901", ResidentType.Owner),
                ("Nguyễn Thị Thu", "079080567890", new DateTime(1980, 9, 14), "0956789012", ResidentType.FamilyMember),
                ("Võ Văn Tâm", "079082678901", new DateTime(1982, 12, 3), "0978901234", ResidentType.Tenant),
                ("Đặng Thị Mai", "079095789012", new DateTime(1995, 4, 17), "0989012345", ResidentType.Owner),
                ("Bùi Quốc Việt", "079078890123", new DateTime(1978, 8, 25), "0990123456", ResidentType.Owner),
                ("Bùi Thị Ngọc", "079082901234", new DateTime(1982, 2, 11), "0901234568", ResidentType.FamilyMember),
                ("Hoàng Văn Dũng", "079070123456", new DateTime(1970, 6, 8), "0912345679", ResidentType.Owner),
                ("Hoàng Thị Liên", "079075234567", new DateTime(1975, 3, 19), "0923456780", ResidentType.FamilyMember),
                ("Trịnh Công Minh", "079065345678", new DateTime(1965, 12, 12), "0934567891", ResidentType.Owner),
                ("Trịnh Thị Hương", "079068456789", new DateTime(1968, 7, 27), "0945678902", ResidentType.FamilyMember),
                ("Đinh Văn Phong", "079083567890", new DateTime(1983, 10, 30), "0956789013", ResidentType.Tenant),
                ("Cao Minh Quân", "079091678901", new DateTime(1991, 1, 5), "0967890124", ResidentType.Owner),
                ("Lý Thị Hồng", "079087789012", new DateTime(1987, 5, 20), "0978901235", ResidentType.FamilyMember),
                ("Phan Thanh Sơn", "079079890123", new DateTime(1979, 8, 15), "0989012346", ResidentType.Owner),
                ("Phan Thị Yến", "079084901234", new DateTime(1984, 11, 25), "0990123457", ResidentType.FamilyMember),
                ("Dương Văn Hải", "079076012345", new DateTime(1976, 4, 10), "0901234569", ResidentType.Tenant),
                ("Ngô Thị Cẩm", "079089123456", new DateTime(1989, 9, 8), "0912345680", ResidentType.Owner),
                ("Tạ Văn Long", "079081234567", new DateTime(1981, 2, 28), "0923456781", ResidentType.Owner),
                ("Tạ Thị Phượng", "079086345678", new DateTime(1986, 6, 15), "0934567892", ResidentType.FamilyMember),
                ("Vương Đức Thịnh", "079073456789", new DateTime(1973, 11, 3), "0945678903", ResidentType.Owner),
                ("Vương Thị Loan", "079078567890", new DateTime(1978, 3, 22), "0956789014", ResidentType.FamilyMember),
            };

            var residents = new List<User>();
            for (int i = 0; i < residentNames.Length; i++)
            {
                var (name, cccd, dob, phone, type) = residentNames[i];
                var aptIndex = (i / 2) % targetApts.Count;

                residents.Add(new User
                {
                    Username = $"resident{i + 1}",
                    FullName = name,
                    Email = $"resident{i + 1}@email.com",
                    IdentityCardNumber = cccd,
                    DateOfBirth = dob,
                    ApartmentId = targetApts[aptIndex].ApartmentId,
                    PhoneNumber = phone,
                    ResidentType = type,
                    ResidencyStatus = "Thường trú",
                    Role = UserRole.Resident,
                    PasswordHash = passwordHash,
                    IsActive = true,
                    CreatedAt = now,
                    MoveInDate = now.AddMonths(-Random.Shared.Next(1, 24)),
                });
            }

            // BQT members (cũng là cư dân)
            residents.Add(new User
            {
                Username = "bqt_head",
                FullName = "Hoàng Văn Thành",
                Email = "thanh.bqt@sunriseapartment.vn",
                IdentityCardNumber = "079085000001",
                DateOfBirth = new DateTime(1970, 1, 15),
                ApartmentId = targetApts[0].ApartmentId,
                PhoneNumber = "0901000010",
                ResidentType = ResidentType.Owner,
                ResidencyStatus = "Thường trú",
                Role = UserRole.BQT_Head,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = now,
                MoveInDate = now.AddYears(-3),
            });

            residents.Add(new User
            {
                Username = "bqt_member1",
                FullName = "Đặng Thị Hương",
                Email = "huong.bqt@sunriseapartment.vn",
                IdentityCardNumber = "079085000002",
                DateOfBirth = new DateTime(1975, 5, 20),
                ApartmentId = targetApts.Count > 1 ? targetApts[1].ApartmentId : targetApts[0].ApartmentId,
                PhoneNumber = "0901000011",
                ResidentType = ResidentType.Owner,
                ResidencyStatus = "Thường trú",
                Role = UserRole.BQT_Member,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = now,
                MoveInDate = now.AddYears(-2),
            });

            context.Users.AddRange(residents);
            await context.SaveChangesAsync();
        }

        var allResidents = await context.Users.Where(u => u.Role == UserRole.Resident || u.Role == UserRole.BQT_Head || u.Role == UserRole.BQT_Member).ToListAsync();
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Role == UserRole.Admin);

        // ========== 4. SERVICE TYPES ==========
        if (!await context.ServiceTypes.AnyAsync())
        {
            var serviceTypes = new List<ServiceType>
            {
                new ServiceType { ServiceTypeName = "Tiền điện", MeasurementUnit = "kWh", Description = "Phí điện sinh hoạt theo chỉ số công tơ", IsActive = true, CreatedAt = now },
                new ServiceType { ServiceTypeName = "Tiền nước", MeasurementUnit = "m³", Description = "Phí nước sinh hoạt theo chỉ số đồng hồ", IsActive = true, CreatedAt = now },
                new ServiceType { ServiceTypeName = "Phí quản lý", MeasurementUnit = "m²/tháng", Description = "Phí quản lý vận hành tòa nhà", IsActive = true, CreatedAt = now },
                new ServiceType { ServiceTypeName = "Phí gửi xe máy", MeasurementUnit = "xe/tháng", Description = "Phí gửi xe máy tại hầm", IsActive = true, CreatedAt = now },
                new ServiceType { ServiceTypeName = "Phí gửi ô tô", MeasurementUnit = "xe/tháng", Description = "Phí gửi ô tô tại hầm", IsActive = true, CreatedAt = now },
                new ServiceType { ServiceTypeName = "Internet", MeasurementUnit = "tháng", Description = "Gói Internet cáp quang 100Mbps", IsActive = true, CreatedAt = now },
                new ServiceType { ServiceTypeName = "Truyền hình cáp", MeasurementUnit = "tháng", Description = "Gói truyền hình cáp cơ bản", IsActive = true, CreatedAt = now },
                new ServiceType { ServiceTypeName = "Dọn vệ sinh", MeasurementUnit = "lần", Description = "Dịch vụ dọn dẹp căn hộ theo yêu cầu", IsActive = true, CreatedAt = now },
                new ServiceType { ServiceTypeName = "Giặt ủi", MeasurementUnit = "kg", Description = "Dịch vụ giặt ủi quần áo", IsActive = true, CreatedAt = now },
                new ServiceType { ServiceTypeName = "Sửa chữa nhỏ", MeasurementUnit = "lần", Description = "Sửa chữa điện nước, thiết bị nhỏ", IsActive = true, CreatedAt = now },
                new ServiceType { ServiceTypeName = "Chuyển đồ", MeasurementUnit = "lần", Description = "Hỗ trợ chuyển đồ nội khu", IsActive = true, CreatedAt = now },
                new ServiceType { ServiceTypeName = "Trông trẻ", MeasurementUnit = "giờ", Description = "Dịch vụ trông trẻ theo giờ", IsActive = true, CreatedAt = now },
            };
            context.ServiceTypes.AddRange(serviceTypes);
            await context.SaveChangesAsync();
        }

        var allServiceTypes = await context.ServiceTypes.ToListAsync();

        // ========== 5. SERVICE PRICES ==========
        if (!await context.ServicePrices.AnyAsync())
        {
            var prices = new Dictionary<string, decimal>
            {
                { "Tiền điện", 3500 },
                { "Tiền nước", 15000 },
                { "Phí quản lý", 18000 },
                { "Phí gửi xe máy", 150000 },
                { "Phí gửi ô tô", 1500000 },
                { "Internet", 200000 },
                { "Truyền hình cáp", 120000 },
                { "Dọn vệ sinh", 300000 },
                { "Giặt ủi", 35000 },
                { "Sửa chữa nhỏ", 150000 },
                { "Chuyển đồ", 200000 },
                { "Trông trẻ", 80000 },
            };

            var servicePrices = new List<ServicePrice>();
            foreach (var st in allServiceTypes)
            {
                if (prices.TryGetValue(st.ServiceTypeName, out var price))
                {
                    servicePrices.Add(new ServicePrice
                    {
                        ServiceTypeId = st.ServiceTypeId,
                        UnitPrice = price,
                        EffectiveFrom = new DateTime(2026, 1, 1),
                        Description = $"Bảng giá {st.ServiceTypeName} áp dụng từ 01/01/2026",
                        CreatedAt = now
                    });
                }
            }
            context.ServicePrices.AddRange(servicePrices);
            await context.SaveChangesAsync();
        }

        // ========== 6. AMENITY TYPES ==========
        if (!await context.AmenityTypes.AnyAsync())
        {
            var amenityTypes = new List<AmenityType>
            {
                new AmenityType { TypeName = "Thể thao", Description = "Các tiện ích thể thao, rèn luyện sức khỏe", IsActive = true, CreatedAt = now },
                new AmenityType { TypeName = "Giải trí", Description = "Các tiện ích giải trí, thư giãn", IsActive = true, CreatedAt = now },
                new AmenityType { TypeName = "Làm việc", Description = "Các tiện ích hỗ trợ công việc", IsActive = true, CreatedAt = now },
                new AmenityType { TypeName = "Trẻ em", Description = "Các tiện ích dành cho trẻ em", IsActive = true, CreatedAt = now },
            };
            context.AmenityTypes.AddRange(amenityTypes);
            await context.SaveChangesAsync();
        }

        var allAmenityTypes = await context.AmenityTypes.ToListAsync();

        // ========== 7. AMENITIES ==========
        if (!await context.Amenities.AnyAsync())
        {
            var amenities = new List<Amenity>
            {
                new Amenity { AmenityName = "Hồ bơi", AmenityTypeId = allAmenityTypes.First(t => t.TypeName == "Thể thao").AmenityTypeId, Location = "Tầng 3, Block A", Capacity = 50, PricePerHour = 50000, IsActive = true, Description = "Hồ bơi ngoài trời 25m x 10m", CreatedAt = now },
                new Amenity { AmenityName = "Phòng Gym", AmenityTypeId = allAmenityTypes.First(t => t.TypeName == "Thể thao").AmenityTypeId, Location = "Tầng 2, Block A", Capacity = 30, PricePerHour = 0, IsActive = true, Description = "Phòng tập gym đầy đủ thiết bị", CreatedAt = now },
                new Amenity { AmenityName = "Sân Tennis", AmenityTypeId = allAmenityTypes.First(t => t.TypeName == "Thể thao").AmenityTypeId, Location = "Tầng 5, Block B", Capacity = 4, PricePerHour = 150000, IsActive = true, Description = "Sân tennis tiêu chuẩn", CreatedAt = now },
                new Amenity { AmenityName = "Sân Cầu lông", AmenityTypeId = allAmenityTypes.First(t => t.TypeName == "Thể thao").AmenityTypeId, Location = "Tầng 5, Block B", Capacity = 4, PricePerHour = 100000, IsActive = true, Description = "2 sân cầu lông trong nhà", CreatedAt = now },
                new Amenity { AmenityName = "Phòng Yoga", AmenityTypeId = allAmenityTypes.First(t => t.TypeName == "Thể thao").AmenityTypeId, Location = "Tầng 2, Block A", Capacity = 20, PricePerHour = 0, IsActive = true, Description = "Phòng yoga và thiền", CreatedAt = now },
                new Amenity { AmenityName = "Phòng BBQ", AmenityTypeId = allAmenityTypes.First(t => t.TypeName == "Giải trí").AmenityTypeId, Location = "Tầng thượng Block A", Capacity = 20, PricePerHour = 500000, IsActive = true, Description = "Khu vực BBQ ngoài trời với view đẹp", CreatedAt = now },
                new Amenity { AmenityName = "Phòng Sauna", AmenityTypeId = allAmenityTypes.First(t => t.TypeName == "Giải trí").AmenityTypeId, Location = "Tầng 3, Block A", Capacity = 10, PricePerHour = 100000, IsActive = true, Description = "Phòng xông hơi khô và ướt", CreatedAt = now },
                new Amenity { AmenityName = "Phòng họp A", AmenityTypeId = allAmenityTypes.First(t => t.TypeName == "Làm việc").AmenityTypeId, Location = "Tầng 1, Block A", Capacity = 10, PricePerHour = 200000, IsActive = true, Description = "Phòng họp nhỏ 10 người", CreatedAt = now },
                new Amenity { AmenityName = "Phòng họp B", AmenityTypeId = allAmenityTypes.First(t => t.TypeName == "Làm việc").AmenityTypeId, Location = "Tầng 1, Block B", Capacity = 20, PricePerHour = 350000, IsActive = true, Description = "Phòng họp lớn 20 người", CreatedAt = now },
                new Amenity { AmenityName = "Khu vui chơi trẻ em", AmenityTypeId = allAmenityTypes.First(t => t.TypeName == "Trẻ em").AmenityTypeId, Location = "Tầng 2, Block B", Capacity = 30, PricePerHour = 0, IsActive = true, Description = "Khu vui chơi trong nhà cho trẻ", CreatedAt = now },
            };
            context.Amenities.AddRange(amenities);
            await context.SaveChangesAsync();
        }

        var allAmenities = await context.Amenities.ToListAsync();

        // ========== 8. AMENITY BOOKINGS ==========
        if (!await context.AmenityBookings.AnyAsync() && allResidents.Any() && allAmenities.Any())
        {
            var bookings = new List<AmenityBooking>();
            var statuses = new[] { "Pending", "Confirmed", "Completed", "Cancelled" };

            for (int i = 0; i < 30; i++)
            {
                var resident = allResidents[Random.Shared.Next(allResidents.Count)];
                var amenity = allAmenities[Random.Shared.Next(allAmenities.Count)];
                var bookingDate = now.AddDays(Random.Shared.Next(-30, 30));
                var startHour = Random.Shared.Next(6, 20);
                var duration = Random.Shared.Next(1, 4);

                bookings.Add(new AmenityBooking
                {
                    AmenityId = amenity.AmenityId,
                    ApartmentId = resident.ApartmentId ?? targetApts[0].ApartmentId,
                    ResidentId = resident.UserId,
                    BookingDate = bookingDate.Date,
                    StartTime = new TimeSpan(startHour, 0, 0),
                    EndTime = new TimeSpan(startHour + duration, 0, 0),
                    TotalHours = duration,
                    TotalAmount = (amenity.PricePerHour ?? 0) * duration,
                    Status = statuses[Random.Shared.Next(statuses.Length)],
                    Notes = i % 3 == 0 ? "Đặt cho sinh nhật" : null,
                    CreatedAt = now.AddDays(-Random.Shared.Next(1, 60))
                });
            }
            context.AmenityBookings.AddRange(bookings);
            await context.SaveChangesAsync();
        }

        // ========== 9. VEHICLES ==========
        if (!await context.Vehicles.AnyAsync() && allResidents.Any())
        {
            var vehicleData = new[]
            {
                ("51A-123.45", "Ô tô", "Toyota Camry", "Trắng"),
                ("51G-234.56", "Ô tô", "Mercedes C200", "Đen"),
                ("51F-345.67", "Ô tô", "BMW X3", "Xám"),
                ("51H-456.78", "Ô tô", "Mazda CX-5", "Đỏ"),
                ("51K-567.89", "Ô tô", "VinFast Lux A", "Trắng"),
                ("51L-678.90", "Ô tô", "Honda CR-V", "Bạc"),
                ("59B1-111.22", "Xe máy", "Honda SH", "Đen"),
                ("59C1-222.33", "Xe máy", "Honda Vision", "Đỏ"),
                ("59D1-333.44", "Xe máy", "Yamaha Grande", "Trắng"),
                ("59E1-444.55", "Xe máy", "Honda Lead", "Xanh"),
                ("59F1-555.66", "Xe máy", "Honda Wave", "Đen"),
                ("59G1-666.77", "Xe máy", "Yamaha Exciter", "Đỏ đen"),
                ("59H1-777.88", "Xe máy", "Honda Air Blade", "Trắng xanh"),
                ("59K1-888.99", "Xe máy", "Vespa Primavera", "Xanh mint"),
                ("59L1-999.00", "Xe máy", "Piaggio Liberty", "Trắng"),
            };

            var vehicles = new List<Vehicle>();
            for (int i = 0; i < vehicleData.Length && i < allResidents.Count; i++)
            {
                var (plate, type, brand, color) = vehicleData[i];
                vehicles.Add(new Vehicle
                {
                    LicensePlate = plate,
                    VehicleType = type,
                    Brand = brand,
                    Color = color,
                    ResidentId = allResidents[i % allResidents.Count].UserId,
                    CreatedAt = now.AddMonths(-Random.Shared.Next(1, 12))
                });
            }
            context.Vehicles.AddRange(vehicles);
            await context.SaveChangesAsync();
        }

        // ========== 10. CONTRACTS ==========
        if (!await context.Contracts.AnyAsync() && adminUser != null)
        {
            var contracts = new List<Contract>();
            var contractApts = targetApts.Take(15).ToList();

            for (int i = 0; i < contractApts.Count; i++)
            {
                var apt = contractApts[i];
                var contractType = i % 3 == 0 ? ContractType.Rental : ContractType.Purchase;
                var startDate = now.AddYears(-Random.Shared.Next(1, 3)).AddMonths(-Random.Shared.Next(0, 12));

                contracts.Add(new Contract
                {
                    ContractNumber = $"HD-{startDate.Year}-{i + 1:D3}",
                    ApartmentId = apt.ApartmentId,
                    ContractType = contractType,
                    StartDate = startDate,
                    EndDate = contractType == ContractType.Rental ? startDate.AddYears(1) : null,
                    MonthlyRent = contractType == ContractType.Rental ? 10000000m + (apt.Area ?? 50) * 100000 : null,
                    DepositAmount = contractType == ContractType.Rental ? 20000000m : null,
                    Status = ContractStatus.Active,
                    Terms = "Hợp đồng theo quy định của Ban Quản Lý",
                    SignedDate = startDate,
                    CreatedBy = adminUser.UserId,
                    CreatedAt = startDate
                });
            }
            context.Contracts.AddRange(contracts);
            await context.SaveChangesAsync();

            // Contract Members
            var allContracts = await context.Contracts.ToListAsync();
            var contractMembers = new List<ContractMember>();
            foreach (var contract in allContracts)
            {
                var residentsInApt = allResidents.Where(r => r.ApartmentId == contract.ApartmentId).ToList();
                bool isFirst = true;
                foreach (var resident in residentsInApt)
                {
                    contractMembers.Add(new ContractMember
                    {
                        ContractId = contract.ContractId,
                        ResidentId = resident.UserId,
                        MemberRole = isFirst ? MemberRole.ContractOwner : MemberRole.FamilyMember,
                        SignatureStatus = SignatureStatus.Signed,
                        SignedDate = contract.SignedDate,
                        IsActive = true,
                        CreatedAt = contract.CreatedAt
                    });
                    isFirst = false;
                }
            }
            if (contractMembers.Any())
            {
                context.ContractMembers.AddRange(contractMembers);
                await context.SaveChangesAsync();
            }
        }

        // ========== 10b. RESIDENT APARTMENTS (chạy độc lập, không phụ thuộc Contracts seed lần đầu) ==========
        if (!await context.ResidentApartments.AnyAsync() && allResidents.Any())
        {
            var residentApts = new List<ResidentApartment>();

            foreach (var resident in allResidents)
            {
                if (!resident.ApartmentId.HasValue) continue;

                var contract = await context.Contracts
                    .FirstOrDefaultAsync(c => c.ApartmentId == resident.ApartmentId && c.Status == ContractStatus.Active);

                if (contract == null) continue;

                var residencyType = resident.ResidentType switch
                {
                    ResidentType.Owner => ResidencyType.Owner,
                    ResidentType.Tenant => ResidencyType.Tenant,
                    ResidentType.FamilyMember => ResidencyType.FamilyMember,
                    _ => ResidencyType.Other
                };

                residentApts.Add(new ResidentApartment
                {
                    UserId = resident.UserId,
                    ApartmentId = resident.ApartmentId.Value,
                    ContractId = contract.ContractId,
                    ResidencyType = residencyType,
                    IsActive = true,
                    MoveInDate = resident.MoveInDate ?? now.AddMonths(-12),
                    CreatedAt = now
                });
            }

            if (residentApts.Any())
            {
                context.ResidentApartments.AddRange(residentApts);
                await context.SaveChangesAsync();
            }
        }

        // ========== 11. INVOICES ==========
        if (!await context.Invoices.AnyAsync() && adminUser != null && targetApts.Any())
        {
            var invoices = new List<Invoice>();
            var invoiceApts = targetApts.Take(20).ToList();

            foreach (var apt in invoiceApts)
            {
                for (int month = 1; month <= 2; month++) // Tháng 1 và 2 năm 2026
                {
                    var totalAmount = 1500000m + (apt.Area ?? 50) * 18000 + Random.Shared.Next(500000, 2000000);
                    var status = month == 1 ? InvoiceStatus.Paid : (Random.Shared.Next(2) == 0 ? InvoiceStatus.Issued : InvoiceStatus.Overdue);
                    
                    invoices.Add(new Invoice
                    {
                        InvoiceNumber = $"INV-2026{month:D2}-{apt.ApartmentNumber.Replace("-", "")}",
                        ApartmentId = apt.ApartmentId,
                        BillingMonth = month,
                        BillingYear = 2026,
                        IssueDate = new DateTime(2026, month, 5),
                        DueDate = new DateTime(2026, month, 20),
                        TotalAmount = totalAmount,
                        PaidAmount = status == InvoiceStatus.Paid ? totalAmount : 0,
                        Status = status,
                        CreatedBy = adminUser.UserId,
                        CreatedAt = new DateTime(2026, month, 1)
                    });
                }
            }
            context.Invoices.AddRange(invoices);
            await context.SaveChangesAsync();
        }

        // ========== 12. RESIDENT CARDS ==========
        if (!await context.ResidentCards.AnyAsync() && allResidents.Any())
        {
            var cards = new List<ResidentCard>();
            int cardNum = 1;
            foreach (var resident in allResidents.Take(20))
            {
                cards.Add(new ResidentCard
                {
                    CardNumber = $"CARD-{resident.ApartmentId:D4}-{cardNum:D3}",
                    CardType = resident.ResidentType == ResidentType.Owner ? CardType.Resident : CardType.Secondary,
                    ResidentId = resident.UserId,
                    IssuedDate = now.AddMonths(-Random.Shared.Next(1, 24)),
                    ExpiryDate = now.AddYears(5),
                    Status = CardStatus.Active,
                    Notes = null,
                    CreatedAt = now.AddMonths(-Random.Shared.Next(1, 24))
                });
                cardNum++;
            }
            context.ResidentCards.AddRange(cards);
            await context.SaveChangesAsync();
        }

        // ========== 13. SERVICE ORDERS ==========
        if (!await context.ServiceOrders.AnyAsync() && allResidents.Any() && allServiceTypes.Any())
        {
            var serviceOrders = new List<ServiceOrder>();
            var onDemandServices = allServiceTypes.Where(s => 
                s.ServiceTypeName == "Dọn vệ sinh" || 
                s.ServiceTypeName == "Giặt ủi" || 
                s.ServiceTypeName == "Sửa chữa nhỏ" ||
                s.ServiceTypeName == "Chuyển đồ").ToList();

            // Guard against empty onDemandServices
            if (!onDemandServices.Any()) onDemandServices = allServiceTypes.Take(3).ToList();

            for (int i = 0; i < 20; i++)
            {
                var resident = allResidents[Random.Shared.Next(allResidents.Count)];
                var service = onDemandServices[Random.Shared.Next(onDemandServices.Count)];
                var requestedDate = now.AddDays(Random.Shared.Next(-30, 15));
                var status = requestedDate < now ? ServiceOrderStatus.Completed : ServiceOrderStatus.Pending;

                serviceOrders.Add(new ServiceOrder
                {
                    OrderNumber = $"SO-{requestedDate:yyyyMMdd}-{i + 1:D3}",
                    ApartmentId = resident.ApartmentId ?? targetApts[0].ApartmentId,
                    ResidentId = resident.UserId,
                    ServiceTypeId = service.ServiceTypeId,
                    RequestedDate = requestedDate,
                    RequestedTimeSlot = new[] { "Sáng", "Chiều", "Tối" }[Random.Shared.Next(3)],
                    Description = $"Yêu cầu {service.ServiceTypeName.ToLower()}",
                    Status = status,
                    EstimatedPrice = 200000m + Random.Shared.Next(100000, 500000),
                    Rating = status == ServiceOrderStatus.Completed ? Random.Shared.Next(3, 6) : null,
                    CreatedAt = requestedDate.AddDays(-Random.Shared.Next(1, 7))
                });
            }
            context.ServiceOrders.AddRange(serviceOrders);
            await context.SaveChangesAsync();
        }

        // ========== 14. REQUESTS ==========
        if (!await context.Requests.AnyAsync() && allResidents.Any())
        {
            var requests = new List<Request>();
            var requestTitles = new[]
            {
                ("Điều hòa không lạnh", RequestType.Repair, RequestPriority.High),
                ("Rò rỉ nước bồn rửa", RequestType.Repair, RequestPriority.Emergency),
                ("Đèn hành lang hỏng", RequestType.Repair, RequestPriority.Normal),
                ("Tiếng ồn từ tầng trên", RequestType.Complaint, RequestPriority.Normal),
                ("Thang máy hay bị kẹt", RequestType.Complaint, RequestPriority.High),
                ("Đề xuất thêm ghế công viên", RequestType.Feedback, RequestPriority.Low),
                ("Wifi lobby yếu", RequestType.Feedback, RequestPriority.Normal),
                ("Cửa kính bị nứt", RequestType.Repair, RequestPriority.High),
                ("Bồn cầu bị tắc", RequestType.Repair, RequestPriority.Emergency),
                ("Nước nóng không chạy", RequestType.Repair, RequestPriority.High),
            };

            for (int i = 0; i < requestTitles.Length; i++)
            {
                var (title, type, priority) = requestTitles[i];
                var resident = allResidents[Random.Shared.Next(allResidents.Count)];
                var createdDate = now.AddDays(-Random.Shared.Next(1, 30));
                var status = Random.Shared.Next(3) == 0 ? RequestStatus.Completed : 
                             (Random.Shared.Next(2) == 0 ? RequestStatus.InProgress : RequestStatus.Pending);

                requests.Add(new Request
                {
                    RequestNumber = $"REQ-{createdDate:yyyyMM}-{i + 1:D3}",
                    ApartmentId = resident.ApartmentId ?? targetApts[0].ApartmentId,
                    ResidentId = resident.UserId,
                    RequestType = type,
                    Title = title,
                    Description = $"Chi tiết: {title}. Vui lòng kiểm tra và xử lý sớm.",
                    Priority = priority,
                    Status = status,
                    CreatedAt = createdDate,
                    ResolvedAt = status == RequestStatus.Completed ? createdDate.AddDays(Random.Shared.Next(1, 7)) : null
                });
            }
            context.Requests.AddRange(requests);
            await context.SaveChangesAsync();
        }

        // ========== 15. VISITORS ==========
        if (!await context.Visitors.AnyAsync() && allResidents.Any())
        {
            var visitorNames = new[]
            {
                ("Nguyễn Văn An", "0901111111", "079095111222"),
                ("Trần Thị Bích", "0902222222", "079088222333"),
                ("Lê Văn Cường", "0903333333", "079090333444"),
                ("Phạm Thị Dung", "0904444444", "079085444555"),
                ("Hoàng Văn Em", "0905555555", "079092555666"),
            };

            var visitors = new List<Visitor>();
            for (int i = 0; i < 15; i++)
            {
                var (name, phone, idCard) = visitorNames[i % visitorNames.Length];
                var resident = allResidents[Random.Shared.Next(allResidents.Count)];
                var visitDate = now.AddDays(Random.Shared.Next(-7, 7));
                var status = visitDate < now ? VisitorStatus.CheckedOut : VisitorStatus.Pending;

                visitors.Add(new Visitor
                {
                    VisitorName = name,
                    PhoneNumber = phone,
                    IdentityCard = idCard,
                    ApartmentId = resident.ApartmentId ?? targetApts[0].ApartmentId,
                    RegisteredBy = resident.UserId,
                    VisitDate = visitDate.Date,
                    CheckInTime = status == VisitorStatus.CheckedOut ? visitDate.Date.AddHours(Random.Shared.Next(8, 18)) : null,
                    CheckOutTime = status == VisitorStatus.CheckedOut ? visitDate.Date.AddHours(Random.Shared.Next(18, 22)) : null,
                    Status = status,
                    Notes = i % 3 == 0 ? "Thăm người thân" : "Giao hàng",
                    CreatedAt = visitDate.AddDays(-1)
                });
            }
            context.Visitors.AddRange(visitors);
            await context.SaveChangesAsync();
        }

        // ========== 16. ANNOUNCEMENTS ==========
        if (!await context.Announcements.AnyAsync() && adminUser != null)
        {
            var announcements = new List<Announcement>
            {
                new Announcement { Title = "Lịch cúp điện bảo trì ngày 20/02/2026", Content = "Thông báo đến quý cư dân, ngày 20/02/2026 từ 8h-12h sẽ tạm ngừng cấp điện để bảo trì hệ thống điện tòa nhà.", AnnouncementType = AnnouncementType.Maintenance, Priority = AnnouncementPriority.High, PublishedDate = now, CreatedBy = adminUser.UserId, CreatedAt = now },
                new Announcement { Title = "Thông báo họp cư dân Quý 1/2026", Content = "Ban Quản Trị kính mời quý cư dân tham dự buổi họp thường niên Quý 1/2026 vào ngày 28/02/2026 lúc 19h tại sảnh tầng 1.", AnnouncementType = AnnouncementType.General, Priority = AnnouncementPriority.Normal, PublishedDate = now.AddDays(-5), CreatedBy = adminUser.UserId, CreatedAt = now.AddDays(-5) },
                new Announcement { Title = "Quy định mới về gửi xe từ 01/03/2026", Content = "Từ ngày 01/03/2026, phí gửi xe máy tăng lên 200.000đ/tháng và ô tô 1.800.000đ/tháng.", AnnouncementType = AnnouncementType.Finance, Priority = AnnouncementPriority.Normal, PublishedDate = now.AddDays(-10), CreatedBy = adminUser.UserId, CreatedAt = now.AddDays(-10) },
                new Announcement { Title = "Thông báo bảo trì thang máy Block A", Content = "Thang máy số 2 Block A sẽ được bảo trì định kỳ từ ngày 18-19/02/2026. Quý cư dân vui lòng sử dụng thang máy số 1.", AnnouncementType = AnnouncementType.Maintenance, Priority = AnnouncementPriority.High, PublishedDate = now.AddDays(-3), CreatedBy = adminUser.UserId, CreatedAt = now.AddDays(-3) },
                new Announcement { Title = "Lịch phun thuốc diệt côn trùng", Content = "Ngày 22/02/2026, BQL sẽ tiến hành phun thuốc diệt côn trùng tại các tầng hầm và khu vực công cộng.", AnnouncementType = AnnouncementType.General, Priority = AnnouncementPriority.Normal, PublishedDate = now.AddDays(-7), CreatedBy = adminUser.UserId, CreatedAt = now.AddDays(-7) },
            };
            context.Announcements.AddRange(announcements);
            await context.SaveChangesAsync();
        }

        // ========== 17. NOTIFICATIONS ==========
        if (!await context.Notifications.AnyAsync() && allResidents.Any())
        {
            var notifications = new List<Notification>();
            foreach (var resident in allResidents.Take(10))
            {
                notifications.Add(new Notification
                {
                    UserId = resident.UserId,
                    Title = "Hóa đơn tháng 02/2026",
                    Content = "Hóa đơn tháng 02/2026 đã được tạo. Vui lòng thanh toán trước ngày 20/02/2026.",
                    NotificationType = NotificationType.Invoice,
                    ReferenceType = ReferenceType.Invoice,
                    IsRead = Random.Shared.Next(2) == 0,
                    Priority = NotificationPriority.Normal,
                    CreatedAt = now.AddDays(-5)
                });
            }
            context.Notifications.AddRange(notifications);
            await context.SaveChangesAsync();
        }
    }
}
