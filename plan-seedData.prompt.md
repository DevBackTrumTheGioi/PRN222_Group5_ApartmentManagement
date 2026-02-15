## Plan: Seed Data toàn bộ hệ thống Quản lý Chung cư

Kế hoạch này mô tả việc tạo dữ liệu mẫu thực tế cho toàn bộ các bảng trong hệ thống, theo đúng thứ tự phụ thuộc (dependency order) để đảm bảo Foreign Key hợp lệ.

### TL;DR
Tạo/cập nhật file `DataSeeder.cs` trong thư mục `Utils` để seed dữ liệu mẫu tiếng Việt cho 20+ bảng, bao gồm: Users (6 vai trò), Apartments (20 căn hộ), Residents, Contracts, Vehicles, Invoices, Requests, Amenities, Visitors, v.v. Dữ liệu phải trông thực tế như một chung cư thật tại Việt Nam.

---

### Steps

#### 1. Seed bảng `Users` (Không phải Resident)
Tạo 8-10 user thuộc các vai trò quản lý:

| Username | Họ tên | Email | Vai trò |
|----------|--------|-------|---------|
| admin | Nguyễn Quốc Anh | admin@sunriseapartment.vn | Admin |
| manager | Trần Minh Tuấn | tuan.manager@sunriseapartment.vn | BQL_Manager |
| staff1 | Lê Thị Hồng Nhung | nhung.staff@sunriseapartment.vn | BQL_Staff |
| staff2 | Phạm Văn Đức | duc.staff@sunriseapartment.vn | BQL_Staff |
| staff3 | Võ Thị Mai Lan | lan.staff@sunriseapartment.vn | BQL_Staff |
| bqt_head | Hoàng Văn Thành | thanh.bqt@sunriseapartment.vn | BQT_Head |
| bqt_member1 | Đặng Thị Hương | huong.bqt@sunriseapartment.vn | BQT_Member |
| bqt_member2 | Bùi Quang Huy | huy.bqt@sunriseapartment.vn | BQT_Member |

**Mật khẩu mặc định:** `123456` (BCrypt hash)

---

#### 2. Seed bảng `Apartments`
Tạo 20 căn hộ thuộc 2 tòa nhà (Block A và Block B):

| Số căn hộ | Tầng | Block | Diện tích | Loại | Trạng thái |
|-----------|------|-------|-----------|------|------------|
| A-0101 | 1 | A | 45 m² | Studio | Occupied |
| A-0102 | 1 | A | 65 m² | 1PN | Occupied |
| A-0201 | 2 | A | 75 m² | 2PN | Occupied |
| A-0202 | 2 | A | 75 m² | 2PN | Occupied |
| A-0301 | 3 | A | 85 m² | 2PN | Occupied |
| A-0302 | 3 | A | 95 m² | 3PN | Occupied |
| A-0401 | 4 | A | 95 m² | 3PN | Occupied |
| A-0501 | 5 | A | 120 m² | Penthouse | Occupied |
| B-0101 | 1 | B | 50 m² | Studio | Occupied |
| B-0102 | 1 | B | 65 m² | 1PN | Occupied |
| B-0201 | 2 | B | 75 m² | 2PN | Occupied |
| B-0202 | 2 | B | 80 m² | 2PN | Occupied |
| B-0301 | 3 | B | 85 m² | 2PN | Available |
| B-0302 | 3 | B | 95 m² | 3PN | Occupied |
| B-0401 | 4 | B | 100 m² | 3PN | Occupied |
| B-0501 | 5 | B | 110 m² | Penthouse | Available |
| A-0601 | 6 | A | 70 m² | 2PN | Occupied |
| A-0602 | 6 | A | 80 m² | 2PN | Occupied |
| B-0601 | 6 | B | 75 m² | 2PN | Occupied |
| B-0602 | 6 | B | 90 m² | 3PN | Maintenance |

---

#### 3. Seed bảng `Residents`
Tạo 35-40 cư dân với thông tin thực tế:

