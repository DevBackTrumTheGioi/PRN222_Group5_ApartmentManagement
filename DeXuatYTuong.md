# Đề xuất Ý tưởng Dự án: Quản lý Chung cư / Cư dân (Apartment Management)

## 1. Giới thiệu (Introduction)
Hệ thống quản lý chung cư (Apartment Management System) là một giải pháp phần mềm nhằm số hóa và tự động hóa các quy trình quản lý vận hành trong các tòa nhà chung cư. Mục tiêu là tạo ra một kênh liên lạc hiệu quả giữa Ban Quản Lý (BQL) và Cư dân, đảm bảo sự minh bạch về tài chính và nâng cao chất lượng dịch vụ.

### 1.1. Thực trạng và Vấn đề (Current Situation & Problems)
Hiện nay, quy trình vận hành tại nhiều tòa nhà chung cư vẫn đang đối mặt với nhiều bất cập do phụ thuộc vào các phương pháp quản lý truyền thống:
- **Lưu trữ phân tán, thủ công:** Dữ liệu cư dân, hợp đồng và hóa đơn thường được quản lý qua nhiều file Excel rời rạc hoặc sổ sách giấy, dẫn đến khó khăn trong việc tra cứu, tổng hợp báo cáo và dễ xảy ra sai sót, thất lạc.
- **Kênh giao tiếp hạn chế:** Thông báo từ BQL thường được dán tại bảng tin sảnh hoặc gửi qua các nhóm chat không chính thức, khiến thông tin quan trọng dễ bị trôi hoặc không đến được đúng đối tượng.
- **Quy trình xử lý sự cố chậm trễ:** Các yêu cầu sửa chữa, phản ánh của cư dân thường được tiếp nhận qua điện thoại hoặc gặp mặt trực tiếp, thiếu công cụ theo dõi tiến độ, dẫn đến tình trạng bỏ sót yêu cầu hoặc xử lý chậm, gây ảnh hưởng đến sự hài lòng của cư dân.
- **Khó khăn trong quản lý tài chính:** Việc chốt chỉ số điện nước và tính toán phí hàng tháng tốn nhiều thời gian; cư dân gặp khó khăn khi muốn tra cứu chi tiết công nợ hoặc lịch sử thanh toán cũ.

## 2. Mục tiêu dự án (Objectives)
- **Tối ưu hóa vận hành:** Giảm thiểu giấy tờ, quản lý dữ liệu tập trung, tra cứu nhanh chóng.
- **Nâng cao trải nghiệm cư dân:** Cư dân có thể tiếp cận thông tin, thanh toán và gửi yêu cầu mọi lúc mọi nơi.
- **Minh bạch tài chính:** Quản lý thu chi rõ ràng, giảm sai sót trong tính toán phí dịch vụ.

## 3. Các tính năng chính (Key Features)

### A. Phân hệ dành cho Ban Quản Lý (Admin Module)
1. **Quản lý Căn hộ & Cư dân:**
   - Quản lý danh sách các tòa nhà, tầng, căn hộ.
   - Quản lý hồ sơ cư dân (chủ hộ, người thuê, thành viên gia đình).
   - Quản lý thẻ cư dân, thẻ thang máy, thẻ xe.

2. **Quản lý Dịch vụ & Hóa đơn:**
   - Thiết lập đơn giá các loại phí (phí quản lý, điện, nước, gửi xe, phí vệ sinh...).
   - Chốt chỉ số điện/nước hàng tháng.
   - Tự động tính toán và tạo hóa đơn hàng tháng cho từng căn hộ.
   - **Thanh toán tự động:** Tích hợp xác nhận thanh toán qua Cổng thanh toán (VNPay, Momo) và tự động gạch nợ.
   - Theo dõi trạng thái thanh toán (Đã thanh toán / Chưa thanh toán / Quá hạn).

3. **Quản lý Yêu cầu & Phản ánh:**
   - Tiếp nhận các yêu cầu sửa chữa, bảo trì hoặc khiếu nại từ cư dân.
   - Phân công nhân viên xử lý.
   - Cập nhật trạng thái xử lý (Đang xử lý / Hoàn thành).

