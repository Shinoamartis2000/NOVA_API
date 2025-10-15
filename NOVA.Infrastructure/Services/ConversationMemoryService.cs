using NOVA.Application.Interfaces;
using NOVA.Core.Models;
using System.Collections.Concurrent;

namespace NOVA.Infrastructure.Services
{
    public class ConversationMemoryService : IConversationMemoryService
    {
        // Thread-safe dictionary for session storage
        private readonly ConcurrentDictionary<string, List<ChatMessage>> _sessions = new();

        public void AddMessage(string sessionId, string role, string content)
        {
            var messages = _sessions.GetOrAdd(sessionId, _ => new List<ChatMessage>());
            messages.Add(new ChatMessage
            {
                Role = role,
                Content = content,
                Timestamp = DateTime.UtcNow
            });
        }

        public IEnumerable<ChatMessage> GetMessages(string sessionId)
        {
            return _sessions.TryGetValue(sessionId, out var messages)
                ? messages
                : Enumerable.Empty<ChatMessage>();
        }

        public void ClearMemory(string sessionId)
        {
            _sessions.TryRemove(sessionId, out _);
        }
    }
}
