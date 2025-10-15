using NOVA.Core.Models;
using System.Collections.Generic;

namespace NOVA.Application.Interfaces
{
    public interface IConversationMemoryService
    {
        /// <summary>
        /// Add a message to the conversation memory.
        /// </summary>
        void AddMessage(string sessionId, string role, string content);

        /// <summary>
        /// Retrieve all messages for a session.
        /// </summary>
        IEnumerable<ChatMessage> GetMessages(string sessionId);

        /// <summary>
        /// Clear memory for a session.
        /// </summary>
        void ClearMemory(string sessionId);
    }
}
