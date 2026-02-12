# Đề Xuất Cập Nhật Vai Trò & Chức Năng
## Dự án: Hệ Thống Quản Lý Chung Cư
### Nhóm: 5
### Ngày: 11/02/2026

---

## 1. Tổng Quan Cấu Trúc Tổ Chức Mới

### 1.1 Sơ Đồ Phân Cấp

```
┌─────────────────────────────────────────────────────────────────┐
│                         ADMIN (Quản trị hệ thống)               │
│                    Toàn quyền quản lý hệ thống                  │
└─────────────────────────────────────────────────────────────────┘
                                │
        ┌───────────────────────┴───────────────────────┐
        ▼                                               ▼
┌───────────────────────┐                 ┌───────────────────────┐
│   BAN QUẢN LÝ (BQL)   │                 │   BAN QUẢN TRỊ (BQT)  │
│  (Điều hành vận hành) │                 │  (Giám sát & đại diện)│
└───────────────────────┘                 └───────────────────────┘
        │                                               │
    ┌───┴───┐                                       ┌───┴───┐
    ▼       ▼                                       ▼       ▼
┌───────┐ ┌───────┐                           ┌───────┐ ┌───────┐
│Manager│ │ Staff │                           │ Head  │ │Member │
└───────┘ └───────┘                           └───────┘ └───────┘
```

### 1.2 Giải Thích Thuật Ngữ

| Vai Trò | Tên Tiếng Việt | Mô Tả Ngắn |
|---------|----------------|------------|
| **Admin** | Quản trị viên hệ thống | Toàn quyền cấu hình, quản lý kỹ thuật hệ thống |
| **BQL_Manager** | Trưởng Ban Quản Lý | Điều hành hoạt động vận hành chung cư |
| **BQL_Staff** | Nhân viên Ban Quản Lý | Thực hiện công việc vận hành hàng ngày |
| **BQT_Head** | Trưởng Ban Quản Trị | Đại diện cư dân, giám sát BQL, quyết định lớn |
| **BQT_Member** | Thành viên Ban Quản Trị | Hỗ trợ giám sát, đại diện tiếng nói cư dân |
| **Resident** | Cư dân | Người sử dụng dịch vụ chung cư |

---

## 2. Phân Tích Chi Tiết Từng Vai Trò

### 2.1 ADMIN - Quản Trị Viên Hệ Thống

#### 2.1.1 Định Nghĩa
Admin là người có toàn quyền truy cập và quản lý hệ thống ở cấp độ kỹ thuật. Admin không tham gia vào hoạt động vận hành hàng ngày mà tập trung vào cấu hình, bảo trì và hỗ trợ kỹ thuật.

#### 2.1.2 Trách Nhiệm Chính
- Quản lý tài khoản người dùng toàn hệ thống
- Cấu hình hệ thống và các tham số
- Backup/Restore dữ liệu
- Giám sát hiệu năng hệ thống
- Hỗ trợ kỹ thuật cho các vai trò khác

#### 2.1.3 Danh Sách Chức Năng

| STT | Chức Năng | Mô Tả Chi Tiết | Độ Ưu Tiên |
|-----|-----------|----------------|------------|
| A01 | Quản lý tài khoản người dùng | Tạo, sửa, khóa, xóa tài khoản cho tất cả vai trò | **Cao** |
| A02 | Phân quyền vai trò | Gán vai trò (BQL_Manager, BQL_Staff, BQT_Head, BQT_Member) cho người dùng | **Cao** |
| A03 | Cấu hình hệ thống | Thiết lập các tham số chung (tên chung cư, logo, thông tin liên hệ) | **Cao** |
| A04 | Quản lý danh mục dịch vụ | Định nghĩa các loại dịch vụ (Điện, Nước, Internet, Gửi xe...) | **Cao** |
| A05 | Cấu hình giá dịch vụ | Thiết lập đơn giá cho từng loại dịch vụ | **Cao** |
| A06 | Quản lý tiện ích | Cấu hình các tiện ích (Gym, Hồ bơi, BBQ, Phòng họp) | Trung bình |
| A07 | Xem log hệ thống | Theo dõi lịch sử hoạt động, audit trail | Trung bình |
| A08 | Backup/Restore dữ liệu | Sao lưu và khôi phục dữ liệu hệ thống | Trung bình |
| A09 | Quản lý thông báo hệ thống | Gửi thông báo bảo trì, cập nhật hệ thống | Thấp |
| A10 | Xem báo cáo tổng hợp | Truy cập tất cả báo cáo trong hệ thống | Trung bình |

---

### 2.2 BQL_MANAGER - Trưởng Ban Quản Lý

#### 2.2.1 Định Nghĩa
BQL_Manager là người đứng đầu đội ngũ vận hành chung cư, chịu trách nhiệm điều phối công việc, giám sát nhân viên, và đảm bảo chất lượng dịch vụ. Là cầu nối giữa BQL và BQT.

#### 2.2.2 Trách Nhiệm Chính
- Điều hành hoạt động vận hành hàng ngày
- Giám sát và phân công công việc cho BQL_Staff
- Phê duyệt các nghiệp vụ quan trọng
- Báo cáo cho BQT về tình hình hoạt động
- Xử lý các vấn đề escalation từ nhân viên

#### 2.2.3 Danh Sách Chức Năng

| STT | Chức Năng | Mô Tả Chi Tiết | Độ Ưu Tiên |
|-----|-----------|----------------|------------|
| **QUẢN LÝ CĂN HỘ & CƯ DÂN** |
| M01 | Xem tổng quan căn hộ | Dashboard hiển thị trạng thái tất cả căn hộ | **Cao** |
| M02 | Quản lý hồ sơ cư dân | Xem, cập nhật thông tin cư dân, phê duyệt đăng ký mới | **Cao** |
| M03 | Quản lý hợp đồng thuê/mua | Tạo, gia hạn, chấm dứt hợp đồng | **Cao** |
| M04 | Phê duyệt chuyển đi/đến | Xác nhận thủ tục chuyển căn hộ | **Cao** |
| **QUẢN LÝ TÀI CHÍNH** |
| M05 | Xem tổng quan tài chính | Dashboard doanh thu, công nợ theo tháng/quý/năm | **Cao** |
| M06 | Phê duyệt hóa đơn | Kiểm tra và phê duyệt hóa đơn trước khi gửi cư dân | **Cao** |
| M07 | Xử lý công nợ | Gửi nhắc nợ, áp dụng phí phạt chậm thanh toán | **Cao** |
| M08 | Phê duyệt hoàn tiền | Xác nhận các khoản hoàn trả cho cư dân | Trung bình |
| **QUẢN LÝ NHÂN SỰ & CÔNG VIỆC** |
| M09 | Phân công công việc | Giao việc cho BQL_Staff, theo dõi tiến độ | **Cao** |
| M10 | Đánh giá hiệu suất nhân viên | Xem báo cáo công việc của từng nhân viên | Trung bình |
| M11 | Quản lý ca trực | Lên lịch trực cho nhân viên bảo vệ, lễ tân | Trung bình |
| **QUẢN LÝ YÊU CẦU & SỰ CỐ** |
| M12 | Giám sát yêu cầu | Xem tất cả yêu cầu, phân loại theo độ ưu tiên | **Cao** |
| M13 | Phân công xử lý yêu cầu | Giao yêu cầu cho nhân viên phù hợp | **Cao** |
| M14 | Xử lý escalation | Tiếp nhận và giải quyết các vấn đề phức tạp | **Cao** |
| M15 | Đóng yêu cầu | Xác nhận hoàn thành và đóng ticket | **Cao** |
| **TRUYỀN THÔNG** |
| M16 | Đăng thông báo | Tạo và gửi thông báo cho cư dân | **Cao** |
| M17 | Quản lý thông báo | Chỉnh sửa, xóa, ghim thông báo quan trọng | Trung bình |
| **BÁO CÁO** |
| M18 | Xem báo cáo vận hành | Thống kê yêu cầu, sự cố, thời gian xử lý | **Cao** |
| M19 | Xuất báo cáo tài chính | Tạo báo cáo thu chi cho BQT | **Cao** |
| M20 | Báo cáo định kỳ cho BQT | Chuẩn bị báo cáo tuần/tháng cho BQT | Trung bình |

