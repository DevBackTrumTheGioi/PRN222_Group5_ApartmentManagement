# 📋 Project Tracking - Apartment Management System

> **Last Updated:** 01/02/2026  
> **Project:** PRN222_ApartmentManagement  
> **Technology:** .NET Core 8.0 + Razor Pages + SQL Server + SignalR

---

## 📊 Tổng quan Dự án

| **Phase** | **Mô tả** | **Số Screen** | **Tiến độ** | **Trạng thái** |
|-----------|-----------|---------------|-------------|----------------|
| Phase 1 | Authentication & Layout | 5 | 0% | ⬜ Not Started |
| Phase 2 | Quản lý Căn hộ & Cư dân | 12 | 0% | ⬜ Not Started |
| Phase 3 | Dịch vụ & Hóa đơn | 14 | 0% | ⬜ Not Started |
| Phase 4 | Yêu cầu & Thông báo | 10 | 0% | ⬜ Not Started |
| Phase 5 | Tiện ích & Cộng đồng | 12 | 0% | ⬜ Not Started |
| Phase 6 | Messaging & Real-time | 6 | 0% | ⬜ Not Started |
| Phase 7 | Báo cáo & Dashboard | 6 | 0% | ⬜ Not Started |
| **TOTAL** | | **65** | **0%** | |

---

## 🗄️ Database Schema

### Danh sách Tables (25 bảng)

| **#** | **Table Name** | **Module** | **Mô tả** |
|-------|----------------|------------|-----------|
| 1 | Apartments | Core | Thông tin căn hộ |
| 2 | Residents | Core | Thông tin cư dân |
| 3 | ResidentCards | Core | Thẻ cư dân/thang máy |
| 4 | Vehicles | Core | Phương tiện đăng ký |
| 5 | Users | Auth | Tài khoản đăng nhập |
| 6 | ServiceTypes | Billing | Loại dịch vụ/phí |
| 7 | UtilityReadings | Billing | Chỉ số điện/nước |
| 8 | Invoices | Billing | Hóa đơn |
| 9 | InvoiceDetails | Billing | Chi tiết hóa đơn |
| 10 | Requests | Service | Yêu cầu sửa chữa/phản ánh |
| 11 | Announcements | Community | Thông báo |
| 12 | Notifications | Community | Thông báo cá nhân |
| 13 | Visitors | Security | Khách đến thăm |
| 14 | Parcels | Security | Bưu phẩm |
| 15 | Amenities | Facility | Tiện ích chung |
| 16 | AmenityBookings | Facility | Đặt chỗ tiện ích |
| 17 | Documents | Community | Tài liệu/Sổ tay |
| 18 | Conversations | Messaging | Cuộc hội thoại |
| 19 | ConversationParticipants | Messaging | Thành viên hội thoại |
| 20 | Messages | Messaging | Tin nhắn |
| 21 | MessageReadReceipts | Messaging | Trạng thái đã đọc |
| 22 | MessageReactions | Messaging | Reaction tin nhắn |

---

## 🔐 PHASE 1: Authentication & Common Layout

| **ID** | **Screen Name** | **Route** | **Role** | **Priority** | **Status** | **Assignee** | **Start** | **End** | **Notes** |
|--------|-----------------|-----------|----------|--------------|------------|--------------|-----------|---------|-----------|
| 1.1 | Login Page | `/Login` | All | 🔴 High | ⬜ | | | | JWT Auth |
| 1.2 | Register Page | `/Register` | Public | 🔴 High | ⬜ | | | | Resident self-register |
| 1.3 | Forgot Password | `/ForgotPassword` | All | 🟡 Medium | ⬜ | | | | Email reset |
| 1.4 | Reset Password | `/ResetPassword` | All | 🟡 Medium | ⬜ | | | | Token validation |
| 1.5 | Main Layout/Shell | `_Layout.cshtml` | All | 🔴 High | ⬜ | | | | Sidebar, Header, Footer |

---

## 🏠 PHASE 2: Quản lý Căn hộ & Cư dân

### 2A. Admin/Staff Screens

