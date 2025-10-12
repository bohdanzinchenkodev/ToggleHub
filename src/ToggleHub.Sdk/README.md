# ToggleHub.SDK

Lightweight .NET SDK for evaluating feature flags via the ToggleHub API.

---

## 🧩 Installation

```bash
dotnet add package ToggleHub.SDK
```

---

## 🚀 Quickstart

### ASP.NET Core (recommended)

Register the SDK in your `Program.cs`:

```csharp
using ToggleHub.SDK.Configuration;
using ToggleHub.SDK.Flags;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddToggleHubClient(o =>
{
    o.BaseAddress = "https://api.toggle-hub.com";
    o.ApiKey = "YOUR_API_KEY";
});

var app = builder.Build();
```

Inject and call:

```csharp
public class MyService
{
    private readonly IFlagsClient _flags;

    public MyService(IFlagsClient flags) => _flags = flags;

    public async Task<bool> IsNewDashboardEnabledAsync(string userId)
    {
        var request = new FlagEvaluationRequest
        {
            UserId = userId,
            FlagKey = "new-dashboard",
            ConditionAttributes = new Dictionary<string, string?> { ["country"] = "CA" }
        };

        var result = await _flags.EvaluateAsync<FlagEvaluationResult>(request);
        return result?.Enabled ?? false;
    }
}

public sealed class FlagEvaluationResult
{
    public bool Enabled { get; set; }
    public string? Variant { get; set; }
}
```

---

### Console App (static setup)

For small tools:

```csharp
using ToggleHub.SDK.Configuration;
using ToggleHub.SDK.Flags;

var opts = new ToggleHubClientOptions
{
    BaseAddress = "https://api.toggle-hub.com",
    ApiKey = "YOUR_API_KEY"
};

var http = new HttpClient { BaseAddress = new Uri(opts.BaseAddress) };
var client = new FlagsClient(http, opts);

var request = new FlagEvaluationRequest
{
    UserId = "user-123",
    FlagKey = "new-dashboard"
};

var result = await client.EvaluateAsync<FlagEvaluationResult>(request);
Console.WriteLine($"Enabled: {result?.Enabled}");
```

---

## ⚙️ Configuration

| Option | Description | Default                       |
|--------|--------------|-------------------------------|
| `BaseAddress` | Base URL of your ToggleHub API | `api.toggle-hub.com` (demo url) |
| `ApiKey` | Your organization’s API key | —                             |
| `TimeoutSeconds` | HTTP request timeout | `10`                          |

---

## 🧠 Notes

- Targets: `net9.0`, `netstandard2.0`
- Dependencies: `System.Text.Json`, `Microsoft.Extensions.Http`
- Thread-safe and DI-friendly via `IHttpClientFactory`
- Fully async and context-agnostic (`ConfigureAwait(false)` used internally)
- Open-source under the [MIT License](./LICENSE)

---

## 🪪 License

This project is licensed under the **MIT License** — see [LICENSE](./LICENSE) for details.

---

## 🔗 Links

- **Repository:** https://github.com/bohdanzinchenkodev/togglehub
- **Issues:** https://github.com/bohdanzinchenkodev/togglehub/issues
