using System.Text;
using System.Text.Json;
using CapstoneGenerator.API.Models;

namespace CapstoneGenerator.API.Services;

public class ResearchPaperService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<ResearchPaperService> _logger;

    private const string GroqUrl = "https://api.groq.com/openai/v1/chat/completions";
    private const string Model = "llama-3.3-70b-versatile";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ResearchPaperService(HttpClient httpClient, IConfiguration config, ILogger<ResearchPaperService> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<string> GenerateAsync(CapstoneResponse capstone)
    {
        var apiKey = _config["Groq:ApiKey"]
            ?? throw new InvalidOperationException("Groq API key is not configured.");

        var prompt = BuildPrompt(capstone);

        var body = new
        {
            model = Model,
            max_tokens = 4000,
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

        _logger.LogInformation("Generating research paper for: {Title}", capstone.Title);

        var response = await _httpClient.SendAsync(httpRequest);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogError("Groq API error {StatusCode}: {Body}", response.StatusCode, errorBody);
            throw new HttpRequestException($"Groq API error {response.StatusCode}: {errorBody}");
        }

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? throw new InvalidOperationException("Empty response from Groq.");

        return content;
    }

    private static string BuildPrompt(CapstoneResponse capstone)
    {
        return $@"
You are an expert academic researcher. Write a complete, professional research paper for the following capstone project.

Project Title: {capstone.Title}
Description: {capstone.Description}
Key Features: {string.Join(", ", capstone.Features)}
Technology Stack: {string.Join(", ", capstone.Tech_Stack)}
Methodology: {capstone.Methodology}

Write a full academic research paper with the following sections using markdown headers:

# Abstract
# 1. Introduction
# 2. Review of Related Literature
# 3. Methodology
# 4. System Architecture and Design
# 5. Implementation
# 6. Results and Discussion
# 7. Conclusion
# 8. References

Requirements:
- Each section must be detailed and at least 2-3 paragraphs
- Use ## for subsections where appropriate
- Write in formal academic tone
- References section should include at least 5 plausible academic references in APA format
- Total length should be at least 1500 words
- Do NOT use markdown bold (**) or italic (*) formatting, plain text only
- Do NOT include any preamble, just start with the paper directly";
    }
}