| Họ tên | CCCD | Ngày sinh | Căn hộ | Loại cư dân | SĐT |
|--------|------|-----------|--------|-------------|-----|
| Nguyễn Văn Hùng | 079085012345 | 15/03/1985 | A-0101 | Owner | 0901234567 |
| Trần Thị Lan | 079090123456 | 22/07/1990 | A-0101 | Member | 0912345678 |
| Lê Hoàng Nam | 079088234567 | 10/11/1988 | A-0102 | Owner | 0923456789 |
| Phạm Thị Hoa | 079092345678 | 05/05/1992 | A-0102 | Member | 0934567890 |
| Lê Minh Khoa | 079075456789 | 28/01/1975 | A-0201 | Owner | 0945678901 |
| Nguyễn Thị Thu | 079080567890 | 14/09/1980 | A-0201 | Member | 0956789012 |
| Lê Minh Tuấn | 079005678901 | 20/06/2005 | A-0201 | Member | 0967890123 |
| Võ Văn Tâm | 079082678901 | 03/12/1982 | A-0202 | Tenant | 0978901234 |
| Đặng Thị Mai | 079095789012 | 17/04/1995 | A-0301 | Owner | 0989012345 |
| Bùi Quốc Việt | 079078890123 | 25/08/1978 | A-0302 | Owner | 0990123456 |
| Bùi Thị Ngọc | 079082901234 | 11/02/1982 | A-0302 | Member | 0901234568 |
| Bùi Gia Bảo | 079010012345 | 30/10/2010 | A-0302 | Member | - |
| Hoàng Văn Dũng | 079070123456 | 08/06/1970 | A-0401 | Owner | 0912345679 |
| Hoàng Thị Liên | 079075234567 | 19/03/1975 | A-0401 | Member | 0923456780 |
| Trịnh Công Minh | 079065345678 | 12/12/1965 | A-0501 | Owner | 0934567891 |
| Trịnh Thị Hương | 079068456789 | 27/07/1968 | A-0501 | Member | 0945678902 |
| ... | ... | ... | ... | ... | ... |

*(Tiếp tục với 20+ cư dân khác cho các căn hộ Block B)*

---

#### 4. Seed bảng `Contracts`
Tạo 18 hợp đồng:

| Số HĐ | Căn hộ | Loại | Ngày bắt đầu | Ngày kết thúc | Tiền thuê/tháng | Tiền cọc | Trạng thái |
|-------|--------|------|--------------|---------------|-----------------|----------|------------|
| HD-2024-001 | A-0101 | Mua | 01/03/2024 | - | - | - | Active |
| HD-2024-002 | A-0102 | Mua | 15/04/2024 | - | - | - | Active |
| HD-2024-003 | A-0201 | Mua | 01/06/2024 | - | - | - | Active |
| HD-2024-004 | A-0202 | Thuê | 01/07/2024 | 30/06/2025 | 12,000,000 | 24,000,000 | Active |
| HD-2024-005 | A-0301 | Mua | 15/08/2024 | - | - | - | Active |
| HD-2024-006 | A-0302 | Mua | 01/09/2024 | - | - | - | Active |
| HD-2024-007 | A-0401 | Mua | 10/10/2024 | - | - | - | Active |
| HD-2024-008 | A-0501 | Mua | 01/11/2024 | - | - | - | Active |
| HD-2024-009 | B-0101 | Thuê | 01/12/2024 | 30/11/2025 | 8,000,000 | 16,000,000 | Active |
| HD-2024-010 | B-0102 | Mua | 15/12/2024 | - | - | - | Active |
| HD-2025-001 | B-0201 | Thuê | 01/01/2025 | 31/12/2025 | 10,000,000 | 20,000,000 | Active |
| HD-2025-002 | B-0202 | Mua | 15/01/2025 | - | - | - | Active |
| HD-2025-003 | B-0302 | Mua | 01/02/2025 | - | - | - | Active |
| HD-2025-004 | B-0401 | Thuê | 15/02/2025 | 14/02/2026 | 15,000,000 | 30,000,000 | Active |
| HD-2025-005 | A-0601 | Mua | 01/03/2025 | - | - | - | Active |
| HD-2025-006 | A-0602 | Thuê | 15/03/2025 | 14/03/2026 | 11,000,000 | 22,000,000 | Active |
| HD-2025-007 | B-0601 | Mua | 01/04/2025 | - | - | - | Active |
| HD-2023-001 | B-0301 | Thuê | 01/06/2023 | 31/05/2024 | 9,000,000 | 18,000,000 | Expired |

