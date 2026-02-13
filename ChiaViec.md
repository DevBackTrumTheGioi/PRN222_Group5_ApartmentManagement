# Phân Chia Công Việc - Nhóm 5
## Dự án: Hệ Thống Quản Lý Chung Cư
### Ngày: 13/02/2026 (Cập nhật theo CapNhatYTuong.md)

---

## 📋 Tổng Quan Vai Trò Mới

> **Lưu ý quan trọng:** Cấu trúc vai trò đã được cập nhật theo tài liệu `CapNhatYTuong.md`

| Vai Trò Cũ | Vai Trò Mới | Mô Tả |
|:-----------|:------------|:------|
| Admin | **Admin** | Quản trị viên hệ thống - Toàn quyền cấu hình |
| Staff | **BQL_Manager** | Trưởng Ban Quản Lý - Điều hành vận hành |
| Staff | **BQL_Staff** | Nhân viên Ban Quản Lý - Thực hiện công việc hàng ngày |
| Security | **BQL_Staff** | Gộp vào BQL_Staff |
| *(Mới)* | **BQT_Head** | Trưởng Ban Quản Trị - Đại diện cư dân, giám sát BQL |
| *(Mới)* | **BQT_Member** | Thành viên Ban Quản Trị - Hỗ trợ giám sát |
| Resident | **Resident** | Cư dân - Người sử dụng dịch vụ |

---

## 👤 MODULE 1 - Authentication & Admin System

### Module: Xác thực & Quản trị hệ thống

**Mô tả:** Xây dựng hệ thống đăng nhập, phân quyền, quản lý người dùng và cấu hình hệ thống.

### Danh sách chức năng:

#### 1.1 Authentication (Xác thực)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 1 | Đăng nhập | Đăng nhập bằng username/password | Tất cả |
| 2 | Đăng xuất | Logout và xóa session | Tất cả |
| 3 | Đổi mật kh���u | Cho phép user đổi password | Tất cả |
| 4 | Quên mật khẩu | Reset password qua email | Tất cả |
| 5 | Middleware phân quyền | Kiểm tra quyền truy cập theo role (6 vai trò mới) | Hệ thống |

#### 1.2 Quản lý người dùng (Admin)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 6 | CRUD User | Tạo, sửa, xóa, khóa tài khoản cho tất cả vai trò | Admin |
| 7 | Phân quyền vai trò | Gán role: Admin, BQL_Manager, BQL_Staff, BQT_Head, BQT_Member, Resident | Admin |
| 8 | Xem danh sách users | Danh sách + tìm kiếm + lọc theo vai trò | Admin |
| 9 | Reset mật khẩu user | Admin reset password cho user | Admin |

#### 1.3 Cấu hình hệ thống (Admin)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 10 | Cấu hình chung | Tên, logo, thông tin chung cư | Admin |
| 11 | Quản lý danh mục dịch vụ | CRUD ServiceTypes | Admin |
| 12 | Cấu hình giá dịch vụ | CRUD ServicePrices | Admin |
| 13 | Xem log hệ thống | Audit trail, activity log | Admin |
| 14 | Backup/Restore dữ liệu | Sao lưu và khôi phục dữ liệu hệ thống | Admin |

### Database Tables liên quan:
- `Users` (Cập nhật giá trị cột Role)
- `ServiceTypes`
- `ServicePrices`
- `ActivityLogs`

### ⚠️ Migration cần thực hiện:
```sql
-- Cập nhật vai trò mới
UPDATE Users SET Role = 'BQL_Staff' WHERE Role = 'Staff';
UPDATE Users SET Role = 'BQL_Staff' WHERE Role = 'Security';
```

### Pages cần tạo:
```
/Login
/Logout
/ChangePassword
/ForgotPassword
/Admin/Users (Index, Create, Edit, Delete)
/Admin/Settings
/Admin/ServiceTypes (Index, Create, Edit)
/Admin/ServicePrices (Index, Create, Edit)
/Admin/Logs
```

### Thời gian ước tính: **2 tuần**

---

## 👤 MODULE 2 - Tài chính & Thanh toán

### Module: Hóa đơn, Thanh toán, Báo cáo tài chính

**Mô tả:** Xây dựng luồng thu phí hoàn chỉnh từ ghi chỉ số → tạo hóa đơn → phê duyệt → thanh toán → báo cáo.

### Danh sách chức năng:

#### 2.1 Ghi chỉ số điện nước
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 1 | Nhập chỉ số | Ghi chỉ số điện, nước theo căn hộ | BQL_Staff |
| 2 | Import chỉ số từ Excel | Upload file Excel chỉ số | BQL_Staff |
| 3 | Xem lịch sử chỉ số | Tra cứu chỉ số các tháng | BQL_Manager, BQL_Staff |

#### 2.2 Quản lý hóa đơn
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 4 | Tạo hóa đơn tự động | Tính toán từ chỉ số + dịch vụ | BQL_Staff, System |
| 5 | Xem danh sách hóa đơn | Lọc theo tháng, trạng thái | BQL_Manager, BQL_Staff |
| 6 | **Phê duyệt hóa đơn** | Manager duyệt trước khi gửi (ApprovalStatus) | BQL_Manager |
| 7 | Gửi hóa đơn cho cư dân | Notification + Email | System |
| 8 | Xem hóa đơn (Cư dân) | Cư dân xem hóa đơn của mình | Resident |
| 9 | In hóa đơn/PDF | Xuất hóa đơn ra PDF | All |

