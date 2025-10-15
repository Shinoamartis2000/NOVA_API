using NOVA.Core.Models;

namespace NOVA.Application.Interfaces
{
    public interface ISessionService
    {
        Task<ChatSession?> GetByIdAsync(Guid sessionId);
        Task<IEnumerable<ChatSession>> GetSessionsByUserAsync(Guid userId);
        Task<ChatSession> CreateSessionAsync(Guid userId, string sessionName);
        Task<bool> ActivateSessionAsync(Guid sessionId);
        Task<bool> DeleteSessionAsync(Guid sessionId);
        Task ClearSessionMemoryAsync(Guid sessionId);
    }
}
