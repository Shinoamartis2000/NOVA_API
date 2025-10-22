using Microsoft.Extensions.Logging;
using NOVA.Application.Interfaces;
using NOVA.Core.Models;

namespace NOVA.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly IOpenAIService _openAIService;
        private readonly IConversationMemoryService _memoryService;
        private readonly ILogger<ChatService> _logger;

        public ChatService(IOpenAIService openAIService, IConversationMemoryService memoryService, ILogger<ChatService> logger)
        {
            _openAIService = openAIService;
            _memoryService = memoryService;
            _logger = logger;
        }

        public async Task<ChatResponse> GetResponseAsync(ChatRequest request)
        {
            try
            {
                _logger.LogInformation("ChatService: Processing request for session: {SessionId}", request.SessionId);

                // Add user message to memory
                _memoryService.AddMessage(request.SessionId, "user", request.Prompt);
                _logger.LogInformation("ChatService: Added user message to memory");

                // Get AI response (OllamaService will handle the conversation history)
                var response = await _openAIService.GenerateResponseAsync(request);
                _logger.LogInformation("ChatService: Received response from AI service");

                // Note: Assistant response is added to memory by OllamaService

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ChatService: Error processing request");
                throw;
            }
        }
    }
}