#### 2.3 Thanh toán
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 10 | Thu tiền mặt | Ghi nhận thanh toán cash | BQL_Staff |
| 11 | Xác nhận chuyển khoản | Đối chiếu bank transfer | BQL_Staff |
| 12 | Thanh toán online | Tích hợp VNPay/Momo (optional) | Resident |
| 13 | Xem lịch sử thanh toán | Tra cứu giao dịch | All |
| 14 | In biên lai | Xuất biên lai thu tiền | BQL_Staff |

#### 2.4 Công nợ & Nhắc nợ
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 15 | Xem danh sách công nợ | Các hóa đơn chưa thanh toán | BQL_Manager |
| 16 | Gửi nhắc nợ | Notification/SMS nhắc thanh toán | BQL_Manager |
| 17 | Áp dụng phí phạt | Tính phí chậm thanh toán | System |

#### 2.5 Báo cáo tài chính
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 18 | Dashboard tài chính | Tổng thu, công nợ, biểu đồ | BQL_Manager, **BQT_Head** |
| 19 | Báo cáo thu chi theo tháng | Chi tiết doanh thu | BQL_Manager, **BQT_Head** |
| 20 | Xuất báo cáo Excel | Export báo cáo | BQL_Manager |
| 21 | **Xem báo cáo (BQT)** | BQT giám sát tài chính | **BQT_Head, BQT_Member (chỉ xem)** |

### ⚠️ Cột mới cần thêm cho bảng Invoices:
```sql
ALTER TABLE Invoices ADD ApprovalStatus integer DEFAULT 0;  -- 0:Pending, 1:Approved, 2:Rejected
ALTER TABLE Invoices ADD ApprovedBy integer NULL;
ALTER TABLE Invoices ADD ApprovedAt datetime NULL;
ALTER TABLE Invoices ADD RejectionReason nvarchar(500) NULL;
```

### Database Tables liên quan:
- `MeterReadings`
- `Invoices` (Thêm cột ApprovalStatus, ApprovedBy, ApprovedAt)
- `InvoiceDetails`
- `PaymentTransactions`

### Pages cần tạo:
```
/MeterReadings (Index, Create, Import)
/Invoices (Index, Create, Details, Approve)
/Invoices/MyInvoices (cho Resident)
/Payments (Index, Create, Confirm)
/Payments/History
/Reports/Finance (Dashboard, Monthly, Export)
/BQT/Reports/Finance (cho BQT_Head, BQT_Member - chỉ xem)
```

### Thời gian ước tính: **2.5 tuần**

---

## 👤 MODULE 3 - Quản lý Căn hộ & Cư dân

### Module: Căn hộ, Cư dân, Hợp đồng, Xe & Thẻ

**Mô tả:** Xây dựng chức năng quản lý thông tin căn hộ, cư dân, hợp đồng thuê/mua, phương tiện và thẻ ra vào.

### Danh sách chức năng:

#### 3.1 Quản lý căn hộ
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 1 | Xem danh sách căn hộ | Danh sách + tìm kiếm + lọc | BQL_Manager, BQL_Staff, **BQT_Head, BQT_Member** |
| 2 | Xem chi tiết căn hộ | Thông tin + cư dân + hóa đơn | BQL_Manager, BQL_Staff |
| 3 | Cập nhật thông tin căn hộ | Sửa diện tích, loại căn hộ | BQL_Manager |
| 4 | Dashboard căn hộ | Thống kê trạng thái căn hộ | BQL_Manager |

#### 3.2 Quản lý cư dân
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 5 | Xem danh sách cư dân | Danh sách + tìm kiếm | BQL_Manager, BQL_Staff, **BQT_Head** |
| 6 | Thêm cư dân mới | Đăng ký cư dân vào căn hộ | BQL_Staff |
| 7 | Cập nhật thông tin cư dân | Sửa thông tin cá nhân | BQL_Staff |
| 8 | Xóa/Vô hiệu hóa cư dân | Soft delete khi chuyển đi | BQL_Manager |
| 9 | Xem hồ sơ cá nhân | Cư dân xem thông tin mình | Resident |
| 10 | Cập nhật hồ sơ cá nhân | Cư dân sửa thông tin | Resident |
| 11 | **Phê duyệt chuyển đi/đến** | Xác nhận thủ tục chuyển căn hộ | BQL_Manager |

#### 3.3 Quản lý hợp đồng
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 12 | Xem danh sách hợp đồng | Lọc theo trạng thái | BQL_Manager |
| 13 | Tạo hợp đồng mới | Thuê/Mua căn hộ | BQL_Manager |
| 14 | Gia hạn hợp đồng | Extend thời hạn | BQL_Manager |
| 15 | Chấm dứt hợp đồng | Kết thúc hợp đồng | BQL_Manager |
| 16 | Xem hợp đồng (Cư dân) | Cư dân xem hợp đồng của mình | Resident |

#### 3.4 Quản lý xe & thẻ
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 17 | Đăng ký xe | Thêm xe cho cư dân | BQL_Staff |
| 18 | Xem danh sách xe | Lọc theo căn hộ, loại xe | BQL_Staff |
| 19 | Hủy đăng ký xe | Xóa xe khi bán/chuyển | BQL_Staff |
| 20 | Cấp thẻ cư dân | Tạo thẻ ra vào mới | BQL_Staff |
| 21 | Khóa/Mở khóa thẻ | Vô hiệu hóa thẻ bị mất | BQL_Staff |
| 22 | Xem xe của tôi | Cư dân xem danh sách xe | Resident |

