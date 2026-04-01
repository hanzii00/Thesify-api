using CapstoneGenerator.API.Data;
using CapstoneGenerator.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CapstoneGenerator.API.Services;

public class AnalyticsService
{
    private readonly AnalyticsDbContext _context;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(AnalyticsDbContext context, ILogger<AnalyticsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task LogGenerationAsync(string? course, string? difficulty, bool success)
    {
        try
        {
            var entry = new AnalyticsEntry
            {
                Course = course,
                Difficulty = difficulty,
                GeneratedAt = DateTime.UtcNow,
                Success = success
            };

            _context.AnalyticsEntries.Add(entry);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log analytics");
        }
    }

    public async Task<AnalyticsStats> GetStatsAsync()
    {
        try
        {
            var totalGenerations = await _context.AnalyticsEntries.CountAsync();
            var successfulGenerations = await _context.AnalyticsEntries.CountAsync(e => e.Success);
            var failedGenerations = totalGenerations - successfulGenerations;

            var courseStats = await _context.AnalyticsEntries
                .Where(e => !string.IsNullOrEmpty(e.Course))
                .GroupBy(e => e.Course)
                .Select(g => new CourseUsage
                {
                    Course = g.Key ?? "Unknown",
                    Count = g.Count()
                })
                .OrderByDescending(c => c.Count)
                .ToListAsync();

            var difficultyStats = await _context.AnalyticsEntries
                .Where(e => !string.IsNullOrEmpty(e.Difficulty))
                .GroupBy(e => e.Difficulty)
                .Select(g => new DifficultyUsage
                {
                    Difficulty = g.Key ?? "Unknown",
                    Count = g.Count()
                })
                .OrderByDescending(d => d.Count)
                .ToListAsync();

            var last7Days = DateTime.UtcNow.AddDays(-7);
            var generationsLast7Days = await _context.AnalyticsEntries
                .CountAsync(e => e.GeneratedAt >= last7Days);

            return new AnalyticsStats
            {
                TotalGenerations = totalGenerations,
                SuccessfulGenerations = successfulGenerations,
                FailedGenerations = failedGenerations,
                SuccessRate = totalGenerations > 0 ? (successfulGenerations * 100.0 / totalGenerations) : 0,
                TopCourses = courseStats,
                DifficultyDistribution = difficultyStats,
                GenerationsLast7Days = generationsLast7Days,
                GeneratedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve analytics stats");
            return new AnalyticsStats();
        }
    }
}

public class AnalyticsStats
{
    public int TotalGenerations { get; set; }
    public int SuccessfulGenerations { get; set; }
    public int FailedGenerations { get; set; }
    public double SuccessRate { get; set; }
    public List<CourseUsage> TopCourses { get; set; } = new();
    public List<DifficultyUsage> DifficultyDistribution { get; set; } = new();
    public int GenerationsLast7Days { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class CourseUsage
{
    public string Course { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class DifficultyUsage
{
    public string Difficulty { get; set; } = string.Empty;
    public int Count { get; set; }
}
