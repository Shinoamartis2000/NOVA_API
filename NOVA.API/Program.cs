using NOVA.Application.Interfaces;
using NOVA.Application.Services;
using NOVA.Infrastructure.Config;
using NOVA.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// OpenRouter / OpenAI configuration
var openAIConfig = new OpenAIConfig();
builder.Configuration.GetSection("OpenAI").Bind(openAIConfig);
builder.Services.AddSingleton(openAIConfig);

// Register services with configured HttpClient
builder.Services.AddHttpClient<IOpenAIService, OllamaService>(client =>
{
    client.BaseAddress = new Uri(openAIConfig.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddScoped<IChatService, ChatService>();

// Controllers + Swagger + CORS
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();