### Database Tables liên quan:
- `Apartments`
- `Residents`
- `Contracts`
- `ContractMembers`
- `Vehicles`
- `ResidentCards`

### Pages cần tạo:
```
/Apartments (Index, Details, Edit)
/Residents (Index, Create, Edit, Details)
/Residents/Profile (cho Resident)
/Contracts (Index, Create, Edit, Extend, Terminate)
/Contracts/MyContract (cho Resident)
/Vehicles (Index, Create, Delete)
/Vehicles/MyVehicles (cho Resident)
/ResidentCards (Index, Create, Lock)
```

### Thời gian ước tính: **2 tuần**

---

## 👤 MODULE 4 - Yêu cầu & Thông báo

### Module: Yêu cầu sửa chữa, Khiếu nại, Thông báo

**Mô tả:** Xây dựng hệ thống tiếp nhận và xử lý yêu cầu từ cư dân, quản lý thông báo. **Bổ sung: Khiếu nại lên BQT, Escalation**.

### Danh sách chức năng:

#### 4.1 Yêu cầu sửa chữa (Cư dân)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 1 | Gửi yêu cầu mới | Tạo ticket sửa chữa/báo sự cố | Resident |
| 2 | Đính kèm ảnh/file | Upload hình ảnh mô tả | Resident |
| 3 | Xem yêu cầu của tôi | Danh sách yêu cầu đã gửi | Resident |
| 4 | Xem chi tiết yêu cầu | Trạng thái, comment, lịch sử | Resident |
| 5 | Đánh giá sau xử lý | Rate chất lượng dịch vụ | Resident |
| 6 | **Gửi khiếu nại lên BQT** | Khiếu nại trực tiếp đến Ban Quản Trị | Resident |

#### 4.2 Xử lý yêu cầu (Staff/Manager)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 7 | Xem tất cả yêu cầu | Danh sách + lọc theo trạng thái | BQL_Manager |
| 8 | Xem yêu cầu được giao | Danh sách công việc của mình | BQL_Staff |
| 9 | Phân công yêu cầu | Giao việc cho Staff | BQL_Manager |
| 10 | Cập nhật trạng thái | Đang xử lý, Hoàn thành | BQL_Staff |
| 11 | Thêm ghi chú/comment | Cập nhật tiến độ | BQL_Staff |
| 12 | Đóng yêu cầu | Xác nhận hoàn thành | BQL_Manager, BQL_Staff |
| 13 | **Escalate lên Manager** | Chuyển vấn đề phức tạp lên BQL_Manager | BQL_Staff |
| 14 | **Xử lý escalation** | Tiếp nhận và giải quyết vấn đề từ Staff | BQL_Manager |

#### 4.3 Khiếu nại lên BQT (Mới)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 15 | **Xem khiếu nại từ cư dân** | Danh sách khiếu nại gửi lên BQT | BQT_Head |
| 16 | **Phản hồi khiếu nại** | Trả lời và đưa ra hướng giải quyết | BQT_Head |
| 17 | **Chuyển khiếu nại cho BQL** | Yêu cầu BQL xử lý và báo cáo | BQT_Head |
| 18 | **Xem phản hồi cư dân** | Theo dõi ý kiến, góp ý từ cư dân | BQT_Head, BQT_Member |

#### 4.4 Thông báo
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 19 | Tạo thông báo (BQL) | Đăng thông báo vận hành | BQL_Manager |
| 20 | **Tạo thông báo (BQT)** | Đăng thông báo chính sách, quyết định | **BQT_Head** |
| 21 | Xem danh sách thông báo | Quản lý thông báo đã đăng | BQL_Manager |
| 22 | Sửa/Xóa thông báo | Chỉnh sửa nội dung | BQL_Manager |
| 23 | Ghim thông báo quan trọng | Pin to top | BQL_Manager, BQT_Head |
| 24 | Xem thông báo (Cư dân) | Danh sách thông báo | Resident |
| 25 | Đánh dấu đã đọc | Mark as read | Resident |

#### 4.5 Notification (Real-time)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 26 | Bell notification | Icon chuông + số chưa đọc | All |
| 27 | Dropdown notifications | Danh sách nhanh | All |
| 28 | Xem tất cả notifications | Trang full list | All |

### ⚠️ Cột mới cần thêm cho bảng Requests:
```sql
ALTER TABLE Requests ADD EscalatedTo integer NULL;  -- FK đến Users (BQT_Head)
ALTER TABLE Requests ADD EscalatedAt datetime NULL;
ALTER TABLE Requests ADD EscalationReason nvarchar(500) NULL;
```

### ⚠️ Cột mới cần thêm cho bảng Announcements:
```sql
ALTER TABLE Announcements ADD Source nvarchar(20) DEFAULT 'BQL';  -- 'BQL', 'BQT', 'System'
```

### Database Tables liên quan:
- `Requests` (Thêm cột EscalatedTo, EscalatedAt, EscalationReason)
- `RequestAttachments`
- `Announcements` (Thêm cột Source)
- `Notifications`

### Pages cần tạo:
```
/Requests/Create (cho Resident)
/Requests/MyRequests (cho Resident)
/Requests/Details/{id}
/Requests (Index - cho Staff/Manager)
/Requests/Assigned (cho Staff)
/Requests/Escalate (cho Staff -> Manager)
/BQT/Complaints (cho BQT_Head - xem khiếu nại)
/BQT/Complaints/Response (cho BQT_Head - phản hồi)
/Announcements (Index, Create, Edit)
/Announcements/View (cho Resident)
/Notifications (Index)
```

