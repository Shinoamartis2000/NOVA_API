namespace NOVA.Core.Models
{
    public class ChatRequest
    {
        public string Prompt { get; set; } = string.Empty;
        public string? Context { get; set; }
    }
}
