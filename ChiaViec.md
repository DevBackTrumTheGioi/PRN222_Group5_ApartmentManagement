# Phân Chia Công Việc - Group 5
## Project: Apartment Management System

---

## Thành Viên Nhóm

| STT | Họ Tên | Skill Level | Vai Trò |
|:---:|:---|:---:|:---|
| 1 | Thành viên A | Giỏi | Tech Lead - Full-stack Developer |
| 2 | Thành viên B | Giỏi | Senior Developer |
| 3 | Thành viên C | Trung Bình | Mid-level Developer |
| 4 | Thành viên D | Kém | Junior Developer |
| 5 | Thành viên E | Kém | Junior Developer |

---

## Nguyên Tắc Phân Chia

1. Mỗi thành viên phụ trách 1 module hoàn chỉnh (từ database → backend → frontend)
2. Độ khó tương ứng với năng lực của từng người
3. Các module có mối liên hệ để dễ tích hợp
4. Junior developers làm CRUD đơn giản, Senior làm business logic phức tạp

---

## Phân Công Chi Tiết

### **Thành viên A** (Giỏi) - Tech Lead
**Module: Service & Billing Management (Core Business Logic)**

#### Use Cases Đảm Nhiệm:
- UC-18: Record Meter Readings
- UC-19: Register Apt Services
- UC-20: Generate Invoice (Complex calculation)
- UC-21: View Invoices
- UC-22: Process Payment
- UC-23: View Payment History

#### Deliverables:
**1. Database & Models** (Đã có - Review & Optimize)
- ServiceType
- ServicePrice
- MeterReading
- Invoice
- InvoiceDetail
- PaymentTransaction
- ApartmentService

**2. Backend - Repositories & Services**
- `IServiceTypeRepository` + Implementation
- `IServicePriceRepository` + Implementation
- `IMeterReadingRepository` + Implementation
- `IInvoiceRepository` + Implementation
  - Method: `GenerateMonthlyInvoicesAsync(int month, int year)` - Complex logic
  - Method: `CalculateInvoiceAmountAsync(int apartmentId, int month, int year)`
  - Method: `GetOverdueInvoicesAsync()`
- `IPaymentTransactionRepository` + Implementation
- `IApartmentServiceRepository` + Implementation

**3. Frontend - Razor Pages**
- `/Pages/Invoices/Index.cshtml` - List invoices với filter
- `/Pages/Invoices/Details.cshtml` - Invoice chi tiết
- `/Pages/Invoices/Generate.cshtml` - Generate invoices hàng loạt
- `/Pages/MeterReadings/Index.cshtml` - List meter readings
- `/Pages/MeterReadings/Create.cshtml` - Input meter readings
- `/Pages/Payments/Process.cshtml` - Xử lý thanh toán
- `/Pages/Payments/History.cshtml` - Lịch sử thanh toán

**4. Advanced Features**
- Auto-calculate invoice từ meter readings
- Payment status automation
- Invoice PDF export (optional)
- Payment statistics dashboard

**Thời gian ước tính:** 7-10 ngày

---

### **Thành viên B** (Giỏi) - Senior Developer
**Module: Request Management & Notifications (Complex Workflow)**

#### Use Cases Đảm Nhiệm:
- UC-24: Submit Request
- UC-25: Process Request (với workflow state machine)
- UC-26: Post Announcement
- UC-27: View Notifications (Real-time)

#### Deliverables:
**1. Database & Models** (Đã có - Review)
- Request
- RequestAttachment
- Announcement
- Notification

**2. Backend - Repositories & Services**
- `IRequestRepository` + Implementation
  - Method: `GetRequestsByStatusAsync(int status)`
  - Method: `AssignRequestToStaffAsync(int requestId, int staffId)`
  - Method: `UpdateRequestStatusAsync(int requestId, int newStatus)`
  - Method: `GetRequestStatisticsAsync()` - Dashboard data
- `IRequestAttachmentRepository` + Implementation
- `IAnnouncementRepository` + Implementation
  - Method: `GetActiveAnnouncementsAsync()`
  - Method: `PublishAnnouncementAsync(Announcement announcement)`
