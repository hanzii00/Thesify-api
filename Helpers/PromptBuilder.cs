using CapstoneGenerator.API.Models;

namespace CapstoneGenerator.API.Helpers;

public static class PromptBuilder
{
    public static string Build(CapstoneRequest req)
    {
        return $@"
You are an expert academic advisor specializing in capstone project development.

Generate a detailed capstone project based on the following student profile:

Course: {req.Course}
Difficulty Level: {req.Difficulty}
Student Interests: {string.Join(", ", req.Interests)}
Timeframe: {req.Timeframe}
Budget: {req.Budget}
Additional Notes: {req.Notes}

Return ONLY a strict JSON object — no explanation, no markdown, no code fences.

{{
  ""title"": ""Project title here"",
  ""description"": ""2-3 sentence project overview"",
  ""features"": [""Feature 1"", ""Feature 2"", ""Feature 3""],
  ""tech_stack"": [""Technology 1"", ""Technology 2""],
  ""methodology"": ""Brief methodology description (Agile, Waterfall, etc.)""
}}";
    }
}