---

### 2.3 BQL_STAFF - Nhân Viên Ban Quản Lý

#### 2.3.1 Định Nghĩa
BQL_Staff là nhân viên thực hiện các công việc vận hành hàng ngày tại chung cư. Bao gồm: lễ tân, bảo vệ, nhân viên kỹ thuật, nhân viên thu phí.

#### 2.3.2 Trách Nhiệm Chính
- Thực hiện công việc được giao từ Manager
- Tiếp nhận và xử lý yêu cầu của cư dân
- Ghi nhận chỉ số điện nước
- Quản lý ra/vào tòa nhà
- Báo cáo công việc hàng ngày

#### 2.3.3 Danh Sách Chức Năng

| STT | Chức Năng | Mô Tả Chi Tiết | Độ Ưu Tiên |
|-----|-----------|----------------|------------|
| **QUẢN LÝ CƯ DÂN** |
| S01 | Xem danh sách cư dân | Tra cứu thông tin cư dân theo căn hộ | **Cao** |
| S02 | Cập nhật thông tin cư dân | Sửa thông tin liên hệ, thành viên trong hộ | Trung bình |
| S03 | Quản lý thẻ cư dân | Cấp mới, khóa, kích hoạt thẻ ra vào | **Cao** |
| S04 | Đăng ký xe | Thêm/xóa phương tiện của cư dân | **Cao** |
| **QUẢN LÝ KHÁCH** |
| S05 | Đăng ký khách đến | Ghi nhận thông tin khách, căn hộ đến thăm | **Cao** |
| S06 | Xác nhận khách ra | Đánh dấu thời gian khách rời đi | **Cao** |
| S07 | Quản lý khách đăng ký trước | Xem danh sách khách được cư dân đăng ký trước | Trung bình |
| **QUẢN LÝ DỊCH VỤ & HÓA ĐƠN** |
| S08 | Ghi chỉ số điện nước | Nhập chỉ số công tơ hàng tháng | **Cao** |
| S09 | Tạo hóa đơn | Tính toán và tạo hóa đơn theo chỉ số | **Cao** |
| S10 | Thu tiền mặt | Ghi nhận thanh toán tiền mặt tại quầy | **Cao** |
| S11 | Xác nhận chuyển khoản | Đối chiếu và xác nhận thanh toán chuyển khoản | **Cao** |
| S12 | In hóa đơn/biên lai | Xuất hóa đơn, biên lai cho cư dân | Trung bình |
| **QUẢN LÝ YÊU CẦU** |
| S13 | Xem yêu cầu được giao | Danh sách công việc cần thực hiện | **Cao** |
| S14 | Cập nhật trạng thái yêu cầu | Đánh dấu đang xử lý, hoàn thành | **Cao** |
| S15 | Ghi chú xử lý | Thêm comment, ảnh vào yêu cầu | Trung bình |
| S16 | Chuyển tiếp yêu cầu | Escalate lên Manager nếu cần | Trung bình |
| **QUẢN LÝ BƯU KIỆN** |
| S17 | Nhận bưu kiện | Ghi nhận bưu kiện đến cho cư dân | **Cao** |
| S18 | Thông báo bưu kiện | Gửi thông báo cho cư dân đến nhận | **Cao** |
| S19 | Xác nhận giao bưu kiện | Đánh dấu đã giao cho cư dân | **Cao** |
| **TIỆN ÍCH** |
| S20 | Xem lịch đặt tiện ích | Tra cứu lịch đặt phòng, sân, hồ bơi | Trung bình |
| S21 | Xác nhận sử dụng tiện ích | Check-in khi cư dân đến sử dụng | Trung bình |

---

### 2.4 BQT_HEAD - Trưởng Ban Quản Trị

#### 2.4.1 Định Nghĩa
BQT_Head là đại diện cao nhất của cư dân, được bầu để giám sát hoạt động của BQL và đưa ra các quyết định quan trọng ảnh hưởng đến toàn bộ chung cư.

#### 2.4.2 Trách Nhiệm Chính
- Giám sát hoạt động của BQL
- Phê duyệt các quyết định tài chính lớn
- Đại diện cư dân trong các vấn đề pháp lý
- Chủ trì các cuộc họp cư dân
- Giải quyết khiếu nại, tranh chấp

#### 2.4.3 Danh Sách Chức Năng