- `INotificationRepository` + Implementation
  - Method: `CreateNotificationForUserAsync(int userId, Notification notification)`
  - Method: `CreateNotificationForRoleAsync(string role, Notification notification)`
  - Method: `GetUnreadNotificationsAsync(int userId)`
  - Method: `MarkAsReadAsync(int notificationId)`

**3. Frontend - Razor Pages**
- `/Pages/Requests/Index.cshtml` - List requests với filter by status
- `/Pages/Requests/Create.cshtml` - Submit request với file upload
- `/Pages/Requests/Details.cshtml` - View & update request status
- `/Pages/Requests/Assign.cshtml` - Assign to staff
- `/Pages/Announcements/Index.cshtml` - List announcements
- `/Pages/Announcements/Create.cshtml` - Post announcement
- `/Pages/Notifications/Index.cshtml` - Notification center
- `/Pages/Shared/_NotificationPartial.cshtml` - Notification dropdown

**4. Advanced Features**
- File upload for request attachments
- Request workflow state machine
- Notification system (SignalR for real-time - optional)
- Announcement broadcast to all users

**Thời gian ước tính:** 7-10 ngày

---

### **Thành viên C** (Trung Bình) - Mid-level Developer
**Module: Apartment & Resident Management (Standard CRUD với relationships)**

#### Use Cases Đảm Nhiệm:
- UC-10: View Apartment List
- UC-11: Manage Residents
- UC-12: Manage Resident Cards
- UC-13: Manage Vehicles

#### Deliverables:
**1. Database & Models** (Đã có - Review)
- Apartment
- Resident
- ResidentCard
- Vehicle

**2. Backend - Repositories & Services**
- `IApartmentRepository` + Implementation (Đã có - Complete)
  - Method: `GetApartmentsByFloorAsync(int floor)`
  - Method: `GetApartmentsByStatusAsync(string status)`
  - Method: `UpdateApartmentStatusAsync(int id, string status)`
- `IResidentRepository` + Implementation
  - Method: `GetResidentsByApartmentAsync(int apartmentId)`
  - Method: `GetResidentByUserIdAsync(int userId)`
  - Method: `UpdateResidencyStatusAsync(int userId, string status)`
- `IResidentCardRepository` + Implementation
  - Method: `GetCardsByResidentAsync(int residentId)`
  - Method: `DeactivateCardAsync(int cardId)`
  - Method: `GetActiveCardsAsync()`
- `IVehicleRepository` + Implementation
  - Method: `GetVehiclesByResidentAsync(int residentId)`
  - Method: `GetVehicleByLicensePlateAsync(string licensePlate)`

**3. Frontend - Razor Pages**
- `/Pages/Apartments/Index.cshtml` - List apartments với filter
- `/Pages/Apartments/Details.cshtml` - View apartment details
- `/Pages/Residents/Index.cshtml` - List residents
- `/Pages/Residents/Create.cshtml` - Add resident
- `/Pages/Residents/Edit.cshtml` - Update resident info
- `/Pages/Residents/Details.cshtml` - View resident profile
- `/Pages/ResidentCards/Index.cshtml` - Manage cards
- `/Pages/ResidentCards/Create.cshtml` - Issue new card
- `/Pages/Vehicles/Index.cshtml` - List vehicles
- `/Pages/Vehicles/Create.cshtml` - Register vehicle

**4. Features**
- Apartment filtering by floor, status
- Resident move-in/move-out processing
- Card activation/deactivation
- Vehicle registration with validation

**Thời gian ước tính:** 6-8 ngày

---

### **Thành viên D** (Kém) - Junior Developer
**Module: User Authentication & System Administration (Đơn giản, có template)**

#### Use Cases Đảm Nhiệm:
- UC-01: User Login
- UC-02: View Dashboard
- UC-03: Update Profile
- UC-04: Change Password
- UC-05: Manage Users

#### Deliverables:
**1. Database & Models** (Đã có)
- User

