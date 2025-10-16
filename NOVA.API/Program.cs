using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NOVA.Application.Interfaces;
using NOVA.Application.Services;
using NOVA.Infrastructure.Config;
using NOVA.Infrastructure.Data;
using NOVA.Infrastructure.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// ? Load configuration
builder.Services.Configure<OpenAIConfig>(builder.Configuration.GetSection("OpenAI"));
var openAiConfig = builder.Configuration.GetSection("OpenAI").Get<OpenAIConfig>();
builder.Services.AddSingleton(openAiConfig);

// ? Register Database
builder.Services.AddDbContext<NovaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ? Core services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IConversationMemoryService, ConversationMemoryService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<PersonalityService>(); // ?? FIX #1

// ? AI backend
builder.Services.AddHttpClient<IOpenAIService, OllamaService>(client =>
{
    if (!string.IsNullOrEmpty(openAiConfig.BaseUrl))
        client.BaseAddress = new Uri(openAiConfig.BaseUrl);
});

// ? Controllers, Swagger, CORS, etc.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<NovaDbContext>();
    db.Database.Migrate();
}

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