| STT | Chức Năng | Mô Tả Chi Tiết | Độ Ưu Tiên |
|-----|-----------|----------------|------------|
| **GIÁM SÁT HOẠT ĐỘNG** |
| H01 | Dashboard tổng quan | Xem tình hình chung: tài chính, yêu cầu, cư dân | **Cao** |
| H02 | Xem báo cáo tài chính | Truy cập báo cáo thu chi chi tiết | **Cao** |
| H03 | Xem báo cáo vận hành | Thống kê yêu cầu, sự cố, hiệu suất xử lý | **Cao** |
| H04 | So sánh kỳ | So sánh các chỉ số theo tháng/quý/năm | Trung bình |
| **PHÊ DUYỆT** |
| H05 | Phê duyệt chi tiêu lớn | Duyệt các khoản chi vượt ngưỡng quy định | **Cao** |
| H06 | Phê duyệt thay đổi giá dịch vụ | Xác nhận điều chỉnh đơn giá | **Cao** |
| H07 | Phê duyệt hợp đồng nhà thầu | Duyệt hợp đồng bảo trì, sửa chữa lớn | Trung bình |
| **QUẢN LÝ CUỘC HỌP** |
| H08 | Tạo cuộc họp cư dân | Lên lịch họp định kỳ hoặc bất thường | **Cao** |
| H09 | Gửi thông báo họp | Thông báo cho cư dân về cuộc họp | **Cao** |
| H10 | Ghi biên bản họp | Lưu trữ nội dung, quyết định cuộc họp | Trung bình |
| H11 | Tạo khảo sát/bỏ phiếu | Thu thập ý kiến cư dân về các vấn đề | Trung bình |
| **GIẢI QUYẾT KHIẾU NẠI** |
| H12 | Xem khiếu nại | Danh sách khiếu nại từ cư dân | **Cao** |
| H13 | Phản hồi khiếu nại | Trả lời và đưa ra hướng giải quyết | **Cao** |
| H14 | Chuyển khiếu nại cho BQL | Yêu cầu BQL xử lý và báo cáo | Trung bình |
| **TRUYỀN THÔNG** |
| H15 | Đăng thông báo quan trọng | Thông báo quyết định, thay đổi chính sách | **Cao** |
| H16 | Xem phản hồi cư dân | Theo dõi ý kiến, góp ý từ cư dân | Trung bình |
| **QUẢN LÝ QUỸ** |
| H17 | Xem quỹ bảo trì | Theo dõi số dư quỹ bảo trì chung cư | **Cao** |
| H18 | Phê duyệt sử dụng quỹ | Duyệt chi từ quỹ bảo trì | **Cao** |

---

### 2.5 BQT_MEMBER - Thành Viên Ban Quản Trị

#### 2.5.1 Định Nghĩa
BQT_Member là thành viên được bầu vào Ban Quản Trị, hỗ trợ BQT_Head trong việc giám sát và đại diện cho tiếng nói của cư dân.

#### 2.5.2 Trách Nhiệm Chính
- Hỗ trợ giám sát hoạt động BQL
- Tiếp nhận ý kiến từ cư dân
- Tham gia các cuộc họp BQT
- Đề xuất ý kiến cải thiện

#### 2.5.3 Danh Sách Chức Năng

| STT | Chức Năng | Mô Tả Chi Tiết | Độ Ưu Tiên |
|-----|-----------|----------------|------------|
| **GIÁM SÁT** |
| B01 | Xem dashboard tổng quan | Xem tình hình chung của chung cư | **Cao** |
| B02 | Xem báo cáo tài chính | Truy cập báo cáo thu chi (chỉ xem) | **Cao** |
| B03 | Xem báo cáo vận hành | Thống kê yêu cầu, sự cố | Trung bình |
| **THAM GIA QUYẾT ĐỊNH** |
| B04 | Xem đề xuất cần duyệt | Danh sách đề xuất đang chờ BQT quyết định | **Cao** |
| B05 | Bỏ phiếu/Góp ý | Tham gia biểu quyết các vấn đề | **Cao** |
| B06 | Xem lịch sử quyết định | Tra cứu các quyết định đã đưa ra | Trung bình |
| **TIẾP NHẬN Ý KIẾN** |
| B07 | Xem phản hồi cư dân | Đọc ý kiến, góp ý từ cư dân | **Cao** |
| B08 | Chuyển tiếp vấn đề | Đề xuất lên BQT_Head các vấn đề cần giải quyết | Trung bình |
| B09 | Phản hồi cư dân | Trả lời trực tiếp các thắc mắc đơn giản | Trung bình |
| **CUỘC HỌP** |
| B10 | Xem lịch họp | Tra cứu lịch họp BQT, họp cư dân | **Cao** |
| B11 | Xem biên bản họp | Đọc nội dung các cuộc họp đã diễn ra | Trung bình |
| B12 | Đề xuất nội dung họp | Thêm vấn đề vào agenda cuộc họp | Trung bình |

---

### 2.6 RESIDENT - Cư Dân (Giữ nguyên, bổ sung)

#### 2.6.1 Danh Sách Chức Năng

| STT | Chức Năng | Mô Tả Chi Tiết | Độ Ưu Tiên |
|-----|-----------|----------------|------------|
| **TÀI KHOẢN** |
| R01 | Đăng nhập | Đăng nhập vào hệ thống | **Cao** |
| R02 | Xem/Cập nhật hồ sơ | Chỉnh sửa thông tin cá nhân | Trung bình |
| R03 | Đổi mật khẩu | Thay đổi mật khẩu đăng nhập | Trung bình |
| **HÓA ĐƠN & THANH TOÁN** |
| R04 | Xem hóa đơn | Danh sách hóa đơn theo tháng | **Cao** |
| R05 | Xem chi tiết hóa đơn | Các khoản phí chi tiết | **Cao** |
| R06 | Thanh toán online | Thanh toán qua cổng thanh toán | **Cao** |
| R07 | Xem lịch sử thanh toán | Tra cứu các giao dịch đã thực hiện | Trung bình |
| **YÊU CẦU & KHIẾU NẠI** |
| R08 | Gửi yêu cầu | Tạo ticket sửa chữa, báo sự cố | **Cao** |
| R09 | Theo dõi yêu cầu | Xem trạng thái xử lý yêu cầu | **Cao** |
| R10 | Đánh giá dịch vụ | Rate chất lượng xử lý yêu cầu | Trung bình |
| R11 | Gửi khiếu nại lên BQT | Khiếu nại trực tiếp đến Ban Quản Trị | Trung bình |
| **TIỆN ÍCH** |
| R12 | Đặt tiện ích | Book phòng gym, hồ bơi, BBQ | Trung bình |
| R13 | Xem lịch đặt | Tra cứu các booking của mình | Trung bình |
| R14 | Hủy đặt tiện ích | Hủy booking trước thời hạn | Trung bình |
| **THÔNG TIN** |
| R15 | Xem thông báo | Đọc thông báo từ BQL, BQT | **Cao** |
| R16 | Xem hợp đồng | Tra cứu hợp đồng thuê/mua | Trung bình |
| R17 | Đăng ký khách | Đăng ký trước khách đến thăm | Trung bình |
| R18 | Xem bưu kiện | Danh sách bưu kiện đang chờ nhận | Trung bình |
| **GÓP Ý** |
| R19 | Gửi góp ý | Đóng góp ý kiến cho BQL/BQT | Trung bình |
| R20 | Tham gia khảo sát | Trả lời các khảo sát từ BQT | Trung bình |
| R21 | Bỏ phiếu | Tham gia bỏ phiếu các vấn đề chung | Trung bình |

---

## 3. Ma Trận Phân Quyền Tổng Hợp

### 3.1 Quản Lý Căn Hộ & Cư Dân

