# Phân Chia Công Việc - Nhóm 5
## Dự án: Hệ Thống Quản Lý Chung Cư
### Ngày: 11/02/2026

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
| 5 | Middleware phân quyền | Kiểm tra quyền truy cập theo role | Hệ thống |

#### 1.2 Quản lý người dùng (Admin)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 6 | CRUD User | Tạo, sửa, xóa, khóa tài khoản | Admin |
| 7 | Phân quyền vai trò | Gán role cho user | Admin |
| 8 | Xem danh sách users | Danh sách + tìm kiếm + lọc | Admin |
| 9 | Reset mật khẩu user | Admin reset password cho user | Admin |

#### 1.3 Cấu hình hệ thống (Admin)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 10 | Cấu hình chung | Tên, logo, thông tin chung cư | Admin |
| 11 | Quản lý danh mục dịch vụ | CRUD ServiceTypes | Admin |
| 12 | Cấu hình giá dịch vụ | CRUD ServicePrices | Admin |
| 13 | Xem log hệ thống | Audit trail, activity log | Admin |

### Database Tables liên quan:
- `Users`
- `ServiceTypes`
- `ServicePrices`

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
| 6 | Phê duyệt hóa đơn | Manager duyệt trước khi gửi | BQL_Manager |
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
| 18 | Dashboard tài chính | Tổng thu, công nợ, biểu đồ | BQL_Manager, BQT_Head |
| 19 | Báo cáo thu chi theo tháng | Chi tiết doanh thu | BQL_Manager, BQT_Head |
| 20 | Xu���t báo cáo Excel | Export báo cáo | BQL_Manager |

### Database Tables liên quan:
- `MeterReadings`
- `Invoices`
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
| 1 | Xem danh sách căn hộ | Danh sách + tìm kiếm + lọc | BQL_Manager, BQL_Staff |
| 2 | Xem chi tiết căn hộ | Thông tin + cư dân + hóa đơn | BQL_Manager, BQL_Staff |
| 3 | Cập nhật thông tin căn hộ | Sửa diện tích, loại căn hộ | BQL_Manager |
| 4 | Dashboard căn hộ | Thống kê trạng thái căn hộ | BQL_Manager |

#### 3.2 Quản lý cư dân
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 5 | Xem danh sách cư dân | Danh sách + tìm kiếm | BQL_Manager, BQL_Staff |
| 6 | Thêm cư dân mới | Đăng ký cư dân vào căn hộ | BQL_Staff |
| 7 | Cập nhật thông tin cư dân | Sửa thông tin cá nhân | BQL_Staff |
| 8 | Xóa/Vô hiệu hóa cư dân | Soft delete khi chuyển đi | BQL_Manager |
| 9 | Xem hồ sơ cá nhân | Cư dân xem thông tin mình | Resident |
| 10 | Cập nhật hồ sơ cá nhân | Cư dân sửa thông tin | Resident |

#### 3.3 Quản lý hợp đồng
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 11 | Xem danh sách hợp đồng | Lọc theo trạng thái | BQL_Manager |
| 12 | Tạo hợp đồng mới | Thuê/Mua căn hộ | BQL_Manager |
| 13 | Gia hạn hợp đồng | Extend thời hạn | BQL_Manager |
| 14 | Chấm dứt hợp đồng | Kết thúc hợp đồng | BQL_Manager |
| 15 | Xem hợp đồng (Cư dân) | Cư dân xem hợp đồng của mình | Resident |

#### 3.4 Quản lý xe & thẻ
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 16 | Đăng ký xe | Thêm xe cho cư dân | BQL_Staff |
| 17 | Xem danh sách xe | Lọc theo căn hộ, loại xe | BQL_Staff |
| 18 | Hủy đăng ký xe | Xóa xe khi bán/chuyển | BQL_Staff |
| 19 | Cấp thẻ cư dân | Tạo thẻ ra vào mới | BQL_Staff |
| 20 | Khóa/Mở khóa thẻ | Vô hiệu hóa thẻ bị mất | BQL_Staff |
| 21 | Xem xe của tôi | Cư dân xem danh sách xe | Resident |

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

**Mô tả:** Xây dựng hệ thống tiếp nhận và xử lý yêu cầu từ cư dân, quản lý thông báo.

### Danh sách chức năng:

#### 4.1 Yêu cầu sửa chữa (Cư dân)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 1 | Gửi yêu cầu mới | Tạo ticket sửa chữa/báo sự cố | Resident |
| 2 | Đính kèm ảnh/file | Upload hình ảnh mô tả | Resident |
| 3 | Xem yêu cầu của tôi | Danh sách yêu cầu đã gửi | Resident |
| 4 | Xem chi tiết yêu cầu | Trạng thái, comment, lịch sử | Resident |
| 5 | Đánh giá sau xử lý | Rate chất lượng dịch vụ | Resident |

#### 4.2 Xử lý yêu cầu (Staff/Manager)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 6 | Xem tất cả yêu cầu | Danh sách + lọc theo trạng thái | BQL_Manager |
| 7 | Xem yêu cầu được giao | Danh sách công việc của mình | BQL_Staff |
| 8 | Phân công yêu cầu | Giao việc cho Staff | BQL_Manager |
| 9 | Cập nhật trạng thái | Đang xử lý, Hoàn thành | BQL_Staff |
| 10 | Thêm ghi chú/comment | Cập nhật tiến độ | BQL_Staff |
| 11 | Đóng yêu cầu | Xác nhận hoàn thành | BQL_Manager, BQL_Staff |
| 12 | Escalate lên Manager | Chuyển vấn đề phức tạp | BQL_Staff |

