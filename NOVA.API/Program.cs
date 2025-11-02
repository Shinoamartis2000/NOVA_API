using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NOVA.Application.Interfaces;
using NOVA.Application.Services;
using NOVA.Infrastructure.Config;
using NOVA.Infrastructure.Data;
using NOVA.Infrastructure.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// ? Configure Kestrel to use Render's PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

// ? Configure logging FIRST - before any other services
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// ? Load configuration
builder.Services.Configure<OpenAIConfig>(builder.Configuration.GetSection("OpenAI"));
var openAiConfig = builder.Configuration.GetSection("OpenAI").Get<OpenAIConfig>();
builder.Services.AddSingleton(openAiConfig);

// ? Register Database with dynamic connection string for Render
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// If using persistent disk on Render, use /data path
if (connectionString?.Contains("Data Source=") == true && !connectionString.Contains("/data/"))
{
    connectionString = "Data Source=/data/nova.db";
}

builder.Services.AddDbContext<NovaDbContext>(options =>
    options.UseSqlite(connectionString));

// ? Core services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IConversationMemoryService, ConversationMemoryService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<PersonalityService>();

// ? AI backend - make sure to add ILogger parameter
builder.Services.AddHttpClient<IOpenAIService, OllamaService>(client =>
{
    if (!string.IsNullOrEmpty(openAiConfig?.BaseUrl))
        client.BaseAddress = new Uri(openAiConfig.BaseUrl);
});

// ? Controllers, Swagger, CORS, etc.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ? Updated CORS for your frontend domain
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
            "https://neuraloperationalvirtualassistant.com",
            "http://localhost:3000", // For local testing
            "http://localhost:5173"  // For Vite local testing
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();

// ? Database migration/creation
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<NovaDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Ensuring database is created...");
        db.Database.Migrate();
        logger.LogInformation("Database ready!");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// ? Enable Swagger in production too (optional, but useful for testing)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

// ? Remove HTTPS redirection for Render (Render handles SSL at edge)
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

// ? Add a health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();