### Thời gian ước tính: **2 tuần** (tăng từ 1.5 tuần do thêm chức năng BQT)

---

## 👤 MODULE 5 - Tiện ích & Bưu kiện

### Module: Đặt tiện ích, Quản lý bưu kiện, Khách thăm

**Mô tả:** Xây dựng chức năng đặt tiện ích (gym, hồ bơi, BBQ), quản lý bưu kiện và đăng ký khách.

### Danh sách chức năng:

#### 5.1 Quản lý tiện ích (Admin)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 1 | Xem danh sách tiện ích | Gym, Pool, BBQ, Meeting Room | Admin |
| 2 | Thêm/Sửa tiện ích | CRUD amenities | Admin |
| 3 | Cấu hình giá/giờ | Thiết lập phí sử dụng | Admin |

#### 5.2 Đặt tiện ích (Cư dân)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 4 | Xem lịch tiện ích | Calendar view các slot | Resident |
| 5 | Đặt tiện ích | Book slot theo ngày/giờ | Resident |
| 6 | Xem booking của tôi | Danh sách đã đặt | Resident |
| 7 | Hủy booking | Cancel trước thời hạn | Resident |
| 8 | Xác nhận sử dụng | Check-in khi đến | BQL_Staff |

#### 5.3 Quản lý bưu kiện
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 9 | Nhận bưu kiện | Ghi nhận bưu kiện đến | BQL_Staff |
| 10 | Xem danh sách bưu kiện | Lọc theo trạng thái | BQL_Staff |
| 11 | Thông báo cư dân | Gửi notification đến nhận | BQL_Staff |
| 12 | Xác nhận đã giao | Đánh dấu picked up | BQL_Staff |
| 13 | Xem bưu kiện của tôi | Cư dân xem danh sách | Resident |

#### 5.4 Đăng ký khách
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 14 | Đăng ký khách trước | Cư dân đăng ký khách sẽ đến | Resident |
| 15 | Xem khách đã đăng ký | Danh sách khách | Resident |
| 16 | Ghi nhận khách đến | Check-in tại sảnh | BQL_Staff |
| 17 | Xác nhận khách ra | Check-out khi rời đi | BQL_Staff |
| 18 | Xem lịch sử khách | Tra cứu khách đã đến | BQL_Staff |

### Database Tables liên quan:
- `Amenities`
- `AmenityBookings`
- `Parcels`
- `Visitors`

### Pages cần tạo:
```
/Admin/Amenities (Index, Create, Edit)
/Amenities (Calendar view)
/Amenities/Book
/Amenities/MyBookings (cho Resident)
/Parcels (Index, Create, Deliver)
/Parcels/MyParcels (cho Resident)
/Visitors/Register (cho Resident)
/Visitors/MyVisitors (cho Resident)
/Visitors (Index - cho Staff)
/Visitors/CheckIn
/Visitors/CheckOut
```

### Thời gian ước tính: **1.5 tuần**

---

## 👤 MODULE 6 - Face Recognition & Biometric Authentication

### Module: Đăng ký khuôn mặt, Xác thực sinh trắc học tại tiện ích

**Mô tả:** Xây dựng hệ thống nhận diện khuôn mặt cho phép cư dân đăng ký và xác thực khi sử dụng tiện ích (Gym, Pool, BBQ). Tăng cường bảo mật, chống mượn thẻ.

### Danh sách chức năng:

#### 6.1 Đăng ký khuôn mặt (Cư dân)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 1 | Đăng ký khuôn mặt | Chụp ảnh và lưu face data qua webcam | Resident |
| 2 | Cập nhật khuôn mặt | Chụp lại khi thay đổi ngoại hình | Resident |
| 3 | Xóa dữ liệu khuôn mặt | Xóa face data của mình (GDPR) | Resident |
| 4 | Xem trạng thái đăng ký | Kiểm tra đã đăng ký chưa | Resident |
| 5 | Xem lịch sử xác thực | Tra cứu lịch sử check-in bằng face | Resident |

#### 6.2 Xác thực tại tiện ích
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 6 | Scan khuôn mặt | Camera scan và match với DB | Resident, System |
| 7 | Check-in tự động | Tự động check-in khi match thành công | System |
| 8 | Hiển thị kết quả | Hiển thị tên cư dân, căn hộ khi match | System |
| 9 | Check-in thủ công | Staff check-in khi face ID gặp lỗi | BQL_Staff |
| 10 | Retry & fallback | Cho phép thử lại, chuyển manual | System |

#### 6.3 Quản lý Face Data (Staff/Manager)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 11 | Hỗ trợ đăng ký | Hỗ trợ cư dân đăng ký tại quầy | BQL_Staff |
| 12 | Xem danh sách đã đăng ký | Tra cứu cư dân có face ID | BQL_Staff |
| 13 | Reset face data | Xóa và yêu cầu đăng ký lại | BQL_Staff |
| 14 | Xem log xác thực | Lịch sử xác thực tại tiện ích | BQL_Staff, BQL_Manager |
| 15 | Dashboard Face ID | Thống kê đăng ký, tỉ lệ sử dụng | BQL_Manager |

