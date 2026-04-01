using CapstoneGenerator.API.Data;
using Microsoft.EntityFrameworkCore;

namespace CapstoneGenerator.API.Services;

public class HealthCheckService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<HealthCheckService> _logger;

    // Check every 1 minute
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

    public HealthCheckService(
        IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        ILogger<HealthCheckService> logger)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait 5s after startup before first check
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Check Supabase
                await CheckSupabaseAsync(stoppingToken);

                // Check Render
                await CheckRenderAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[HealthCheck] Unexpected error during health check");
            }

            // Wait for next check
            await Task.Delay(Interval, stoppingToken);
        }
    }

    private async Task CheckSupabaseAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AnalyticsDbContext>();

            // Simple query to verify database connection
            var count = await dbContext.AnalyticsEntries.CountAsync();

            _logger.LogInformation(
                "[HealthCheck] Supabase: OK — {Count} analytics entries",
                count
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[HealthCheck] Supabase: FAILED — {Message}", ex.Message);
        }
    }

    private async Task CheckRenderAsync(CancellationToken stoppingToken)
    {
        try
        {
            var renderUrl = _config["App:RenderUrl"];

            if (string.IsNullOrWhiteSpace(renderUrl))
            {
                _logger.LogDebug("[HealthCheck] Render: App:RenderUrl not set — skipping");
                return;
            }

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            var response = await client.GetAsync($"{renderUrl}/health", stoppingToken);

            _logger.LogInformation(
                "[HealthCheck] Render: {Status} — {Url}/health",
                response.StatusCode,
                renderUrl
            );
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "[HealthCheck] Render: FAILED — {Message}", ex.Message);
        }
    }
}
