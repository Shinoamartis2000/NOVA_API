using System.Text;
using System.Text.Json;
using NOVA.Application.Interfaces;
using NOVA.Core.Models;
using NOVA.Infrastructure.Config;

namespace NOVA.Infrastructure.Services
{
    public class OllamaService : IOpenAIService
    {
        private readonly IConversationMemoryService _memoryService;
        private readonly HttpClient _httpClient;
        private readonly OpenAIConfig _config;

        public OllamaService(HttpClient httpClient, OpenAIConfig config, IConversationMemoryService memoryService)
        {
            _config = config;
            _httpClient = httpClient;
            _memoryService = memoryService;
            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        }

        public async Task<ChatResponse> GenerateResponseAsync(ChatRequest request)
        {
            try
            {
                // 🔹 Retrieve prior conversation context
                var sessionId = request.SessionId ?? "default";
                var history = _memoryService.GetMessages(sessionId);

                // Build full conversation history (system + memory + current user message)
                var messages = new List<object>
                {
                    new { role = "system", content = "You are N.O.V.A, a futuristic, loyal, and highly intelligent AI assistant. Respond naturally and maintain continuity in context." }
                };

                messages.AddRange(history.Select(m => new { role = m.Role, content = m.Content }));
                messages.Add(new { role = "user", content = request.Prompt });

                // Prepare payload for Ollama API
                var payload = new
                {
                    model = _config.Model,
                    messages = messages,
                    stream = false
                };

                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // ✅ Correct Ollama endpoint - no leading slash
                var response = await _httpClient.PostAsync("api/chat", content);
                var body = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Ollama API Error ({response.StatusCode}): {body}. Make sure Ollama is running and model '{_config.Model}' is installed.");
                }

                using var doc = JsonDocument.Parse(body);

                // ✅ Correct response parsing for Ollama
                string? reply = null;
                if (doc.RootElement.TryGetProperty("message", out var msgElem))
                {
                    reply = msgElem.GetProperty("content").GetString();
                }
                else
                {
                    reply = body; // fallback if structure differs
                }

                var finalReply = reply ?? "N.O.V.A couldn't generate a response.";

                // 🧠 Store this conversation into memory
                _memoryService.AddMessage(sessionId, "user", request.Prompt);
                _memoryService.AddMessage(sessionId, "assistant", finalReply);

                return new ChatResponse
                {
                    Reply = finalReply,
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
