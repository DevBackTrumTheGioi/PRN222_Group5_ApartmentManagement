# Plan: Gộp bảng Resident vào User với Role = Resident

Hiện tại `Resident` đang kế thừa từ `User` theo mô hình TPT (Table-Per-Type), tạo 2 bảng riêng biệt. Plan này sẽ gộp các cột của `Resident` vào `User`, xóa bảng `Residents`, và cập nhật toàn bộ code liên quan bao gồm Face Auth module.

## Steps

### 1. Thêm các cột từ Resident vào User.cs

**File:** `PRN222_ApartmentManagement/Models/User.cs`

Thêm các properties sau:
- `DateOfBirth` (DateTime?)
- `IdentityCardNumber` (string?, MaxLength 20)
- `ResidentType` (ResidentType?)
- `ResidencyStatus` (string?, MaxLength 20)
- `ApartmentId` (int?) - FK đến Apartment
- `MoveInDate` (DateTime?)
- `MoveOutDate` (DateTime?)
- `Note` (string?, MaxLength 500)
- `FaceDescriptor` (string?) - JSON chứa face descriptor vector
- `IsFaceRegistered` (bool, default false)

Thêm navigation properties:
- `Apartment` (Apartment?)
- `Vehicles` (ICollection<Vehicle>)
- `ResidentCards` (ICollection<ResidentCard>)
- `Requests` (ICollection<Request>)
- `RegisteredVisitors` (ICollection<Visitor>)
- `AmenityBookings` (ICollection<AmenityBooking>)
- `ContractMembers` (ICollection<ContractMember>)
- `ServiceOrders` (ICollection<ServiceOrder>)

### 2. Xóa Resident.cs và cập nhật ApartmentDbContext.cs

**File cần xóa:** `PRN222_ApartmentManagement/Models/Resident.cs`

**File cần sửa:** `PRN222_ApartmentManagement/Data/ApartmentDbContext.cs`

- Xóa `DbSet<Resident> Residents`
- Xóa `.UseTptMappingStrategy()` 
- Xóa `.ToTable("Residents")`
- Xóa cấu hình index cho `Resident.IdentityCardNumber` → chuyển sang `User`
- Cập nhật tất cả relationship từ `Resident` sang `User`:
  - `Resident` → `Apartment` relationship
  - `ResidentCard` → `Resident` relationship  
  - `Vehicle` → `Resident` relationship
  - `Request` → `Resident` relationship
  - `Visitor` → `RegisteredByResident` relationship
  - `ServiceOrder` → `Resident` relationship

### 3. Cập nhật các Model có FK đến Resident

Thay đổi navigation property type từ `Resident` sang `User`:

| File | Property cần sửa |
|------|------------------|
| `Models/Vehicle.cs` | `Resident` → `User` |
| `Models/ResidentCard.cs` | `Resident` → `User` |
| `Models/Request.cs` | `Resident` → `User` |
| `Models/AmenityBooking.cs` | `Resident` → `User` |
| `Models/Visitor.cs` | `RegisteredByResident` → `RegisteredByUser` |
| `Models/ContractMember.cs` | `Resident` → `User` |
| `Models/ServiceOrder.cs` | `Resident` → `User` |
| `Models/FaceAuthHistory.cs` | `Resident` → `User` |

### 4. Cập nhật Face Auth Module

**Files cần sửa:**

#### 4.1. RegisterModel.cshtml.cs
- Thay `_context.Residents.FindAsync(userId)` → `_context.Users.FindAsync(userId)`
- Kiểm tra `user.Role == UserRole.Resident`

#### 4.2. Test.cshtml.cs
- Thay `_context.Residents.Where(r => r.IsFaceRegistered...)` → `_context.Users.Where(u => u.Role == UserRole.Resident && u.IsFaceRegistered...)`

#### 4.3. StatusModel.cshtml.cs
- Thay `Models.Resident` → `Models.User`
- Thay `_context.Residents.FindAsync(userId)` → `_context.Users.FindAsync(userId)`

