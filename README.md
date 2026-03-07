# 🍽️ NutriFood – MealPrepService

Ứng dụng web hỗ trợ người dùng quản lý sức khỏe và thực đơn cá nhân hóa bằng AI, xây dựng trên nền tảng **ASP.NET Core Razor Pages** với kiến trúc 3 lớp.

---

## 🏗️ Kiến trúc dự án

```
WebAppRazor.sln
├── WebAppRazor.DAL          # Data Access Layer – Entity Framework Core, Models, Repositories
├── WebAppRazor.BLL          # Business Logic Layer – Services, DTOs, tích hợp OpenAI
└── WebAppRazor.Web          # Presentation Layer – Razor Pages, SignalR Hub, Background Services
```

### Sơ đồ phụ thuộc

```
Web  →  BLL  →  DAL  →  SQL Server
```

---

## 🛠️ Công nghệ sử dụng

| Thành phần | Công nghệ |
|---|---|
| Framework | ASP.NET Core 8, Razor Pages |
| ORM | Entity Framework Core (SQL Server) |
| Xác thực | Cookie Authentication |
| Real-time | SignalR (`NotificationHub`) |
| AI | OpenAI API (GPT) qua `HttpClient` |
| Background Jobs | `IHostedService` (NotificationScheduler, ReminderBackground) |
| Frontend | Bootstrap 5, Font Awesome 6, Nunito font |

---

## ⚙️ Cấu hình & Khởi chạy

### 1. Chuỗi kết nối SQL Server

Mở `WebAppRazor.Web/appsettings.json` và cập nhật:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(local);Database=WebAppRazorDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 2. OpenAI API Key (tùy chọn)

Thêm vào `appsettings.json` hoặc User Secrets:

```json
{
  "OpenAI": {
    "ApiKey": "sk-..."
  }
}
```

> Nếu không cấu hình API key, hệ thống tự động fallback sang menu mặc định.

### 3. Khởi chạy

```bash
cd WebAppRazor.Web
dotnet run
```

Database sẽ tự động được tạo khi khởi động lần đầu thông qua `InitializeDatabaseAsync()`.

---

## 📦 Cấu trúc chi tiết

### WebAppRazor.DAL – Data Access Layer

#### Models (Entities)

| Model | Mô tả |
|---|---|
| `User` | Người dùng: username, password hash, email, subscription tier, review points |
| `HealthProfile` | Hồ sơ sức khỏe: tuổi, giới tính, chiều cao, cân nặng, mức hoạt động, mục tiêu, BMI/BMR/TDEE |
| `MealPlan` | Kế hoạch thực đơn theo ngày; chứa nhiều `MealItem` |
| `MealItem` | Món ăn trong thực đơn: loại bữa, tên, mô tả, calories, protein, carbs, fat, nguyên liệu, cách nấu (Premium) |
| `MealReview` | Đánh giá món ăn (1–5 sao), nhận xét, điểm thưởng |
| `ProgressEntry` | Lịch sử cân nặng & chỉ số (BMI, BMR, TDEE) theo thời gian |
| `Notification` | Thông báo: tiêu đề, nội dung, loại, trạng thái đã đọc, thời điểm gửi (có thể lên lịch) |
| `ReminderSchedule` | Lịch nhắc (bữa sáng/trưa/tối/mua sắm): giờ nhắc, chế độ lặp (Daily/Weekdays/Weekends/Once) |

#### Repositories

Mỗi model có interface `I*Repository` và implementation tương ứng:
`UserRepository`, `HealthProfileRepository`, `MealPlanRepository`, `MealReviewRepository`, `ProgressRepository`, `NotificationRepository`, `ReminderScheduleRepository`

#### AppDbContext

Cấu hình quan hệ giữa các entity (Cascade Delete), unique index trên `Username`.

---

### WebAppRazor.BLL – Business Logic Layer

#### Services

| Service | Chức năng |
|---|---|
| `AuthService` | Đăng ký, đăng nhập (password hashing) |
| `HealthProfileService` | Lưu/lấy hồ sơ sức khỏe, tính toán BMI/BMR/TDEE/DailyCalorieTarget |
| `MealPlanService` | Tạo/lấy thực đơn, lưu menu AI vào database |
| `OpenAIService` (`IAIService`) | Gọi OpenAI API sinh menu cá nhân hóa theo calories/mục tiêu; parse JSON response |
| `MealReviewService` | Tạo đánh giá, cộng điểm thưởng cho user |
| `ProgressService` | Ghi lịch sử tiến trình, lấy dữ liệu theo user |
| `NotificationService` | Tạo/đọc/đánh dấu đã đọc thông báo |
| `SubscriptionService` | Nâng cấp/hủy gói Premium (Weekly/Monthly/Yearly), kiểm tra hết hạn tự động |
| `ReminderScheduleService` | Quản lý lịch nhắc cá nhân của user |