#### 6.4 Cấu hình (Admin)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 16 | Bật/tắt face auth | Cấu hình yêu cầu face cho từng tiện ích | Admin |
| 17 | Thiết lập ngưỡng match | Cấu hình threshold (95%, 90%...) | Admin |
| 18 | Xem audit log | Log toàn bộ hoạt động face ID | Admin |
| 19 | Xóa toàn bộ face data | Xóa data của cư dân đã chuyển đi | Admin |

### Database Tables MỚI cần tạo:

```sql
-- Bảng FaceData
CREATE TABLE FaceData (
  FaceDataId INT PRIMARY KEY IDENTITY,
  ResidentId INT NOT NULL,
  FaceVector VARBINARY(MAX) NOT NULL,
  FaceImagePath NVARCHAR(500),
  Quality FLOAT,
  IsActive BIT DEFAULT 1,
  RegisteredAt DATETIME DEFAULT GETDATE(),
  RegisteredBy INT,
  LastVerifiedAt DATETIME,
  VerificationCount INT DEFAULT 0,
  FailedAttempts INT DEFAULT 0,
  UpdatedAt DATETIME,
  FOREIGN KEY (ResidentId) REFERENCES Residents(UserId),
  FOREIGN KEY (RegisteredBy) REFERENCES Users(UserId)
);

-- Bảng FaceAuthLogs
CREATE TABLE FaceAuthLogs (
  LogId INT PRIMARY KEY IDENTITY,
  ResidentId INT,
  AmenityId INT,
  AuthResult NVARCHAR(20),
  MatchScore FLOAT,
  DeviceId NVARCHAR(100),
  CapturedImagePath NVARCHAR(500),
  AuthTime DATETIME DEFAULT GETDATE(),
  OverrideBy INT,
  OverrideReason NVARCHAR(200),
  IPAddress NVARCHAR(50),
  FOREIGN KEY (ResidentId) REFERENCES Residents(UserId),
  FOREIGN KEY (AmenityId) REFERENCES Amenities(AmenityId),
  FOREIGN KEY (OverrideBy) REFERENCES Users(UserId)
);

-- Cập nhật bảng Amenities
ALTER TABLE Amenities ADD RequireFaceAuth BIT DEFAULT 0;
ALTER TABLE Amenities ADD FaceAuthThreshold FLOAT DEFAULT 0.95;

-- Cập nhật bảng AmenityBookings
ALTER TABLE AmenityBookings ADD CheckInMethod NVARCHAR(20);
ALTER TABLE AmenityBookings ADD FaceAuthLogId INT;
```

### Database Tables liên quan:
- `FaceData` (Mới)
- `FaceAuthLogs` (Mới)
- `Amenities` (Thêm cột RequireFaceAuth, FaceAuthThreshold)
- `AmenityBookings` (Thêm cột CheckInMethod, FaceAuthLogId)
- `Residents`

### Pages cần tạo:
```
/FaceAuth/Register (cho Resident - đăng ký face)
/FaceAuth/Update (cho Resident - cập nhật face)
/FaceAuth/Status (cho Resident - xem trạng thái)
/FaceAuth/History (cho Resident - lịch sử xác thực)
/FaceAuth/Verify (cho kiosk - màn hình xác thực)
/FaceAuth/ManualCheckIn (cho Staff)
/FaceAuth/Admin/List (danh sách cư dân đã đăng ký)
/FaceAuth/Admin/Logs (log xác thực)
/FaceAuth/Admin/Dashboard (thống kê)
/Admin/Amenities/FaceAuth (cấu hình face cho tiện ích)
```

### Công nghệ sử dụng:
- **face-api.js**: Face detection trên browser (JavaScript)
- **ML.NET + ONNX**: Face recognition trên server
- **MediaDevices API**: Truy cập webcam HTML5
- **SignalR**: Real-time kết quả xác thực

### Thời gian ước tính: **2 tuần**

---

## 👤 MODULE 7 - Ban Quản Trị (BQT) - MỚI (Phát sinh - thực hiện sau)

### Module: Giám sát, Phê duyệt, Cuộc họp, Khảo sát

**Mô tả:** Xây dựng các chức năng dành riêng cho Ban Quản Trị (BQT_Head, BQT_Member) để giám sát hoạt động BQL, phê duyệt chi tiêu, tổ chức họp và khảo sát cư dân.

### Danh sách chức năng:

#### 7.1 Giám sát hoạt động (BQT)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 1 | Dashboard tổng quan BQT | Xem tình hình chung: tài chính, yêu cầu, cư dân | BQT_Head, BQT_Member |
| 2 | Xem báo cáo tài chính | Truy cập báo cáo thu chi chi tiết | BQT_Head, BQT_Member (chỉ xem) |
| 3 | Xem báo cáo vận hành | Thống kê yêu cầu, sự cố, hiệu suất xử lý | BQT_Head, BQT_Member |
| 4 | So sánh kỳ | So sánh các chỉ số theo tháng/quý/năm | BQT_Head |

#### 7.2 Phê duyệt chi tiêu (BQT_Head)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 5 | Xem đề xuất chi tiêu | Danh sách chi tiêu vượt ngưỡng cần duyệt | BQT_Head |
| 6 | Phê duyệt chi tiêu | Duyệt các khoản chi lớn | BQT_Head |
| 7 | Từ chối chi tiêu | Từ chối với lý do | BQT_Head |
| 8 | Phê duyệt thay đổi giá dịch vụ | Xác nhận điều chỉnh đơn giá | BQT_Head |
| 9 | Phê duyệt hợp đồng nhà thầu | Duyệt hợp đồng bảo trì, sửa chữa lớn | BQT_Head |

