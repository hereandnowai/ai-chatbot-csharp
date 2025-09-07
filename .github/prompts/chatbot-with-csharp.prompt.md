# AI ChatBot Development Prompt for Claude Sonnet 4

This prompt is designed to guide an AI assistant (Claude Sonnet 4) through creating a complete AI-powered chatbot application using C# and .NET 8.0. The chatbot should support multiple AI providers through a unified interface.

## Project Requirements

### Primary Objective
Create a professional-grade AI chatbot console application in C# that can interact with multiple AI providers (OpenAI, Anthropic Claude, Google Gemini, and local Ollama models) through a single unified interface.

### Core Specifications

#### Technology Stack
- **Framework**: .NET 8.0 Console Application
- **Language**: C# 12 with nullable reference types enabled
- **Architecture**: Dependency Injection with Microsoft.Extensions.Hosting
- **Configuration**: JSON configuration with user secrets support
- **HTTP**: HttpClientFactory for proper resource management
- **Logging**: Microsoft.Extensions.Logging

#### Required NuGet Packages
```xml
<PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="9.0.8" />
<PackageReference Include="Microsoft.Extensions.Configuration.UserSecrets" Version="9.0.8" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.8" />
<PackageReference Include="Microsoft.Extensions.Http" Version="9.0.8" />
<PackageReference Include="System.Text.Json" Version="9.0.8" />
```

### Architecture Requirements

#### 1. Service Interface Design
Create an `IAIService` interface that all AI providers must implement:
```csharp
public interface IAIService
{
    Task<string> GetResponseAsync(string userInput);
}
```

#### 2. Core Services to Implement

**LiteLLMService.cs** - The unified AI service that:
- Automatically detects which provider to use based on model name
- Configures HTTP client settings per provider
- Handles provider-specific request/response formats
- Implements proper error handling and fallback messages
- Supports these providers:
  - OpenAI (models: gpt-*, o1-*)
  - Anthropic (models: claude-*)
  - Google Gemini (models: gemini-*)
  - Ollama (models: llama*, mistral*, deepseek*, qwen*, etc.)

**ChatBotService.cs** - The main conversation manager that:
- Handles user input/output with colorful console interface
- Manages conversation flow and exit conditions
- Provides "thinking" animations during API calls
- Implements proper error handling for user experience

**MockAIService.cs** - A fallback service that:
- Provides predefined responses when no API key is configured
- Helps with testing and demonstration
- Returns contextual responses based on input patterns

#### 3. Provider-Specific Implementation Details

**OpenAI Integration:**
- Endpoint: `https://api.openai.com/v1/chat/completions`
- Authentication: Bearer token in Authorization header
- Request format: Standard OpenAI chat completions API
- Models: Support gpt-3.5-turbo, gpt-4, gpt-4-turbo, o1-preview, o1-mini

**Anthropic Integration:**
- Endpoint: `https://api.anthropic.com/v1/messages`
- Authentication: x-api-key header + anthropic-version: 2023-06-01
- Request format: Anthropic messages API
- Models: Support claude-3-haiku, claude-3-sonnet, claude-3-opus

**Google Gemini Integration:**
- Endpoint: `https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent`
- Authentication: API key as query parameter
- Request format: Gemini generate content API
- Models: Support gemini-1.5-pro, gemini-1.5-flash, gemini-pro

**Ollama Integration:**
- Endpoint: `http://localhost:11434/api/chat`
- Authentication: None (local service)
- Request format: Ollama chat API
- Models: Support any locally installed model (llama3.1:8b, mistral:7b, etc.)

### Configuration System

#### appsettings.json Structure
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
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.Extensions.Http": "Warning"
        }
    },
    "ChatBot": {
        "Name": "AI Assistant",
        "WelcomeMessage": "Hello! I'm your AI assistant. How can I help you today?",
        "GoodbyeMessage": "Goodbye! Have a great day!"
    }
}
```

#### User Secrets Configuration
- Use `dotnet user-secrets` for storing API keys securely
- Configure: `LiteLLM:Model` and `LiteLLM:ApiKey`
- Never store API keys in source code or appsettings.json

### Program.cs Requirements

#### Dependency Injection Setup
```csharp
var builder = Host.CreateApplicationBuilder(args);

// Add configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddUserSecrets<Program>();

// Register services
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IAIService>(serviceProvider => {
    // Logic to choose between LiteLLMService and MockAIService
    // based on configuration availability
});
builder.Services.AddSingleton<ChatBotService>();

