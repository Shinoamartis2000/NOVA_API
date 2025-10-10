namespace NOVA.Infrastructure.Config
{
    public class OpenAIConfig
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "http://localhost:11434";
        public string Model { get; set; } = "llama3.2";
    }
}