#### 7.3 Quản lý cuộc họp (Tùy chọn)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 10 | Tạo cuộc họp cư dân | Lên lịch họp định kỳ hoặc bất thường | BQT_Head |
| 11 | Gửi thông báo họp | Thông báo cho cư dân về cuộc họp | BQT_Head |
| 12 | Ghi biên bản họp | Lưu trữ nội dung, quyết định cuộc họp | BQT_Head |
| 13 | Xem lịch họp | Tra cứu lịch họp BQT, họp cư dân | BQT_Head, BQT_Member |
| 14 | Xem biên bản họp | Đọc nội dung các cuộc họp đã diễn ra | BQT_Head, BQT_Member |
| 15 | Đề xuất nội dung họp | Thêm vấn đề vào agenda cuộc họp | BQT_Member |

#### 7.4 Khảo sát & Bỏ phiếu (Tùy chọn)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 16 | Tạo khảo sát | Thu thập ý kiến cư dân | BQT_Head |
| 17 | Tạo bỏ phiếu | Tạo cuộc bỏ phiếu về vấn đề chung | BQT_Head |
| 18 | Xem kết quả khảo sát | Thống kê kết quả | BQT_Head, BQT_Member |
| 19 | Tham gia khảo sát | Cư dân trả lời khảo sát | Resident |
| 20 | Tham gia bỏ phiếu | Cư dân bỏ phiếu | Resident |

#### 7.5 Quản lý quỹ bảo trì
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 21 | Xem quỹ bảo trì | Theo dõi số dư quỹ bảo trì chung cư | BQT_Head |
| 22 | Phê duyệt sử dụng quỹ | Duyệt chi từ quỹ bảo trì | BQT_Head |

### Database Tables MỚI (Tùy chọn):

```sql
-- Bảng Meetings (Tùy chọn)
CREATE TABLE Meetings (
  MeetingId INT PRIMARY KEY IDENTITY,
  Title NVARCHAR(200) NOT NULL,
  Description NVARCHAR(MAX),
  MeetingType NVARCHAR(50), -- 'BQT', 'Resident', 'Emergency'
  Location NVARCHAR(200),
  ScheduledDate DATETIME NOT NULL,
  StartTime TIME,
  EndTime TIME,
  Status NVARCHAR(20), -- 'Scheduled', 'InProgress', 'Completed', 'Cancelled'
  CreatedBy INT NOT NULL,
  MinutesContent NVARCHAR(MAX),
  Attendees NVARCHAR(MAX), -- JSON
  CreatedAt DATETIME DEFAULT GETDATE(),
  UpdatedAt DATETIME,
  FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
);

-- Bảng Surveys (Tùy chọn)
CREATE TABLE Surveys (
  SurveyId INT PRIMARY KEY IDENTITY,
  Title NVARCHAR(200) NOT NULL,
  Description NVARCHAR(MAX),
  SurveyType NVARCHAR(50), -- 'Poll', 'Feedback', 'Voting'
  StartDate DATETIME NOT NULL,
  EndDate DATETIME NOT NULL,
  IsAnonymous BIT DEFAULT 0,
  Status NVARCHAR(20), -- 'Draft', 'Active', 'Closed'
  CreatedBy INT NOT NULL,
  CreatedAt DATETIME DEFAULT GETDATE(),
  FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
);

-- Bảng SurveyQuestions (Tùy chọn)
CREATE TABLE SurveyQuestions (
  QuestionId INT PRIMARY KEY IDENTITY,
  SurveyId INT NOT NULL,
  QuestionText NVARCHAR(500) NOT NULL,
  QuestionType NVARCHAR(50), -- 'SingleChoice', 'MultiChoice', 'Text', 'Rating'
  Options NVARCHAR(MAX), -- JSON
  IsRequired BIT DEFAULT 1,
  OrderIndex INT,
  FOREIGN KEY (SurveyId) REFERENCES Surveys(SurveyId)
);

-- Bảng SurveyResponses (Tùy chọn)
CREATE TABLE SurveyResponses (
  ResponseId INT PRIMARY KEY IDENTITY,
  SurveyId INT NOT NULL,
  QuestionId INT NOT NULL,
  ResidentId INT,
  Answer NVARCHAR(MAX),
  CreatedAt DATETIME DEFAULT GETDATE(),
  FOREIGN KEY (SurveyId) REFERENCES Surveys(SurveyId),
  FOREIGN KEY (QuestionId) REFERENCES SurveyQuestions(QuestionId),
  FOREIGN KEY (ResidentId) REFERENCES Residents(UserId)
);

-- Bảng ExpenseApprovals (Tùy chọn)
CREATE TABLE ExpenseApprovals (
  ApprovalId INT PRIMARY KEY IDENTITY,
  Title NVARCHAR(200) NOT NULL,
  Description NVARCHAR(MAX),
  Amount DECIMAL(18,2) NOT NULL,
  Category NVARCHAR(50), -- 'Maintenance', 'Equipment', 'Service', 'Other'
  RequestedBy INT NOT NULL,
  Status INT, -- 0:Pending, 1:Approved, 2:Rejected
  ApprovedBy INT,
  ApprovedAt DATETIME,
  RejectionReason NVARCHAR(500),
  Attachments NVARCHAR(MAX), -- JSON
  CreatedAt DATETIME DEFAULT GETDATE(),
  UpdatedAt DATETIME,
  FOREIGN KEY (RequestedBy) REFERENCES Users(UserId),
  FOREIGN KEY (ApprovedBy) REFERENCES Users(UserId)
);
```