var host = builder.Build();
```

#### Service Selection Logic
Implement logic to automatically choose between real AI service and mock service:
- Use LiteLLMService when API key is configured for the chosen model
- Use MockAIService when no API key is available
- Special handling for Ollama models (no API key required)

### User Experience Requirements

#### Console Interface
- Colorful output using Console.ForegroundColor
- Clear welcome message and instructions
- "AI Assistant is thinking..." animation during API calls
- Proper formatting for user input and AI responses
- Graceful exit on commands: "exit", "quit", "bye"

#### Error Handling
- Network errors: "I'm sorry, I'm having trouble connecting to my AI service right now. Please try again later."
- Invalid configuration: Clear instructions on how to set up API keys
- Rate limiting: Appropriate user-facing messages
- Never expose internal errors, API keys, or technical details to users

### Provider Detection Algorithm

Implement automatic provider detection based on model names:
```csharp
private string GetProviderFromModel(string model)
{
    if (model.StartsWith("gpt-") || model.StartsWith("o1-")) return "openai";
    if (model.StartsWith("claude-")) return "anthropic";
    if (model.StartsWith("gemini-")) return "gemini";
    if (model.StartsWith("llama") || model.StartsWith("mistral") || 
        model.StartsWith("deepseek") || model.StartsWith("qwen") || 
        model.Contains("local")) return "ollama";
    return "custom";
}
```

### Advanced Features to Implement

#### HTTP Client Configuration
- Provider-specific base URLs and headers
- Proper timeout configuration
- Connection pooling and reuse
- Request/response logging for debugging

#### JSON Response Parsing
Each provider returns different JSON structures. Implement parsing for:
- OpenAI: `choices[0].message.content`
- Anthropic: `content[0].text`
- Gemini: `candidates[0].content.parts[0].text`
- Ollama: `message.content`

#### Fallback Strategy
- Primary: Use configured AI provider
- Secondary: Use mock service if API fails
- Tertiary: Graceful error messages for users

### Testing Strategy

#### Mock Service Responses
Implement contextual mock responses:
- Greetings: Friendly welcome messages
- Questions: Helpful placeholder answers
- Code requests: Basic code examples
- General: "I'm a mock AI service" with helpful suggestions

#### Configuration Testing
- Test with valid API keys for each provider
- Test with invalid/missing API keys
- Test with different model configurations
- Test Ollama integration with local models

### Documentation Requirements

Create comprehensive documentation:
1. **README.md** - Setup instructions, examples, troubleshooting
2. **LITELLM_CONFIG.md** - Detailed configuration examples for each provider
3. **MODEL_SWITCHING_GUIDE.md** - Quick reference for switching between models
4. **Comments in code** - Explain complex logic and provider-specific handling

### Security Considerations

#### API Key Management
- Use user secrets in development
- Never commit API keys to source control
- Provide clear instructions for secure configuration
- Validate API keys before making requests

#### Input Validation
- Sanitize user inputs before sending to AI providers
- Implement reasonable length limits
- Handle special characters and edge cases

### Performance Requirements

#### Response Time
- Display "thinking" animation for requests taking >1 second
- Implement reasonable timeouts (30 seconds for API calls)
- Use async/await properly throughout the application

#### Resource Management
- Properly dispose HttpClient responses
- Use HttpClientFactory to avoid socket exhaustion
- Implement memory-efficient JSON parsing

### Extensibility Design

#### Adding New Providers
The architecture should make it easy to add new AI providers by:
- Adding provider detection logic
- Implementing provider-specific HTTP configuration
- Adding request/response format handling
- Updating documentation and examples

#### Configuration Flexibility
- Support custom base URLs for compatible APIs
- Allow override of default settings per provider
- Support multiple models per provider

### Success Criteria

The completed application should:

1. **Run out of the box** with mock service (no API key required)
2. **Switch providers easily** by changing model name and API key
3. **Handle errors gracefully** with user-friendly messages
4. **Provide consistent experience** regardless of AI provider
5. **Include comprehensive documentation** with setup examples
6. **Follow C# best practices** with proper DI, async/await, and error handling
7. **Support all specified providers** with their respective authentication methods
8. **Include debug logging** for troubleshooting without exposing sensitive data

### Implementation Order

1. **Project Setup** - Create console app with required NuGet packages
2. **Core Interfaces** - Define IAIService and basic service contracts
3. **Configuration System** - Set up appsettings.json and user secrets
4. **Mock Service** - Implement fallback service for testing
5. **LiteLLM Service** - Implement unified multi-provider service
6. **Chat Service** - Implement user interaction and conversation flow
7. **Program.cs** - Wire up dependency injection and service selection
8. **Testing** - Test with multiple providers and configurations
9. **Documentation** - Create comprehensive setup and usage guides
10. **Refinement** - Polish error handling, user experience, and performance

### Example Usage Scenarios

#### Scenario 1: Quick Start (No API Key)
```bash
git clone <repository>
cd ai-chatbot-csharp
dotnet run
# Should work immediately with mock service
```

#### Scenario 2: OpenAI Integration
```bash
dotnet user-secrets set "LiteLLM:Model" "gpt-4"
dotnet user-secrets set "LiteLLM:ApiKey" "sk-..."
dotnet run
# Should connect to OpenAI GPT-4
```

#### Scenario 3: Local Ollama
```bash
# Assuming Ollama is running with llama3.1:8b installed
dotnet user-secrets set "LiteLLM:Model" "llama3.1:8b"
dotnet user-secrets set "LiteLLM:ApiKey" ""
dotnet run
# Should connect to local Ollama service
```

### Code Quality Standards

- **Nullable Reference Types**: Enable and handle properly
- **Async/Await**: Use consistently for all I/O operations
- **Error Handling**: Comprehensive try-catch with appropriate logging
- **Code Comments**: Document complex logic and provider-specific quirks
- **Separation of Concerns**: Each class has a single responsibility
- **SOLID Principles**: Follow dependency inversion and interface segregation
- **Performance**: Efficient HTTP client usage and JSON parsing

This prompt should guide the AI assistant to create a production-ready, extensible, and user-friendly AI chatbot application that demonstrates modern C# development practices while providing a seamless experience across multiple AI providers.
