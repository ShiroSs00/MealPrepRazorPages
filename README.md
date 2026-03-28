# NutriFood App 🥗

NutriFood là một ứng dụng web chăm sóc sức khỏe và dinh dưỡng toàn diện được xây dựng bằng **ASP.NET Core Razor Pages** áp dụng kiến trúc 3 tầng chuẩn (DAL, BLL, UI). Ứng dụng cung cấp các công cụ thông minh để theo dõi Calo, tư vấn dinh dưỡng bằng Trí tuệ nhân tạo (AI), hệ thống đánh giá thời gian thực và quản trị chuyên nghiệp.

## 🔥 Tổng hợp Hệ thống Tính năng

### 1. Phân quyền và Hồ sơ Sức khỏe
*   **Authentication & Authorization:** Hệ thống Đăng nhập / Đăng ký sử dụng Claims-based authentication. Tự động cấp quyền `Admin` cho các tài khoản đăng ký chứa từ khoá "admin".
*   **Hồ sơ Cá nhân (Profile):** Người dùng nhập chỉ số cơ thể (Cân nặng, Chiều cao, Giới tính, Tuổi) và Mục tiêu (Giảm cân, Tăng cơ, Duy trì).
*   **Tính toán Khoa học:** Hệ thống tự động phân tích và cấp các chỉ số **BMI** (Thể trạng) và **TDEE** (Lượng calo cần thiết mỗi ngày).

### 2. Bảng điều khiển (User Dashboard)
*   **Theo dõi Năng lượng (Daily Log):** Nhập lượng calo đã ăn mỗi bữa.
*   **Biểu đồ Thống kê (Chart.js):** Giám sát tiến độ nạp Calo 7 ngày gần nhất so sánh với vạch đích TDEE.
*   **Khám phá Thực đơn:** Xem các thực đơn thịnh hành được đề xuất bởi Admin.

### 3. Tương tác Thời gian thực (Real-time Reviews & SignalR)
*   **Đánh giá món ăn (Rating & Comment):** Khi nhấn "Đánh giá món" từ Dashboard, bạn sẽ tham gia vào phòng thảo luận Real-time.
*   **Cập nhật Live:** Các bình luận mới, hay các thao tác Sửa/Xóa (CRUD) của bạn và người khác sẽ nhảy ngay trên màn hình mà không cần tự F5 tải lại trang.
*   **Điểm thưởng:** Mỗi đánh giá chất lượng mang về cho bạn `+10 điểm` vào quỹ đổi quà.

### 4. Hệ thống AI Chat & Gen Menu (Groq API)
*   **AI Menu:** Tính năng độc quyền cho tài khoản **VIP & Admin**. Trò chuyện trực tiếp với NutriBot để tham khảo dinh dưỡng.
*   **Tạo Menu AI:** Chỉ 1 click, AI đọc các thông số sức khoẻ (BMI, TDEE) của riêng bạn và generate một thực đơn cho ngày hôm đó.

### 5. Thương mại hóa (Payment & Rewards)
*   **Thanh toán Online (Mock):** Tính năng nạp gói VIP hoặc mua gói Điểm thưởng. Quy trình checkout giả lập mượt mà.
*   **Đổi quà (Rewards Store):** Sử dụng Điểm thưởng tích luỹ được để quy đổi lấy các Coupon / Quà tặng sức khoẻ do Admin phát hành.

### 6. Admin Dashboard 
*   **Tổng quan:** Theo dõi biểu đồ số lượng Người dùng, Tổng thu nhập, Đơn đổi quà.
*   **Quản lý (CRUD):** Các mô-đun quản lý chuyên sâu bao gồm:
    *   **Thực đơn (Meals)**
    *   **Quà tặng (Gifts)**
    *   **Người dùng & Giao dịch (Users & Transactions)**

