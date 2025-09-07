# GitHub Copilot Workspace Instructions

## Project Overview

This is a sophisticated AI-powered chatbot application built with C# .NET 8.0 that demonstrates modern software architecture patterns and multi-provider AI integration. The project uses a unified LiteLLM service to support multiple AI providers including OpenAI, Anthropic Claude, Google Gemini, and local Ollama models.

## Architecture Summary

### Core Design Principles
- **Unified Interface**: Single `IAIService` interface for all AI providers
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection container
- **Configuration Management**: JSON configuration with secure user secrets
- **Provider Abstraction**: Automatic provider detection based on model names
- **Fallback Strategy**: Mock service when no API configuration is available
- **Clean Separation**: Each service has distinct responsibilities

- [x] Customize the Project
	<!--
	Verify that all previous steps have been completed successfully and you have marked the step as completed.
### Project Structure
```
AIChatBot/
├── Program.cs                          # Entry point with DI setup
├── Services/
│   ├── IAIService.cs                   # Common interface for all AI services
│   ├── LiteLLMService.cs              # Unified multi-provider AI service  
│   ├── ChatBotService.cs              # Main conversation management
│   └── MockAIService.cs               # Fallback/testing service
├── appsettings.json                    # Configuration file
├── .github/
│   ├── instructions/
│   │   └── copilot-instructions.md    # Detailed development guidelines
│   └── prompts/
│       └── chatbot-with-csharp.prompt.md # Replication instructions
└── Documentation/
    ├── README.md                       # Main project documentation
    ├── LITELLM_CONFIG.md              # Provider configuration guide
    └── MODEL_SWITCHING_GUIDE.md       # Quick model switching reference
```

## Supported AI Providers

| Provider | Models | Authentication | Status |
|----------|--------|---------------|---------|
| **OpenAI** | gpt-3.5-turbo, gpt-4, o1-* | Bearer Token | ✅ Active |
| **Anthropic** | claude-3-haiku, claude-3-sonnet, claude-3-opus | API Key Header | ✅ Active |
| **Google Gemini** | gemini-1.5-pro, gemini-1.5-flash | Query Parameter | ✅ Active |
| **Ollama** | llama3.1:8b, mistral:7b, deepseek-coder:6.7b, qwen2:7b | None (Local) | ✅ Active |
| **Mock Service** | Predefined responses | None | ✅ Fallback |

## Key Technologies

### Dependencies
- **Microsoft.Extensions.Hosting** (v9.0.8) - Application hosting and DI
- **Microsoft.Extensions.Http** (v9.0.8) - HTTP client factory
- **Microsoft.Extensions.Configuration.Json** (v9.0.8) - JSON configuration
- **Microsoft.Extensions.Configuration.UserSecrets** (v9.0.8) - Secure configuration
- **System.Text.Json** (v9.0.8) - JSON serialization

### Design Patterns
- **Dependency Injection**: Constructor injection for all services
- **Strategy Pattern**: Provider-specific implementations via interface
- **Factory Pattern**: HTTP client factory for resource management
- **Configuration Pattern**: Strongly-typed configuration binding
- **Fallback Pattern**: Graceful degradation to mock service

## Configuration System

### User Secrets (Secure)
```bash
dotnet user-secrets set "LiteLLM:Model" "gemini-1.5-flash"
dotnet user-secrets set "LiteLLM:ApiKey" "your-actual-api-key"
```

### appsettings.json (Public)
```json
{
    "LiteLLM": {
        "Model": "gpt-3.5-turbo",
        "MaxTokens": 150,
        "Temperature": 0.7,
        "OllamaUrl": "http://localhost:11434/"
    },
    "ChatBot": {
        "Name": "AI Assistant",
        "WelcomeMessage": "Hello! I'm your AI assistant. How can I help you today?",
        "GoodbyeMessage": "Goodbye! Have a great day!"
    }
}
```

## Quick Start Commands

### Development Setup
```bash
# Clone and setup
git clone <repository-url>
cd ai-chatbot-csharp
dotnet restore

# Test with mock service (no API key needed)
dotnet run

# Configure for Gemini
dotnet user-secrets set "LiteLLM:Model" "gemini-1.5-flash"
dotnet user-secrets set "LiteLLM:ApiKey" "your-gemini-key"

# Configure for OpenAI
dotnet user-secrets set "LiteLLM:Model" "gpt-4"
dotnet user-secrets set "LiteLLM:ApiKey" "your-openai-key"

# Configure for Ollama (local)
dotnet user-secrets set "LiteLLM:Model" "llama3.1:8b" 
dotnet user-secrets set "LiteLLM:ApiKey" ""
```

