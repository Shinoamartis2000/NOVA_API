using System;

namespace NOVA.Core.Models
{
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SessionId { get; set; }
        public Guid? UserId { get; set; }           // optional - message owner
        public string Role { get; set; } = "user";  // user / assistant / system
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // navigation
        public ChatSession? Session { get; set; }
    }
}