#### 4.4. HistoryModel.cshtml.cs
- Không cần thay đổi lớn (chỉ query FaceAuthHistories)

#### 4.5. Status.cshtml (View)
- Thay `Model.Resident` → `Model.User` hoặc tương ứng với property name mới

### 5. Xóa/Cập nhật Repository

**Files cần xóa:**
- `Repositories/Interfaces/IResidentRepository.cs`
- `Repositories/Implementations/ResidentRepository.cs`

**Files cần sửa:**

#### ServiceOrderRepository.cs
- Thay tất cả `.Include(so => so.Resident)` → `.Include(so => so.User)` (hoặc tên property mới)

### 6. Cập nhật Utils

**File:** `Utils/ValidationUtils.cs`
- Cập nhật method `ValidateResident()` để validate User có Role=Resident

**File:** `Utils/DataSeeder.cs`
- Cập nhật seed data nếu cần

### 7. Tạo Migration

```bash
dotnet ef migrations add MergeResidentIntoUser
```

**Trong migration cần thêm data migration script:**

```csharp
// Trong method Up()
// 1. Thêm các cột mới vào Users
// 2. Copy data từ Residents sang Users
migrationBuilder.Sql(@"
    UPDATE u
    SET u.DateOfBirth = r.DateOfBirth,
        u.IdentityCardNumber = r.IdentityCardNumber,
        u.ResidentType = r.ResidentType,
        u.ResidencyStatus = r.ResidencyStatus,
        u.ApartmentId = r.ApartmentId,
        u.MoveInDate = r.MoveInDate,
        u.MoveOutDate = r.MoveOutDate,
        u.Note = r.Note,
        u.FaceDescriptor = r.FaceDescriptor,
        u.IsFaceRegistered = r.IsFaceRegistered
    FROM Users u
    INNER JOIN Residents r ON u.UserId = r.UserId
");
// 3. Xóa bảng Residents
```

## Considerations

### Data Migration
- Cần backup database trước khi chạy migration
- Tạo SQL script để copy data từ `Residents` sang các cột mới trong `Users` trước khi xóa bảng `Residents`
- Nên làm trong migration bằng `migrationBuilder.Sql()`

### Nullable Columns
Các cột mới như `ApartmentId`, `FaceDescriptor`... nên để **nullable** vì không phải User nào cũng là Resident:
- Admin không cần thông tin cư dân
- Staff không cần FaceDescriptor
- Chỉ User có `Role = Resident` mới sử dụng các trường này

### Authorization
- Các page trong `/Resident/FaceAuth/` vẫn giữ `[Authorize(Roles = "Resident")]`
- Logic kiểm tra quyền không thay đổi

### Breaking Changes
- API/Service nào đang dùng `Resident` entity cần cập nhật
- Các query dùng `_context.Residents` cần đổi sang `_context.Users.Where(u => u.Role == UserRole.Resident)`

## Files Summary

| Action | Files |
|--------|-------|
| **Modify** | `Models/User.cs` |
| **Delete** | `Models/Resident.cs` |
| **Modify** | `Data/ApartmentDbContext.cs` |
| **Modify** | `Models/Vehicle.cs`, `Models/ResidentCard.cs`, `Models/Request.cs`, `Models/AmenityBooking.cs`, `Models/Visitor.cs`, `Models/ContractMember.cs`, `Models/ServiceOrder.cs`, `Models/FaceAuthHistory.cs` |
| **Modify** | `Pages/Resident/FaceAuth/RegisterModel.cshtml.cs`, `Test.cshtml.cs`, `StatusModel.cshtml.cs`, `Status.cshtml` |
| **Delete** | `Repositories/Interfaces/IResidentRepository.cs`, `Repositories/Implementations/ResidentRepository.cs` |
| **Modify** | `Repositories/Implementations/ServiceOrderRepository.cs` |
| **Modify** | `Utils/ValidationUtils.cs` |
| **Create** | New Migration file |

