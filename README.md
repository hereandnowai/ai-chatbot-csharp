# AI ChatBot

An AI-powered chatbot built with C# that supports multiple AI providers through a unified LiteLLM service.

## Features

- 🤖 **Multi-Provider Support**: OpenAI, Anthropic Claude, Google Gemini, Ollama (local models)
- 🌐 **Unified Interface**: Single configuration for all AI providers
- 🏠 **Local Models**: Support for Ollama with Llama, Mistral, DeepSeek, Qwen, and more
- 🔄 **Fallback Mode**: Built-in mock AI service when no API key is configured
- ⚙️ **Easy Configuration**: Simple JSON configuration or secure user secrets
- 🪵 **Structured Logging**: Built-in logging with Microsoft.Extensions.Logging
- 💉 **Clean Architecture**: Dependency injection and modular design
- 🎨 **Colorful Interface**: User-friendly console with colors and animations
- 🔧 **Extensible**: Easy to add new providers or customize behavior

## Supported AI Providers

| Provider | Models | Example |
|----------|--------|---------|
| **OpenAI** | GPT-3.5, GPT-4, o1 | `gpt-4`, `gpt-3.5-turbo` |
| **Anthropic** | Claude 3 | `claude-3-sonnet-20240229` |
| **Google** | Gemini | `gemini-1.5-pro`, `gemini-1.5-flash` |
| **Ollama** | Local models | `llama3.1:8b`, `mistral:7b` |
| **Custom** | Any OpenAI-compatible API | Configure via `BaseUrl` |

## Prerequisites

- .NET 8.0 SDK or later
- (Optional) API key for your chosen provider
- (Optional) Ollama installed for local models

## Quick Start

### 1. Clone the Repository
```bash
git clone https://github.com/hereandnowai/ai-chatbot-csharp.git
cd ai-chatbot-csharp
```

### 2. Choose Your AI Provider

#### Option A: Quick Test (No API Key Required)
Just run the app - it will use the mock AI service:
```bash
dotnet run
```

#### Option B: Use OpenAI GPT-4
```bash
dotnet user-secrets set "LiteLLM:ApiKey" "your-openai-api-key"
dotnet user-secrets set "LiteLLM:Model" "gpt-4"
dotnet run
```

#### Option C: Use Claude
```bash
dotnet user-secrets set "LiteLLM:ApiKey" "your-anthropic-api-key" 
dotnet user-secrets set "LiteLLM:Model" "claude-3-sonnet-20240229"
dotnet run
```

#### Option D: Use Local Ollama
```bash
# Start Ollama and pull a model
ollama pull llama3.1:8b

# Configure the app
dotnet user-secrets set "LiteLLM:Model" "llama3.1:8b"
dotnet run
```

### 3. Configuration

The application uses a unified configuration approach. Edit `appsettings.json` or use user secrets:

```json
{
  "LiteLLM": {
    "Model": "gpt-3.5-turbo",
    "ApiKey": "your-api-key-here",
    "MaxTokens": 150,
    "Temperature": 0.7,
    "BaseUrl": "",
    "OllamaUrl": "http://localhost:11434/"
  },
  "ChatBot": {
    "Name": "AI Assistant",
    "WelcomeMessage": "Hello! I'm your AI assistant. How can I help you today?",
    "GoodbyeMessage": "Goodbye! Have a great day!"
  }
}
```

See [LITELLM_CONFIG.md](LITELLM_CONFIG.md) for detailed configuration examples for all providers.

### 4. 🔒 Secure API Key Setup

> **⚠️ SECURITY NOTICE:** Never store API keys directly in `appsettings.json` or commit them to version control!

#### Using User Secrets (Recommended)

1. Initialize user secrets:
   ```bash
   dotnet user-secrets init
   ```

2. Store your API key securely:
   ```bash
   # For any provider
   dotnet user-secrets set "LiteLLM:ApiKey" "your-actual-api-key"
   dotnet user-secrets set "LiteLLM:Model" "your-preferred-model"
   ```

#### Popular API Key Sources