| **ID** | **Screen Name** | **Route** | **Role** | **Priority** | **Status** | **Assignee** | **Start** | **End** | **DB Tables** |
|--------|-----------------|-----------|----------|--------------|------------|--------------|-----------|---------|---------------|
| 2.1 | Dashboard Admin | `/Admin/Dashboard` | Admin | 🔴 High | ⬜ | | | | All |
| 2.2 | Apartment List | `/Admin/Apartments` | Admin, Staff | 🔴 High | ⬜ | | | | Apartments |
| 2.3 | Apartment Create/Edit | `/Admin/Apartments/Edit/{id?}` | Admin | 🔴 High | ⬜ | | | | Apartments |
| 2.4 | Apartment Detail | `/Admin/Apartments/Detail/{id}` | Admin, Staff | 🔴 High | ⬜ | | | | Apartments, Residents |
| 2.5 | Resident List | `/Admin/Residents` | Admin, Staff | 🔴 High | ⬜ | | | | Residents |
| 2.6 | Resident Create/Edit | `/Admin/Residents/Edit/{id?}` | Admin | 🔴 High | ⬜ | | | | Residents |
| 2.7 | Resident Detail | `/Admin/Residents/Detail/{id}` | Admin, Staff | 🔴 High | ⬜ | | | | Residents, Vehicles |
| 2.8 | Vehicle List | `/Admin/Vehicles` | Admin, Staff | 🟡 Medium | ⬜ | | | | Vehicles |
| 2.9 | Vehicle Create/Edit | `/Admin/Vehicles/Edit/{id?}` | Admin | 🟡 Medium | ⬜ | | | | Vehicles |
| 2.10 | Resident Card List | `/Admin/ResidentCards` | Admin, Staff | 🟡 Medium | ⬜ | | | | ResidentCards |
| 2.11 | Resident Card Create/Edit | `/Admin/ResidentCards/Edit/{id?}` | Admin | 🟡 Medium | ⬜ | | | | ResidentCards |
| 2.12 | User Management | `/Admin/Users` | Admin | 🔴 High | ⬜ | | | | Users |

---

## 💰 PHASE 3: Dịch vụ & Hóa đơn

### 3A. Admin/Staff Screens

| **ID** | **Screen Name** | **Route** | **Role** | **Priority** | **Status** | **Assignee** | **Start** | **End** | **DB Tables** |
|--------|-----------------|-----------|----------|--------------|------------|--------------|-----------|---------|---------------|
| 3.1 | Service Type List | `/Admin/ServiceTypes` | Admin | 🔴 High | ⬜ | | | | ServiceTypes |
| 3.2 | Service Type Create/Edit | `/Admin/ServiceTypes/Edit/{id?}` | Admin | 🔴 High | ⬜ | | | | ServiceTypes |
| 3.3 | Utility Reading List | `/Admin/UtilityReadings` | Admin, Staff | 🔴 High | ⬜ | | | | UtilityReadings |
| 3.4 | Utility Reading Entry | `/Admin/UtilityReadings/Entry` | Staff | 🔴 High | ⬜ | | | | UtilityReadings |
| 3.5 | Utility Reading Import | `/Admin/UtilityReadings/Import` | Admin | 🟡 Medium | ⬜ | | | | Excel Import |
| 3.6 | Invoice List | `/Admin/Invoices` | Admin, Staff | 🔴 High | ⬜ | | | | Invoices |
| 3.7 | Invoice Generate | `/Admin/Invoices/Generate` | Admin | 🔴 High | ⬜ | | | | Invoices, InvoiceDetails |
| 3.8 | Invoice Detail | `/Admin/Invoices/Detail/{id}` | Admin, Staff | 🔴 High | ⬜ | | | | Invoices, InvoiceDetails |
| 3.9 | Invoice Payment Confirm | `/Admin/Invoices/ConfirmPayment/{id}` | Admin, Staff | 🔴 High | ⬜ | | | | Invoices |
| 3.10 | Payment History | `/Admin/Payments` | Admin | 🟡 Medium | ⬜ | | | | Invoices |

### 3B. Resident Screens

