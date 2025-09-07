# LiteLLM Configuration Examples

This project uses a unified LiteLLM service that supports multiple AI providers. You can easily switch between different LLM models by updating the configuration.

## Supported Providers

- **OpenAI** (GPT-3.5, GPT-4, o1 models)
- **Anthropic** (Claude models)
- **Google Gemini** (Gemini models)
- **Ollama** (Local models: Llama, Mistral, DeepSeek, Qwen, etc.)
- **Custom endpoints** (Any OpenAI-compatible API)

## Configuration Examples

### 1. OpenAI GPT-4
```json
{
  "LiteLLM": {
    "Model": "gpt-4",
    "ApiKey": "your-openai-api-key",
    "MaxTokens": 150,
    "Temperature": 0.7
  }
}
```

### 2. Anthropic Claude
```json
{
  "LiteLLM": {
    "Model": "claude-3-sonnet-20240229",
    "ApiKey": "your-anthropic-api-key",
    "MaxTokens": 150,
    "Temperature": 0.7
  }
}
```

### 3. Google Gemini
```json
{
  "LiteLLM": {
    "Model": "gemini-1.5-pro",
    "ApiKey": "your-gemini-api-key",
    "MaxTokens": 150,
    "Temperature": 0.7
  }
}
```

### 4. Ollama Local Models
```json
{
  "LiteLLM": {
    "Model": "llama3.1:8b",
    "ApiKey": "",
    "MaxTokens": 150,
    "Temperature": 0.7,
    "OllamaUrl": "http://localhost:11434/"
  }
}
```

### 5. Custom OpenAI-compatible endpoint
```json
{
  "LiteLLM": {
    "Model": "your-custom-model",
    "ApiKey": "your-api-key",
    "MaxTokens": 150,
    "Temperature": 0.7,
    "BaseUrl": "https://your-custom-endpoint.com/v1/"
  }
}
```

## Secure API Key Storage

### Using User Secrets (Recommended)
```bash
# Set your API key securely
dotnet user-secrets set "LiteLLM:ApiKey" "your-actual-api-key"
dotnet user-secrets set "LiteLLM:Model" "gpt-4"
```

### Using Environment Variables
```bash
export LiteLLM__ApiKey="your-actual-api-key"
export LiteLLM__Model="claude-3-sonnet-20240229"
```

## Popular Models to Try

### OpenAI
- `gpt-3.5-turbo` - Fast and cost-effective
- `gpt-4` - More capable, higher quality
- `gpt-4-turbo` - Latest GPT-4 with larger context
- `o1-preview` - Advanced reasoning model

### Anthropic
- `claude-3-haiku-20240307` - Fast and efficient
- `claude-3-sonnet-20240229` - Balanced performance
- `claude-3-opus-20240229` - Most capable

### Gemini
- `gemini-1.5-flash` - Fast and efficient
- `gemini-1.5-pro` - High performance

### Ollama (Local)
- `llama3.1:8b` - Meta's Llama 3.1 8B
- `mistral:7b` - Mistral 7B
- `deepseek-coder:6.7b` - DeepSeek Coder
- `qwen2:7b` - Alibaba's Qwen2
- `codellama:7b` - Code-specialized Llama

## Quick Setup

1. Choose your preferred model from the examples above
2. Update `appsettings.json` or use user secrets
3. Run the application: `dotnet run`

## Switching Models on the Fly

You can switch models by updating the configuration and restarting the application. The service automatically detects the provider based on the model name and configures the appropriate API endpoints.
