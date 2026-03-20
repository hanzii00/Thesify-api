namespace CapstoneGenerator.API.Models;

public class CapstoneRequest
{
    public string Course { get; set; } = string.Empty;
    public string Difficulty { get; set; } = string.Empty;
    public List<string> Interests { get; set; } = new();
    public string Timeframe { get; set; } = string.Empty;
    public string Budget { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}