- **OpenAI**: Get from [platform.openai.com](https://platform.openai.com/)
- **Anthropic**: Get from [console.anthropic.com](https://console.anthropic.com/)
- **Google Gemini**: Get from [aistudio.google.com](https://aistudio.google.com/)
- **Ollama**: No API key needed - run locally

#### 📚 For Complete Security Guide
See [SECURITY.md](SECURITY.md) for detailed instructions on:
- User Secrets setup
- Environment variables for production
- Security best practices
- Team collaboration guidelines

**Note:** If you don't configure an API key, the application will automatically use the built-in mock AI service.

## Running the Application

### Option 1: Using .NET CLI

```bash
dotnet run
```

### Option 2: Build and Run

```bash
dotnet build
dotnet run --project AIChatBot.csproj
```

### Option 3: Build and Run Executable

```bash
dotnet publish -c Release -o ./publish
./publish/AIChatBot
```

## Usage

1. Start the application
2. Type your messages and press Enter
3. The AI will respond to your messages
4. Type `exit`, `quit`, `bye`, or `goodbye` to end the conversation

### Example Conversation

```
🤖 AI Assistant
==================================================
Hello! I'm your AI assistant. How can I help you today?
Type 'exit', 'quit', or 'bye' to end the conversation.

You: What is the weather like today?
AI Assistant: I don't have access to real-time weather data, but I hope it's nice where you are! Is there something specific about weather you'd like to discuss?

You: Tell me a joke
AI Assistant: That's an interesting question! Let me think about that... You mentioned: 'Tell me a joke'. What else would you like to know?

You: exit
AI Assistant: Goodbye! Have a great day!
```

## Architecture

The application is built with a clean architecture using:

- **Unified AI Service**: Single LiteLLMService supporting multiple providers
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Configuration**: Microsoft.Extensions.Configuration with secure user secrets
- **Logging**: Microsoft.Extensions.Logging
- **HTTP Client**: Microsoft.Extensions.Http for API calls

### Project Structure

```
AIChatBot/
├── Services/
│   ├── IAIService.cs          # AI service interface
│   ├── LiteLLMService.cs      # Unified multi-provider service
│   ├── MockAIService.cs       # Mock AI fallback service
│   └── ChatBotService.cs      # Main chatbot logic
├── Program.cs                 # Application entry point
├── appsettings.json          # Configuration file
├── LITELLM_CONFIG.md         # Provider configuration examples
├── MODEL_SWITCHING_GUIDE.md  # Quick model switching reference
├── AIChatBot.csproj          # Project file
└── README.md                 # This file
```

## Adding New AI Providers

The unified LiteLLMService makes it easy to add new providers:

1. **Add Provider Detection**: Update `GetProviderFromModel()` method
2. **Add API Implementation**: Add a new `Call[Provider]Async()` method  
3. **Configure HTTP Client**: Add provider-specific headers in `ConfigureHttpClientForProvider()`
4. **Update Documentation**: Add examples to `LITELLM_CONFIG.md`

### Example: Adding a New Provider

```csharp
private async Task<string> CallCustomProviderAsync(string userInput)
{
    var requestBody = new
    {
        model = _model,
        prompt = userInput,
        max_tokens = _maxTokens,
        temperature = _temperature
    };
    
    var json = JsonSerializer.Serialize(requestBody);
    var content = new StringContent(json, Encoding.UTF8, "application/json");
    
    var response = await _httpClient.PostAsync("generate", content);
    // Handle response...
}
```

## Extending the Chatbot

### Switching Models at Runtime

You can easily switch between different models by updating the configuration:

```bash
# Switch to GPT-4
dotnet user-secrets set "LiteLLM:Model" "gpt-4"

# Switch to Claude
dotnet user-secrets set "LiteLLM:Model" "claude-3-sonnet-20240229" 
dotnet user-secrets set "LiteLLM:ApiKey" "your-anthropic-key"

# Switch to local Llama
dotnet user-secrets set "LiteLLM:Model" "llama3.1:8b"
dotnet user-secrets set "LiteLLM:ApiKey" ""
```

### Customizing Responses

- Modify `MockAIService.cs` for local responses
- Adjust system messages in `LiteLLMService.cs` for AI behavior
- Add new conversation logic in `ChatBotService.cs`

## Troubleshooting

### Common Issues

1. **OpenAI API Errors**
   - Verify your API key is correct
   - Check your OpenAI account has available credits
   - Ensure your API key has the necessary permissions

2. **Network Issues**
   - Check your internet connection
   - Verify firewall settings allow outbound HTTPS connections

3. **Configuration Issues**
   - Ensure `appsettings.json` is in the same directory as the executable
   - Verify JSON syntax is correct

### Mock Service Mode

If you see "Using mock AI service (OpenAI not configured)" in the logs, the application is running in mock mode. This happens when:
- No API key is configured
- The API key is set to the placeholder value
- The OpenAI service fails to initialize

## Dependencies

The project uses the following NuGet packages:

- Microsoft.Extensions.Hosting (9.0.8)
- Microsoft.Extensions.Configuration.Json (9.0.8)
- Microsoft.Extensions.Http (9.0.8)
- System.Text.Json (9.0.8)

## License

This project is open source and available under the MIT License.

## Contributing

Feel free to submit issues, feature requests, or pull requests to improve the chatbot!