---

#### 5. Seed bảng `ContractMembers`
Gắn cư dân vào hợp đồng với vai trò:

| Hợp đồng | Cư dân | Vai trò |
|----------|--------|---------|
| HD-2024-001 | Nguyễn Văn Hùng | Chủ hợp đồng |
| HD-2024-001 | Trần Thị Lan | Thành viên |
| HD-2024-002 | Lê Hoàng Nam | Chủ hợp đồng |
| HD-2024-002 | Phạm Thị Hoa | Thành viên |
| ... | ... | ... |

---

#### 6. Seed bảng `Vehicles`
Tạo 30 phương tiện:

| Biển số | Loại xe | Màu sắc | Hãng | Cư dân | Trạng thái |
|---------|---------|---------|------|--------|------------|
| 51A-123.45 | Ô tô | Trắng | Toyota Camry | Nguyễn Văn Hùng | Active |
| 59B1-234.56 | Xe máy | Đen | Honda SH | Trần Thị Lan | Active |
| 51G-345.67 | Ô tô | Đen | Mercedes C200 | Lê Hoàng Nam | Active |
| 59C1-456.78 | Xe máy | Đỏ | Honda Vision | Phạm Thị Hoa | Active |
| 51F-567.89 | Ô tô | Xám | BMW X3 | Lê Minh Khoa | Active |
| 59D1-678.90 | Xe máy | Trắng | Yamaha Grande | Nguyễn Thị Thu | Active |
| 59E1-789.01 | Xe máy | Xanh | Honda Wave | Lê Minh Tuấn | Active |
| 51H-890.12 | Ô tô | Bạc | Mazda CX-5 | Võ Văn Tâm | Active |
| 59F1-901.23 | Xe máy | Đen | Honda Lead | Đặng Thị Mai | Active |
| 51K-012.34 | Ô tô | Trắng | Vinfast Lux A | Bùi Quốc Việt | Active |
| ... | ... | ... | ... | ... | ... |

---

#### 7. Seed bảng `ResidentCards`
Tạo 45 thẻ cư dân:

| Mã thẻ | Cư dân | Loại thẻ | Ngày cấp | Ngày hết hạn | Trạng thái |
|--------|--------|----------|----------|--------------|------------|
| CARD-A0101-001 | Nguyễn Văn Hùng | Chủ hộ | 01/03/2024 | 01/03/2029 | Active |
| CARD-A0101-002 | Trần Thị Lan | Thành viên | 01/03/2024 | 01/03/2029 | Active |
| CARD-A0102-001 | Lê Hoàng Nam | Chủ hộ | 15/04/2024 | 15/04/2029 | Active |
| CARD-A0102-002 | Phạm Thị Hoa | Thành viên | 15/04/2024 | 15/04/2029 | Active |
| ... | ... | ... | ... | ... | ... |

---

#### 8. Seed bảng `ServiceTypes`
Tạo 12 loại dịch vụ:

| Tên dịch vụ | Đơn vị | Mô tả | Trạng thái |
|-------------|--------|-------|------------|
| Tiền điện | kWh | Phí điện sinh hoạt | Active |
| Tiền nước | m³ | Phí nước sinh hoạt | Active |
| Phí quản lý | tháng | Phí quản lý chung cư theo diện tích | Active |
| Phí gửi xe máy | xe/tháng | Phí gửi xe máy hàng tháng | Active |
| Phí gửi ô tô | xe/tháng | Phí gửi ô tô hàng tháng | Active |
| Internet | tháng | Gói Internet cáp quang | Active |
| Giặt ủi | kg | Dịch vụ giặt ủi quần áo | Active |
| Dọn vệ sinh | lần | Dọn dẹp căn hộ theo yêu cầu | Active |
| Sửa chữa nhỏ | lần | Sửa chữa điện nước, thiết bị | Active |
| Chuyển đồ | lần | Hỗ trợ chuyển đồ nội khu | Active |
| Trông trẻ | giờ | Dịch vụ trông trẻ theo giờ | Active |
| Nấu ăn tại nhà | buổi | Dịch vụ nấu ăn tại căn hộ | Active |