| Chức Năng | Admin | BQL_Manager | BQL_Staff | BQT_Head | BQT_Member | Resident |
|-----------|:-----:|:-----------:|:---------:|:--------:|:----------:|:--------:|
| Xem danh sách căn hộ | ✅ | ✅ | ✅ | ✅ | ✅ | ❌ |
| Xem thông tin cư dân | ✅ | ✅ | ✅ | ✅ | 👁️ | ❌ |
| Thêm/Sửa cư dân | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ |
| Xóa cư dân | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ |
| Quản lý thẻ cư dân | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ |
| Quản lý xe | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ |

*Chú thích: ✅ = Toàn quyền, 👁️ = Chỉ xem, ❌ = Không có quyền*

### 3.2 Quản Lý Tài Chính

| Chức Năng | Admin | BQL_Manager | BQL_Staff | BQT_Head | BQT_Member | Resident |
|-----------|:-----:|:-----------:|:---------:|:--------:|:----------:|:--------:|
| Cấu hình giá dịch vụ | ✅ | ❌ | ❌ | ✅* | ❌ | ❌ |
| Ghi chỉ số điện nước | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| Tạo hóa đơn | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| Phê duyệt hóa đơn | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| Thu tiền | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| Xem báo cáo tài chính | ✅ | ✅ | ❌ | ✅ | 👁️ | ❌ |
| Xem hóa đơn của mình | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |
| Phê duyệt chi tiêu lớn | ❌ | ❌ | ❌ | ✅ | ❌ | ❌ |

*✅* = Phê duyệt thay đổi giá*

### 3.3 Quản Lý Yêu Cầu & Sự Cố

| Chức Năng | Admin | BQL_Manager | BQL_Staff | BQT_Head | BQT_Member | Resident |
|-----------|:-----:|:-----------:|:---------:|:--------:|:----------:|:--------:|
| Tạo yêu cầu | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |
| Xem tất cả yêu cầu | ✅ | ✅ | ❌ | ✅ | 👁️ | ❌ |
| Xem yêu cầu được giao | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| Phân công yêu cầu | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| Xử lý yêu cầu | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| Đóng yêu cầu | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| Xem yêu cầu của mình | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |

### 3.4 Truyền Thông & Thông Báo

| Chức Năng | Admin | BQL_Manager | BQL_Staff | BQT_Head | BQT_Member | Resident |
|-----------|:-----:|:-----------:|:---------:|:--------:|:----------:|:--------:|
| Đăng thông báo hệ thống | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Đăng thông báo vận hành | ❌ | ✅ | ❌ | ❌ | ❌ | ❌ |
| Đăng thông báo BQT | ❌ | ❌ | ❌ | ✅ | ❌ | ❌ |
| Xem thông báo | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| Tạo khảo sát | ❌ | ❌ | ❌ | ✅ | ❌ | ❌ |
| Tham gia khảo sát | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |

---

## 4. Workflow Chính

### 4.1 Quy Trình Xử Lý Yêu Cầu Cư Dân

```
┌──────────┐     ┌──────────────┐     ┌─────────────┐     ┌──────────┐
│ Resident │────▶│  BQL_Staff   │────▶│ BQL_Manager │────▶│ BQT_Head │
│ Gửi yêu  │     │ Tiếp nhận    │     │ Escalation  │     │ Khiếu nại│
│ cầu      │     │ & Xử lý      │     │ nếu cần     │     │ nghiêm   │
└──────────┘     └──────────────┘     └─────────────┘     └──────────┘
     │                  │                    │                  │
     │                  ▼                    ▼                  ▼
     │           ┌─────────────┐      ┌─────────────┐    ┌─────────────┐
     │           │ Cập nhật    │      │ Phân công   │    │ Yêu cầu BQL │
     │           │ trạng thái  │      │ lại / Hỗ trợ│    │ giải quyết  │
     │           └─────────────┘      └─────────────┘    └─────────────┘
     │                  │                    │                  │
     └──────────────────┴────────────────────┴──────────────────┘
                               │
                               ▼
                        ┌─────────────┐
                        │  Hoàn thành │
                        │  & Đánh giá │
                        └─────────────┘
```

### 4.2 Quy Trình Thu Phí Hàng Tháng

```
                                    ┌─────────────┐
                                    │ BQL_Staff   │
                                    │ Ghi chỉ số  │
                                    └──────┬──────┘
                                           │
                                           ▼
                                    ┌─────────────┐
                                    │ BQL_Staff   │
                                    │ Tạo hóa đơn │
                                    └──────┬──────┘
                                           │
                                           ▼
                                    ┌─────────────┐
                                    │ BQL_Manager │
                                    │ Phê duyệt   │
                                    └──────┬──────┘
                                           │
                         ┌─────────────────┴─────────────────┐
                         ▼                                   ▼
                  ┌─────────────┐                     ┌─────────────┐
                  │   Gửi hóa   │                     │  Từ chối    │
                  │   đơn cho   │                     │  (sửa lại)  │
                  │   Resident  │                     └─────────────┘
                  └──────┬──────┘
                         │
           ┌─────────────┴─────────────┐
           ▼                           ▼
    ┌─────────────┐             ┌─────────────┐
    │ Thanh toán  │             │ Thanh toán  │
    │ Online      │             │ Tiền mặt    │
    └──────┬──────┘             └──────┬──────┘
           │                           │
           │                           ▼
           │                    ┌─────────────┐
           │                    │ BQL_Staff   │
           │                    │ Ghi nhận    │
           │                    └──────┬──────┘
           │                           │
           └───────────┬───────────────┘
                       ▼
                ┌─────────────┐
                │  Hoàn thành │
                └─────────────┘
```

### 4.3 Quy Trình Phê Duyệt Chi Tiêu Lớn

```
┌─────────────┐     ┌─────────────┐     ┌─────────────┐     ┌─────────────┐
│ BQL_Manager │────▶│  BQT_Head   │────▶│ BQT_Member  │────▶│   Quyết     │
│ Đề xuất chi │     │  Xem xét    │     │  Góp ý/Vote │     │   định      │
└─────────────┘     └─────────────┘     └─────────────┘     └─────────────┘
                           │
                           ▼
                    ┌─────────────┐
                    │  Phê duyệt  │───▶ Thực hiện chi
                    │  / Từ chối  │
                    └─────────────┘
```

---

## 5. Đề Xuất Cải Tiến Use Case

### 5.1 Use Case Mới Cần Bổ Sung

