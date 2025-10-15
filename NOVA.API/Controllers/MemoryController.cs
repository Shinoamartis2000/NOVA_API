using Microsoft.AspNetCore.Mvc;
using NOVA.Application.Interfaces;
using System.Linq;

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
        [HttpGet("{sessionId}")]
        public IActionResult GetMemory(string sessionId)
        {
            var memory = _memoryService.GetMessages(sessionId)?.ToList() ?? new List<NOVA.Core.Models.ChatMessage>();

            if (!memory.Any())
            {
                return Ok(new
                {
                    message = $"🧠 N.O.V.A: No memory found for session '{sessionId}'.",
                    timestamp = DateTime.UtcNow
                });
            }

            var result = memory.Select(m => new
            {
                role = m.Role,
                content = m.Content
            });

            return Ok(new
            {
                sessionId,
                messageCount = memory.Count,
                history = result,
                timestamp = DateTime.UtcNow
            });
        }
    }
}