---

#### 9. Seed bảng `ServicePrices`
Tạo bảng giá hiện hành (Áp dụng từ 01/01/2026):

| Dịch vụ | Giá | Đơn vị | Ngày áp dụng |
|---------|-----|--------|--------------|
| Tiền điện | 3,500 | đ/kWh | 01/01/2026 |
| Tiền nước | 15,000 | đ/m³ | 01/01/2026 |
| Phí quản lý | 18,000 | đ/m²/tháng | 01/01/2026 |
| Phí gửi xe máy | 150,000 | đ/xe/tháng | 01/01/2026 |
| Phí gửi ô tô | 1,500,000 | đ/xe/tháng | 01/01/2026 |
| Internet | 200,000 | đ/tháng | 01/01/2026 |
| Giặt ủi | 35,000 | đ/kg | 01/01/2026 |
| Dọn vệ sinh | 300,000 | đ/lần | 01/01/2026 |
| Sửa chữa nhỏ | 150,000 | đ/lần (chưa vật tư) | 01/01/2026 |
| Chuyển đồ | 200,000 | đ/lần | 01/01/2026 |
| Trông trẻ | 80,000 | đ/giờ | 01/01/2026 |
| Nấu ăn tại nhà | 350,000 | đ/buổi | 01/01/2026 |

---

#### 10. Seed bảng `Amenities`
Tạo 8 tiện ích:

| Tên tiện ích | Vị trí | Sức chứa | Giá/giờ | Giờ mở cửa | Trạng thái |
|--------------|--------|----------|---------|------------|------------|
| Hồ bơi | Tầng 3, Block A | 30 người | 50,000 | 06:00 - 21:00 | Active |
| Phòng Gym | Tầng 2, Block A | 20 người | Miễn phí | 05:00 - 22:00 | Active |
| Phòng BBQ | Tầng thượng Block A | 15 người | 500,000 | 10:00 - 22:00 | Active |
| Phòng họp A | Tầng 1, Block A | 10 người | 200,000 | 08:00 - 20:00 | Active |
| Phòng họp B | Tầng 1, Block B | 20 người | 350,000 | 08:00 - 20:00 | Active |
| Sân Tennis | Tầng 4, Block B | 4 người | 150,000 | 06:00 - 21:00 | Active |
| Khu vui chơi trẻ em | Tầng 2, Block B | 25 người | Miễn phí | 08:00 - 20:00 | Active |
| Phòng Sauna | Tầng 3, Block A | 8 người | 100,000 | 10:00 - 21:00 | Maintenance |

---

#### 11. Seed bảng `AmenityBookings`
Tạo 25 lượt đặt tiện ích:

| Tiện ích | Cư dân | Ngày đặt | Giờ bắt đầu | Giờ kết thúc | Trạng thái |
|----------|--------|----------|-------------|--------------|------------|
| Phòng BBQ | Nguyễn Văn Hùng | 10/02/2026 | 18:00 | 21:00 | Completed |
| Sân Tennis | Lê Hoàng Nam | 12/02/2026 | 07:00 | 08:00 | Completed |
| Phòng họp A | Bùi Quốc Việt | 14/02/2026 | 14:00 | 16:00 | Confirmed |
| Hồ bơi | Đặng Thị Mai | 15/02/2026 | 08:00 | 10:00 | Confirmed |
| Phòng BBQ | Trịnh Công Minh | 16/02/2026 | 17:00 | 20:00 | Confirmed |
| Phòng họp B | Hoàng Văn Dũng | 18/02/2026 | 09:00 | 11:00 | Pending |
| Sân Tennis | Võ Văn Tâm | 08/02/2026 | 06:00 | 07:00 | Completed |
| Hồ bơi | Lê Minh Khoa | 05/02/2026 | 16:00 | 17:00 | Completed |
| Phòng BBQ | Lê Hoàng Nam | 20/02/2026 | 18:00 | 21:00 | Pending |
| ... | ... | ... | ... | ... | ... |

