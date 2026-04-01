using CapstoneGenerator.API.Models;

namespace CapstoneGenerator.API.Services;

public class AnalyticsService
{
    private static readonly List<AnalyticsEntry> _analyticsData = new();
    private static readonly object _lock = new();
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(ILogger<AnalyticsService> logger)
    {
        _logger = logger;
    }

    public Task LogGenerationAsync(string? course, string? difficulty, bool success)
    {
        try
        {
            var entry = new AnalyticsEntry
            {
                Id = _analyticsData.Count + 1,
                Course = course,
                Difficulty = difficulty,
                GeneratedAt = DateTime.UtcNow,
                Success = success
            };

            lock (_lock)
            {
                _analyticsData.Add(entry);
            }

            _logger.LogInformation($"Analytics logged: {course} - {difficulty} - Success: {success}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log analytics");
        }

        return Task.CompletedTask;
    }

    public Task<AnalyticsStats> GetStatsAsync()
    {
        try
        {
            lock (_lock)
            {
                var totalGenerations = _analyticsData.Count;
                var successfulGenerations = _analyticsData.Count(e => e.Success);
                var failedGenerations = totalGenerations - successfulGenerations;

                var courseStats = _analyticsData
                    .Where(e => !string.IsNullOrEmpty(e.Course))
                    .GroupBy(e => e.Course)
                    .Select(g => new CourseUsage
                    {
                        Course = g.Key ?? "Unknown",
                        Count = g.Count()
                    })
                    .OrderByDescending(c => c.Count)
                    .ToList();

                var difficultyStats = _analyticsData
                    .Where(e => !string.IsNullOrEmpty(e.Difficulty))
                    .GroupBy(e => e.Difficulty)
                    .Select(g => new DifficultyUsage
                    {
                        Difficulty = g.Key ?? "Unknown",
                        Count = g.Count()
                    })
                    .OrderByDescending(d => d.Count)
                    .ToList();

                var last7Days = DateTime.UtcNow.AddDays(-7);
                var generationsLast7Days = _analyticsData
                    .Count(e => e.GeneratedAt >= last7Days);

                var stats = new AnalyticsStats
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

                return Task.FromResult(stats);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve analytics stats");
            return Task.FromResult(new AnalyticsStats());
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
