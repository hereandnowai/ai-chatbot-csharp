using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AIChatBot.Services
{
    public class MockAIService : IAIService
    {
        private readonly ILogger<MockAIService> _logger;
        private readonly Random _random;
        private readonly string[] _responses = new[]
        {
            "That's an interesting question! Let me think about that...",
            "I understand what you're asking. Here's my perspective...",
            "Great point! I'd like to add that...",
            "That's a complex topic. From what I know...",
            "I appreciate you sharing that with me.",
            "That reminds me of something similar...",
            "I can help you with that. Here's what I suggest...",
            "That's a good observation. Let me expand on that...",
            "I see where you're coming from. My thoughts are...",
            "Interesting! I hadn't considered that angle before."
        };

        public MockAIService(ILogger<MockAIService> logger)
        {
            _logger = logger;
            _random = new Random();
        }

        public async Task<string> GetResponseAsync(string userInput)
        {
            _logger.LogInformation("Using mock AI service (OpenAI not configured)");

            // Simulate some processing time
            await Task.Delay(500);

            // Simple response logic based on user input
            if (string.IsNullOrWhiteSpace(userInput))
            {
                return "I didn't receive any input. Could you please say something?";
            }

            var input = userInput.ToLowerInvariant();

            if (input.Contains("hello") || input.Contains("hi") || input.Contains("hey"))
            {
                return "Hello! Nice to meet you. How can I assist you today?";
            }

            if (input.Contains("bye") || input.Contains("goodbye") || input.Contains("exit"))
            {
                return "Goodbye! It was nice chatting with you. Have a wonderful day!";
            }

            if (input.Contains("help"))
            {
                return "I'm here to help! You can ask me questions, have a conversation, or just chat. What would you like to talk about?";
            }

            if (input.Contains("weather"))
            {
                return "I don't have access to real-time weather data, but I hope it's nice where you are! Is there something specific about weather you'd like to discuss?";
            }

            if (input.Contains("time") || input.Contains("date"))
            {
                return $"I don't have access to the current time, but it's always a good time to chat! The current system time on your machine would be: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            }

            // Return a random response for other inputs
            var randomResponse = _responses[_random.Next(_responses.Length)];
            return $"{randomResponse} You mentioned: '{userInput}'. What else would you like to know?";
        }
    }
}