---

#### 12. Seed bảng `Invoices`
Tạo 54 hóa đơn cho 18 căn hộ x 3 tháng (12/2025, 01/2026, 02/2026):

| Số HĐ | Căn hộ | Tháng | Tổng tiền | Đã thanh toán | Trạng thái |
|-------|--------|-------|-----------|---------------|------------|
| INV-202512-A0101 | A-0101 | 12/2025 | 2,850,000 | 2,850,000 | Paid |
| INV-202512-A0102 | A-0102 | 12/2025 | 3,420,000 | 3,420,000 | Paid |
| INV-202512-A0201 | A-0201 | 12/2025 | 4,150,000 | 4,150,000 | Paid |
| INV-202512-A0202 | A-0202 | 12/2025 | 3,890,000 | 3,890,000 | Paid |
| INV-202601-A0101 | A-0101 | 01/2026 | 2,950,000 | 2,950,000 | Paid |
| INV-202601-A0102 | A-0102 | 01/2026 | 3,280,000 | 3,280,000 | Paid |
| INV-202601-A0201 | A-0201 | 01/2026 | 4,350,000 | 4,350,000 | Paid |
| INV-202601-A0202 | A-0202 | 01/2026 | 3,720,000 | 0 | Overdue |
| INV-202602-A0101 | A-0101 | 02/2026 | 2,780,000 | 0 | Pending |
| INV-202602-A0102 | A-0102 | 02/2026 | 3,150,000 | 0 | Pending |
| ... | ... | ... | ... | ... | ... |

---

#### 13. Seed bảng `InvoiceDetails`
Chi tiết cho mỗi hóa đơn (Ví dụ INV-202602-A0101):

| Hóa đơn | Dịch vụ | Số lượng | Đơn giá | Thành tiền |
|---------|---------|----------|---------|------------|
| INV-202602-A0101 | Tiền điện | 320 kWh | 3,500 | 1,120,000 |
| INV-202602-A0101 | Tiền nước | 12 m³ | 15,000 | 180,000 |
| INV-202602-A0101 | Phí quản lý | 45 m² | 18,000 | 810,000 |
| INV-202602-A0101 | Phí gửi xe máy | 1 xe | 150,000 | 150,000 |
| INV-202602-A0101 | Phí gửi ô tô | 1 xe | 1,500,000 | 520,000 |
| **Tổng** | | | | **2,780,000** |

---

#### 14. Seed bảng `PaymentTransactions`
Tạo 45 giao dịch thanh toán:

| Mã GD | Hóa đơn | Số tiền | Phương thức | Ngày thanh toán | Trạng thái |
|-------|---------|---------|-------------|-----------------|------------|
| TXN-20260105-001 | INV-202512-A0101 | 2,850,000 | Chuyển khoản | 05/01/2026 | Success |
| TXN-20260106-001 | INV-202512-A0102 | 3,420,000 | Tiền mặt | 06/01/2026 | Success |
| TXN-20260107-001 | INV-202512-A0201 | 4,150,000 | VNPay | 07/01/2026 | Success |
| TXN-20260108-001 | INV-202512-A0202 | 3,890,000 | Chuyển khoản | 08/01/2026 | Success |
| TXN-20260203-001 | INV-202601-A0101 | 2,950,000 | MoMo | 03/02/2026 | Success |
| TXN-20260204-001 | INV-202601-A0102 | 3,280,000 | Tiền mặt | 04/02/2026 | Success |
| TXN-20260205-001 | INV-202601-A0201 | 4,350,000 | Chuyển khoản | 05/02/2026 | Success |
| ... | ... | ... | ... | ... | ... |