**2. Backend - Repositories & Services**
- `IUserRepository` + Implementation
  - Method: `GetUserByUsernameAsync(string username)`
  - Method: `ValidateCredentialsAsync(string username, string password)`
  - Method: `UpdatePasswordAsync(int userId, string newPasswordHash)`
  - Method: `UpdateProfileAsync(int userId, User user)`
  - Method: `GetUsersByRoleAsync(string role)`
  - Method: `ActivateUserAsync(int userId)`
  - Method: `DeactivateUserAsync(int userId)`

**3. Frontend - Razor Pages**
- `/Pages/Account/Login.cshtml` - Login form
- `/Pages/Account/Logout.cshtml` - Logout handler
- `/Pages/Dashboard/Index.cshtml` - Role-based dashboard (4 loại: Admin, Staff, Resident, Security)
- `/Pages/Profile/Index.cshtml` - View profile
- `/Pages/Profile/Edit.cshtml` - Update profile
- `/Pages/Profile/ChangePassword.cshtml` - Change password form
- `/Pages/Users/Index.cshtml` - List users (Admin only)
- `/Pages/Users/Create.cshtml` - Create user
- `/Pages/Users/Edit.cshtml` - Edit user

**4. Features**
- Session-based authentication
- Role-based authorization
- Password hashing (BCrypt hoặc Identity)
- Dashboard widgets (summary statistics)

**Hỗ trợ từ Tech Lead:**
- Authentication middleware setup
- Password hashing utilities
- Authorization policies

**Thời gian ước tính:** 5-7 ngày

---

### **Thành viên E** (Kém) - Junior Developer
**Module: Visitor & Amenity Management (CRUD đơn giản)**

#### Use Cases Đảm Nhiệm:
- UC-14: Register Visitor
- UC-28: Book Amenity
- UC-29: Manage Parcels
- UC-07: Manage Service Types (Admin)
- UC-09: Manage Amenities (Admin)

#### Deliverables:
**1. Database & Models** (Đã có)
- Visitor
- Amenity
- AmenityBooking
- Parcel
- ServiceType (simple CRUD)

**2. Backend - Repositories & Services**
- `IVisitorRepository` + Implementation
  - Method: `GetVisitorsByApartmentAsync(int apartmentId)`
  - Method: `GetVisitorsByDateAsync(DateTime date)`
  - Method: `CheckInVisitorAsync(int visitorId)`
  - Method: `CheckOutVisitorAsync(int visitorId)`
- `IAmenityRepository` + Implementation
  - Method: `GetActiveAmenitiesAsync()`
  - Method: `GetAmenityByIdAsync(int id)`
- `IAmenityBookingRepository` + Implementation
  - Method: `GetBookingsByAmenityAsync(int amenityId, DateTime date)`
  - Method: `CheckAvailabilityAsync(int amenityId, DateTime date, TimeSpan start, TimeSpan end)`
  - Method: `CancelBookingAsync(int bookingId)`
- `IParcelRepository` + Implementation
  - Method: `GetParcelsByApartmentAsync(int apartmentId)`
  - Method: `GetPendingParcelsAsync()`
  - Method: `MarkAsPickedUpAsync(int parcelId, int residentId)`

**3. Frontend - Razor Pages**
- `/Pages/Visitors/Index.cshtml` - List visitors
- `/Pages/Visitors/Register.cshtml` - Register visitor
- `/Pages/Visitors/CheckIn.cshtml` - Check-in form
- `/Pages/Amenities/Index.cshtml` - List amenities
- `/Pages/Amenities/Create.cshtml` - Add amenity (Admin)
- `/Pages/Amenities/Book.cshtml` - Book amenity time slot
- `/Pages/AmenityBookings/MyBookings.cshtml` - View my bookings
- `/Pages/Parcels/Index.cshtml` - List parcels
- `/Pages/Parcels/Receive.cshtml` - Receive parcel
- `/Pages/ServiceTypes/Index.cshtml` - List service types (Admin)
- `/Pages/ServiceTypes/Create.cshtml` - Add service type

**4. Features**
- Visitor check-in/check-out
- Time slot booking for amenities
- Parcel notification
- Simple CRUD for service types

**Hỗ trợ từ Senior Developer:**
- Time slot validation logic
- QR code generation for visitors (optional)

**Thời gian ước tính:** 5-7 ngày

---

## Module Không Assign (Tạm hoãn hoặc không triển khai)

