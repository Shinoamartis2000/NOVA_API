using Microsoft.EntityFrameworkCore;
using NOVA.Infrastructure.Config;
using NOVA.Infrastructure.Data;
using NOVA.Infrastructure.Services;
using NOVA.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ? Load configuration
builder.Services.Configure<OpenAIConfig>(builder.Configuration.GetSection("OpenAI"));

// ? Register SQLite database
builder.Services.AddDbContext<NovaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ? Register Core Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IConversationMemoryService, ConversationMemoryService>();

// ? Choose your AI backend (Ollama / OpenAI / OpenRouter)
builder.Services.AddHttpClient<IOpenAIService, OllamaService>(client =>
{
    var config = builder.Configuration.GetSection("OpenAI").Get<OpenAIConfig>();
    if (config != null && !string.IsNullOrEmpty(config.BaseUrl))
    {
        client.BaseAddress = new Uri(config.BaseUrl);
    }
});

// ? Add Controllers
builder.Services.AddControllers();

// ? Enable CORS (for front-end or local testing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ? Swagger (for easy API testing)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ? Apply migrations automatically (optional)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NovaDbContext>();
    db.Database.Migrate();
}

// ? Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
