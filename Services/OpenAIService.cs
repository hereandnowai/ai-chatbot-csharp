using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AIChatBot.Services
{
    public class OpenAIService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OpenAIService> _logger;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly int _maxTokens;
        private readonly double _temperature;

        public OpenAIService(HttpClient httpClient, IConfiguration configuration, ILogger<OpenAIService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            _apiKey = _configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API key not configured");
            _model = _configuration["OpenAI:Model"] ?? "gpt-3.5-turbo";
            _maxTokens = _configuration.GetValue<int>("OpenAI:MaxTokens", 150);
            _temperature = _configuration.GetValue<double>("OpenAI:Temperature", 0.7);

            _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> GetResponseAsync(string userInput)
        {
            try
            {
                _logger.LogInformation("Sending request to OpenAI API");

                var requestBody = new
                {
                    model = _model,
                    messages = new[]
                    {
                        new { role = "system", content = "You are a helpful AI assistant. Provide concise and helpful responses." },
                        new { role = "user", content = userInput }
                    },
                    max_tokens = _maxTokens,
                    temperature = _temperature
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("chat/completions", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("OpenAI API request failed with status: {StatusCode}", response.StatusCode);
                    return "I'm sorry, I'm having trouble connecting to my AI service right now. Please try again later.";
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                using var document = JsonDocument.Parse(responseContent);

                var choices = document.RootElement.GetProperty("choices");
                if (choices.GetArrayLength() > 0)
                {
                    var message = choices[0].GetProperty("message").GetProperty("content").GetString();
                    return message?.Trim() ?? "I didn't understand that. Could you please rephrase?";
                }

                return "I didn't understand that. Could you please rephrase?";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calling OpenAI API");
                return "I'm sorry, something went wrong. Please try again.";
            }
        }
    }
}