#### DTOs

`HealthProfileDto`, `MealPlanDto`, `MealReviewDto`, `NotificationDto`, `ProgressEntryDto`, `ReminderScheduleDto`

---

### WebAppRazor.Web – Presentation Layer

#### Razor Pages

| Route | Trang |
|---|---|
| `/` | Trang chủ |
| `/Account/Login` | Đăng nhập |
| `/Account/Register` | Đăng ký |
| `/Account/Logout` | Đăng xuất |
| `/Health/Index` | Nhập/xem hồ sơ sức khỏe, tính BMI/BMR/TDEE |
| `/Menu/Index` | Xem/tạo thực đơn AI theo ngày |
| `/Reviews/Index` | Danh sách đánh giá của user |
| `/Reviews/Create` | Tạo đánh giá mới |
| `/Progress/Index` | Xem biểu đồ tiến trình sức khỏe |
| `/Notifications/Index` | Danh sách thông báo |
| `/Notifications/Schedules` | Quản lý lịch nhắc |
| `/Subscription/Index` | Xem/nâng cấp gói Premium |
| `/Subscription/Payment` | Xác nhận thanh toán |
| `/Subscription/PaymentMethod` | Chọn phương thức thanh toán |

#### SignalR

- `NotificationHub` (`/notificationHub`): push thông báo real-time đến client.

#### Background Services

- `NotificationSchedulerService`: kiểm tra và gửi các thông báo được lên lịch.
- `ReminderBackgroundService`: kiểm tra lịch nhắc bữa ăn/mua sắm và tạo notification tự động.

---

## 🚀 Flows & Chức năng chính

### Flow 1: Đăng ký + Tính toán chỉ số
- Người dùng đăng ký, nhập thông tin cá nhân.
- Hệ thống tự động tính toán các chỉ số sức khỏe: **BMI, BMR, TDEE, DailyCalorieTarget**.
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
- Người dùng đánh giá món ăn (1–5 sao), nhận xét; mỗi review nhận **10 điểm thưởng** (`ReviewPoints`).
- **Mở rộng**:
  - Gamification: điểm thưởng khi review, ranking món ăn.
  - Social feature: chia sẻ thực đơn hoặc review với cộng đồng.
  - AI học từ review để điều chỉnh menu cho lần sau.

---

### Flow 4: Progress Tracking
- Theo dõi cân nặng, chỉ số sức khỏe (BMI, BMR, TDEE) theo thời gian.
- **Mở rộng**:
  - Dashboard trực quan (chart).
  - SignalR cập nhật real-time khi có dữ liệu mới.

---

### Flow 5: Notification & Reminder
- Nhắc giờ ăn, nhắc mua nguyên liệu, nhắc review sau khi dùng món.
- Hỗ trợ lên lịch nhắc với các chế độ: **Daily / Weekdays / Weekends / Once**.
- SignalR push notification real-time qua `NotificationHub`.

---

### Flow 6: Admin Dashboard *(kế hoạch)*
- **Quản lý gói Premium**:
  - Thêm/sửa/xóa gói (Free, Basic Premium, Advanced Premium…).
  - Thay đổi giá gói theo tuần/tháng/năm.
  - Cấu hình quyền lợi (menu AI, chi tiết dinh dưỡng, hướng dẫn nấu ăn…).
- **SignalR Real-time Update**:
  - Khi admin thay đổi giá gói, hệ thống push thông báo ngay đến tất cả client.
  - Người dùng sẽ thấy banner hoặc popup: *"Giá gói Premium đã được cập nhật"*.
- **Quản lý người dùng**:
  - Xem danh sách user, trạng thái gói hiện tại.
  - Thống kê số lượng đăng ký theo từng gói.
- **Quản lý Feedback/Review**:
  - Xem đánh giá món ăn từ Flow 3.
  - Thống kê điểm trung bình, món ăn được yêu thích nhất.

---