| ID | Use Case Name | Primary Actor | Mô Tả |
|:---|:---|:---|:---|
| UC-30 | Phê duyệt chi tiêu | BQT_Head | Duyệt các khoản chi vượt ngưỡng |
| UC-31 | Tạo cuộc họp | BQT_Head | Lên lịch và thông báo họp cư dân |
| UC-32 | Ghi biên bản họp | BQT_Head | Lưu trữ nội dung cuộc họp |
| UC-33 | Tạo khảo sát | BQT_Head | Thu thập ý kiến cư dân |
| UC-34 | Bỏ phiếu | Resident | Tham gia biểu quyết vấn đề chung |
| UC-35 | Xem báo cáo (BQT) | BQT_Head, BQT_Member | Giám sát hoạt động BQL |
| UC-36 | Gửi khiếu nại lên BQT | Resident | Khiếu nại trực tiếp đến BQT |
| UC-37 | Phản hồi khiếu nại | BQT_Head | Trả lời khiếu nại cư dân |
| UC-38 | Phân công công việc | BQL_Manager | Giao việc cho Staff |
| UC-39 | Đánh giá nhân viên | BQL_Manager | Xem hiệu suất xử lý công việc |
| UC-40 | Phê duyệt hóa đơn | BQL_Manager | Duyệt hóa đơn trước khi gửi |

### 5.2 Use Case Cần Cập Nhật Actor

| ID | Use Case Name | Actor Cũ | Actor Mới |
|:---|:---|:---|:---|
| UC-05 | Manage Users | Admin | Admin (tất cả), BQL_Manager (BQL_Staff) |
| UC-20 | Generate Invoice | Admin, System | BQL_Staff, BQL_Manager (duyệt), System |
| UC-22 | Process Payment | Staff | BQL_Staff |
| UC-25 | Process Request | Staff | BQL_Staff, BQL_Manager (escalation) |
| UC-26 | Post Announcement | Admin | BQL_Manager (vận hành), BQT_Head (chính sách) |

---

## 6. Kết Luận

### 6.1 Lợi Ích Của Cấu Trúc Mới

1. **Phân quyền rõ ràng**: Mỗi vai trò có trách nhiệm cụ thể, tránh chồng chéo
2. **Giám sát hiệu quả**: BQT có thể giám sát hoạt động của BQL
3. **Minh bạch tài chính**: Quy trình phê duyệt chi tiêu rõ ràng
4. **Tiếng nói cư dân**: Cư dân có thể khiếu nại trực tiếp đến BQT
5. **Quy trình chuẩn hóa**: Workflow rõ ràng cho từng nghiệp vụ

### 6.2 Các Bước Triển Khai Tiếp Theo

1. ✅ Cập nhật database schema cho các vai trò mới
2. ✅ Cập nhật bảng phân quyền trong code
3. ✅ Tạo các trang quản lý theo từng vai trò
4. ✅ Cập nhật UI/UX cho dashboard từng role
5. ✅ Testing phân quyền
6. ✅ Viết documentation hướng dẫn sử dụng

---

*Tài liệu này được tạo để đề xuất cập nhật hệ thống quản lý chung cư với cấu trúc vai trò mới, phù hợp với thực tế vận hành tại Việt Nam.*

---

## 7. Đánh Giá Cập Nhật Database

### 7.1 Phân Tích Hiện Trạng Database

#### 7.1.1 Bảng Users Hiện Tại
```sql
Table Users {
  UserId integer [pk]
  Username nvarchar(50)
  PasswordHash nvarchar(255)
  FullName nvarchar(200)
  Email nvarchar(100)
  PhoneNumber nvarchar(20)
  Role nvarchar(50) [note: 'Admin, Staff, Resident, Security']  -- CẦN CẬP NHẬT
  IsActive bit
  IsDeleted bit
  LastLogin datetime
  CreatedAt datetime
  UpdatedAt datetime
}
```

**Vấn đề**: Cột `Role` hiện tại chỉ hỗ trợ 4 giá trị: `Admin, Staff, Resident, Security`

#### 7.1.2 Vai Trò Mới Cần Hỗ Trợ
| Vai Trò Cũ | Vai Trò Mới | Ghi Chú |
|:-----------|:------------|:--------|
| Admin | Admin | Giữ nguyên |
| Staff | BQL_Manager, BQL_Staff | Tách thành 2 vai trò |
| Resident | Resident | Giữ nguyên |
| Security | BQL_Staff | Gộp vào BQL_Staff |
| *(Mới)* | BQT_Head | Thêm mới |
| *(Mới)* | BQT_Member | Thêm mới |

---

### 7.2 Các Thay Đổi Cần Thiết

#### ✅ 7.2.1 CẬP NHẬT GIÁ TRỊ CỘT ROLE (Bắt buộc)

**Mức độ ảnh hưởng**: 🔴 **CAO**

Cần cập nhật giá trị cho cột `Role` trong bảng `Users`:

```sql
-- Cập nhật comment/note cho cột Role
-- Giá trị mới: 'Admin, BQL_Manager, BQL_Staff, BQT_Head, BQT_Member, Resident'

-- Migration script để chuyển đổi dữ liệu cũ
UPDATE Users SET Role = 'BQL_Staff' WHERE Role = 'Staff';
UPDATE Users SET Role = 'BQL_Staff' WHERE Role = 'Security';
```

**Không cần thay đổi cấu trúc bảng** - chỉ cần cập nhật giá trị.

---

#### ✅ 7.2.2 THÊM BẢNG CUỘC HỌP (Tùy chọn - Ưu tiên Trung bình)

**Mức độ ảnh hưởng**: 🟡 **TRUNG BÌNH**

```sql
Table Meetings {
  MeetingId integer [pk, increment]
  Title nvarchar(200) [not null]
  Description nvarchar(max)
  MeetingType nvarchar(50) [note: 'BQT, Resident, Emergency']
  Location nvarchar(200)
  ScheduledDate datetime [not null]
  StartTime time
  EndTime time
  Status nvarchar(20) [note: 'Scheduled, InProgress, Completed, Cancelled']
  CreatedBy integer [not null, ref: > Users.UserId]
  MinutesContent nvarchar(max) [note: 'Biên bản họp']
  Attendees nvarchar(max) [note: 'JSON danh sách tham dự']
  CreatedAt datetime [default: `GETDATE()`]
  UpdatedAt datetime
}
```

---

#### ✅ 7.2.3 THÊM BẢNG KHẢO SÁT (Tùy chọn - Ưu tiên Thấp)

**Mức độ ảnh hưởng**: 🟢 **THẤP**

```sql
Table Surveys {
  SurveyId integer [pk, increment]
  Title nvarchar(200) [not null]
  Description nvarchar(max)
  SurveyType nvarchar(50) [note: 'Poll, Feedback, Voting']
  StartDate datetime [not null]
  EndDate datetime [not null]
  IsAnonymous bit [default: 0]
  Status nvarchar(20) [note: 'Draft, Active, Closed']
  CreatedBy integer [not null, ref: > Users.UserId]
  CreatedAt datetime [default: `GETDATE()`]
}

Table SurveyQuestions {
  QuestionId integer [pk, increment]
  SurveyId integer [not null, ref: > Surveys.SurveyId]
  QuestionText nvarchar(500) [not null]
  QuestionType nvarchar(50) [note: 'SingleChoice, MultiChoice, Text, Rating']
  Options nvarchar(max) [note: 'JSON cho các lựa chọn']
  IsRequired bit [default: 1]
  OrderIndex integer
}

Table SurveyResponses {
  ResponseId integer [pk, increment]
  SurveyId integer [not null, ref: > Surveys.SurveyId]
  QuestionId integer [not null, ref: > SurveyQuestions.QuestionId]
  ResidentId integer [ref: > Residents.UserId]
  Answer nvarchar(max)
  CreatedAt datetime [default: `GETDATE()`]
}
```

