namespace NOVA.Core.Models
{
    public class ChatResponse
    {
        public string Reply { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
