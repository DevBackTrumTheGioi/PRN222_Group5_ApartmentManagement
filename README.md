# PRN222 Apartment Management System

Hệ thống quản lý chung cư được xây dựng bằng ASP.NET Core 8.0 Razor Pages với Entity Framework Core.

## Yêu Cầu Hệ Thống

- .NET 8.0 SDK
- SQL Server 2016 hoặc cao hơn (hoặc SQL Server Express)
- Visual Studio 2022 / JetBrains Rider (khuyến nghị)
- SQL Server Management Studio (tùy chọn)

## Cấu Hình Connection String

1. Mở file `appsettings.json`
2. Cập nhật connection string phù hợp với SQL Server của bạn:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ApartmentManagementDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Ví dụ connection strings:
- **SQL Server Express LocalDB**: `Server=(localdb)\\mssqllocaldb;Database=ApartmentManagementDB;Trusted_Connection=True;TrustServerCertificate=True`
- **SQL Server Express**: `Server=localhost\\SQLEXPRESS;Database=ApartmentManagementDB;Trusted_Connection=True;TrustServerCertificate=True`
- **SQL Server với authentication**: `Server=YOUR_SERVER;Database=ApartmentManagementDB;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True`

## Khởi Tạo Database Lần Đầu

### Bước 1: Di chuyển vào thư mục project

```bash
cd PRN222_ApartmentManagement
```

### Bước 2: Cài đặt EF Core Tools (nếu chưa có)

```bash
dotnet tool install --global dotnet-ef
```

Hoặc cập nhật lên phiên bản mới nhất:

```bash
dotnet tool update --global dotnet-ef
```

### Bước 3: Verify EF Tools

```bash
dotnet ef
```

### Bước 4: Apply Migrations để tạo Database

```bash
dotnet ef database update
```

Lệnh này sẽ:
- Tạo database `ApartmentManagementDB` nếu chưa tồn tại
- Chạy tất cả migrations trong thư mục `Migrations/`
- Tạo các bảng: Users, Apartments, Residents, Invoices, Requests, Notifications, v.v.

### Bước 5: Verify Database

Kiểm tra database đã được tạo bằng SQL Server Management Studio hoặc:

```bash
dotnet ef database update --verbose
```

## Làm Việc Với Entity Framework Migrations

### Tạo Migration Mới

Khi bạn thay đổi models (thêm/sửa/xóa properties hoặc entities):

```bash
dotnet ef migrations add TenMigration
```

Ví dụ:
```bash
dotnet ef migrations add AddPhoneToResident
dotnet ef migrations add UpdateInvoiceStatus
dotnet ef migrations add RemoveMessagingSystem
```

### Áp Dụng Migration

Sau khi tạo migration, áp dụng vào database:

```bash
dotnet ef database update
```

### Xem Danh Sách Migrations

```bash
dotnet ef migrations list
```

### Rollback Migration

Quay lại migration trước đó:

```bash
dotnet ef database update TenMigrationTruocDo
```

Ví dụ:
```bash
dotnet ef database update InitialDB
```

### Xóa Migration Chưa Apply

Nếu bạn vừa tạo migration nhưng chưa chạy `update`:

```bash
dotnet ef migrations remove
```

**Lưu ý**: Chỉ xóa được migration cuối cùng và chưa được apply vào database.

### Xem SQL Script của Migration

Xem SQL code mà migration sẽ thực thi:

```bash
dotnet ef migrations script
```

Hoặc tạo script từ migration A đến B:

```bash
dotnet ef migrations script MigrationA MigrationB
```

### Tạo Script SQL để Deploy

Tạo file SQL từ tất cả migrations:

```bash
dotnet ef migrations script --output migrations.sql
```

### Xóa Database Hoàn Toàn

**CẢNH BÁO: Lệnh này sẽ xóa toàn bộ database và dữ liệu!**

```bash
dotnet ef database drop
```

Với force (không hỏi xác nhận):

```bash
dotnet ef database drop --force
```

### Reset Database (Xóa và Tạo Lại)

```bash
dotnet ef database drop --force
dotnet ef database update
```

## Migrations Hiện Có Trong Project

1. **20260209095900_InitialDB** - Tạo toàn bộ database schema ban đầu
2. **20260209100120_UserIsDeleteColumn** - Thêm cột IsDeleted vào bảng Users
3. **20260209110452_RemoveMessagingSystem** - Xóa hệ thống messaging (Conversations, Messages, MessageReactions, MessageReadReceipts, ConversationParticipants)

## Các Lệnh Hữu Ích Khác

### Build Project

```bash
dotnet build
```

### Clean Build

```bash
dotnet clean
dotnet build
```

### Run Project

```bash
dotnet run
```

Hoặc với watch mode (auto-reload khi code thay đổi):

```bash
dotnet watch run
```

### Kiểm Tra EF Core Context

```bash
dotnet ef dbcontext info
```

### Tạo Database Schema Diagram

```bash
dotnet ef dbcontext scaffold "YourConnectionString" Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Data
```

## Troubleshooting

### Lỗi: "Unable to create an object of type 'ApartmentDbContext'"

Đảm bảo connection string trong `appsettings.json` đúng và SQL Server đang chạy.

### Lỗi: "Login failed for user"

Kiểm tra lại username/password trong connection string hoặc dùng `Trusted_Connection=True` cho Windows Authentication.

### Lỗi: "A network-related or instance-specific error"

Kiểm tra:
- SQL Server Service đang chạy
- Firewall không chặn port 1433
- Server name đúng (localhost, ., (localdb)\mssqllocaldb)

### Migration bị conflict

Nếu có nhiều người cùng tạo migration:

1. Pull code mới nhất từ git
2. Xóa migration của bạn: `dotnet ef migrations remove`
3. Apply migrations từ người khác: `dotnet ef database update`
4. Tạo lại migration của bạn: `dotnet ef migrations add YourMigration`

### Database đã có sẵn nhưng không đúng schema

```bash
dotnet ef database drop --force
dotnet ef database update
```

## Best Practices

1. **Luôn backup database trước khi drop**: Sử dụng SQL Server Management Studio để backup
2. **Đặt tên migration có ý nghĩa**: `AddEmailToUser` thay vì `Update1`
3. **Review migration code**: Kiểm tra file migration trước khi apply
4. **Commit migration vào git**: Để team đồng bộ schema
5. **Không sửa migration đã apply**: Tạo migration mới để sửa
6. **Test migration trên dev trước**: Đừng apply trực tiếp lên production

## Database Schema

Xem file `db.dbml` để hiểu rõ cấu trúc database và các mối quan hệ giữa các bảng.

## Liên Hệ

Nếu gặp vấn đề, liên hệ team qua:
- Issue tracker trên GitHub
- Email: [your-email@example.com]