---

#### ✅ 7.2.4 THÊM BẢNG KHIẾU NẠI (Tùy chọn - Ưu tiên Trung bình)

**Mức độ ảnh hưởng**: 🟡 **TRUNG BÌNH**

Hiện tại bảng `Requests` có thể dùng với `RequestType = 'Complaint'`, nhưng nếu muốn tách riêng khiếu nại lên BQT:

```sql
Table Complaints {
  ComplaintId integer [pk, increment]
  ComplaintNumber nvarchar(50) [not null, unique]
  ApartmentId integer [not null, ref: > Apartments.ApartmentId]
  ResidentId integer [not null, ref: > Residents.UserId]
  Title nvarchar(200) [not null]
  Description nvarchar(max)
  Category nvarchar(50) [note: 'BQL_Service, Finance, Security, Noise, Other']
  Priority integer [note: '1:Low, 2:Medium, 3:High']
  Status integer [note: '0:New, 1:Reviewing, 2:Resolved, 3:Rejected']
  AssignedTo integer [ref: > Users.UserId, note: 'BQT_Head hoặc BQL_Manager']
  Resolution nvarchar(max)
  CreatedAt datetime [default: `GETDATE()`]
  UpdatedAt datetime
  ResolvedAt datetime
  ResolvedBy integer [ref: > Users.UserId]
}
```

**Lựa chọn thay thế**: Có thể sử dụng bảng `Requests` hiện có với thêm cột `EscalateTo` để chuyển lên BQT.

---

#### ✅ 7.2.5 THÊM CỘT CHO BẢNG REQUESTS (Khuyến nghị)

**Mức độ ảnh hưởng**: 🟢 **THẤP**

```sql
ALTER TABLE Requests ADD EscalatedTo integer NULL;  -- FK đến Users (BQT_Head)
ALTER TABLE Requests ADD EscalatedAt datetime NULL;
ALTER TABLE Requests ADD EscalationReason nvarchar(500) NULL;
```

---

#### ✅ 7.2.6 THÊM CỘT CHO BẢNG INVOICES (Khuyến nghị)

**Mức độ ảnh hưởng**: 🟢 **THẤP**

Thêm workflow phê duyệt:

```sql
ALTER TABLE Invoices ADD ApprovalStatus integer DEFAULT 0;  -- 0:Pending, 1:Approved, 2:Rejected
ALTER TABLE Invoices ADD ApprovedBy integer NULL;  -- FK đến Users (BQL_Manager)
ALTER TABLE Invoices ADD ApprovedAt datetime NULL;
ALTER TABLE Invoices ADD RejectionReason nvarchar(500) NULL;
```

---

#### ✅ 7.2.7 THÊM BẢNG PHÊ DUYỆT CHI TIÊU (Tùy chọn)

**Mức độ ảnh hưởng**: 🟡 **TRUNG BÌNH**

```sql
Table ExpenseApprovals {
  ApprovalId integer [pk, increment]
  Title nvarchar(200) [not null]
  Description nvarchar(max)
  Amount decimal(18,2) [not null]
  Category nvarchar(50) [note: 'Maintenance, Equipment, Service, Other']
  RequestedBy integer [not null, ref: > Users.UserId, note: 'BQL_Manager']
  Status integer [note: '0:Pending, 1:Approved, 2:Rejected']
  ApprovedBy integer [ref: > Users.UserId, note: 'BQT_Head']
  ApprovedAt datetime
  RejectionReason nvarchar(500)
  Attachments nvarchar(max) [note: 'JSON danh sách file đính kèm']
  CreatedAt datetime [default: `GETDATE()`]
  UpdatedAt datetime
}
```

---

#### ✅ 7.2.8 THÊM CỘT CHO BẢNG ANNOUNCEMENTS (Khuyến nghị)

**Mức độ ảnh hưởng**: 🟢 **THẤP**

Phân biệt thông báo từ BQL và BQT:

```sql
ALTER TABLE Announcements ADD Source nvarchar(20) DEFAULT 'BQL';  -- 'BQL', 'BQT', 'System'
```

---

### 7.3 Tóm Tắt Các Thay Đổi Database

| STT | Thay Đổi | Loại | Mức Độ Ưu Tiên | Bắt Buộc |
|:---:|:---------|:----:|:--------------:|:--------:|
| 1 | Cập nhật giá trị Role trong Users | Update | 🔴 **Cao** | ✅ Có |
| 2 | Thêm cột ApprovalStatus cho Invoices | Alter | 🟡 Trung bình | ⚠️ Khuyến nghị |
| 3 | Thêm cột Escalation cho Requests | Alter | 🟢 Thấp | ⚠️ Khuyến nghị |
| 4 | Thêm cột Source cho Announcements | Alter | 🟢 Thấp | ⚠️ Khuyến nghị |
| 5 | Tạo bảng Meetings | Create | 🟡 Trung bình | ❌ Tùy chọn |
| 6 | Tạo bảng Surveys + Questions + Responses | Create | 🟢 Thấp | ❌ Tùy chọn |
| 7 | Tạo bảng Complaints | Create | 🟡 Trung bình | ❌ Tùy chọn |
| 8 | Tạo bảng ExpenseApprovals | Create | 🟡 Trung bình | ❌ Tùy chọn |

---

### 7.4 Migration Script Đề Xuất

```sql
-- ================================================
-- MIGRATION SCRIPT: Cập nhật vai trò mới
-- Ngày: 11/02/2026
-- ================================================

-- 1. Backup dữ liệu trước khi migration
-- SELECT * INTO Users_Backup FROM Users;

-- 2. Cập nhật Role từ giá trị cũ sang mới
UPDATE Users SET Role = 'BQL_Staff' WHERE Role = 'Staff';
UPDATE Users SET Role = 'BQL_Staff' WHERE Role = 'Security';

-- 3. Thêm cột mới cho Invoices (nếu cần)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Invoices') AND name = 'ApprovalStatus')
BEGIN
    ALTER TABLE Invoices ADD ApprovalStatus integer DEFAULT 0;
    ALTER TABLE Invoices ADD ApprovedBy integer NULL;
    ALTER TABLE Invoices ADD ApprovedAt datetime NULL;
END

-- 4. Thêm cột mới cho Requests (nếu cần)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Requests') AND name = 'EscalatedTo')
BEGIN
    ALTER TABLE Requests ADD EscalatedTo integer NULL;
    ALTER TABLE Requests ADD EscalatedAt datetime NULL;
    ALTER TABLE Requests ADD EscalationReason nvarchar(500) NULL;
END

-- 5. Thêm cột mới cho Announcements
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Announcements') AND name = 'Source')
BEGIN
    ALTER TABLE Announcements ADD Source nvarchar(20) DEFAULT 'BQL';
END

-- 6. Cập nhật Source cho thông báo hiện có
UPDATE Announcements SET Source = 'BQL' WHERE Source IS NULL;

PRINT 'Migration completed successfully!';
```

