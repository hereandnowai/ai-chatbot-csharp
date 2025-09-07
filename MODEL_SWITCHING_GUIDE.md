# Quick Model Switching Guide

## Your Available Models

### 🌐 Cloud Models (API Key Required)
- **Gemini 1.5 Flash** - Fast and efficient Google model
- **Gemini 1.5 Pro** - High-performance Google model

### 🏠 Local Models (Ollama - No API Key Required)
- **gpt-oss:20b** (13 GB) - Large GPT-style model
- **llama3.1:8b** (4.9 GB) - Meta's Llama 3.1
- **deepseek-coder:6.7b** (3.8 GB) - DeepSeek Coder (great for programming)
- **stable-code:3b-code-q4_0** (1.6 GB) - Stable Code (lightweight coding model)
- **qwen3:8b** (5.2 GB) - Alibaba's Qwen3
- **deepseek-r1:latest** (5.2 GB) - DeepSeek R1

## Quick Switch Commands

### Switch to Gemini 1.5 Flash (Cloud)
```bash
dotnet user-secrets set "LiteLLM:Model" "gemini-1.5-flash"
dotnet user-secrets set "LiteLLM:ApiKey" "YOUR_GEMINI_API_KEY"
```

### Switch to Gemini 1.5 Pro (Cloud)
```bash
dotnet user-secrets set "LiteLLM:Model" "gemini-1.5-pro" 
dotnet user-secrets set "LiteLLM:ApiKey" "YOUR_GEMINI_API_KEY"
```

### Switch to Llama 3.1 (Local)
```bash
dotnet user-secrets set "LiteLLM:Model" "llama3.1:8b"
dotnet user-secrets set "LiteLLM:ApiKey" ""
```

### Switch to DeepSeek Coder (Local - Best for Programming)
```bash
dotnet user-secrets set "LiteLLM:Model" "deepseek-coder:6.7b"
dotnet user-secrets set "LiteLLM:ApiKey" ""
```

### Switch to GPT-OSS (Local - Largest Model)
```bash
dotnet user-secrets set "LiteLLM:Model" "gpt-oss:20b"
dotnet user-secrets set "LiteLLM:ApiKey" ""
```

### Switch to Qwen3 (Local)
```bash
dotnet user-secrets set "LiteLLM:Model" "qwen3:8b"
dotnet user-secrets set "LiteLLM:ApiKey" ""
```

### Switch to DeepSeek R1 (Local)
```bash
dotnet user-secrets set "LiteLLM:Model" "deepseek-r1:latest"
dotnet user-secrets set "LiteLLM:ApiKey" ""
```

### Switch to Stable Code (Local - Lightweight)
```bash
dotnet user-secrets set "LiteLLM:Model" "stable-code:3b-code-q4_0"
dotnet user-secrets set "LiteLLM:ApiKey" ""
```

## How to Run

After switching models, simply run:
```bash
dotnet run
```

## Model Recommendations

- **For general chat**: `gemini-1.5-flash` or `llama3.1:8b`
- **For programming questions**: `deepseek-coder:6.7b` or `stable-code:3b-code-q4_0`
- **For complex reasoning**: `gemini-1.5-pro` or `gpt-oss:20b`
- **For fast responses**: `stable-code:3b-code-q4_0` (smallest/fastest)
- **For best quality (local)**: `gpt-oss:20b` (largest)

## Current Configuration Check

To see what model you're currently using:
```bash
dotnet user-secrets list | grep LiteLLM
```

## Troubleshooting

- **Ollama models not working?** Make sure Ollama is running: `ollama serve`
- **Cloud models not working?** Check your API key: `dotnet user-secrets list | grep ApiKey`
- **Model switch not taking effect?** Restart the application after changing configuration
