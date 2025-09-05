using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AIChatBot.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AIChatBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "AI ChatBot";

            try
            {
                var host = CreateHostBuilder(args).Build();
                var chatBot = host.Services.GetRequiredService<IChatBotService>();
                await chatBot.RunAsync();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory())
                          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                          .AddUserSecrets<Program>()
                          .AddEnvironmentVariables()
                          .AddCommandLine(args);
                })
                .ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;

                    // Configure HttpClient for AI services
                    services.AddHttpClient<OpenAIService>();
                    services.AddHttpClient<GeminiService>();

                    // Register AI service based on configuration
                    var aiProvider = configuration["AI:Provider"]?.ToLowerInvariant() ?? "openai";

                    switch (aiProvider)
                    {
                        case "gemini":
                            var geminiApiKey = configuration["Gemini:ApiKey"];
                            if (!string.IsNullOrEmpty(geminiApiKey) && geminiApiKey != "your-gemini-api-key-here")
                            {
                                services.AddScoped<IAIService, GeminiService>();
                            }
                            else
                            {
                                services.AddScoped<IAIService, MockAIService>();
                            }
                            break;

                        case "openai":
                        default:
                            var openaiApiKey = configuration["OpenAI:ApiKey"];
                            if (!string.IsNullOrEmpty(openaiApiKey) && openaiApiKey != "your-openai-api-key-here")
                            {
                                services.AddScoped<IAIService, OpenAIService>();
                            }
                            else
                            {
                                services.AddScoped<IAIService, MockAIService>();
                            }
                            break;
                    }

                    // Register ChatBot service
                    services.AddScoped<IChatBotService, ChatBotService>();
                })
                .ConfigureLogging(logging =>
                {
                    // Remove default providers. We intentionally do NOT add the console
                    // logging provider so framework/HTTP/info logs don't clutter the console.
                    // Chat output uses Console.WriteLine directly and will remain visible.
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Warning);
                });
    }
}
