using Microsoft.AspNetCore.Mvc;
using NOVA.Application.Interfaces;
using NOVA.Core.Models;

namespace NOVA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IUserService _userService;
        private readonly ILogger<SessionController> _logger;

        public SessionController(
            ISessionService sessionService,
            IUserService userService,
            ILogger<SessionController> logger)
        {
            _sessionService = sessionService;
            _userService = userService;
            _logger = logger;
        }

        // ✅ Create a new chat session
        [HttpPost("create")]
        public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest request)
        {
            if (request.UserId == Guid.Empty)
                return BadRequest(new { message = "Invalid User ID." });

            try
            {
                var user = await _userService.GetByIdAsync(request.UserId);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                var session = await _sessionService.CreateSessionAsync(request.UserId, request.SessionName);
                return Ok(new
                {
                    message = "Session created successfully.",
                    session
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating session for User {UserId}", request.UserId);
                return StatusCode(500, new { message = "Internal server error creating session." });
            }
        }

        // ✅ Get all sessions for a specific user
        [HttpGet("user/{userId:guid}")]
        public async Task<IActionResult> GetSessionsByUser(Guid userId)
        {
            try
            {
                var user = await _userService.GetByIdAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                var sessions = await _sessionService.GetSessionsByUserAsync(userId);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching sessions for User {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error fetching sessions." });
            }
        }

        // ✅ Activate a session
        [HttpPut("{sessionId:guid}/activate")]
        public async Task<IActionResult> ActivateSession(Guid sessionId)
        {
            try
            {
                var success = await _sessionService.ActivateSessionAsync(sessionId);
                if (!success)
                    return NotFound(new { message = "Session not found." });

                return Ok(new { message = "Session activated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating session {SessionId}", sessionId);
                return StatusCode(500, new { message = "Internal server error activating session." });
            }
        }

        // ✅ Delete a session
        [HttpDelete("{sessionId:guid}")]
        public async Task<IActionResult> DeleteSession(Guid sessionId)
        {
            try
            {
                var success = await _sessionService.DeleteSessionAsync(sessionId);
                if (!success)
                    return NotFound(new { message = "Session not found or already deleted." });

                return Ok(new { message = "Session deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting session {SessionId}", sessionId);
                return StatusCode(500, new { message = "Internal server error deleting session." });
            }
        }

        // ✅ Clear session memory (reset conversation)
        [HttpPost("{sessionId:guid}/clear-memory")]
        public async Task<IActionResult> ClearSessionMemory(Guid sessionId)
        {
            try
            {
                var session = await _sessionService.GetByIdAsync(sessionId);
                if (session == null)
                    return NotFound(new { message = "Session not found." });

                await _sessionService.ClearSessionMemoryAsync(sessionId);
                return Ok(new { message = "Session memory cleared successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing memory for session {SessionId}", sessionId);
                return StatusCode(500, new { message = "Internal server error clearing session memory." });
            }
        }
    }

    // DTO for creating sessions
    public class CreateSessionRequest
    {
        public Guid UserId { get; set; }
        public string SessionName { get; set; } = "Default";
    }
}