---

### 7.5 Kết Luận Đánh Giá Database

#### ✅ Tin vui: Không cần thay đổi lớn về cấu trúc!

Cấu trúc database hiện tại **đủ linh hoạt** để hỗ trợ vai trò mới:

1. **Bắt buộc thay đổi**: Chỉ cần **cập nhật giá trị** cột `Role` trong bảng `Users`
2. **Khuyến nghị**: Thêm một số cột cho workflow phê duyệt
3. **Tùy chọn**: Tạo bảng mới cho Meetings, Surveys nếu cần tính năng nâng cao

#### 📌 Thứ tự ưu tiên triển khai:

1. **Phase 1** (Ngay lập tức):
   - Cập nhật giá trị Role trong Users
   - Cập nhật code xử lý phân quyền

2. **Phase 2** (Trong 1-2 tuần):
   - Thêm cột ApprovalStatus cho Invoices
   - Thêm cột Escalation cho Requests
   - Thêm cột Source cho Announcements

3. **Phase 3** (Sau khi cơ bản hoàn thành):
   - Tạo bảng Meetings (nếu cần)
   - Tạo bảng Surveys (nếu cần)
   - Tạo bảng ExpenseApprovals (nếu cần)

---

*Đánh giá này dựa trên phân tích cấu trúc database hiện tại (`db.dbml`) và yêu cầu chức năng của các vai trò mới.*

---

## 8. Tính Năng Face Recognition & Biometric Authentication

### 8.1 Tổng Quan

Hệ thống nhận diện khuôn mặt cho phép cư dân đăng ký và xác thực sinh trắc học khi sử dụng các tiện ích của chung cư (Gym, Hồ bơi, BBQ, Phòng họp). Tính năng này giúp:
- Tăng cường bảo mật khi sử dụng tiện ích
- Loại bỏ việc mượn/chia sẻ thẻ cư dân
- Tự động hóa quá trình check-in
- Theo dõi lịch sử sử dụng tiện ích chính xác

### 8.2 Phân Tích Chức Năng Theo Vai Trò

#### 8.2.1 RESIDENT - Cư Dân

| STT | Chức Năng | Mô Tả Chi Tiết | Độ Ưu Tiên |
|-----|-----------|----------------|------------|
| FR01 | Đăng ký khuôn mặt | Chụp và lưu dữ liệu khuôn mặt thông qua camera | **Cao** |
| FR02 | Cập nhật khuôn mặt | Chụp lại khuôn mặt khi cần (thay đổi ngoại hình) | Trung bình |
| FR03 | Xóa dữ liệu khuôn mặt | Xóa dữ liệu sinh trắc học của mình | Thấp |
| FR04 | Xem trạng thái đăng ký | Kiểm tra đã đăng ký khuôn mặt hay chưa | Trung bình |
| FR05 | Xác thực tại tiện ích | Scan khuôn mặt để check-in sử dụng tiện ích | **Cao** |
| FR06 | Xem lịch sử xác thực | Tra cứu lịch sử sử dụng tiện ích qua face ID | Trung bình |

#### 8.2.2 BQL_STAFF - Nhân Viên Ban Quản Lý

| STT | Chức Năng | Mô Tả Chi Tiết | Độ Ưu Tiên |
|-----|-----------|----------------|------------|
| FR07 | Hỗ trợ đăng ký khuôn mặt | Hỗ trợ cư dân đăng ký tại quầy lễ tân | **Cao** |
| FR08 | Xem danh sách đã đăng ký | Tra cứu cư dân đã đăng ký face ID | Trung bình |
| FR09 | Reset dữ liệu khuôn mặt | Xóa và yêu cầu đăng ký lại khi có vấn đề | Trung bình |
| FR10 | Xác nhận manual | Check-in thủ công khi hệ thống face ID gặp lỗi | **Cao** |
| FR11 | Xem log xác thực | Theo dõi lịch sử xác thực tại các tiện ích | Trung bình |

#### 8.2.3 BQL_MANAGER - Trưởng Ban Quản Lý

| STT | Chức Năng | Mô Tả Chi Tiết | Độ Ưu Tiên |
|-----|-----------|----------------|------------|
| FR12 | Dashboard Face ID | Thống kê số cư dân đăng ký, tỉ lệ sử dụng | **Cao** |
| FR13 | Báo cáo sử dụng tiện ích | Thống kê lượt sử dụng tiện ích qua face ID | **Cao** |
| FR14 | Quản lý thiết bị | Theo dõi trạng thái camera/thiết bị nhận diện | Trung bình |
| FR15 | Xem lịch sử xác thực toàn bộ | Tra cứu log xác thực của tất cả cư dân | Trung bình |

#### 8.2.4 ADMIN - Quản Trị Viên

| STT | Chức Năng | Mô Tả Chi Tiết | Độ Ưu Tiên |
|-----|-----------|----------------|------------|
| FR16 | Cấu hình Face Recognition | Thiết lập ngưỡng nhận diện, số lần thử | **Cao** |
| FR17 | Quản lý tiện ích có face ID | Bật/tắt yêu cầu xác thực face cho từng tiện ích | **Cao** |
| FR18 | Xem audit log | Log tất cả hoạt động liên quan đến face ID | Trung bình |
| FR19 | Xóa toàn bộ dữ liệu face | Xóa dữ liệu khuôn mặt của một cư dân (GDPR) | Thấp |

### 8.3 Ma Trận Phân Quyền Face Recognition

| Chức Năng | Admin | BQL_Manager | BQL_Staff | BQT_Head | BQT_Member | Resident |
|-----------|:-----:|:-----------:|:---------:|:--------:|:----------:|:--------:|
| Đăng ký khuôn mặt (self) | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |
| Hỗ trợ đăng ký khuôn mặt | ✅ | ✅ | ✅ | ❌ | ❌ | ❌ |
| Xóa dữ liệu khuôn mặt | ✅ | ✅ | ✅ | ❌ | ❌ | ✅ (self) |
| Xác thực tại tiện ích | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ |
| Check-in thủ công | ❌ | ✅ | ✅ | ❌ | ❌ | ❌ |
| Xem lịch sử xác thực | ✅ | ✅ | ✅ | 👁️ | ❌ | ✅ (self) |
| Cấu hình Face ID | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Báo cáo sử dụng | ✅ | ✅ | ❌ | 👁️ | ❌ | ❌ |

