# HERE AND NOW AI — AI ChatBot (C#)

![HERE AND NOW AI](https://raw.githubusercontent.com/hereandnowai/images/refs/heads/main/logos/logo-of-here-and-now-ai.png)

designed with passion for innovation


This repository contains a production-ready example AI chatbot written in C# (.NET 8.0). The project demonstrates a clean architecture with a single unified LLM service (LiteLLMService) that can target multiple providers: OpenAI, Anthropic Claude, Google Gemini, and local Ollama models. The application also includes a MockAIService for local development and testing.

Table of Contents
- Project overview
- Files and responsibilities
- Branding and contact
- Quick start (clone, configure, run)
- Using GitHub Copilot and the provided prompts
- Provider configuration (OpenAI / Claude / Gemini / Ollama)
- Troubleshooting and tips
- Development notes and how to extend
- License & contribution


## Project overview

Goal: Provide a simple, extensible chatbot console app that lets you switch seamlessly between cloud LLMs and local models using the same codebase.

Key features:
- Unified LiteLLMService to handle provider detection and request/response mapping
- Secure configuration via `dotnet user-secrets`
- Local model support via Ollama (no API key required)
- Mock AI fallback when no provider is configured
- Clear separation of concerns and DI-driven design


## Files and responsibilities

- `Program.cs` — Host and DI setup; chooses `LiteLLMService` or `MockAIService` based on config
- `Services/IAIService.cs` — Interface contract for all AI services (`GetResponseAsync`)
- `Services/LiteLLMService.cs` — Unified multi-provider LLM client (OpenAI, Anthropic, Gemini, Ollama)
- `Services/MockAIService.cs` — Local mock responses for testing and demos
- `Services/ChatBotService.cs` — Console UI, conversation loop, and thinking animation
- `appsettings.json` — Public configuration defaults (do not store secrets here)
- `.github/instructions/copilot-instructions.md` — Copilot developer instructions (created)
- `.github/prompts/chatbot-with-csharp.prompt.md` — Claude Sonnet 4 prompt to recreate the project (created)
- `LITELLM_CONFIG.md` — Configuration examples for each provider
- `MODEL_SWITCHING_GUIDE.md` — Quick reference for switching models
- `branding.json` — Project branding and contact details (HERE AND NOW AI)


## Branding and contact

Organization: HERE AND NOW AI
Website: https://hereandnowai.com
Email: info@hereandnowai.com
Phone: +91 996 296 1000
Slogan: designed with passion for innovation

Follow: https://github.com/hereandnowai


## Quick start (clone, configure, run)

1. Clone the repo:

```bash
git clone https://github.com/hereandnowai/ai-chatbot-csharp.git
cd ai-chatbot-csharp
```

2. Ensure .NET 8.0 SDK is installed (`dotnet --version` >= 8.0)

3. Restore packages and build:

```bash
dotnet restore
dotnet build
```

4. Initialize user secrets (optional but recommended):

```bash
dotnet user-secrets init
```

5. Configure your provider (examples below). For quick testing, run the mock service without keys:

```bash
dotnet run
```

### Configure for Gemini (example)

```bash
dotnet user-secrets set "LiteLLM:Model" "gemini-1.5-flash"
dotnet user-secrets set "LiteLLM:ApiKey" "YOUR_REAL_GEMINI_KEY"
```

Then run:

```bash
dotnet run
```

### Configure for Ollama (local model)

1. Start Ollama and pull a model (example):

```bash
ollama pull llama3.1:8b
```

2. Set model and clear API key:

```bash
dotnet user-secrets set "LiteLLM:Model" "llama3.1:8b"
dotnet user-secrets set "LiteLLM:ApiKey" ""
```

3. Run the app:

```bash
dotnet run
```


## Using GitHub Copilot and the provided prompts

This repository includes two artifacts for AI-assisted development:

- `.github/instructions/copilot-instructions.md` — Use this file to guide GitHub Copilot when making code changes. It contains development rules, architecture overview, and implementation guidelines.

- `.github/prompts/chatbot-with-csharp.prompt.md` — A long-form prompt designed for Claude Sonnet 4 to recreate the project from scratch. Use this prompt as input to an LLM to scaffold or refactor the code.

How to use the prompt with Copilot (conceptual):
1. Open the `.github/prompts/chatbot-with-csharp.prompt.md` file and copy the prompt.
2. Provide it to a model (Claude Sonnet 4) that can accept long prompts (via API or web UI).
3. Ask the model to generate files, tests, and a runnable project following the prompt.


## Provider configuration (examples)

`appsettings.json` holds defaults; use user secrets to store API keys.

```json
{
  "LiteLLM": {
    "Model": "gpt-3.5-turbo",
    "ApiKey": "your-api-key-here",
    "MaxTokens": 150,
    "Temperature": 0.7,
    "BaseUrl": "",
    "OllamaUrl": "http://localhost:11434/"
  }
}
```

- OpenAI: set `LiteLLM:Model` to `gpt-4` and `LiteLLM:ApiKey` to your OpenAI key
- Anthropic: set `LiteLLM:Model` to `claude-3-sonnet-20240229` and `LiteLLM:ApiKey` to your Anthropic key
- Gemini: set `LiteLLM:Model` to `gemini-1.5-flash` and `LiteLLM:ApiKey` to your Gemini key
- Ollama/local: set `LiteLLM:Model` to a local model such as `llama3.1:8b` and `LiteLLM:ApiKey` to an empty string


## Troubleshooting and tips

- If you see `"I'm sorry, I'm having trouble connecting to my AI service right now"`, verify `LiteLLM:ApiKey` in user secrets and ensure the correct `LiteLLM:Model` is set.
- For Ollama issues, verify `http://localhost:11434/api/tags` returns available models and that the model name matches exactly.
- Use `dotnet user-secrets list` to confirm runtime configuration


## Development notes and how to extend

- To add a new provider, update `LiteLLMService.GetProviderFromModel()` and add a provider-specific call method and HTTP configuration.
- Use `MockAIService` for unit tests to avoid external API calls.
- For streaming responses or function calling, enhance `LiteLLMService` with streaming handlers and tool interfaces.


## License & contribution

MIT License — contributions welcome. Please open issues or PRs on GitHub: https://github.com/hereandnowai/ai-chatbot-csharp


---

Built by HERE AND NOW AI — https://hereandnowai.com
Contact: info@hereandnowai.com