### Contract Management (UC-15, UC-16, UC-17)
- **Lý do:** Module phức tạp, cần nhiều business rules
- **Trạng thái:** Models đã có, chưa implement Repository/Pages
- **Có thể assign:** Thành viên A hoặc B nếu còn thời gian

### System Administration - Advanced (UC-06, UC-08)
- **UC-06: Manage Roles** - Dynamic permission (quá phức tạp cho beginner project)
- **UC-08: Manage Service Prices** - Có thể integrate vào module của Thành viên A

---

## Timeline Dự Kiến

### Week 1: Setup & Core Modules
- **All:** Setup project, review database schema
- **Thành viên A:** Start Invoice & Payment repositories
- **Thành viên B:** Start Request & Notification repositories
- **Thành viên C:** Start Apartment & Resident repositories
- **Thành viên D:** Setup Authentication & User management
- **Thành viên E:** Start Visitor & Amenity repositories

### Week 2: Backend Completion
- **All:** Complete repositories & unit tests
- **Thành viên A & B:** Review code của Junior developers
- **Daily standup:** Report progress & blockers

### Week 3: Frontend Development
- **All:** Create Razor Pages
- **Thành viên B:** Setup shared layouts & components
- **Thành viên D & E:** Follow UI templates from seniors

### Week 4: Integration & Testing
- **All:** Integration testing
- **Thành viên A:** Setup CI/CD pipeline (optional)
- **Thành viên B:** Setup notification system
- **All:** Bug fixing & code review

---

## Tiêu Chí Đánh Giá

### Cho Thành Viên Giỏi (A, B):
- Code quality (SOLID, DRY principles)
- Complex business logic implementation
- Code review cho junior members
- Technical leadership
- Advanced features (real-time, calculations, workflows)

### Cho Thành Viên Trung Bình (C):
- Standard CRUD implementation
- Entity relationships handling
- Code consistency
- Basic validation & error handling

### Cho Thành Viên Kém (D, E):
- Complete assigned features
- Follow coding standards
- Basic functionality working
- Learn from code reviews
- Improvement over time

---

## Quy Tắc Làm Việc

1. **Git workflow:**
   - Main branch: Protected
   - Feature branches: `feature/module-name`
   - Pull request required, reviewed by seniors
   - Merge after approval

2. **Code standards:**
   - Follow C# naming conventions
   - XML documentation for public methods
   - No emojis in code/docs
   - Repository pattern mandatory

3. **Daily tasks:**
   - Commit code daily
   - Update progress in group chat
   - Ask for help when stuck
   - Review merge requests from teammates

4. **Meetings:**
   - Daily standup (15 mins): What did you do? What will you do? Any blockers?
   - Weekly review: Demo completed features
   - Code review sessions: Learn from each other

---

## Hỗ Trợ Giữa Các Thành Viên

### Thành viên A hỗ trợ:
- Thành viên D: Authentication setup, password hashing
- Thành viên E: Complex queries nếu cần

### Thành viên B hỗ trợ:
- Thành viên C: Entity relationships, navigation properties
- Thành viên E: File upload, time validation logic

### Thành viên C hỗ trợ:
- Thành viên D & E: Basic CRUD patterns, Razor Pages syntax

---

## Tài Liệu Cần Tham Khảo

1. **db.dbml** - Database schema
2. **README.md** - Setup instructions
3. **UseCase.md** - Use case specifications
4. **Existing code:**
   - `ApartmentRepository.cs` - Repository pattern example
   - `ApartmentDbContext.cs` - DbContext usage
   - Models - Entity examples

---

## Liên Hệ & Báo Cáo

- **Tech Lead (Thành viên A):** Quyết định kỹ thuật cuối cùng
- **Senior Developer (Thành viên B):** Code review, giải đáp thắc mắc
- **Group chat:** Daily updates
- **GitHub Issues:** Track bugs & features
- **Weekly meeting:** Friday 7PM - Demo & retrospective

---

**Ghi chú:** Phân công này có thể điều chỉnh dựa trên tiến độ thực tế và năng lực của từng thành viên sau Week 1.

**Last updated:** February 9, 2026

