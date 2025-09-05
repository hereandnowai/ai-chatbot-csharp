using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AIChatBot.Services
{
    public class GeminiService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GeminiService> _logger;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly int _maxTokens;
        private readonly double _temperature;

        public GeminiService(HttpClient httpClient, IConfiguration configuration, ILogger<GeminiService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            _apiKey = _configuration["Gemini:ApiKey"] ?? throw new InvalidOperationException("Gemini API key not configured");
            _model = _configuration["Gemini:Model"] ?? "gemini-1.5-flash";
            _maxTokens = _configuration.GetValue<int>("Gemini:MaxTokens", 150);
            _temperature = _configuration.GetValue<double>("Gemini:Temperature", 0.7);

            _httpClient.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/");
        }

        public async Task<string> GetResponseAsync(string userInput)
        {
            try
            {
                _logger.LogInformation("Sending request to Gemini API");

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = $"You are a helpful AI assistant. Provide concise and helpful responses. User: {userInput}" }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = _temperature,
                        maxOutputTokens = _maxTokens,
                        topP = 0.8,
                        topK = 10
                    }
                };

                var json = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"models/{_model}:generateContent?key={_apiKey}";
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Gemini API request failed with status: {StatusCode}, Content: {ErrorContent}",
                        response.StatusCode, errorContent);
                    return "I'm sorry, I'm having trouble connecting to my AI service right now. Please try again later.";
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(responseContent);

                var candidates = document.RootElement.GetProperty("candidates");
                if (candidates.GetArrayLength() > 0)
                {
                    var candidate = candidates[0];
                    if (candidate.TryGetProperty("content", out var contentElement) &&
                        contentElement.TryGetProperty("parts", out var partsElement) &&
                        partsElement.GetArrayLength() > 0)
                    {
                        var text = partsElement[0].GetProperty("text").GetString();
                        return text?.Trim() ?? "I didn't understand that. Could you please rephrase?";
                    }
                }

                return "I didn't understand that. Could you please rephrase?";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calling Gemini API");
                return "I'm sorry, something went wrong. Please try again.";
            }
        }
    }
}
