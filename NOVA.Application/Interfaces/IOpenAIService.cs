using NOVA.Core.Models;

namespace NOVA.Application.Interfaces
{
    public interface IOpenAIService
    {
        Task<ChatResponse> GenerateResponseAsync(ChatRequest request);
    }
}
