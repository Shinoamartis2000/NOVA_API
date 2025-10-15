namespace NOVA.Core.DTOs
{
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? DisplayName { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UserProfile
    {
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