| **ID** | **Screen Name** | **Route** | **Role** | **Priority** | **Status** | **Assignee** | **Start** | **End** | **DB Tables** |
|--------|-----------------|-----------|----------|--------------|------------|--------------|-----------|---------|---------------|
| 3.11 | My Invoices | `/Resident/Invoices` | Resident | 🔴 High | ⬜ | | | | Invoices |
| 3.12 | Invoice Detail | `/Resident/Invoices/Detail/{id}` | Resident | 🔴 High | ⬜ | | | | Invoices, InvoiceDetails |
| 3.13 | Online Payment | `/Resident/Invoices/Pay/{id}` | Resident | 🔴 High | ⬜ | | | | VNPay/Momo Integration |
| 3.14 | Payment History | `/Resident/PaymentHistory` | Resident | 🟡 Medium | ⬜ | | | | Invoices |

---

## 📋 PHASE 4: Yêu cầu & Thông báo

### 4A. Admin/Staff Screens

| **ID** | **Screen Name** | **Route** | **Role** | **Priority** | **Status** | **Assignee** | **Start** | **End** | **DB Tables** |
|--------|-----------------|-----------|----------|--------------|------------|--------------|-----------|---------|---------------|
| 4.1 | Request List | `/Admin/Requests` | Admin, Staff | 🔴 High | ⬜ | | | | Requests |
| 4.2 | Request Detail | `/Admin/Requests/Detail/{id}` | Admin, Staff | 🔴 High | ⬜ | | | | Requests |
| 4.3 | Request Assign/Update | `/Admin/Requests/Update/{id}` | Admin | 🔴 High | ⬜ | | | | Requests |
| 4.4 | Announcement List | `/Admin/Announcements` | Admin | 🔴 High | ⬜ | | | | Announcements |
| 4.5 | Announcement Create/Edit | `/Admin/Announcements/Edit/{id?}` | Admin | 🔴 High | ⬜ | | | | Announcements |

### 4B. Resident Screens

| **ID** | **Screen Name** | **Route** | **Role** | **Priority** | **Status** | **Assignee** | **Start** | **End** | **DB Tables** |
|--------|-----------------|-----------|----------|--------------|------------|--------------|-----------|---------|---------------|
| 4.6 | My Requests | `/Resident/Requests` | Resident | 🔴 High | ⬜ | | | | Requests |
| 4.7 | Create Request | `/Resident/Requests/Create` | Resident | 🔴 High | ⬜ | | | | Requests |
| 4.8 | Request Detail | `/Resident/Requests/Detail/{id}` | Resident | 🔴 High | ⬜ | | | | Requests |
| 4.9 | Announcements | `/Resident/Announcements` | Resident | 🔴 High | ⬜ | | | | Announcements |
| 4.10 | Notifications | `/Resident/Notifications` | Resident | 🟡 Medium | ⬜ | | | | Notifications |

---

## 🏢 PHASE 5: Tiện ích & Cộng đồng

### 5A. Staff/Security Screens

| **ID** | **Screen Name** | **Route** | **Role** | **Priority** | **Status** | **Assignee** | **Start** | **End** | **DB Tables** |
|--------|-----------------|-----------|----------|--------------|------------|--------------|-----------|---------|---------------|
| 5.1 | Visitor List | `/Staff/Visitors` | Staff | 🔴 High | ⬜ | | | | Visitors |
| 5.2 | Visitor Check-in/out | `/Staff/Visitors/CheckIn` | Staff | 🔴 High | ⬜ | | | | Visitors |
| 5.3 | Parcel List | `/Staff/Parcels` | Staff | 🔴 High | ⬜ | | | | Parcels |
| 5.4 | Parcel Receive | `/Staff/Parcels/Receive` | Staff | 🔴 High | ⬜ | | | | Parcels |
| 5.5 | Parcel Handover | `/Staff/Parcels/Handover/{id}` | Staff | 🔴 High | ⬜ | | | | Parcels |

### 5B. Admin Screens