### Debugging
```bash
# Check configuration
dotnet user-secrets list

# Build and test
dotnet build && dotnet run

# View available Ollama models
curl -s http://localhost:11434/api/tags
```

## Service Implementation Details

### LiteLLMService (Core AI Integration)
- **Provider Detection**: Automatic based on model name patterns
- **HTTP Configuration**: Dynamic base URLs and headers per provider
- **Error Handling**: User-friendly messages for connection issues
- **Response Parsing**: Provider-specific JSON structure handling
- **Logging**: Comprehensive debug information without sensitive data

### ChatBotService (User Interaction)
- **Console Interface**: Colorful output with thinking animations
- **Input Handling**: Graceful exit commands (exit, quit, bye)
- **Error Display**: User-friendly error messages
- **Conversation Flow**: Continuous chat loop with proper cleanup

### MockAIService (Fallback/Testing)
- **Contextual Responses**: Pattern-based response selection
- **No Dependencies**: Works without internet or API keys
- **Development Aid**: Useful for testing and demonstrations

## Troubleshooting Guide

### Common Issues

**"I'm sorry, I'm having trouble connecting..."**
- Check API key in user secrets: `dotnet user-secrets list`
- Verify internet connectivity for cloud providers
- Confirm model name matches provider pattern
- Check provider service availability

**Mock Service Used Instead of Real Provider**
- Verify API key is properly set in user secrets
- Check model name follows correct naming convention
- Ensure no conflicts in appsettings.json

**Ollama Models Not Working**
- Confirm Ollama is running: `curl http://localhost:11434/api/tags`
- Verify model is installed: `ollama list`
- Check model name matches exactly
- Ensure API key is empty for Ollama models

### Debug Information
- Provider selection logged at startup
- HTTP request/response details available in debug logs
- Configuration validation on application start
- User-friendly error messages with troubleshooting hints

## Development Guidelines

### Code Style
- C# 12 features with nullable reference types
- Async/await for all I/O operations
- Proper resource disposal (using statements, IDisposable)
- Comprehensive error handling with logging

### Security
- API keys only in user secrets, never in code
- Input validation before sending to AI providers
- No sensitive data in logs or error messages
- Secure HTTP client configuration

### Performance
- HttpClientFactory for connection pooling
- Efficient JSON parsing with System.Text.Json
- Proper timeout configuration (30s default)
- Memory-efficient response handling

## Extension Points

### Adding New Providers
1. Update `GetProviderFromModel()` with new model patterns
2. Add HTTP configuration in `ConfigureHttpClientForProvider()`
3. Implement request/response handling in provider-specific method
4. Add documentation and examples

### Adding Features
- **Conversation History**: Store context across messages
- **Streaming Responses**: Real-time response display
- **Function Calling**: Tool integration for specific providers
- **Usage Analytics**: Token usage and cost tracking

## Testing Strategy

### Manual Testing
- Test each provider with valid API keys
- Test fallback to mock service
- Test configuration edge cases
- Test network error scenarios

### Automated Testing
- Unit tests for provider detection logic
- Integration tests for HTTP communication
- Configuration validation tests
- Mock service response tests

## Documentation Structure

- **README.md**: User-facing setup and usage guide
- **LITELLM_CONFIG.md**: Detailed provider configuration examples
- **MODEL_SWITCHING_GUIDE.md**: Quick reference for model switching
- **.github/instructions/copilot-instructions.md**: Development guidelines
- **.github/prompts/chatbot-with-csharp.prompt.md**: AI replication instructions

## Project Goals Achieved

✅ **Multi-Provider Support**: OpenAI, Anthropic, Gemini, Ollama integration
✅ **Unified Architecture**: Single interface for all providers  
✅ **Security**: API keys in user secrets, no hardcoded credentials
✅ **User Experience**: Colorful console with helpful error messages
✅ **Fallback System**: Mock service when no configuration available
✅ **Documentation**: Comprehensive guides for setup and usage
✅ **Extensibility**: Easy to add new providers or features
✅ **Modern C#**: .NET 8.0 with current best practices

This chatbot project demonstrates enterprise-grade C# development with clean architecture, comprehensive error handling, and flexible AI provider integration suitable for production use or educational purposes.
