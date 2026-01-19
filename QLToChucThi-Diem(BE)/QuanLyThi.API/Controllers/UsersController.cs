using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyThi.API.Data;
using QuanLyThi.API.DTOs;
using QuanLyThi.API.Models;
using QuanLyThi.API.Services;
using System.Security.Claims;

namespace QuanLyThi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthService _authService;

        public UsersController(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    Role = u.Role,
                    StudentId = u.StudentId,
                    TeacherId = u.TeacherId,
                    Major = u.Major,
                    Faculty = u.Faculty
                })
                .ToListAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            // Users can only view their own profile unless they are Admin
            if (currentUserRole != "Admin" && currentUserId != id)
            {
                return Forbid();
            }

            return Ok(new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role,
                StudentId = user.StudentId,
                TeacherId = user.TeacherId,
                Major = user.Major,
                Faculty = user.Faculty
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại" });
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = _authService.HashPassword(request.Password),
                FullName = request.FullName,
                Role = request.Role,
                StudentId = request.StudentId,
                TeacherId = request.TeacherId,
                Major = request.Major,
                Faculty = request.Faculty,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Role = user.Role,
                StudentId = user.StudentId,
                TeacherId = user.TeacherId,
                Major = user.Major,
                Faculty = user.Faculty
            });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(request.Username) && request.Username != user.Username)
            {
                if (await _context.Users.AnyAsync(u => u.Username == request.Username && u.Id != id))
                {
                    return BadRequest(new { message = "Tên đăng nhập đã tồn tại" });
                }
                user.Username = request.Username;
            }

            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;
            if (!string.IsNullOrEmpty(request.Role))
                user.Role = request.Role;
            if (request.StudentId != null)
                user.StudentId = request.StudentId;
            if (request.TeacherId != null)
                user.TeacherId = request.TeacherId;
            if (request.Major != null)
                user.Major = request.Major;
            if (request.Faculty != null)
                user.Faculty = request.Faculty;
            if (!string.IsNullOrEmpty(request.Password))
                user.PasswordHash = _authService.HashPassword(request.Password);

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? StudentId { get; set; }
        public string? TeacherId { get; set; }
        public string? Major { get; set; }
        public string? Faculty { get; set; }
    }

    public class UpdateUserRequest
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public string? StudentId { get; set; }
        public string? TeacherId { get; set; }
        public string? Major { get; set; }
        public string? Faculty { get; set; }
    }
}