---

#### 15. Seed bảng `Requests`
Tạo 35 yêu cầu sửa chữa/khiếu nại:

| Mã yêu cầu | Căn hộ | Tiêu đề | Loại | Mức độ | Trạng thái |
|------------|--------|---------|------|--------|------------|
| REQ-202602-001 | A-0101 | Điều hòa phòng khách không lạnh | Sửa chữa | Cao | Đang xử lý |
| REQ-202602-002 | A-0201 | Rò rỉ nước bồn rửa bát | Sửa chữa | Khẩn cấp | Hoàn thành |
| REQ-202602-003 | A-0302 | Đèn hành lang tầng 3 hỏng | Sửa chữa | Trung bình | Chờ xử lý |
| REQ-202602-004 | B-0101 | Cửa ra vào bị kẹt khóa | Sửa chữa | Cao | Đang xử lý |
| REQ-202602-005 | B-0201 | Thang máy Block B hay bị kẹt | Khiếu nại | Cao | Đã chuyển BQT |
| REQ-202602-006 | A-0401 | Tiếng ồn từ căn hộ tầng trên | Khiếu nại | Trung bình | Đang xử lý |
| REQ-202602-007 | A-0501 | Nước nóng không hoạt động | Sửa chữa | Cao | Hoàn thành |
| REQ-202602-008 | B-0102 | Wifi khu vực hành lang yếu | Góp ý | Thấp | Chờ xử lý |
| REQ-202602-009 | A-0602 | Bồn cầu bị tắc | Sửa chữa | Khẩn cấp | Hoàn thành |
| REQ-202602-010 | B-0302 | Cửa kính ban công bị nứt | Sửa chữa | Trung bình | Chờ vật tư |
| ... | ... | ... | ... | ... | ... |

---

#### 16. Seed bảng `Announcements`
Tạo 12 thông báo:

| Tiêu đề | Nguồn | Ngày đăng | Ghim | Trạng thái |
|---------|-------|-----------|------|------------|
| Lịch cúp điện bảo trì ngày 20/02/2026 | BQL | 14/02/2026 | Có | Active |
| Thông báo họp cư dân Quý 1/2026 | BQT | 10/02/2026 | Có | Active |
| Quy định mới về gửi xe từ 01/03/2026 | BQL | 08/02/2026 | Không | Active |
| Chúc mừng năm mới Bính Ngọ 2026 | BQL | 01/01/2026 | Không | Active |
| Thông báo tăng phí quản lý từ 01/01/2026 | BQT | 15/12/2025 | Không | Active |
| Lịch phun thuốc diệt côn trùng | BQL | 05/02/2026 | Không | Active |
| Thông báo bảo trì thang máy Block A | BQL | 12/02/2026 | Có | Active |
| Kết quả cuộc họp Ban Quản Trị tháng 01/2026 | BQT | 02/02/2026 | Không | Active |
| Hướng dẫn sử dụng app quản lý chung cư | BQL | 20/01/2026 | Không | Active |
| Thông báo nghỉ Tết Nguyên Đán 2026 | BQL | 25/01/2026 | Không | Archived |
| Quy định phòng cháy chữa cháy | BQL | 10/01/2026 | Không | Active |
| Danh sách số điện thoại khẩn cấp | BQL | 05/01/2026 | Có | Active |

---

#### 17. Seed bảng `Visitors`
Tạo 25 lượt đăng ký khách:

