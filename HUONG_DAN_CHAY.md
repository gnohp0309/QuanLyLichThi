# Hướng dẫn chạy Hệ thống Quản lý Thi & Điểm

## Bước 1: Chuẩn bị Database

1. Mở SQL Server Management Studio (hoặc PostgreSQL client nếu dùng PostgreSQL)
2. Tạo database mới tên: `QuanLyThiDB`
3. Nếu dùng PostgreSQL, cập nhật connection string trong `appsettings.json`:
   ```
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Database=QuanLyThiDB;Username=postgres;Password=yourpassword"
   }
   ```

## Bước 2: Chạy Backend API

### Cách 1: Dùng Visual Studio
1. Mở folder `QLToChucThi-Diem(BE)\QuanLyThi.API` trong Visual Studio
2. Nhấn F5 để chạy
3. API sẽ chạy tại: `http://localhost:5000` hoặc `https://localhost:7000`

### Cách 2: Dùng Command Line
```bash
cd "QLToChucThi-Diem(BE)\QuanLyThi.API"
dotnet restore
dotnet run
```

**Lưu ý quan trọng:**
- Backend sẽ tự động tạo database và bảng khi lần đầu chạy
- Backend sẽ tự động seed dữ liệu mẫu (admin, giảng viên, sinh viên, môn học, lớp học phần)
- Nếu có lỗi về connection string, kiểm tra lại SQL Server đã chạy chưa

## Bước 3: Kiểm tra API đã chạy

1. Mở trình duyệt và vào: `http://localhost:5000/swagger`
2. Bạn sẽ thấy Swagger UI với tất cả các API endpoints
3. Test đăng nhập bằng cách:
   - POST `/api/Auth/login`
   - Body: 
     ```json
     {
       "username": "admin",
       "password": "admin123",
       "role": "Admin"
     }
     ```

## Bước 4: Chạy Frontend

### Cách 1: Dùng Visual Studio
1. Mở solution file `QLToChucThi-Diem(FE)\QLToChucThi-Diem\QLToChucThi-Diem.slnx`
2. Set project `QuanLyThi.Client` làm Startup Project
3. Nhấn F5 để chạy

### Cách 2: Dùng Command Line
```bash
cd "QLToChucThi-Diem(FE)\QLToChucThi-Diem\QuanLyThi.Client"
dotnet restore
dotnet run
```

## Bước 5: Đăng nhập

### Tài khoản Admin
- **Username**: `admin`
- **Password**: `admin123`
- **Role**: `Admin`

### Tài khoản Giảng viên
- **Username**: `gv1`, `gv2`, `gv3`, hoặc `gv4`
- **Password**: `123456`
- **Role**: `Teacher`

### Tài khoản Sinh viên
- **Username**: `sv01`, `sv02`, ..., `sv11`
- **Password**: `123456`
- **Role**: `Student`

## Xử lý lỗi thường gặp

### Lỗi kết nối API
- **Nguyên nhân**: Backend chưa chạy hoặc URL không đúng
- **Giải pháp**: 
  1. Kiểm tra Backend đã chạy chưa
  2. Kiểm tra URL trong `ApiClient.cs` có đúng port không
  3. Thử đổi từ `https://localhost:7000` sang `http://localhost:5000`

### Lỗi kết nối Database
- **Nguyên nhân**: SQL Server chưa chạy hoặc connection string sai
- **Giải pháp**:
  1. Kiểm tra SQL Server đã chạy chưa
  2. Kiểm tra connection string trong `appsettings.json`
  3. Thử kết nối bằng SQL Server Management Studio

### Lỗi SSL Certificate
- **Nguyên nhân**: HTTPS certificate không hợp lệ trong development
- **Giải pháp**: Đổi URL trong `ApiClient.cs` từ `https://` sang `http://`

### Lỗi build project
- **Nguyên nhân**: Thiếu packages hoặc .NET SDK
- **Giải pháp**:
  ```bash
  dotnet restore
  dotnet build
  ```

## Cấu trúc Database sau khi chạy

Backend sẽ tự động tạo các bảng:
- `Users`: Người dùng (Admin, Teacher, Student)
- `Subjects`: Môn học
- `CourseSections`: Lớp học phần
- `Enrollments`: Đăng ký học phần
- `ExamSchedules`: Lịch thi
- `Scores`: Điểm thi

## Dữ liệu mẫu

Sau khi chạy Backend lần đầu, hệ thống sẽ có:
- 1 Admin account
- 4 Teacher accounts
- 11 Student accounts
- 3 Subjects
- 3 Course Sections
- Một số enrollment và score mẫu

## Tiếp theo

Sau khi hệ thống chạy được, bạn có thể:
1. Thêm các tính năng UI/UX chi tiết hơn
2. Thêm validation và error handling
3. Thêm tính năng export Excel
4. Thêm thông báo (notifications)
5. Tùy chỉnh giao diện theo yêu cầu