#### 4.3 Thông báo
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 13 | Tạo thông báo | Đăng thông báo mới | BQL_Manager, BQT_Head |
| 14 | Xem danh sách thông báo | Quản lý thông báo đã đăng | BQL_Manager |
| 15 | Sửa/Xóa thông báo | Chỉnh sửa nội dung | BQL_Manager |
| 16 | Ghim thông báo quan trọng | Pin to top | BQL_Manager |
| 17 | Xem thông báo (Cư dân) | Danh sách thông báo | Resident |
| 18 | Đánh dấu đã đọc | Mark as read | Resident |

#### 4.4 Notification (Real-time)
| STT | Chức năng | Mô tả | Vai trò liên quan |
|:---:|:----------|:------|:------------------|
| 19 | Bell notification | Icon chuông + số chưa đọc | All |
| 20 | Dropdown notifications | Danh sách nhanh | All |
| 21 | Xem tất cả notifications | Trang full list | All |

### Database Tables liên quan:
- `Requests`
- `RequestAttachments`
- `Announcements`
- `Notifications`

### Pages cần tạo:
```
/Requests/Create (cho Resident)
/Requests/MyRequests (cho Resident)
/Requests/Details/{id}
/Requests (Index - cho Staff/Manager)
/Requests/Assigned (cho Staff)
/Announcements (Index, Create, Edit)
/Announcements/View (cho Resident)
/Notifications (Index)
```

### Thời gian ước tính: **1.5 tuần**

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

## 📅 Timeline Tổng Quan

```
Tuần 1-2:
├── TV1: Authentication + User Management
├── TV2: Ghi chỉ số + Tạo hóa đơn
├── TV3: Căn hộ + Cư dân
├── TV4: Yêu cầu (Resident side)
└── TV5: Tiện ích + Bưu kiện

Tuần 3:
├── TV1: Cấu h��nh hệ thống + Log
├── TV2: Thanh toán + Phê duyệt
├── TV3: Hợp đồng + Xe & Thẻ
├── TV4: Xử lý yêu cầu (Staff/Manager)
└── TV5: Khách thăm

Tuần 4:
├── TV1: Testing + Bug fix
├── TV2: Báo cáo tài chính + Export
├── TV3: Testing + Bug fix
├── TV4: Thông báo + Notification
└── TV5: Testing + Bug fix

Tuần 5:
├── Integration Testing
├── Bug Fixes
└── Documentation
```

---

## 🔗 Phụ Thuộc Giữa Các Module

```
                    ┌─────────────────┐
                    │   THÀNH VIÊN 1  │
                    │  Authentication │
                    │  & User Mgmt    │
                    └────────┬────────┘
                             │
         ┌───────────────────┼───────────────────┐
         │                   │                   │
         ▼                   ▼                   ▼
┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐
│   THÀNH VIÊN 2  │ │   THÀNH VIÊN 3  │ │   THÀNH VIÊN 4  │
│   Tài chính     │◀│  Căn hộ & Cư dân │▶│  Yêu cầu        │
└────────┬────────┘ └────────┬────────┘ └────────┬────────┘
         │                   │                   │
         │                   ▼                   │
         │          ┌─────────────────┐          │
         └─────────▶│   THÀNH VIÊN 5  │◀─────────┘
                    │  Tiện ích       │
                    │  & Bưu kiện     │
                    └─────────────────┘
```

### Ghi chú phụ thuộc:
- **TV1 (Auth)**: Phải hoàn thành đầu tiên, các module khác phụ thuộc vào login/phân quyền
- **TV3 (Căn hộ & Cư dân)**: Cần hoàn thành sớm vì TV2, TV4, TV5 cần thông tin Apartment/Resident
- **TV2 (Tài chính)**: Phụ thuộc vào Apartments, Residents từ TV3
- **TV4 (Yêu cầu)**: Phụ thuộc vào Residents từ TV3
- **TV5 (Tiện ích)**: Phụ thuộc vào Apartments, Residents từ TV3

---

## ✅ Checklist Hoàn Thành

### Thành viên 1:
- [ ] Login/Logout
- [ ] Middleware phân quyền
- [ ] CRUD Users
- [ ] Cấu hình hệ thống
- [ ] CRUD ServiceTypes/Prices
- [ ] Activity Log

### Thành viên 2:
- [ ] Ghi chỉ số điện nước
- [ ] Tạo hóa đơn tự động
- [ ] Phê duyệt hóa đơn
- [ ] Thu tiền mặt/Chuyển khoản
- [ ] Công nợ & Nhắc nợ
- [ ] Báo cáo tài chính

### Thành viên 3:
- [ ] CRUD Apartments
- [ ] CRUD Residents
- [ ] CRUD Contracts
- [ ] CRUD Vehicles
- [ ] CRUD ResidentCards
- [ ] Profile cư dân

### Thành viên 4:
- [ ] Gửi yêu cầu (Resident)
- [ ] Xử lý yêu cầu (Staff)
- [ ] Phân công yêu cầu (Manager)
- [ ] CRUD Announcements
- [ ] Bell Notifications
- [ ] Đánh giá dịch vụ

### Thành viên 5:
- [ ] CRUD Amenities
- [ ] Đặt tiện ích
- [ ] CRUD Parcels
- [ ] CRUD Visitors
- [ ] Check-in/Check-out

---

*Tài liệu phân công công việc - Nhóm 5 - Cập nhật: 11/02/2026*

