using System.Text;
using System.Text.Json;
using CapstoneGenerator.API.Helpers;
using CapstoneGenerator.API.Models;

namespace CapstoneGenerator.API.Services;

public class GroqService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<GroqService> _logger;

    private const string GroqUrl = "https://api.groq.com/openai/v1/chat/completions";
    private const string Model = "llama-3.3-70b-versatile";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public GroqService(HttpClient httpClient, IConfiguration config, ILogger<GroqService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<CapstoneResponse> GenerateAsync(CapstoneRequest request)
    {
        var apiKey = _config["Groq:ApiKey"]
            ?? throw new InvalidOperationException("Groq API key is not configured.");

        var prompt = PromptBuilder.Build(request);

        var body = new
        {
            model = Model,
            max_tokens = 1000,
            messages = new[]
            {
                new { role = "user", content = prompt }
            }
        };

        var httpRequest = new HttpRequestMessage(HttpMethod.Post, GroqUrl);
        httpRequest.Headers.Add("Authorization", $"Bearer {apiKey}");
        httpRequest.Content = new StringContent(
            JsonSerializer.Serialize(body, JsonOptions),
            Encoding.UTF8,
            "application/json"
        );

        _logger.LogInformation("Sending request to Groq API for course: {Course}", request.Course);

        var response = await _httpClient.SendAsync(httpRequest);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogError("Groq API returned {StatusCode}: {Body}", response.StatusCode, errorBody);
            throw new HttpRequestException($"Groq API error {response.StatusCode}: {errorBody}");
        }

        var json = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Groq API responded successfully.");

        using var doc = JsonDocument.Parse(json);
        var rawText = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? throw new InvalidOperationException("Empty response from Groq.");

        // Strip accidental markdown code fences
        rawText = rawText
            .Replace("```json", "")
            .Replace("```", "")
            .Trim();

        var result = JsonSerializer.Deserialize<CapstoneResponse>(rawText, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new InvalidOperationException("Failed to deserialize CapstoneResponse from Groq output.");

        return result;
    }
}