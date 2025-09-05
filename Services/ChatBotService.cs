using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AIChatBot.Services;

namespace AIChatBot.Services
{
    public interface IChatBotService
    {
        Task RunAsync();
    }

    public class ChatBotService : IChatBotService
    {
        private readonly IAIService _aiService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChatBotService> _logger;
        private readonly string _botName;
        private readonly string _welcomeMessage;
        private readonly string _goodbyeMessage;

        public ChatBotService(IAIService aiService, IConfiguration configuration, ILogger<ChatBotService> logger)
        {
            _aiService = aiService;
            _configuration = configuration;
            _logger = logger;

            _botName = _configuration["ChatBot:Name"] ?? "AI Assistant";
            _welcomeMessage = _configuration["ChatBot:WelcomeMessage"] ?? "Hello! I'm your AI assistant. How can I help you today?";
            _goodbyeMessage = _configuration["ChatBot:GoodbyeMessage"] ?? "Goodbye! Have a great day!";
        }

        public async Task RunAsync()
        {
            _logger.LogInformation("Starting chatbot session");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"🤖 {_botName}");
            Console.WriteLine(new string('=', 50));
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(_welcomeMessage);
            Console.WriteLine("Type 'exit', 'quit', or 'bye' to end the conversation.");
            Console.WriteLine();
            Console.ResetColor();

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("You: ");
                Console.ResetColor();

                var userInput = Console.ReadLine();

                // If Console.ReadLine returns null, the input stream was closed
                // (for example when input is piped). Treat this as end-of-session
                // and exit the loop gracefully instead of repeatedly prompting.
                if (userInput == null)
                {
                    Console.WriteLine();
                    break;
                }

                if (string.IsNullOrWhiteSpace(userInput))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Please enter a message.");
                    Console.ResetColor();
                    Console.WriteLine();
                    continue;
                }

                // Check for exit commands
                var input = userInput.Trim().ToLowerInvariant();
                if (input == "exit" || input == "quit" || input == "bye" || input == "goodbye")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{_botName}: {_goodbyeMessage}");
                    Console.ResetColor();
                    break;
                }

                // Show thinking indicator
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"{_botName} is thinking");

                var thinkingTask = ShowThinkingAnimation();

                try
                {
                    var response = await _aiService.GetResponseAsync(userInput);
                    thinkingTask.Cancel();

                    Console.Write("\r" + new string(' ', 50) + "\r"); // Clear the thinking line

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"{_botName}: {response}");
                    Console.ResetColor();
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    thinkingTask.Cancel();
                    Console.Write("\r" + new string(' ', 50) + "\r"); // Clear the thinking line

                    _logger.LogError(ex, "Error getting AI response");
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{_botName}: I'm sorry, I encountered an error. Please try again.");
                    Console.ResetColor();
                    Console.WriteLine();
                }
            }

            _logger.LogInformation("Chatbot session ended");
        }

        private System.Threading.CancellationTokenSource ShowThinkingAnimation()
        {
            var cts = new System.Threading.CancellationTokenSource();

            Task.Run(async () =>
            {
                var dots = 0;
                while (!cts.Token.IsCancellationRequested)
                {
                    Console.Write(".");
                    dots++;
                    if (dots > 3)
                    {
                        Console.Write("\b\b\b\b   \b\b\b\b");
                        dots = 0;
                    }

                    try
                    {
                        await Task.Delay(500, cts.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        break;
                    }
                }
            });

            return cts;
        }
    }
}
