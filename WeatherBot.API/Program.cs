using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherBot.API.Repositories;
using WeatherBot.API.Services;
using System.Net.Http;
using Telegram.Bot;
using DotNetEnv; // Import DotNetEnv

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file using DotNetEnv
Env.Load(); // This loads the .env file into environment variables

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DatabaseHelper
builder.Services.AddSingleton<DatabaseHelper>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new InvalidOperationException("Database connection string is missing or invalid.");
    }
    return new DatabaseHelper(connectionString);
});

// Register HttpClient for OpenWeatherMapService
builder.Services.AddHttpClient<OpenWeatherMapService>();

// Register OpenWeatherMapService
builder.Services.AddSingleton(sp =>
{
    var apiKey = Environment.GetEnvironmentVariable("OPENWEATHERMAP_API_KEY");
    if (string.IsNullOrEmpty(apiKey))
    {
        throw new InvalidOperationException("OpenWeatherMap API key is missing or invalid.");
    }
    var httpClient = sp.GetRequiredService<HttpClient>();
    return new OpenWeatherMapService(httpClient, apiKey);
});

// Register TelegramBotClient
var botToken = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
Console.WriteLine($"botToken: {botToken}");
if (string.IsNullOrEmpty(botToken))
{
    throw new InvalidOperationException("Telegram bot token is missing or invalid.");
}
builder.Services.AddSingleton(new TelegramBotClient(botToken));

// Register TelegramBotService as a hosted service
builder.Services.AddHostedService(sp =>
{
    var weatherService = sp.GetRequiredService<OpenWeatherMapService>();
    var db = sp.GetRequiredService<DatabaseHelper>();
    var botClient = sp.GetRequiredService<TelegramBotClient>();
    return new TelegramBotService(botClient, weatherService, db);
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherBot API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI as the default page
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();