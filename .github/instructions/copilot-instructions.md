# GitHub Copilot Instructions for AI ChatBot Project

This file provides comprehensive instructions for GitHub Copilot to understand and work with this AI-powered chatbot project built with C# and .NET 8.0.

## Project Overview

This is a sophisticated AI chatbot application that demonstrates modern C# development practices with multi-provider AI integration. The project uses a unified LiteLLM service to support multiple AI providers including OpenAI, Anthropic Claude, Google Gemini, and local Ollama models.

## Architecture Guidelines

### Core Principles
- **Dependency Injection**: Use Microsoft.Extensions.DependencyInjection for all services
- **Configuration Management**: Leverage appsettings.json and user secrets for configuration
- **Separation of Concerns**: Each service has a single responsibility
- **Provider Abstraction**: All AI providers implement the same IAIService interface
- **Fallback Strategy**: Mock service available when no API keys are configured
- **Secure Configuration**: API keys stored in user secrets, never in code

### Project Structure
```
AIChatBot/
├── Program.cs                          # Entry point with DI configuration
├── Services/
│   ├── IAIService.cs                   # Common interface for all AI services
│   ├── LiteLLMService.cs              # Unified multi-provider AI service
│   ├── ChatBotService.cs              # Main conversation logic
│   └── MockAIService.cs               # Fallback service for testing
├── appsettings.json                    # Configuration file
├── AIChatBot.csproj                    # Project file with dependencies
└── README.md                          # Project documentation
```

## Key Technologies and Patterns

### Dependencies (NuGet Packages)
- `Microsoft.Extensions.Hosting` (v9.0.8) - Host builder and DI container
- `Microsoft.Extensions.Http` (v9.0.8) - HTTP client factory
- `Microsoft.Extensions.Configuration.Json` (v9.0.8) - JSON configuration
- `Microsoft.Extensions.Configuration.UserSecrets` (v9.0.8) - Secure configuration
- `System.Text.Json` (v9.0.8) - JSON serialization

### Design Patterns Used
1. **Dependency Injection Pattern**: All services injected via constructor
2. **Strategy Pattern**: Different AI providers through IAIService interface
3. **Factory Pattern**: HttpClient factory for HTTP requests
4. **Configuration Pattern**: Strongly typed configuration objects
5. **Fallback Pattern**: Mock service when real providers unavailable

## Service Implementation Guidelines

### IAIService Interface
```csharp
public interface IAIService
{
    Task<string> GetResponseAsync(string userInput);
}
```

### LiteLLMService Implementation Details
- **Multi-Provider Support**: Automatically detects provider from model name
- **Dynamic Configuration**: Base URLs and headers configured per provider
- **Error Handling**: Graceful fallback with user-friendly error messages
- **HTTP Client Management**: Uses IHttpClientFactory for proper resource management
- **JSON Processing**: Handles different response formats from various providers

### Provider Detection Logic
```csharp
private string GetProviderFromModel(string model)
{
    if (model.StartsWith("gpt-") || model.StartsWith("o1-")) return "openai";
    if (model.StartsWith("claude-")) return "anthropic";  
    if (model.StartsWith("gemini-")) return "gemini";
    if (model.StartsWith("llama") || model.StartsWith("mistral") || 
        model.StartsWith("deepseek") || model.StartsWith("qwen")) return "ollama";
    return "custom";
}
```

## Configuration System

### appsettings.json Structure
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

### User Secrets Configuration
- `LiteLLM:Model` - The AI model to use
- `LiteLLM:ApiKey` - API key for cloud providers
- Store sensitive data only in user secrets, never in appsettings.json

## Supported AI Providers

### 1. OpenAI
- **Models**: gpt-3.5-turbo, gpt-4, gpt-4-turbo, o1-preview, o1-mini
- **Endpoint**: https://api.openai.com/v1/
- **Authentication**: Bearer token in Authorization header
- **Configuration**: Set model to any gpt- or o1- prefixed model

### 2. Anthropic Claude
- **Models**: claude-3-haiku-20240307, claude-3-sonnet-20240229, claude-3-opus-20240229
- **Endpoint**: https://api.anthropic.com/v1/
- **Authentication**: x-api-key header + anthropic-version header
- **Configuration**: Set model to any claude- prefixed model

### 3. Google Gemini
- **Models**: gemini-1.5-pro, gemini-1.5-flash, gemini-pro
- **Endpoint**: https://generativelanguage.googleapis.com/v1beta/
- **Authentication**: API key as query parameter
- **Configuration**: Set model to any gemini- prefixed model

### 4. Ollama (Local Models)
- **Models**: llama3.1:8b, mistral:7b, deepseek-coder:6.7b, qwen2:7b, etc.
- **Endpoint**: http://localhost:11434/ (configurable)
- **Authentication**: None (local service)
- **Configuration**: Set model to any locally available model name

## Error Handling Strategy