*Chú thích: ✅ = Toàn quyền, 👁️ = Chỉ xem, ❌ = Không có quyền*

### 8.4 Workflow Xác Thực Khuôn Mặt

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│   Resident   │────▶│   Camera     │────▶│   AI Face    │────▶│   Database   │
│  Đến tiện ích│     │   Scan Face  │     │   Matching   │     │   Verify     │
└──────────────┘     └──────────────┘     └──────────────┘     └──────────────┘
                                                │
                          ┌─────────────────────┴─────────────────────┐
                          ▼                                           ▼
                   ┌──────────────┐                            ┌──────────────┐
                   │   Match OK   │                            │   No Match   │
                   │   ≥ 95%      │                            │   < 95%      │
                   └──────┬───────┘                            └──────┬───────┘
                          │                                           │
                          ▼                                           ▼
                   ┌──────────────┐                            ┌──────────────┐
                   │  Check-in    │                            │  Thử lại     │
                   │  Thành công  │                            │  (tối đa 3)  │
                   └──────────────┘                            └──────┬───────┘
                                                                      │
                                                    ┌─────────────────┴─────────────────┐
                                                    ▼                                   ▼
                                             ┌──────────────┐                    ┌──────────────┐
                                             │  Thành công  │                    │  Liên hệ    │
                                             └──────────────┘                    │  BQL_Staff  │
                                                                                 │  Manual     │
                                                                                 └──────────────┘
```

### 8.5 Workflow Đăng Ký Khuôn Mặt

```
┌──────────────┐     ┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│   Resident   │────▶│  Mở Camera   │────▶│  Chụp ảnh    │────▶│  AI Process  │
│  Đăng ký     │     │  trên App/Web│     │  khuôn mặt   │     │  Face Data   │
└──────────────┘     └──────────────┘     └──────────────┘     └──────────────┘
                                                │
                          ┌─────────────────────┴─────────────────────┐
                          ▼                                           ▼
                   ┌──────────────┐                            ┌──────────────┐
                   │  Detect OK   │                            │  Detect Fail │
                   │  Có khuôn mặt│                            │  Không rõ    │
                   └──────┬───────┘                            └──────┬───────┘
                          │                                           │
                          ▼                                           ▼
                   ┌──────────────┐                            ┌──────────────┐
                   │  Lưu vào DB  │                            │  Chụp lại    │
                   │  Face Vector │                            │  hướng dẫn   │
                   └──────┬───────┘                            └──────────────┘
                          │
                          ▼
                   ┌──────────────┐
                   │  Đăng ký     │
                   │  thành công  │
                   └──────────────┘
```

### 8.6 Yêu Cầu Cập Nhật Database Cho Face Recognition

#### 8.6.1 Thêm Bảng FaceData

```sql
Table FaceData {
  FaceDataId integer [pk, increment]
  ResidentId integer [not null, ref: > Residents.UserId]
  FaceVector varbinary(max) [not null, note: 'Vector encoding của khuôn mặt']
  FaceImagePath nvarchar(500) [note: 'Đường dẫn ảnh gốc (encrypted)']
  Quality float [note: 'Chất lượng ảnh 0-1']
  IsActive bit [default: 1]
  RegisteredAt datetime [default: `GETDATE()`]
  RegisteredBy integer [ref: > Users.UserId, note: 'Staff hỗ trợ hoặc self']
  LastVerifiedAt datetime
  VerificationCount integer [default: 0]
  FailedAttempts integer [default: 0]
  UpdatedAt datetime
}
```

#### 8.6.2 Thêm Bảng FaceAuthLog

```sql
Table FaceAuthLogs {
  LogId integer [pk, increment]
  ResidentId integer [ref: > Residents.UserId]
  AmenityId integer [ref: > Amenities.AmenityId]
  AuthResult nvarchar(20) [note: 'Success, Failed, ManualOverride']
  MatchScore float [note: 'Điểm matching 0-1']
  DeviceId nvarchar(100) [note: 'ID thiết bị camera']
  CapturedImagePath nvarchar(500)
  AuthTime datetime [default: `GETDATE()`]
  OverrideBy integer [ref: > Users.UserId, note: 'Staff nếu manual']
  OverrideReason nvarchar(200)
  IPAddress nvarchar(50)
}
```

#### 8.6.3 Cập Nhật Bảng Amenities

```sql
ALTER TABLE Amenities ADD RequireFaceAuth bit DEFAULT 0;  -- Bật/tắt yêu cầu face auth
ALTER TABLE Amenities ADD FaceAuthThreshold float DEFAULT 0.95;  -- Ngưỡng matching
```

#### 8.6.4 Cập Nhật Bảng AmenityBookings

```sql
ALTER TABLE AmenityBookings ADD CheckInMethod nvarchar(20);  -- 'FaceAuth', 'Manual', 'Card'
ALTER TABLE AmenityBookings ADD FaceAuthLogId integer;  -- FK đến FaceAuthLogs
```

### 8.7 Tóm Tắt Thay Đổi Database Cho Face Recognition

| STT | Thay Đổi | Loại | Mức Độ Ưu Tiên | Bắt Buộc |
|:---:|:---------|:----:|:--------------:|:--------:|
| 1 | Tạo bảng FaceData | Create | 🔴 **Cao** | ✅ Có |
| 2 | Tạo bảng FaceAuthLogs | Create | 🔴 **Cao** | ✅ Có |
| 3 | Thêm cột RequireFaceAuth cho Amenities | Alter | 🟡 Trung bình | ✅ Có |
| 4 | Thêm cột CheckInMethod cho AmenityBookings | Alter | 🟢 Thấp | ⚠️ Khuyến nghị |

### 8.8 Công Nghệ Đề Xuất

| Component | Công Nghệ | Ghi Chú |
|:----------|:----------|:--------|
| Face Detection | **face-api.js** (JavaScript) | Chạy trên browser, không cần server AI |
| Face Recognition | **ML.NET** với ONNX model | Tích hợp tốt với ASP.NET Core |
| Face Vector Storage | **SQL Server varbinary** | Lưu trực tiếp trong DB |
| Camera Access | **MediaDevices API** | HTML5 webcam access |
| Fallback | **Azure Face API** | Nếu cần độ chính xác cao hơn |

### 8.9 Lưu Ý Bảo Mật & GDPR

1. **Mã hóa dữ liệu**: Face vector phải được mã hóa khi lưu trữ
2. **Quyền xóa dữ liệu**: Cư dân có quyền yêu cầu xóa toàn bộ dữ liệu sinh trắc học
3. **Consent**: Cần có checkbox đồng ý khi đăng ký face ID
4. **Retention Policy**: Tự động xóa dữ liệu khi cư dân chuyển đi
5. **Access Log**: Ghi log tất cả truy cập vào dữ liệu face
6. **Không chia sẻ**: Dữ liệu face không được chia sẻ với bên thứ ba

