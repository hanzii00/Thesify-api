using CapstoneGenerator.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ─── Services ────────────────────────────────────────────────────────────────

builder.Services.AddControllers();

builder.Services.AddHttpClient<GroqService>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

builder.Services.AddHttpClient<ResearchPaperService>()
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    });

// Health checks
builder.Services.AddHealthChecks();

// Keep-alive background service
builder.Services.AddHttpClient();
builder.Services.AddHostedService<KeepAliveService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Capstone Generator API",
        Version = "v1",
        Description = "Generates capstone project ideas and research papers using Groq AI (Llama 3.3)"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// ─── App ─────────────────────────────────────────────────────────────────────

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Capstone Generator API v1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();