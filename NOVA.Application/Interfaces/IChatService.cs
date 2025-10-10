using NOVA.Core.Models;

namespace NOVA.Application.Interfaces
{
    public interface IChatService
    {
        Task<ChatResponse> GetResponseAsync(ChatRequest request);
    }
}