### Connection Errors
- Return user-friendly message: "I'm sorry, I'm having trouble connecting to my AI service right now. Please try again later."
- Log detailed error information for debugging
- Never expose API keys or internal errors to users

### Validation Errors
- Check for empty/null inputs before API calls
- Validate configuration on startup
- Provide helpful error messages for misconfiguration

### Rate Limiting
- Handle HTTP 429 responses gracefully
- Implement exponential backoff for retries (if needed)
- Inform users about temporary unavailability

## Development Guidelines

### Code Style
- Use C# 12 features and nullable reference types
- Follow Microsoft C# coding conventions
- Use async/await for all I/O operations
- Implement proper disposal patterns for HttpClient

### Testing Strategy
- Mock service for testing without API calls
- Unit tests for service logic
- Integration tests for provider communication
- Configuration validation tests

### Security Best Practices
- Never commit API keys to source control
- Use user secrets for development
- Use environment variables or key vaults in production
- Validate all user inputs before processing

## Debugging and Logging

### Logging Configuration
```csharp
"Logging": {
    "LogLevel": {
        "Default": "Information",
        "Microsoft.Extensions.Http": "Warning"
    }
}
```

### Debug Information
- Log provider selection and model usage
- Log HTTP request/response details (without sensitive data)
- Track token usage and response times
- Monitor error rates and patterns

## Extension Points

### Adding New Providers
1. Add provider detection logic in `GetProviderFromModel()`
2. Add HTTP client configuration in `ConfigureHttpClientForProvider()`
3. Implement provider-specific request/response handling
4. Add provider documentation and examples

### Adding New Features
- **Conversation History**: Store and manage conversation context
- **Streaming Responses**: Implement real-time response streaming
- **Function Calling**: Add support for tool/function calling
- **Response Caching**: Cache responses for identical queries
- **Usage Analytics**: Track token usage and costs

## Performance Considerations

### HTTP Client Management
- Use IHttpClientFactory to avoid socket exhaustion
- Configure appropriate timeouts for different providers
- Implement connection pooling and reuse

### Memory Management
- Dispose of HttpClient responses properly
- Use streaming for large responses
- Implement response size limits

### Scalability
- Design for horizontal scaling
- Use configuration providers for dynamic updates
- Implement health checks for dependencies

## Troubleshooting Common Issues

### "I'm sorry, I'm having trouble connecting..."
- Check API key configuration in user secrets
- Verify internet connectivity
- Confirm provider service is available
- Check model name is correct for the provider

### Mock Service Being Used Instead of Real Provider
- Verify API key is set in user secrets
- Check model name matches expected provider pattern
- Ensure no configuration conflicts in appsettings.json

### Ollama Not Working
- Confirm Ollama service is running on localhost:11434
- Verify model is downloaded: `ollama list`
- Check model name matches exactly
- Ensure no API key is set for Ollama models

## Quick Commands for Development

### Setup Commands
```bash
# Clone and setup
git clone <repository-url>
cd ai-chatbot-csharp
dotnet restore

# Configure for OpenAI
dotnet user-secrets set "LiteLLM:Model" "gpt-4"
dotnet user-secrets set "LiteLLM:ApiKey" "your-openai-key"

# Configure for Gemini
dotnet user-secrets set "LiteLLM:Model" "gemini-1.5-flash"
dotnet user-secrets set "LiteLLM:ApiKey" "your-gemini-key"

# Configure for Ollama
dotnet user-secrets set "LiteLLM:Model" "llama3.1:8b"
dotnet user-secrets set "LiteLLM:ApiKey" ""

# Run the application
dotnet run
```

### Debugging Commands
```bash
# Check current configuration
dotnet user-secrets list

# View logs with debug info
dotnet run 2>&1 | grep -E "(DEBUG|ERROR|WARNING)"

# Test specific provider API directly
curl -X POST https://api.openai.com/v1/chat/completions \
  -H "Authorization: Bearer YOUR_KEY" \
  -H "Content-Type: application/json" \
  -d '{"model":"gpt-3.5-turbo","messages":[{"role":"user","content":"test"}]}'
```

## When Working with This Codebase

### Understanding the Flow
1. **Program.cs**: Sets up DI container and determines which service to use
2. **ChatBotService**: Manages conversation loop and user interaction
3. **LiteLLMService**: Handles actual AI provider communication
4. **Configuration**: Model and API key determine provider and behavior

### Making Changes
- Always maintain backward compatibility with existing configuration
- Add new features through the IAIService interface
- Update documentation when adding new providers or features
- Test with multiple providers to ensure consistency

### Best Practices for AI Integration
- Handle provider-specific quirks in LiteLLMService
- Maintain consistent response format across providers
- Implement proper timeout and retry logic
- Monitor token usage and costs
- Respect rate limits and usage policies

This project demonstrates enterprise-grade C# development with modern patterns, comprehensive error handling, and flexible architecture for AI integration. When extending or modifying the code, maintain these standards and patterns for consistency and maintainability.
