namespace CapstoneGenerator.API.Models;

public class Review
{
    public int Id { get; set; }
    public string Name { get; set; } = "Anonymous";
    public string Message { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}