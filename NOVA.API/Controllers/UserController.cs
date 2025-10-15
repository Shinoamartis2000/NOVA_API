using Microsoft.AspNetCore.Mvc;
using NOVA.API.Models;
using NOVA.Application.Interfaces;
using NOVA.Core.Models;

namespace NOVA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // ✅ Register new user
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Username and password are required." });

            try
            {
                var (success, message, user) = await _userService.RegisterAsync(request.Username, request.Password);
                if (!success)
                    return BadRequest(new { message });

                return Ok(new
                {
                    message,
                    user = new { user!.Id, user.Username, user.CreatedAt }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for user {Username}", request.Username);
                return StatusCode(500, new { message = "Internal server error during registration." });
            }
        }

        // ✅ User login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Username and password are required." });

            try
            {
                var (success, token, user) = await _userService.LoginAsync(request.Username, request.Password);
                if (!success || user == null)
                    return Unauthorized(new { message = "Invalid credentials." });

                return Ok(new AuthResponse
                {
                    Success = true,
                    Token = token,
                    Message = $"Welcome back, {user.Username}!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Username}", request.Username);
                return StatusCode(500, new { message = "Internal server error during login." });
            }
        }

        // ✅ Get user by ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                return Ok(new
                {
                    user.Id,
                    user.Username,
                    user.CreatedAt,
                    user.LastActive,
                    user.VoiceType
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error fetching user data." });
            }
        }

        // ✅ Update user details
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] User updatedUser)
        {
            try
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                // Update only relevant fields
                user.VoiceType = updatedUser.VoiceType;
                user.PreferencesJson = updatedUser.PreferencesJson;
                user.LastActive = DateTime.UtcNow;

                var success = await _userService.UpdateAsync(user);
                if (!success)
                    return BadRequest(new { message = "Failed to update user." });

                return Ok(new { message = "User updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user {Id}", id);
                return StatusCode(500, new { message = "Internal server error during update." });
            }
        }

        // ✅ Delete user account
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var success = await _userService.DeleteAsync(id);
                if (!success)
                    return NotFound(new { message = "User not found or already deleted." });

                return Ok(new { message = "User deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user {Id}", id);
                return StatusCode(500, new { message = "Internal server error during deletion." });
            }
        }
    }
}
