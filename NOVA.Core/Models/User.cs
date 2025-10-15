using System;

namespace NOVA.Core.Models
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // hashed
        public string VoiceType { get; set; } = "default";
        public string PreferencesJson { get; set; } = "{}";    // flexible json blob
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;

        public ICollection<ChatSession>? Sessions { get; set; }
    }
}
