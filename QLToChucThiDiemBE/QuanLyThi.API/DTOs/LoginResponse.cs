namespace QuanLyThi.API.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserDto User { get; set; } = null!;
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? StudentId { get; set; }
        public string? TeacherId { get; set; }
        public string? Major { get; set; }
        public string? Faculty { get; set; }
    }
}
