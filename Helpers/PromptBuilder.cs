using CapstoneGenerator.API.Models;

namespace CapstoneGenerator.API.Helpers;

public static class PromptBuilder
{
    public static string Build(CapstoneRequest req)
    {
        return $@"
You are an expert academic advisor specializing in capstone project development across all fields of study.

Generate a detailed and creative capstone project based on the following student profile:

Course: {req.Course}
Difficulty Level: {req.Difficulty}
Student Interests: {string.Join(", ", req.Interests)}
Timeframe: {req.Timeframe}
Budget: {req.Budget}
Additional Notes: {req.Notes}

IMPORTANT RULES:
- The project must be appropriate for the course/field provided. For example:
  - Nursing → clinical, patient care, or health education projects
  - Business → marketing, management, finance, or entrepreneurship projects
  - Education → teaching, curriculum, or learning intervention projects
  - Engineering → design, build, or systems projects
  - IT/CS → software, systems, or data projects
  - Social Work → community, advocacy, or program development projects
- Do NOT default to software or programming unless the course is IT, CS, or related
- The tech_stack field should list tools, materials, equipment, or software relevant to the field (not always programming languages)
- Be creative and specific to the student's course and interests

Return ONLY a strict JSON object — no explanation, no markdown, no code fences.

{{
  ""title"": ""Project title here"",
  ""description"": ""2-3 sentence project overview appropriate to the field"",
  ""features"": [""Key component 1"", ""Key component 2"", ""Key component 3""],
  ""tech_stack"": [""Tool/Material/Software 1"", ""Tool/Material/Software 2""],
  ""methodology"": ""Brief methodology description appropriate to the field""
}}";
    }
}