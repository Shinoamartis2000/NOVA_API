using System.Collections.Generic;

namespace NOVA.Application.Interfaces
{
    public interface IConversationMemoryService
    {
        void AddMessage(string sessionId, string role, string content);
        List<(string Role, string Content)> GetMessages(string sessionId);
        void ClearMemory(string sessionId);
    }
}