| Tên khách | CCCD | Căn hộ | Mục đích | Ngày đến | Giờ vào | Giờ ra | Trạng thái |
|-----------|------|--------|----------|----------|---------|--------|------------|
| Nguyễn Văn An | 079095111222 | A-0101 | Thăm người thân | 14/02/2026 | 09:30 | 11:45 | Đã ra |
| Trần Thị Bích | 079088222333 | A-0201 | Thăm bạn bè | 14/02/2026 | 14:00 | - | Đang ở |
| Lê Văn Cường | 079090333444 | A-0302 | Giao hàng Shopee | 13/02/2026 | 10:15 | 10:20 | Đã ra |
| Phạm Thị Dung | 079085444555 | B-0101 | Thăm người thân | 13/02/2026 | 15:00 | 18:30 | Đã ra |
| Hoàng Văn Em | 079092555666 | A-0401 | Sửa chữa điều hòa | 12/02/2026 | 08:00 | 12:00 | Đã ra |
| Vũ Thị Phương | 079088666777 | B-0201 | Giao hàng Lazada | 12/02/2026 | 11:30 | 11:35 | Đã ra |
| Đỗ Văn Giang | 079078777888 | A-0501 | Thợ sửa ống nước | 11/02/2026 | 14:00 | 17:00 | Đã ra |
| ... | ... | ... | ... | ... | ... | ... | ... |

---

#### 18. Seed bảng `Notifications`
Tạo 80 thông báo cá nhân:

| Người nhận | Tiêu đề | Nội dung | Loại | Đã đọc | Ngày tạo |
|------------|---------|----------|------|--------|----------|
| Nguyễn Văn Hùng | Hóa đơn tháng 02/2026 | Hóa đơn tháng 02/2026 đã được tạo. Tổng: 2,780,000đ | Invoice | Chưa | 10/02/2026 |
| Nguyễn Văn Hùng | Yêu cầu REQ-202602-001 đang xử lý | Yêu cầu sửa điều hòa của bạn đang được xử lý | Request | Đã đọc | 13/02/2026 |
| Lê Hoàng Nam | Xác nhận đặt sân Tennis | Đặt sân Tennis ngày 12/02 từ 07:00-08:00 đã xác nhận | Booking | Đã đọc | 11/02/2026 |
| Trần Thị Lan | Thông báo mới từ BQL | Lịch cúp điện bảo trì ngày 20/02/2026 | Announcement | Chưa | 14/02/2026 |
| Bùi Quốc Việt | Thanh toán thành công | Hóa đơn INV-202601-A0302 đã thanh toán thành công | Payment | Đã đọc | 05/02/2026 |
| ... | ... | ... | ... | ... | ... |

---

#### 19. Seed bảng `ServiceOrders`
Tạo 18 đơn đặt dịch vụ:

| Mã đơn | Căn hộ | Cư dân | Dịch vụ | Ngày đặt | Ngày thực hiện | Trạng thái | Đánh giá |
|--------|--------|--------|---------|----------|----------------|------------|----------|
| SO-202602-001 | A-0101 | Nguyễn Văn Hùng | Dọn vệ sinh | 10/02/2026 | 11/02/2026 | Hoàn thành | 5 sao |
| SO-202602-002 | A-0201 | Lê Minh Khoa | Giặt ủi | 11/02/2026 | 12/02/2026 | Hoàn thành | 4 sao |
| SO-202602-003 | A-0302 | Bùi Quốc Việt | Sửa chữa nhỏ | 12/02/2026 | 13/02/2026 | Hoàn thành | 5 sao |
| SO-202602-004 | B-0101 | Đinh Văn Phong | Dọn vệ sinh | 13/02/2026 | 14/02/2026 | Đang xử lý | - |
| SO-202602-005 | A-0401 | Hoàng Văn Dũng | Giặt ủi | 13/02/2026 | - | Chờ xử lý | - |
| SO-202602-006 | B-0201 | Cao Minh Quân | Chuyển đồ | 14/02/2026 | - | Chờ xử lý | - |
| SO-202602-007 | A-0501 | Trịnh Công Minh | Nấu ăn tại nhà | 08/02/2026 | 08/02/2026 | Hoàn thành | 5 sao |
| SO-202602-008 | A-0602 | Phan Thị Yến | Trông trẻ | 09/02/2026 | 09/02/2026 | Hoàn thành | 4 sao |
| ... | ... | ... | ... | ... | ... | ... | ... |

---

#### 20. Seed bảng `Documents`
Tạo 10 tài liệu:

