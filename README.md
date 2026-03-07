# 🍽️ MealPrepService

Ứng dụng hỗ trợ người dùng quản lý sức khỏe và thực đơn cá nhân hóa bằng AI.

---

## 🚀 Flows & Chức năng chính

### Flow 1: Đăng ký + Tính toán chỉ số
- Người dùng đăng ký, nhập thông tin cá nhân.
- Hệ thống tự động tính toán các chỉ số sức khỏe: **BMI, BMR, TDEE,…**
- **Mở rộng**:
  - Xác thực email/số điện thoại.
  - Onboarding UI (giải thích cách dùng app).
  - Lưu lịch sử chỉ số để theo dõi sự thay đổi theo thời gian.

---

### Flow 2: AI Gen Menu + Thanh toán nâng cấp
- AI tự động tạo thực đơn cá nhân hóa dựa trên chỉ số từ Flow 1.
- **Mở rộng**:
  - Tùy chỉnh menu (loại bỏ món dị ứng, thêm món yêu thích).
  - Tích hợp gợi ý mua nguyên liệu hoặc liên kết với siêu thị/đối tác giao hàng.
  - Subscription (gói tuần/tháng/năm) thay vì chỉ one-time payment.
  - SignalR hiển thị real-time thông báo (menu mới, khuyến mãi).

#### Gói nâng cấp
- **Miễn phí**: chỉ gen ra menu từ chỉ số người dùng.
- **Basic Premium**: thêm tùy chỉnh menu + thông tin dinh dưỡng chi tiết + cách nấu ăn cho từng món.

---

### Flow 3: Meal Review
- Người dùng đánh giá món ăn, feedback sẽ nhận được số điểm thưởng và quy đổi ra món quà tặng tương đương với số điểm thưởng đấy.
- **Mở rộng**:
  - Gamification: điểm thưởng khi review, ranking món ăn.
  - Social feature: chia sẻ thực đơn hoặc review với cộng đồng.
  - AI học từ review để điều chỉnh menu cho lần sau.

---

### Flow 4: Progress Tracking
- Theo dõi cân nặng, chỉ số sức khỏe theo thời gian.
- **Mở rộng**:
  - Dashboard trực quan (chart).
  - SignalR cập nhật real-time khi có dữ liệu mới.

---

### Flow 5: Notification & Reminder
- Nhắc giờ ăn, nhắc mua nguyên liệu, nhắc review sau khi dùng món.
- SignalR: áp dụng mạnh nhất ở flow này để push notification real-time.

---

### Flow 6: Admin Dashboard
- **Quản lý gói Premium**:
  - Thêm/sửa/xóa gói (Free, Basic Premium, Advanced Premium…).
  - Thay đổi giá gói theo tuần/tháng/năm.
  - Cấu hình quyền lợi (menu AI, chi tiết dinh dưỡng, hướng dẫn nấu ăn…).
- **SignalR Real-time Update**:
  - Khi admin thay đổi giá gói, hệ thống push thông báo ngay đến tất cả client.
  - Người dùng sẽ thấy banner hoặc popup: *“Giá gói Premium đã được cập nhật”*.
- **Quản lý người dùng**:
  - Xem danh sách user, trạng thái gói hiện tại.
  - Thống kê số lượng đăng ký theo từng gói.
- **Quản lý Feedback/Review**:
  - Xem đánh giá món ăn từ Flow 3.
  - Thống kê điểm trung bình, món ăn được yêu thích nhất.

---



