using NOVA.Application.Interfaces;
using NOVA.Core.Models;

namespace NOVA.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IOpenAIService _openAIService;

        public ChatService(IOpenAIService openAIService)
        {
            _openAIService = openAIService;
        }

        public async Task<ChatResponse> GetResponseAsync(ChatRequest request)
        {
            return await _openAIService.GenerateResponseAsync(request);
        }
    }
}
