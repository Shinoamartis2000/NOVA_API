using Microsoft.AspNetCore.Mvc;
using NOVA.Application.Interfaces;
using NOVA.Core.Models;

namespace NOVA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskNova([FromBody] ChatRequest request)
        {
            try
            {
                _logger.LogInformation("Received chat request for session: {SessionId}", request.SessionId);

                // Ensure session ID is not null
                if (string.IsNullOrEmpty(request.SessionId))
                {
                    request.SessionId = "session_" + Guid.NewGuid().ToString();
                    _logger.LogInformation("Generated new session ID: {SessionId}", request.SessionId);
                }

                var response = await _chatService.GetResponseAsync(request);
                _logger.LogInformation("Successfully processed chat request");

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing chat request for session {SessionId}", request.SessionId);
                return StatusCode(500, new { error = "Internal server error", details = ex.Message });
            }
        }
    }
}