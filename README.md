# AI ChatBot

An AI-powered chatbot built with C# that can integrate with OpenAI's GPT models or run with a built-in mock AI service.

## Features

- 🤖 AI-powered responses using OpenAI's GPT models
- 🔄 Fallback to mock AI service when OpenAI is not configured
- ⚙️ Configurable via `appsettings.json`
- 🪵 Structured logging with Microsoft.Extensions.Logging
- 💉 Dependency injection for clean architecture
- 🎨 Colorful console interface
- 🔧 Easy to extend and customize

## Prerequisites

- .NET 8.0 SDK or later
- (Optional) OpenAI API key for full AI functionality

## Setup

### 1. Clone or Download the Project

Ensure you have all the project files in your directory.

### 2. Configure the Application

Edit the `appsettings.json` file to configure the chatbot:

```json
{
  "OpenAI": {
    "ApiKey": "your-openai-api-key-here",
    "Model": "gpt-3.5-turbo",
    "MaxTokens": 150,
    "Temperature": 0.7
  },
  "Gemini": {
    "ApiKey": "your-gemini-api-key-here",
    "Model": "gemini-1.5-flash",
    "MaxTokens": 150,
    "Temperature": 0.7
  },
  "AI": {
    "Provider": "Gemini"
  },
  "ChatBot": {
    "Name": "AI Assistant",
    "WelcomeMessage": "Hello! I'm your AI assistant. How can I help you today?",
    "GoodbyeMessage": "Goodbye! Have a great day!"
  }
}
```

#### Configuration Options

**AI Provider Settings:**
- `Provider`: Choose which AI service to use ("OpenAI" or "Gemini")

**OpenAI Settings:**
- `ApiKey`: Your OpenAI API key (leave as placeholder to use mock service)
- `Model`: The GPT model to use (e.g., "gpt-3.5-turbo", "gpt-4")
- `MaxTokens`: Maximum number of tokens in the response (default: 150)
- `Temperature`: Response creativity (0.0 to 1.0, default: 0.7)

**Gemini Settings:**
- `ApiKey`: Your Google Gemini API key (leave as placeholder to use mock service)
- `Model`: The Gemini model to use (e.g., "gemini-1.5-flash", "gemini-1.5-pro")
- `MaxTokens`: Maximum number of tokens in the response (default: 150)
- `Temperature`: Response creativity (0.0 to 1.0, default: 0.7)

**ChatBot Settings:**
- `Name`: The name displayed for the bot
- `WelcomeMessage`: Message shown when the chatbot starts
- `GoodbyeMessage`: Message shown when exiting

### 3. 🔒 Secure API Key Setup

> **⚠️ SECURITY NOTICE:** Never store API keys directly in `appsettings.json` or commit them to version control!

#### Getting a Google Gemini API Key (Recommended)

1. Visit [Google AI Studio](https://aistudio.google.com/)
2. Sign in with your Google account
3. Click "Get API key" in the left sidebar
4. Create a new API key or use an existing one
5. **Securely store the key using User Secrets** (see below)

#### Secure Storage Using User Secrets (Recommended)

1. Initialize user secrets:
   ```bash
   dotnet user-secrets init
   ```

2. Store your API key securely:
   ```bash
   # For Gemini (recommended)
   dotnet user-secrets set "Gemini:ApiKey" "your-actual-gemini-api-key"
   
   # Or for OpenAI
   dotnet user-secrets set "OpenAI:ApiKey" "your-actual-openai-api-key"
   ```

3. The keys are now stored securely outside your project directory!

#### Getting an OpenAI API Key (Alternative)

1. Visit [OpenAI's website](https://platform.openai.com/)
2. Sign up or log in to your account
3. Navigate to the API section
4. Generate a new API key
5. Store securely: `dotnet user-secrets set "OpenAI:ApiKey" "your-actual-openai-api-key"`
6. Set `"Provider": "OpenAI"` in `appsettings.json`

#### 📚 For Complete Security Guide
See [SECURITY.md](SECURITY.md) for detailed instructions on:
- User Secrets setup
- Environment variables for production
- Security best practices
- Team collaboration guidelines

**Note:** If you don't configure an API key for your chosen provider, the application will automatically use the built-in mock AI service.

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

- **Dependency Injection**: Microsoft.Extensions.DependencyInjection
- **Configuration**: Microsoft.Extensions.Configuration
- **Logging**: Microsoft.Extensions.Logging
- **HTTP Client**: Microsoft.Extensions.Http for API calls

### Project Structure

```
AIChatBot/
├── Services/
│   ├── IAIService.cs          # AI service interface
│   ├── OpenAIService.cs       # OpenAI implementation
│   ├── GeminiService.cs       # Google Gemini implementation
│   ├── MockAIService.cs       # Mock AI implementation
│   └── ChatBotService.cs      # Main chatbot logic
├── Program.cs                 # Application entry point
├── appsettings.json          # Configuration file
├── AIChatBot.csproj          # Project file
└── README.md                 # This file
```

## Extending the Chatbot

### Adding New AI Services

1. Implement the `IAIService` interface
2. Register your service in `Program.cs`
3. Configure any required settings in `appsettings.json`

### Customizing Responses

- Modify `MockAIService.cs` for local responses
- Adjust the system message in `OpenAIService.cs` for OpenAI behavior
- Add new conversation logic in `ChatBotService.cs`

### Adding New Features

The dependency injection setup makes it easy to add new services:

```csharp
services.AddScoped<INewService, NewServiceImplementation>();
```

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