### Database Tables liên quan:
- `Meetings` (Mới - Tùy chọn)
- `Surveys` (Mới - Tùy chọn)
- `SurveyQuestions` (Mới - Tùy chọn)
- `SurveyResponses` (Mới - Tùy chọn)
- `ExpenseApprovals` (Mới - Tùy chọn)

### Pages cần tạo:
```
/BQT/Dashboard (Dashboard tổng quan BQT)
/BQT/Reports/Finance (Báo cáo tài chính)
/BQT/Reports/Operations (Báo cáo vận hành)
/BQT/Expenses (Danh sách chi tiêu cần duyệt)
/BQT/Expenses/Approve/{id}
/BQT/Meetings (Index, Create, Edit - Tùy chọn)
/BQT/Meetings/Minutes/{id} (Biên bản họp - Tùy chọn)
/BQT/Surveys (Index, Create - Tùy chọn)
/BQT/Surveys/Results/{id} (Kết quả khảo sát - Tùy chọn)
/Surveys/Participate/{id} (Cư dân tham gia - Tùy chọn)
```

### Thời gian ước tính: **1.5 tuần** (Chỉ tính các chức năng cơ bản giám sát & phê duyệt)

---

## 📅 Timeline Tổng Quan

```
Tuần 1-2:
├── TV1: Authentication + User Management (6 vai trò mới)
├── TV2: Ghi chỉ số + Tạo hóa đơn + Phê duyệt workflow
├── TV3: Căn hộ + Cư dân
├── TV4: Yêu cầu (Resident side) + Khiếu nại BQT
├── TV5: Tiện ích + Bưu kiện
├── TV6: Face Recognition - Setup + Đăng ký khuôn mặt
└── [NEW] TV7: BQT Dashboard + Giám sát cơ bản

Tuần 3:
├── TV1: Cấu hình hệ thống + Log + Backup/Restore
├── TV2: Thanh toán + Báo cáo cho BQT
├── TV3: Hợp đồng + Xe & Thẻ
├── TV4: Xử lý yêu cầu (Staff/Manager) + Escalation
├── TV5: Khách thăm
├── TV6: Face Recognition - Xác thực + Admin
└── [NEW] TV7: BQT Phê duyệt chi tiêu

Tuần 4:
├── TV1: Testing + Bug fix
├── TV2: Báo cáo tài chính + Export
├── TV3: Testing + Bug fix
├── TV4: Thông báo + Notification (BQL + BQT)
├── TV5: Testing + Bug fix
├── TV6: Face Recognition - Dashboard + Testing
└── [NEW] TV7: Cuộc họp + Khảo sát (Tùy chọn)

Tuần 5:
├── Integration Testing
├── Bug Fixes
├── Permission Testing (6 vai trò)
└── Documentation
```

---

## 🔗 Phụ Thuộc Giữa Các Module

```
                    ┌─────────────────┐
                    │   THÀNH VIÊN 1  │
                    │  Authentication │
                    │  & User Mgmt    │
                    │  (6 VAI TRÒ)    │
                    └────────┬────────┘
                             │
         ┌───────────────────┼───────────────────┐
         │                   │                   │
         ▼                   ▼                   ▼
┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐
│   THÀNH VIÊN 2  │ │   THÀNH VIÊN 3  │ │   THÀNH VIÊN 4  │
│   Tài chính     │◀│  Căn hộ & Cư dân │▶│  Yêu cầu        │
│  + Phê duyệt    │ └────────┬────────┘ │  + Khiếu nại BQT│
└────────┬────────┘          │          └────────┬────────┘
         │                   ▼                   │
         │          ┌─────────────────┐          │
         └─────────▶│   THÀNH VIÊN 5  │◀─────────┘
                    │  Tiện ích       │
                    │  & Bưu kiện     │
                    └────────┬────────┘
                             │
                             ▼
                    ┌─────────────────┐
                    │   THÀNH VIÊN 6  │
                    │ Face Recognition│
                    │ & Biometric Auth│
                    └────────┬────────┘
                             │
                    ┌────────┴────────┐
                    │   THÀNH VIÊN 7  │
                    │   Ban Quản Trị  │
                    │  (BQT) - MỚI    │
                    └─────────────────┘
```

### Ghi chú phụ thuộc:
- **TV1 (Auth)**: Phải hoàn thành đầu tiên, cập nhật 6 vai trò mới
- **TV3 (Căn hộ & Cư dân)**: Cần hoàn thành sớm vì TV2, TV4, TV5, TV6 cần thông tin Apartment/Resident
- **TV2 (Tài chính)**: Phụ thuộc vào Apartments, Residents từ TV3; cần workflow phê duyệt cho BQT
- **TV4 (Yêu cầu)**: Phụ thuộc vào Residents từ TV3; thêm chức năng khiếu nại lên BQT
- **TV5 (Tiện ích)**: Phụ thuộc vào Apartments, Residents từ TV3
- **TV6 (Face Recognition)**: Phụ thuộc vào Residents từ TV3, Amenities từ TV5
- **[NEW] TV7 (BQT)**: Phụ thuộc vào TV1 (phân quyền), TV2 (báo cáo tài chính), TV4 (khiếu nại)

---

