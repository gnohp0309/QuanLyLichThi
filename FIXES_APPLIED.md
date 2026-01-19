 Danh sách các lỗi đã sửa

## Admin
1. ✅ Sửa LoadCourseSections - refresh sau khi thêm
2. ✅ Sửa LoadSectionsTable - load đúng dữ liệu với query params
3. ✅ Đã sửa Scores - cho phép nhập điểm cho sinh viên chưa có điểm (đã có sẵn)
4. ✅ Sửa ExamSchedules - refresh sau khi thêm và mapping dữ liệu đúng
5. ⚠️ Cần thêm chức năng thông báo (đang dùng mock data)

## Student  
1. ✅ Sửa LoadAvailableClasses - hiển thị đúng lớp học phần với mapping đúng
2. ✅ Sửa LoadStudentExamSchedule - hiển thị lịch thi với mapping đúng
3. ⚠️ Cần thêm chức năng thông báo (đang dùng mock data)
4. ✅ Bảng điểm đã có summary và đã kiểm tra

## Teacher
1. ✅ Sửa dashboard - hiển thị lớp học đang giảng dạy với mapping đúng
2. ✅ Đã sửa score management - chọn lớp và nhập điểm (đã có sẵn)
3. ✅ Sửa exam schedule - hiển thị lịch thi với mapping đúng
4. ⚠️ Cần thêm chức năng thông báo (đang dùng mock data)

## Các sửa đổi chính
1. ✅ Cấu hình API trả về camelCase JSON trong Program.cs
2. ✅ Cấu hình ApiClient deserialize đúng camelCase
3. ✅ Thêm helper methods GetPropertyValue, GetIntProperty, GetExamDateString để đọc properties từ dynamic objects
4. ✅ Sửa tất cả mapping dữ liệu trong Admin, Student, Teacher forms

## UI Improvements
- Cần cải thiện giao diện theo hình ảnh đã cung cấp- Cần cải thiện giao diện theo hình ảnh đã cung cấp
