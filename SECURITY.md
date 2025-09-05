# 🔒 Secure API Key Management Guide

## ⚠️ IMPORTANT: Never commit API keys to version control!

This guide shows you how to securely manage API keys in C# applications, similar to how `.env` files work in Python.

## 🔧 Method 1: User Secrets (Recommended for Development)

User Secrets is C#'s built-in equivalent to `.env` files. It stores sensitive data outside your project directory.

### Setup User Secrets:

1. **Initialize user secrets:**
   ```bash
   dotnet user-secrets init
   ```

2. **Add your API keys:**
   ```bash
   # For Gemini API
   dotnet user-secrets set "Gemini:ApiKey" "your-actual-gemini-api-key"
   
   # For OpenAI API  
   dotnet user-secrets set "OpenAI:ApiKey" "your-actual-openai-api-key"
   ```

3. **List stored secrets:**
   ```bash
   dotnet user-secrets list
   ```

4. **Remove a secret:**
   ```bash
   dotnet user-secrets remove "Gemini:ApiKey"
   ```

### How it works:
- Secrets are stored in: `~/.microsoft/usersecrets/<user-secrets-id>/secrets.json`
- This location is outside your project and won't be committed to Git
- The secrets automatically override values in `appsettings.json`

## 🌍 Method 2: Environment Variables (Production)

For production deployments, use environment variables:

### On Linux/macOS:
```bash
export GEMINI__APIKEY="your-actual-gemini-api-key"
export OPENAI__APIKEY="your-actual-openai-api-key"
export AI__PROVIDER="Gemini"
dotnet run
```

### On Windows:
```cmd
set GEMINI__APIKEY=your-actual-gemini-api-key
set OPENAI__APIKEY=your-actual-openai-api-key
set AI__PROVIDER=Gemini
dotnet run
```

### Using .env file (with additional package):
If you want Python-style `.env` files, add this package:
```bash
dotnet add package DotNetEnv
```

## 📁 Method 3: Azure Key Vault (Enterprise)

For enterprise applications, use Azure Key Vault:
```bash
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
```

## 🛡️ Security Best Practices:

1. **Never commit these files:**
   - `secrets.json`
   - `.env`
   - Any file containing actual API keys

2. **Use .gitignore:**
   The project includes a comprehensive `.gitignore` that excludes:
   - User secrets
   - Environment files
   - Build artifacts

3. **Use different keys for different environments:**
   - Development keys (limited scope)
   - Production keys (full access)

4. **Rotate keys regularly:**
   - Change API keys periodically
   - Revoke old keys

## 🔍 How to Check Your Current Setup:

1. **Check if secrets are configured:**
   ```bash
   dotnet user-secrets list
   ```

2. **Verify configuration in code:**
   The app automatically detects available API keys and chooses the appropriate service.

3. **Test without API keys:**
   Remove API keys temporarily to test the Mock AI fallback:
   ```bash
   dotnet user-secrets remove "Gemini:ApiKey"
   dotnet user-secrets remove "OpenAI:ApiKey"
   ```

## 📋 Quick Setup Checklist:

- [ ] Run `dotnet user-secrets init`
- [ ] Add your API key: `dotnet user-secrets set "Gemini:ApiKey" "your-key"`
- [ ] Remove API keys from `appsettings.json`
- [ ] Verify `.gitignore` includes sensitive files
- [ ] Test the application
- [ ] Commit only the safe configuration files

## 🚀 Ready for GitHub:

After following this guide:
- ✅ `appsettings.json` contains no sensitive data
- ✅ API keys are stored securely in user secrets
- ✅ `.gitignore` prevents accidental commits
- ✅ Your code is safe to push to GitHub!

## 🤝 Sharing with Team Members:

1. **Share the `.env.example` file** - shows what keys are needed
2. **Include setup instructions in README** - how to configure their own keys
3. **Document the user secrets commands** - for easy onboarding

Your API keys remain private while your code can be safely shared!
