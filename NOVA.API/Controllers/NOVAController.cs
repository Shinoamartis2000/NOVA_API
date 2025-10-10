using Microsoft.AspNetCore.Mvc;
using NOVA.Infrastructure.Services;

namespace NOVA.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NOVAController : ControllerBase
    {
        private readonly PersonalityService _personalityService;

        public NOVAController(PersonalityService personalityService)
        {
            _personalityService = personalityService;
        }

        [HttpGet("init")]
        public IActionResult Initialize()
        {
            return Ok(new
            {
                message = $"{_personalityService.Personality.Name} is online and fully operational.",
                personality = _personalityService.Personality
            });
        }
    }
}
