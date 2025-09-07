using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AIChatBot.Services
{
    public class LiteLLMService : IAIService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LiteLLMService> _logger;
        private readonly string _model;
        private readonly int _maxTokens;
        private readonly double _temperature;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public LiteLLMService(HttpClient httpClient, IConfiguration configuration, ILogger<LiteLLMService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;

            // Read configuration with flexible fallbacks
            _model = _configuration["LiteLLM:Model"] ?? "gpt-3.5-turbo";
            _maxTokens = _configuration.GetValue<int>("LiteLLM:MaxTokens", 150);
            _temperature = _configuration.GetValue<double>("LiteLLM:Temperature", 0.7);
            _apiKey = _configuration["LiteLLM:ApiKey"] ?? "";
            _baseUrl = _configuration["LiteLLM:BaseUrl"] ?? "https://api.openai.com/v1/";

            // Configure HttpClient based on provider
            ConfigureHttpClientForProvider();
        }

        private void ConfigureHttpClientForProvider()
        {
            var provider = GetProviderFromModel(_model);

            switch (provider.ToLowerInvariant())
            {
                case "openai":
                    _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
                    if (!string.IsNullOrEmpty(_apiKey))
                        _httpClient.DefaultRequestHeaders.Authorization =
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
                    break;

                case "anthropic":
                    _httpClient.BaseAddress = new Uri("https://api.anthropic.com/v1/");
                    if (!string.IsNullOrEmpty(_apiKey))
                    {
                        _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
                        _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");
                    }
                    break;

                case "gemini":
                    _httpClient.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/");
                    break;

                case "ollama":
                    var ollamaUrl = _configuration["LiteLLM:OllamaUrl"] ?? "http://localhost:11434/";
                    _httpClient.BaseAddress = new Uri(ollamaUrl);
                    break;

                default:
                    // Use custom base URL if provided
                    if (!string.IsNullOrEmpty(_baseUrl))
                    {
                        _httpClient.BaseAddress = new Uri(_baseUrl);
                        if (!string.IsNullOrEmpty(_apiKey))
                            _httpClient.DefaultRequestHeaders.Authorization =
                                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
                    }
                    break;
            }
        }

        private string GetProviderFromModel(string model)
        {
            if (model.StartsWith("gpt-", StringComparison.OrdinalIgnoreCase) ||
                model.StartsWith("o1-", StringComparison.OrdinalIgnoreCase))
                return "openai";

            if (model.StartsWith("claude-", StringComparison.OrdinalIgnoreCase))
                return "anthropic";

            if (model.StartsWith("gemini-", StringComparison.OrdinalIgnoreCase))
                return "gemini";

            if (model.StartsWith("llama", StringComparison.OrdinalIgnoreCase) ||
                model.StartsWith("mistral", StringComparison.OrdinalIgnoreCase) ||
                model.StartsWith("deepseek", StringComparison.OrdinalIgnoreCase) ||
                model.StartsWith("qwen", StringComparison.OrdinalIgnoreCase) ||
                model.Contains("local"))
                return "ollama";

            // Default fallback
            return "openai";
        }

        public async Task<string> GetResponseAsync(string userInput)
        {
            try
            {
                var provider = GetProviderFromModel(_model);
                _logger.LogInformation("Sending request to {Provider} with model {Model}", provider, _model);

                return provider.ToLowerInvariant() switch
                {
                    "openai" => await CallOpenAIAsync(userInput),
                    "anthropic" => await CallAnthropicAsync(userInput),
                    "gemini" => await CallGeminiAsync(userInput),
                    "ollama" => await CallOllamaAsync(userInput),
                    _ => await CallOpenAIAsync(userInput) // Default fallback
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calling LLM provider");
                return "I'm sorry, something went wrong. Please try again.";
            }
        }

        private async Task<string> CallOpenAIAsync(string userInput)
        {
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

        private async Task<string> CallAnthropicAsync(string userInput)
        {
            var requestBody = new
            {
                model = _model,
                max_tokens = _maxTokens,
                temperature = _temperature,
                messages = new[]
                {
                    new { role = "user", content = userInput }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("messages", content);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Anthropic API request failed with status: {StatusCode}", response.StatusCode);
                return "I'm sorry, I'm having trouble connecting to my AI service right now. Please try again later.";
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseContent);

            var contentArray = document.RootElement.GetProperty("content");
            if (contentArray.GetArrayLength() > 0)
            {
                var text = contentArray[0].GetProperty("text").GetString();
                return text?.Trim() ?? "I didn't understand that. Could you please rephrase?";
            }

            return "I didn't understand that. Could you please rephrase?";
        }

        private async Task<string> CallGeminiAsync(string userInput)
        {
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

        private async Task<string> CallOllamaAsync(string userInput)
        {
            var requestBody = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = "You are a helpful AI assistant. Provide concise and helpful responses." },
                    new { role = "user", content = userInput }
                },
                stream = false,
                options = new
                {
                    temperature = _temperature,
                    num_predict = _maxTokens
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/chat", content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Ollama API request failed with status: {StatusCode}, Content: {ErrorContent}",
                    response.StatusCode, errorContent);
                return "I'm sorry, I'm having trouble connecting to my AI service right now. Please try again later.";
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(responseContent);

            if (document.RootElement.TryGetProperty("message", out var messageElement) &&
                messageElement.TryGetProperty("content", out var contentElement))
            {
                var text = contentElement.GetString();
                return text?.Trim() ?? "I didn't understand that. Could you please rephrase?";
            }

            return "I didn't understand that. Could you please rephrase?";
        }
    }
}
