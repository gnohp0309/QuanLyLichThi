# Hệ thống Quản lý Tổ chức Thi & Điểm

## Mô tả
Hệ thống quản lý tổ chức thi và điểm cho trường đại học, được xây dựng bằng:
- **Backend**: ASP.NET Core 8.0 Web API
- **Frontend**: Windows Forms (.NET 8.0)
- **Database**: PostgreSQL (Render PostgreSQL)
- **Authentication**: JWT Bearer Token
- **Hosting**: Render.com (Backend API + PostgreSQL)

## Cấu trúc dự án

### Backend (QLToChucThi-Diem(BE)/QuanLyThi.API)
- `Models/`: Entity models (User, Subject, CourseSection, Score, ExamSchedule, Enrollment)
- `Controllers/`: API Controllers
  - `AuthController`: Đăng nhập
  - `UsersController`: Quản lý người dùng
  - `SubjectsController`: Quản lý môn học
  - `CourseSectionsController`: Quản lý lớp học phần
  - `ScoresController`: Quản lý điểm
  - `ExamSchedulesController`: Quản lý lịch thi
  - `EnrollmentsController`: Quản lý đăng ký học phần
- `Data/`: DbContext và database configuration
- `Services/`: Business logic (AuthService)
- `DTOs/`: Data Transfer Objects

### Frontend (QLToChucThi-Diem(FE)/QuanLyThi.Client)
- `Forms/`: Windows Forms
  - `frmDangNhap`: Form đăng nhập
  - `frmAdmin`: Dashboard Admin
  - `frmTeacher`: Dashboard Giảng viên
  - `frmStudent`: Dashboard Sinh viên
- `Services/`: API Client sử dụng HttpClient
- `Models/`: View Models

## Hướng dẫn cài đặt và chạy

### 1. Cài đặt Database (PostgreSQL)

#### Local Development:
1. Cài đặt PostgreSQL (https://www.postgresql.org/download/)
2. Tạo database mới:
   ```sql
   CREATE DATABASE QuanLyThiDB;
   ```
3. Cập nhật connection string trong `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=QuanLyThiDB;Username=postgres;Password=yourpassword"
   }
   ```

#### Production (Render):
- Xem file `DEPLOY_RENDER.md` để hướng dẫn deploy lên Render
- Render sẽ tự động cung cấp connection string qua environment variable `DATABASE_URL`

### 2. Chạy Backend API
```bash
cd QLToChucThi-Diem(BE)/QuanLyThi.API
dotnet restore
dotnet run
```
Backend sẽ chạy tại: `https://localhost:7000` hoặc `http://localhost:5000`

**Lưu ý**: Backend sẽ tự động tạo database và seed dữ liệu mẫu khi lần đầu chạy.

### 3. Chạy Frontend
1. Mở solution trong Visual Studio
2. Cập nhật API URL trong `ApiClient.cs` nếu cần
3. Build và Run project `QuanLyThi.Client`

## Tài khoản mặc định

### Admin
- Username: `admin`
- Password: `admin123`
- Role: `Admin`

### Giảng viên
- Username: `gv1`, `gv2`, `gv3`, `gv4`
- Password: `123456`
- Role: `Teacher`

### Sinh viên
- Username: `sv01`, `sv02`, ..., `sv11`
- Password: `123456`
- Role: `Student`

## API Endpoints

### Authentication
- `POST /api/Auth/login`: Đăng nhập

### Users (Admin only)
- `GET /api/Users`: Lấy danh sách người dùng
- `GET /api/Users/{id}`: Lấy thông tin người dùng
- `POST /api/Users`: Tạo người dùng mới
- `PUT /api/Users/{id}`: Cập nhật người dùng
- `DELETE /api/Users/{id}`: Xóa người dùng

### Subjects
- `GET /api/Subjects`: Lấy danh sách môn học
- `GET /api/Subjects/{code}`: Lấy thông tin môn học
- `POST /api/Subjects`: Tạo môn học mới (Admin)
- `PUT /api/Subjects/{code}`: Cập nhật môn học (Admin)
- `DELETE /api/Subjects/{code}`: Xóa môn học (Admin)

### Course Sections
- `GET /api/CourseSections`: Lấy danh sách lớp học phần
- `GET /api/CourseSections/{code}`: Lấy thông tin lớp học phần
- `POST /api/CourseSections`: Tạo lớp học phần mới (Admin)
- `PUT /api/CourseSections/{code}`: Cập nhật lớp học phần (Admin)
- `DELETE /api/CourseSections/{code}`: Xóa lớp học phần (Admin)

### Exam Schedules
- `GET /api/ExamSchedules`: Lấy danh sách lịch thi
- `POST /api/ExamSchedules`: Tạo lịch thi mới (Admin)
- `PUT /api/ExamSchedules/{id}`: Cập nhật lịch thi (Admin)
- `DELETE /api/ExamSchedules/{id}`: Xóa lịch thi (Admin)

### Scores
- `GET /api/Scores`: Lấy danh sách điểm
- `POST /api/Scores/bulk`: Nhập/sửa điểm (Admin/Teacher)
- `POST /api/Scores/publish/{sectionCode}`: Công bố điểm (Admin/Teacher)

### Enrollments
- `GET /api/Enrollments`: Lấy danh sách đăng ký
- `POST /api/Enrollments`: Đăng ký học phần
- `DELETE /api/Enrollments/{id}`: Hủy đăng ký

## Công nghệ sử dụng

### Backend
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- JWT Authentication
- BCrypt.Net for password hashing
- Swagger/OpenAPI

### Frontend
- Windows Forms (.NET 8.0)
- HttpClient for API calls
- Newtonsoft.Json for JSON serialization
- FontAwesome.Sharp for icons

## Tính năng chính

### Admin
- Quản lý tài khoản (CRUD)
- Quản lý môn học (CRUD)
- Quản lý lớp học phần (CRUD)
- Quản lý điểm thi
- Quản lý lịch thi
- Xem báo cáo và thống kê

### Giảng viên
- Xem danh sách lớp giảng dạy
- Nhập/sửa điểm cho sinh viên
- Công bố điểm
- Xem lịch thi

### Sinh viên
- Xem điểm thi
- Đăng ký học phần
- Xem lịch học và lịch thi
- Xem thông báo

## Lưu ý

1. **Connection String**: Cập nhật connection string trong `appsettings.json` phù hợp với môi trường của bạn
2. **API URL**: Cập nhật BaseAddress trong `ApiClient.cs` nếu API chạy trên port khác
3. **SSL Certificate**: Khi chạy với HTTPS, có thể cần bỏ qua SSL validation trong development

## Phát triển tiếp

Để hoàn thiện hệ thống, bạn có thể:
1. Thêm validation và error handling đầy đủ
2. Hoàn thiện UI/UX cho các forms
3. Thêm tính năng export Excel
4. Thêm thông báo (Notifications)
5. Thêm backup/restore database
6. Thêm phân quyền chi tiết hơn
