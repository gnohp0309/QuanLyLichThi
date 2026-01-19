using Microsoft.AspNetCore.Mvc;
using QuanLyThi.API.DTOs;
using QuanLyThi.API.Services;

namespace QuanLyThi.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized(new { message = "Tên đăng nhập, mật khẩu hoặc vai trò không đúng" });
            }
            return Ok(response);
        }
    }
}
