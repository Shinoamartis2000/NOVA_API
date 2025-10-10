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

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> AskNova([FromBody] ChatRequest request)
        {
            var response = await _chatService.GetResponseAsync(request);
            return Ok(response);
        }
    }
}