| **ID** | **Screen Name** | **Route** | **Role** | **Priority** | **Status** | **Assignee** | **Start** | **End** | **DB Tables** |
|--------|-----------------|-----------|----------|--------------|------------|--------------|-----------|---------|---------------|
| 5.6 | Amenity List | `/Admin/Amenities` | Admin | 🟡 Medium | ⬜ | | | | Amenities |
| 5.7 | Amenity Create/Edit | `/Admin/Amenities/Edit/{id?}` | Admin | 🟡 Medium | ⬜ | | | | Amenities |
| 5.8 | Amenity Booking List | `/Admin/AmenityBookings` | Admin, Staff | 🟡 Medium | ⬜ | | | | AmenityBookings |
| 5.9 | Document Management | `/Admin/Documents` | Admin | 🟡 Medium | ⬜ | | | | Documents |

### 5C. Resident Screens

| **ID** | **Screen Name** | **Route** | **Role** | **Priority** | **Status** | **Assignee** | **Start** | **End** | **DB Tables** |
|--------|-----------------|-----------|----------|--------------|------------|--------------|-----------|---------|---------------|
| 5.10 | Register Visitor | `/Resident/Visitors/Register` | Resident | 🔴 High | ⬜ | | | | Visitors, QR Code |
| 5.11 | My Parcels | `/Resident/Parcels` | Resident | 🟡 Medium | ⬜ | | | | Parcels |
| 5.12 | Book Amenity | `/Resident/Amenities` | Resident | 🟡 Medium | ⬜ | | | | Amenities, AmenityBookings |
| 5.13 | Documents/Handbook | `/Resident/Documents` | Resident | 🟢 Low | ⬜ | | | | Documents |

---

## 💬 PHASE 6: Messaging & Real-time (SignalR)

| **ID** | **Screen Name** | **Route** | **Role** | **Priority** | **Status** | **Assignee** | **Start** | **End** | **DB Tables** |
|--------|-----------------|-----------|----------|--------------|------------|--------------|-----------|---------|---------------|
| 6.1 | Conversation List | `/Messages` | All | 🟡 Medium | ⬜ | | | | Conversations, ConversationParticipants |
| 6.2 | Chat View | `/Messages/Chat/{conversationId}` | All | 🟡 Medium | ⬜ | | | | Messages, MessageReadReceipts |
| 6.3 | New Conversation | `/Messages/New` | All | 🟡 Medium | ⬜ | | | | Conversations |
| 6.4 | Create Group Chat | `/Messages/CreateGroup` | All | 🟢 Low | ⬜ | | | | Conversations |
| 6.5 | Group Settings | `/Messages/GroupSettings/{id}` | All | 🟢 Low | ⬜ | | | | ConversationParticipants |
| 6.6 | SignalR Chat Hub | `/hubs/chat` | All | 🟡 Medium | ⬜ | | | | Real-time connection |

---

## 📊 PHASE 7: Báo cáo & Dashboard

| **ID** | **Screen Name** | **Route** | **Role** | **Priority** | **Status** | **Assignee** | **Start** | **End** | **Notes** |
|--------|-----------------|-----------|----------|--------------|------------|--------------|-----------|---------|-----------|
| 7.1 | Revenue Report | `/Admin/Reports/Revenue` | Admin | 🟡 Medium | ⬜ | | | | Charts, Export |
| 7.2 | Occupancy Report | `/Admin/Reports/Occupancy` | Admin | 🟡 Medium | ⬜ | | | | Apartments, Residents |
| 7.3 | Request Statistics | `/Admin/Reports/Requests` | Admin | 🟡 Medium | ⬜ | | | | Requests |
| 7.4 | Resident Dashboard | `/Resident/Dashboard` | Resident | 🔴 High | ⬜ | | | | Overview for resident |
| 7.5 | Staff Dashboard | `/Staff/Dashboard` | Staff | 🔴 High | ⬜ | | | | Daily tasks summary |
| 7.6 | Export Reports (PDF/Excel) | `/Admin/Reports/Export` | Admin | 🟢 Low | ⬜ | | | | Export functionality |

---

## 👤 Resident Profile Screens

