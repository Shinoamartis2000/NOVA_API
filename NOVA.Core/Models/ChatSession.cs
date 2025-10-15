using System;

namespace NOVA.Core.Models
{
    public class ChatSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string SessionName { get; set; } = "default";
        public bool IsActive { get; set; } = true;
        public bool WakeWordEnabled { get; set; } = false;
        public string MicrophoneProfile { get; set; } = "default";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        // navigation
        public User? User { get; set; }
        public ICollection<ChatMessage>? Messages { get; set; }
    }
}