| Tên tài liệu | Loại | Người tải | Ngày tải |
|--------------|------|-----------|----------|
| Nội quy chung cư Sunrise 2026.pdf | Nội quy | Admin | 01/01/2026 |
| Quy định sử dụng tiện ích.pdf | Quy định | BQL_Manager | 05/01/2026 |
| Biên bản họp BQT tháng 01-2026.pdf | Biên bản | BQT_Head | 02/02/2026 |
| Báo cáo tài chính Quý 4-2025.xlsx | Báo cáo | BQL_Manager | 10/01/2026 |
| Hướng dẫn sử dụng app cư dân.pdf | Hướng dẫn | Admin | 15/01/2026 |
| ... | ... | ... | ... |

---

#### 21. Seed bảng `ActivityLogs`
Tạo 100 log hoạt động:

| Thời gian | User | Hành động | Đối tượng | Mô tả |
|-----------|------|-----------|-----------|-------|
| 14/02/2026 08:30 | admin | Login | User | Đăng nhập hệ thống |
| 14/02/2026 09:00 | staff1 | Create | Invoice | Tạo hóa đơn INV-202602-A0101 |
| 14/02/2026 09:15 | manager | Approve | Invoice | Phê duyệt hóa đơn INV-202602-A0101 |
| 14/02/2026 10:00 | resident | Create | Request | Tạo yêu cầu REQ-202602-001 |
| 14/02/2026 10:30 | staff2 | Update | Request | Cập nhật trạng thái REQ-202602-001 |
| 14/02/2026 11:00 | bqt_head | Create | Announcement | Đăng thông báo họp cư dân |
| ... | ... | ... | ... | ... |

---

#### 22. Seed bảng `FaceAuthHistory`
Tạo 25 lịch sử xác thực (cho các cư dân đã đăng ký face):

| Cư dân | Thời gian | Kết quả | Độ khớp | IP | Thiết bị |
|--------|-----------|---------|---------|----|---------| 
| Nguyễn Văn Hùng | 14/02/2026 07:45 | Thành công | 95.2% | 192.168.1.100 | Cổng chính |
| Lê Hoàng Nam | 14/02/2026 08:00 | Thành công | 92.8% | 192.168.1.100 | Cổng chính |
| Trần Thị Lan | 14/02/2026 08:15 | Thành công | 88.5% | 192.168.1.101 | Cổng phụ |
| Phạm Thị Hoa | 14/02/2026 09:30 | Thất bại | 45.2% | 192.168.1.100 | Cổng chính |
| Lê Minh Khoa | 13/02/2026 18:00 | Thành công | 91.0% | 192.168.1.100 | Cổng chính |
| ... | ... | ... | ... | ... | ... |

---

### Further Considerations

1. **Thứ tự thực hiện:** Phải seed theo đúng thứ tự dependency:
   ```
   Users → Apartments → Residents → Contracts → ContractMembers 
   → Vehicles → ResidentCards → ServiceTypes → ServicePrices 
   → Amenities → AmenityBookings → Invoices → InvoiceDetails 
   → PaymentTransactions → Requests → Announcements → Visitors 
   → Notifications → ServiceOrders → Documents → ActivityLogs 
   → FaceAuthHistory
   ```

2. **Tính nhất quán dữ liệu:**
   - Tổng tiền hóa đơn = Σ chi tiết
   - Xe phải thuộc cư dân đang ở căn hộ có trạng thái Occupied
   - Hợp đồng Active chỉ có 1 per căn hộ
   - Thẻ cư dân chỉ cấp cho cư dân có hợp đồng Active

3. **Dữ liệu thực tế Việt Nam:**
   - Tên: Họ + Tên đệm + Tên (Nguyễn Văn Hùng, Trần Thị Lan...)
   - CCCD: 12 số, bắt đầu bằng mã tỉnh (079 = TP.HCM)
   - Biển số xe: Theo format thực tế (51A-123.45 cho ô tô, 59B1-234.56 cho xe máy)
   - Số điện thoại: 10 số, bắt đầu 09x, 08x, 07x, 03x
   - Giá cả: Theo mức giá thực tế tại TP.HCM năm 2026