| **ID** | **Screen Name** | **Route** | **Role** | **Priority** | **Status** | **Assignee** | **Start** | **End** | **DB Tables** |
|--------|-----------------|-----------|----------|--------------|------------|--------------|-----------|---------|---------------|
| R.1 | My Profile | `/Resident/Profile` | Resident | 🔴 High | ⬜ | | | | Residents, Users |
| R.2 | Edit Profile | `/Resident/Profile/Edit` | Resident | 🔴 High | ⬜ | | | | Residents |
| R.3 | My Vehicles | `/Resident/Vehicles` | Resident | 🟡 Medium | ⬜ | | | | Vehicles |
| R.4 | Change Password | `/Resident/ChangePassword` | Resident | 🔴 High | ⬜ | | | | Users |

---

## 📈 Tổng kết theo Role

| **Role** | **Total Screens** | **🔴 High** | **🟡 Medium** | **🟢 Low** |
|----------|-------------------|-------------|---------------|------------|
| Admin | 28 | 18 | 8 | 2 |
| Staff | 15 | 10 | 4 | 1 |
| Resident | 22 | 14 | 6 | 2 |
| All/Common | 10 | 6 | 3 | 1 |
| **TOTAL** | **65** | **38** | **18** | **9** |

---

## 🎯 Sprint Planning Suggestion

### Sprint 1 (2 weeks) - Foundation
- [ ] 1.1 Login Page
- [ ] 1.2 Register Page  
- [ ] 1.5 Main Layout
- [ ] 2.1 Dashboard Admin
- [ ] 2.12 User Management

### Sprint 2 (2 weeks) - Apartment & Resident CRUD
- [ ] 2.2 - 2.4 Apartment (List, Create/Edit, Detail)
- [ ] 2.5 - 2.7 Resident (List, Create/Edit, Detail)

### Sprint 3 (2 weeks) - Billing Core
- [ ] 3.1 - 3.2 Service Types
- [ ] 3.3 - 3.4 Utility Readings
- [ ] 3.6 - 3.9 Invoices (Admin)

### Sprint 4 (2 weeks) - Resident Portal
- [ ] 3.11 - 3.14 Invoices (Resident)
- [ ] 4.6 - 4.9 Requests (Resident)
- [ ] R.1 - R.4 Profile screens

### Sprint 5 (2 weeks) - Admin Service
- [ ] 4.1 - 4.5 Requests & Announcements (Admin)
- [ ] 5.1 - 5.5 Visitor & Parcel (Staff)

### Sprint 6 (2 weeks) - Extended Features
- [ ] 5.6 - 5.13 Amenities & Documents
- [ ] 2.8 - 2.11 Vehicles & Resident Cards

### Sprint 7 (2 weeks) - Messaging & Reports
- [ ] 6.1 - 6.6 Messaging System
- [ ] 7.1 - 7.6 Reports & Dashboards

### Sprint 8 (1 week) - Testing & Polish
- [ ] Integration Testing
- [ ] Bug fixes
- [ ] Performance optimization

---

## ✅ Status Legend

| Symbol | Status | Meaning |
|--------|--------|---------|
| ⬜ | Not Started | Chưa bắt đầu |
| 🟡 | In Progress | Đang thực hiện |
| 🔵 | In Review | Đang review/test |
| ✅ | Completed | Hoàn thành |
| ❌ | Blocked | Bị chặn/chờ |
| ⏸️ | On Hold | Tạm dừng |

## 🔴🟡🟢 Priority Legend

| Color | Priority | Meaning |
|-------|----------|---------|
| 🔴 | High | Bắt buộc, cần làm trước |
| 🟡 | Medium | Quan trọng, làm sau High |
| 🟢 | Low | Tùy chọn, làm nếu còn thời gian |

---

## 📝 Notes

- **Technology Stack:** .NET Core 8.0, Razor Pages, SQL Server, SignalR, Bootstrap 5
- **Authentication:** JWT Token / ASP.NET Identity
- **Payment Integration:** VNPay, Momo (Phase 3)
- **Real-time:** SignalR for messaging & notifications
- **Export:** PDF (iTextSharp), Excel (EPPlus/ClosedXML)

---

*Document generated based on DBML schema and DeXuatYTuong.md*

