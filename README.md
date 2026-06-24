# Bliss Events - Event Booking System

Bliss Events là một nền tảng Marketplace giúp kết nối Khách hàng với các Thợ chụp ảnh (Photographers) chuyên nghiệp. Hệ thống cho phép người dùng tìm kiếm dịch vụ, đặt lịch chụp ảnh, quản lý lịch trình cá nhân và để lại đánh giá sau mỗi sự kiện.

## 🚀 Tính năng nổi bật (Features)

* **Xác thực & Phân quyền (Authentication):** Đăng ký, đăng nhập an toàn với JWT Token và mã hóa mật khẩu BCrypt. Phân quyền rõ ràng giữa Khách hàng và Thợ chụp ảnh.
* **Quản lý sự kiện (Event Management):** Tạo, xem và quản lý các gói dịch vụ chụp ảnh.
* **Đặt lịch thông minh (Booking System):** Gửi yêu cầu đặt lịch, chấp nhận/từ chối đơn hàng.
* **Quản lý thời gian (Locked Schedule):** Thợ ảnh có thể chủ động khóa các ngày bận để tránh bị trùng lịch.
* **Hệ thống đánh giá (Reviews):** Khách hàng để lại đánh giá và điểm số cho thợ ảnh sau khi hoàn tất dịch vụ.

## 💻 Công nghệ sử dụng (Tech Stack)

### Frontend (`/frontend-ui`)
* **Framework:** React (v18+)
* **Build Tool:** Vite
* **Styling:** Tailwind CSS
* **Routing:** React Router DOM

### Backend (`/backend-api`)
* **Framework:** ASP.NET Core Web API (.NET 8/9)
* **Language:** C#
* **Database:** Microsoft SQL Server
* **ORM:** Entity Framework Core
* **Security:** ASP.NET Core Identity, JWT, BCrypt.Net

## 📂 Cấu trúc dự án (Folder Structure)

```text
event-booking-support/
├── backend-api/          # Chứa toàn bộ source code ASP.NET Core Web API
│   └── EventBooking.API/ # Project Backend chính (Controllers, Models, Data)
├── frontend-ui/          # Chứa toàn bộ source code React/Vite
│   ├── src/
│   │   ├── components/   # Các UI Component dùng chung (Navbar, Forms,...)
│   │   ├── pages/        # Các trang giao diện chính (Dashboard, Auth,...)
│   │   └── App.jsx       # File cấu hình Routing chính
├── database/             # (Tùy chọn) Chứa các script SQL khởi tạo
└── README.md             # Tài liệu dự án
