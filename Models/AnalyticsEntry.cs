namespace CapstoneGenerator.API.Models;

public class AnalyticsEntry
{
    public int Id { get; set; }
    public string? Course { get; set; }
    public string? Difficulty { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public bool Success { get; set; }
}
