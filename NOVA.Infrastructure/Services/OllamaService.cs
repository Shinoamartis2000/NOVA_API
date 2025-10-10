using System.Text;
using System.Text.Json;
using NOVA.Application.Interfaces;
using NOVA.Core.Models;
using NOVA.Infrastructure.Config;

namespace NOVA.Infrastructure.Services
{
    public class OllamaService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAIConfig _config;

        public OllamaService(HttpClient httpClient, OpenAIConfig config)
        {
            _config = config;
            _httpClient = httpClient;

            // Set proper base address and timeout
            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
            _httpClient.Timeout = TimeSpan.FromSeconds(60); // Longer timeout for local AI
        }

        public async Task<ChatResponse> GenerateResponseAsync(ChatRequest request)
        {
            try
            {
                var payload = new
                {
                    model = _config.Model,
                    messages = new[]
                    {
                        new { role = "system", content = "You are N.O.V.A, an advanced, futuristic personal AI assistant." },
                        new { role = "user", content = request.Prompt }
                    },
                    stream = false
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // ✅ Correct Ollama endpoint - no leading slash
                var response = await _httpClient.PostAsync("api/chat", content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // More detailed error information
                    throw new Exception($"Ollama API Error ({response.StatusCode}): {body}. Make sure Ollama is running and model '{_config.Model}' is installed.");
                }

                using var doc = JsonDocument.Parse(body);

                // ✅ Correct response parsing for Ollama
                var reply = doc.RootElement
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                return new ChatResponse
                {
                    Reply = reply ?? "N.O.V.A couldn't generate a response.",
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new ChatResponse
                {
                    Reply = $"⚠️ N.O.V.A experienced a local AI issue: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };
            }
        }
    }
}