4. **Thông tin & Cộng đồng:**
   - Soạn thảo và gửi thông báo chung (lịch cắt nước, pccc, họp tổ dân phố...).
   - **Quản lý Tài liệu:** Đăng tải Sổ tay cư dân, Quy định tòa nhà, Hướng dẫn PCCC.
   - **Kiểm duyệt Diễn đàn (Tùy chọn):** Quản lý bài đăng, bình luận trong cộng đồng cư dân.

5. **Báo cáo & Thống kê:**
   - Báo cáo doanh thu phí dịch vụ.
   - Thống kê tình trạng cư trú (tỷ lệ lấp đầy).
   - Thống kê các yêu cầu xử lý sự cố.

### B. Phân hệ dành cho Cư dân (Resident Module)
1. **Thông tin cá nhân & Tiện ích:**
   - Xem và cập nhật thông tin liên hệ, quản lý phương tiện.
   - **Đăng ký Khách (Visitor Registration):** Tạo mã QR hoặc đăng ký trước cho khách đến thăm để ra vào nhanh chóng.
   - **Tra cứu thông tin:** Xem Sổ tay cư dân, danh bạ khẩn cấp, quy định tòa nhà.

2. **Hóa đơn & Thanh toán:**
   - Nhận thông báo hóa đơn mới hàng tháng.
   - Xem chi tiết các khoản phí.
   - **Thanh toán Online:** Quét mã QR hoặc thanh toán qua ví điện tử trực tiếp trên hệ thống.
   - Xem lịch sử thanh toán.

3. **Dịch vụ & Tiện ích:**
   - Gửi báo cáo sự cố (hỏng đèn hành lang, rò rỉ nước...).
   - Đăng ký sử dụng tiện ích chung (phòng BBQ, sân tennis...).
   - **Bưu phẩm (Parcel):** Nhận thông báo khi có bưu phẩm gửi đến quầy lễ tân. Xác nhận trạng thái đã nhận.

4. **Cộng đồng:**
   - Xem các thông báo mới nhất từ BQL.
   - Tham gia thảo luận tại Diễn đàn cư dân (nếu có).

### C. Phân hệ dành cho Lễ tân / Bảo vệ (Reception & Security Module)
1. **Quản lý Khách ra vào (Visitor Check-in):**
   - Kiểm tra thông tin khách đến dựa trên đăng ký của cư dân (quét QR code).
   - Cấp thẻ khách tạm thời và ghi nhận giờ vào/ra.
   - Ghi nhận thông tin khách vãng lai (không hẹn trước) và liên hệ căn hộ để xác nhận.

2. **Quản lý Bưu phẩm & Giao hàng (Delivery Management):**
   - Tiếp nhận bưu phẩm, hàng hóa từ shipper.
   - Nhập thông tin lên hệ thống -> Hệ thống tự động báo cho cư dân.
   - Xác nhận bàn giao khi cư dân xuống lấy hàng.

3. **Tiếp nhận phản ánh trực tiếp:**
   - Ghi nhận nhanh các yêu cầu/phàn nàn của cư dân tại quầy lễ tân để chuyển lên bộ phận kỹ thuật/quản lý.

## 4. Phân quyền người dùng (User Roles)
- **Admin:** Quản trị toàn bộ hệ thống.
- **Staff (Nhân viên):** (Kế toán, Kỹ thuật, Lễ tân) có quyền hạn giới hạn theo chức năng.
- **Resident (Cư dân):** Truy cập thông tin cá nhân và tương tác với BQL.

## 5. Công nghệ đề xuất (Technology Stack)
Dựa trên cấu trúc dự án hiện tại:
- **Nền tảng:** .NET Core 8.0
- **Framework:** ASP.NET Core Razor Pages
- **Cơ sở dữ liệu:** SQL Server
- **Frontend:** Bootstrap, CSS, JavaScript (jQuery Unobtrusive Validation)
- **Công cụ phát triển:** Visual Studio / JetBrains Rider

## 6. Kế hoạch phát triển sơ bộ (Roadmap)
1.  **Phase 1:** Thiết kế Database & Giao diện cơ bản (Layout, Login).
2.  **Phase 2:** Phát triển tính năng Quản lý Căn hộ & Cư dân (CRUD).
3.  **Phase 3:** Phát triển tính năng Dịch vụ & Hóa đơn.
4.  **Phase 4:** Phát triển tính năng Phản ánh & Thông báo.
5.  **Phase 5:** Kiểm thử (Testing) và hoàn thiện.