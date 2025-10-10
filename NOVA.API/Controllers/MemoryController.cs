using Microsoft.AspNetCore.Mvc;
using NOVA.Application.Interfaces;

namespace NOVA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemoryController : ControllerBase
    {
        private readonly IConversationMemoryService _memoryService;

        public MemoryController(IConversationMemoryService memoryService)
        {
            _memoryService = memoryService;
        }

        /// <summary>
        /// Clears N.O.V.A's memory for a specific session.
        /// </summary>
        /// <param name="sessionId">The session ID to clear memory for.</param>
        [HttpDelete("{sessionId}")]
        public IActionResult ClearMemory(string sessionId)
        {
            _memoryService.ClearMemory(sessionId);
            return Ok(new
            {
                message = $"🧠 N.O.V.A: Memory for session '{sessionId}' has been cleared successfully.",
                timestamp = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Retrieves all stored messages for a specific session.
        /// </summary>
        /// <param name="sessionId">The session ID to retrieve memory for.</param>
        [HttpGet("{sessionId}")]
        public IActionResult GetMemory(string sessionId)
        {
            var memory = _memoryService.GetMessages(sessionId);
            if (memory == null || memory.Count == 0)
            {
                return Ok(new
                {
                    message = $"🧠 N.O.V.A: No memory found for session '{sessionId}'.",
                    timestamp = DateTime.UtcNow
                });
            }

            return Ok(new
            {
                sessionId,
                messageCount = memory.Count,
                history = memory.Select(m => new { role = m.Role, content = m.Content }),
                timestamp = DateTime.UtcNow
            });
        }
    }
}
