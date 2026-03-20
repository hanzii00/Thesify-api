namespace CapstoneGenerator.API.Models;

public class CapstoneResponse
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Features { get; set; } = new();
    public List<string> Tech_Stack { get; set; } = new();
    public string Methodology { get; set; } = string.Empty;
}