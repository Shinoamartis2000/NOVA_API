using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using NOVA.Application.Interfaces;
using NOVA.Core.Models;
using NOVA.Infrastructure.Config;

namespace NOVA.Infrastructure.Services
{
    public class OllamaService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAIConfig _config;

        public OllamaService(HttpClient httpClient, IOptions<OpenAIConfig> options)
        {
            _config = options.Value;
            _httpClient = httpClient;

            if (!string.IsNullOrEmpty(_config.BaseUrl))
                _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        }

        public async Task<ChatResponse> GenerateResponseAsync(ChatRequest request)
        {
            try
            {
                // Check if streaming is requested
                bool stream = request.Stream; // You'll need to add this property to ChatRequest

                var payload = new
                {
                    model = _config.Model,
                    prompt = request.Prompt,
                    stream = stream
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/generate", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Ollama API Error: {await response.Content.ReadAsStringAsync()}");
                }

                if (stream)
                {
                    // For streaming, you'd need to handle the stream on the backend
                    // and forward it to the frontend. This is a simplified version.
                    var streamContent = await response.Content.ReadAsStreamAsync();
                    // You'd need to implement proper streaming response handling here
                }

                var body = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(body);
                var reply = doc.RootElement.GetProperty("response").GetString();

                return new ChatResponse
                {
                    Reply = reply ?? "I'm here to help!",
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new ChatResponse
                {
                    Reply = $"Sorry, I encountered an error: {ex.Message}",
                    Timestamp = DateTime.UtcNow
                };
            }
        }
    }
}