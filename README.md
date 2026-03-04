#  MealPrepService

Ứng dụng hỗ trợ người dùng quản lý sức khỏe và thực đơn cá nhân hóa bằng AI.

---

##  Flows & Chức năng chính

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
- Người dùng đánh giá món ăn, feedback cho AI để cải thiện menu.
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

##  Công nghệ đề xuất
- **Backend**: ASP.NET Core + SignalR
- **Frontend**: React / Angular
- **Database**: SQL Server / PostgreSQL
- **AI Menu Generation**: tích hợp mô hình AI (ví dụ: OpenAI API)
- **Authentication**: JWT + Email/SMS OTP
- **Deployment**: Docker + Kubernetes

---