## ✅ Checklist Hoàn Thành

### Thành viên 1 - Authentication & Admin:
- [ ] Login/Logout
- [ ] Middleware phân quyền (6 vai trò mới)
- [ ] CRUD Users (tất cả vai trò)
- [ ] Phân quyền: Admin, BQL_Manager, BQL_Staff, BQT_Head, BQT_Member, Resident
- [ ] Cấu hình hệ thống
- [ ] CRUD ServiceTypes/Prices
- [ ] Activity Log
- [ ] Backup/Restore dữ liệu
- [ ] Migration: Cập nhật Role từ Staff/Security -> BQL_Staff

### Thành viên 2 - Tài chính:
- [ ] Ghi chỉ số điện nước
- [ ] Tạo hóa đơn tự động
- [ ] **Phê duyệt hóa đơn** (BQL_Manager)
- [ ] Thu tiền mặt/Chuyển khoản
- [ ] Công nợ & Nhắc nợ
- [ ] Báo cáo tài chính
- [ ] **Xem báo cáo cho BQT** (BQT_Head, BQT_Member)
- [ ] Migration: Thêm cột ApprovalStatus cho Invoices

### Thành viên 3 - Căn hộ & Cư dân:
- [ ] CRUD Apartments
- [ ] CRUD Residents
- [ ] CRUD Contracts
- [ ] CRUD Vehicles
- [ ] CRUD ResidentCards
- [ ] Profile cư dân
- [ ] **Phê duyệt chuyển đi/đến** (BQL_Manager)

### Thành viên 4 - Yêu cầu & Thông báo:
- [ ] Gửi yêu cầu (Resident)
- [ ] Xử lý yêu cầu (Staff)
- [ ] Phân công yêu cầu (Manager)
- [ ] **Escalation lên Manager** (BQL_Staff -> BQL_Manager)
- [ ] **Gửi khiếu nại lên BQT** (Resident -> BQT_Head)
- [ ] **Phản hồi khiếu nại** (BQT_Head)
- [ ] CRUD Announcements (BQL_Manager + BQT_Head)
- [ ] Bell Notifications
- [ ] Đánh giá dịch vụ
- [ ] Migration: Thêm cột EscalatedTo cho Requests, Source cho Announcements

### Thành viên 5 - Tiện ích & Bưu kiện:
- [ ] CRUD Amenities
- [ ] Đặt tiện ích
- [ ] CRUD Parcels
- [ ] CRUD Visitors
- [ ] Check-in/Check-out

### Thành viên 6 - Face Recognition:
- [ ] Đăng ký khuôn mặt (Resident)
- [ ] Cập nhật/Xóa khuôn mặt
- [ ] Xác thực tại tiện ích (Kiosk)
- [ ] Check-in thủ công (Staff)
- [ ] Dashboard Face ID (Manager)
- [ ] Cấu hình Face Auth (Admin)
- [ ] Log xác thực & Báo cáo
- [ ] Migration: Tạo bảng FaceData, FaceAuthLogs; Cập nhật Amenities, AmenityBookings

### [NEW] Thành viên 7 - Ban Quản Trị (BQT):
- [ ] Dashboard tổng quan BQT
- [ ] Xem báo cáo tài chính (chỉ xem)
- [ ] Xem báo cáo vận hành (chỉ xem)
- [ ] Phê duyệt chi tiêu lớn (BQT_Head)
- [ ] Phê duyệt thay đổi giá dịch vụ (BQT_Head)
- [ ] Xem khiếu nại từ cư dân
- [ ] Phản hồi khiếu nại
- [ ] (Tùy chọn) Quản lý cuộc họp
- [ ] (Tùy chọn) Tạo khảo sát/bỏ phiếu
- [ ] Migration: Tạo bảng Meetings, Surveys, SurveyQuestions, SurveyResponses, ExpenseApprovals (nếu cần)

---

## 📊 Tóm Tắt Thay Đổi Database

### Bắt buộc:
| STT | Thay đổi | Mức độ |
|:---:|:---------|:------:|
| 1 | Cập nhật giá trị cột Role trong Users | 🔴 Cao |
| 2 | Thêm cột ApprovalStatus, ApprovedBy, ApprovedAt cho Invoices | 🟡 TB |
| 3 | Thêm cột EscalatedTo, EscalatedAt cho Requests | 🟢 Thấp |
| 4 | Thêm cột Source cho Announcements | 🟢 Thấp |

### Cho Face Recognition:
| STT | Thay đổi | Mức độ |
|:---:|:---------|:------:|
| 5 | Tạo bảng FaceData | 🔴 Cao |
| 6 | Tạo bảng FaceAuthLogs | 🔴 Cao |
| 7 | Thêm cột RequireFaceAuth, FaceAuthThreshold cho Amenities | 🟡 TB |
| 8 | Thêm cột CheckInMethod, FaceAuthLogId cho AmenityBookings | 🟢 Thấp |

### Tùy chọn (BQT nâng cao):
| STT | Thay đổi | Mức độ |
|:---:|:---------|:------:|
| 9 | Tạo bảng Meetings | 🟡 TB |
| 10 | Tạo bảng Surveys, SurveyQuestions, SurveyResponses | 🟢 Thấp |
| 11 | Tạo bảng ExpenseApprovals | 🟡 TB |

---

*Tài liệu phân công công việc - Nhóm 5 - Cập nhật: 13/02/2026*
*Dựa trên CapNhatYTuong.md ngày 11/02/2026*
