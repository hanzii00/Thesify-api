namespace CapstoneGenerator.API.Services;

public class KeepAliveService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<KeepAliveService> _logger;

    // Ping every 1 minute
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

    public KeepAliveService(
        IHttpClientFactory httpClientFactory,
        IConfiguration config,
        ILogger<KeepAliveService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait 30s after startup before first ping
        await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var renderUrl = _config["App:RenderUrl"];

                if (!string.IsNullOrWhiteSpace(renderUrl))
                {
                    var client = _httpClientFactory.CreateClient();
                    var response = await client.GetAsync($"{renderUrl}/health", stoppingToken);

                    _logger.LogInformation(
                        "[KeepAlive] Pinged {Url}/health — {Status}",
                        renderUrl,
                        response.StatusCode
                    );
                }
                else
                {
                    _logger.LogDebug("[KeepAlive] App:RenderUrl not set — skipping ping.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("[KeepAlive] Ping failed: {Message}", ex.Message);
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}