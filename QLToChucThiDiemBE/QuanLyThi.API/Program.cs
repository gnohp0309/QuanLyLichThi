using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuanLyThi.API.Data;
using QuanLyThi.API.Models;
using QuanLyThi.API.Services;
using System.Text;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "QuanLyThi API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database - Hỗ trợ Render PostgreSQL và local PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? Environment.GetEnvironmentVariable("DATABASE_URL") // Render cung cấp DATABASE_URL
    ?? "Host=localhost;Database=QuanLyThiDB;Username=postgres;Password=postgres";

// Xử lý DATABASE_URL từ Render (format: postgresql://user:password@host:port/database)
if (connectionString.StartsWith("postgresql://"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTTokenGeneration12345678901234567890";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "QuanLyThiAPI";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "QuanLyThiClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    
    // Seed initial data
    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
    SeedData(context, authService);
}

app.Run();

// HÀM SEED DATA (Đã sửa lỗi Models.)
static void SeedData(ApplicationDbContext context, IAuthService authService)
{
    if (context.Users.Any()) return; // Đã có dữ liệu thì không thêm nữa

    // Create Admin
    // Sửa: Bỏ "Models." -> chỉ dùng "User"
    var admin = new User
    {
        Username = "admin",
        PasswordHash = authService.HashPassword("admin123"),
        FullName = "Quản trị viên",
        Role = "Admin",
        CreatedAt = DateTime.UtcNow
    };
    context.Users.Add(admin);

    // Create Teachers
    var teachers = new[]
    {
        new User { Username = "gv1", PasswordHash = authService.HashPassword("123456"), FullName = "Nguyễn Văn A", Role = "Teacher", TeacherId = "GV01", Faculty = "Công nghệ thông tin", CreatedAt = DateTime.UtcNow },
        new User { Username = "gv2", PasswordHash = authService.HashPassword("123456"), FullName = "Trần Thị B", Role = "Teacher", TeacherId = "GV02", Faculty = "Công nghệ thông tin", CreatedAt = DateTime.UtcNow },
        new User { Username = "gv3", PasswordHash = authService.HashPassword("123456"), FullName = "Lê Quốc C", Role = "Teacher", TeacherId = "GV03", Faculty = "Công nghệ thông tin", CreatedAt = DateTime.UtcNow },
        new User { Username = "gv4", PasswordHash = authService.HashPassword("123456"), FullName = "TS Lê Huy", Role = "Teacher", TeacherId = "GV04", Faculty = "Công nghệ thông tin", CreatedAt = DateTime.UtcNow }
    };
    context.Users.AddRange(teachers);

    // Create Students
    var students = new List<User>();
    for (int i = 1; i <= 11; i++)
    {
        students.Add(new User
        {
            Username = $"sv{i:D2}",
            PasswordHash = authService.HashPassword("123456"),
            FullName = i == 1 ? "Sinh viên 01" : $"Sinh viên {i:D2}",
            Role = "Student",
            StudentId = $"SV{i:D2}",
            Major = "Công nghệ thông tin",
            CreatedAt = DateTime.UtcNow
        });
    }
    context.Users.AddRange(students);

    context.SaveChanges();

    // Create Subjects
    var subjects = new[]
    {
        new Subject { SubjectCode = "CSPM", SubjectName = "Lập trình C# .NET", Credits = 3, CreatedAt = DateTime.UtcNow },
        new Subject { SubjectCode = "SQLDB", SubjectName = "Cơ sở dữ liệu SQL", Credits = 3, CreatedAt = DateTime.UtcNow },
        new Subject { SubjectCode = "IT03", SubjectName = "Lập trình C++", Credits = 3, CreatedAt = DateTime.UtcNow }
    };
    context.Subjects.AddRange(subjects);
    context.SaveChanges();

    // Create Course Sections
    var courseSections = new[]
    {
        new CourseSection { SectionCode = "CSPM01", SubjectCode = "CSPM", TeacherId = teachers[1].Id, Semester = "HK1/2025", DefaultRoom = "A101", CreatedAt = DateTime.UtcNow },
        new CourseSection { SectionCode = "SQL01", SubjectCode = "SQLDB", TeacherId = teachers[2].Id, Semester = "HK1/2025", DefaultRoom = "B202", CreatedAt = DateTime.UtcNow },
        new CourseSection { SectionCode = "LOP02", SubjectCode = "SQLDB", TeacherId = teachers[1].Id, Semester = "HK2/2024-2025", DefaultRoom = "A201", CreatedAt = DateTime.UtcNow }
    };
    context.CourseSections.AddRange(courseSections);
    context.SaveChanges();

    // Create Enrollments
    var enrollments = new[]
    {
        new Enrollment { StudentId = students[0].Id, SectionCode = "CSPM01", Status = "Active", EnrollmentDate = DateTime.UtcNow },
        new Enrollment { StudentId = students[0].Id, SectionCode = "SQL01", Status = "Active", EnrollmentDate = DateTime.UtcNow }
    };
    context.Enrollments.AddRange(enrollments);
    
    // Cập nhật số lượng
    courseSections[0].EnrollmentCount = 1;
    courseSections[1].EnrollmentCount = 1;

    // Create Scores
    var scores = new[]
    {
        new Score
        {
            StudentId = students[0].Id,
            SectionCode = "CSPM01",
            AttendanceScore = 8.0m,
            MidtermScore = 7.5m,
            FinalScore = 9.0m,
            TotalScore = 8.45m,
            Grade = "B",
            Classification = "Khá",
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            PublishedAt = DateTime.UtcNow
        }
    };
    context.Scores.AddRange(scores);
    context.SaveChanges();
}
