# Hướng dẫn Deploy lên Render

## Bước 1: Tạo PostgreSQL Database trên Render

1. Đăng nhập vào [Render Dashboard](https://dashboard.render.com)
2. Click "New +" → "PostgreSQL"
3. Cấu hình:
   - **Name**: `quanlythi-db`
   - **Database**: `quanlythidb`
   - **User**: `quanlythi_user`
   - **Region**: Singapore (hoặc region gần bạn)
   - **Plan**: Free (hoặc plan phù hợp)
4. Click "Create Database"
5. Lưu lại thông tin connection string (Render sẽ tự động tạo)

## Bước 2: Deploy Backend API

### Cách 1: Sử dụng render.yaml (Tự động)

1. Đẩy code lên GitHub/GitLab
2. Trong Render Dashboard, click "New +" → "Blueprint"
3. Kết nối repository của bạn
4. Render sẽ tự động đọc file `render.yaml` và tạo services

### Cách 2: Deploy thủ công

1. Trong Render Dashboard, click "New +" → "Web Service"
2. Kết nối repository của bạn (GitHub/GitLab)
3. Cấu hình:
   - **Name**: `quanlythi-api`
   - **Environment**: `Dotnet`
   - **Root Directory**: `QLToChucThi-Diem(BE)/QuanLyThi.API`
   - **Build Command**: `dotnet restore && dotnet publish -c Release -o ./publish`
   - **Start Command**: `cd publish && dotnet QuanLyThi.API.dll`
   - **Plan**: Free (hoặc plan phù hợp)

4. **Environment Variables**:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   DATABASE_URL=<Connection string từ PostgreSQL database>
   ConnectionStrings__DefaultConnection=<Connection string từ PostgreSQL database>
   Jwt__Key=<Generate random key>
   Jwt__Issuer=QuanLyThiAPI
   Jwt__Audience=QuanLyThiClient
   ```

5. **Auto-Deploy**: Yes (tự động deploy khi push code)

6. Click "Create Web Service"

## Bước 3: Cập nhật Frontend

1. Mở file `ApiClient.cs` trong Frontend project
2. Thay đổi `BaseAddress` thành URL của Render:
   ```csharp
   BaseAddress = new Uri("https://quanlythi-api.onrender.com/")
   ```
   (Thay bằng URL thực tế của bạn)

3. Build và chạy Frontend

## Lưu ý quan trọng

1. **Connection String từ Render**: 
   - Render cung cấp connection string dạng: `postgresql://user:pass@host:port/dbname`
   - Code đã tự động xử lý format này trong `Program.cs`

2. **SSL**: PostgreSQL trên Render yêu cầu SSL, code đã cấu hình sẵn `SSL Mode=Require`

3. **JWT Key**: Nên tạo key dài và an toàn (ít nhất 32 ký tự)

4. **Free Plan**:
   - Service sẽ sleep sau 15 phút không hoạt động
   - Lần đầu wake up có thể mất vài giây

5. **Database Migration**:
   - Code sử dụng `EnsureCreated()` để tự động tạo database schema
   - Lần đầu deploy sẽ tự động seed dữ liệu mẫu

## Kiểm tra

1. Sau khi deploy xong, truy cập: `https://your-api-url.onrender.com/swagger`
2. Kiểm tra các API endpoints
3. Test đăng nhập với tài khoản mặc định:
   - Username: `admin`
   - Password: `admin123`

## Troubleshooting

- **Lỗi kết nối database**: Kiểm tra connection string và đảm bảo database đã được tạo
- **Lỗi build**: Kiểm tra .NET SDK version (cần .NET 8.0)
- **Service không chạy**: Xem logs trong Render Dashboard
- **CORS error**: Đã cấu hình `AllowAll` trong code, nếu vẫn lỗi có thể cần whitelist domain cụ thể
