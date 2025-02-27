using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WeatherBot.API.Repositories;
using WeatherBot.API.Models;

namespace WeatherBot.API.Services
{
    public class TelegramBotService : IHostedService
    {
        private readonly TelegramBotClient _botClient;
        private readonly OpenWeatherMapService _weatherService;
        private readonly DatabaseHelper _db;
        private CancellationTokenSource _cancellationTokenSource;

        // Update constructor to accept TelegramBotClient
        public TelegramBotService(TelegramBotClient botClient, OpenWeatherMapService weatherService, DatabaseHelper db)
        {
            _botClient = botClient;
            _weatherService = weatherService;
            _db = db;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _botClient.StartReceiving(UpdateHandler, ErrorHandler, cancellationToken: _cancellationTokenSource.Token);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            return Task.CompletedTask;
        }

        private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message?.Text == "/start")
            {
                // Send welcome message with a button
                var replyMarkup = new ReplyKeyboardMarkup(new[]
                {
                    new KeyboardButton("Get Weather")
                })
                {
                    ResizeKeyboard = true,
                    OneTimeKeyboard = true
                };

                await botClient.SendTextMessageAsync(
                    update.Message.Chat.Id,
                    "Welcome to the Weather Bot! 🌦️\n\nHere's how you can use me:\n- Use the command /weather {city} to get the weather for a specific city.\n- Or click the 'Get Weather' button below to start.",
                    replyMarkup: replyMarkup);
            }
            else if (update.Message?.Text == "Get Weather")
            {
                // Ask the user to enter a city name
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Please enter the name of the city:");
            }
            else if (update.Message?.Text != null && !update.Message.Text.StartsWith("/"))
            {
                // Handle city name input after clicking the button
                var city = update.Message.Text;
                var weather = await _weatherService.GetWeatherAsync(city);

                // Store user information if not already stored
                var user = await _db.QueryAsync<WeatherBot.API.Models.User>("SELECT * FROM Users WHERE ChatId = @ChatId", new { ChatId = update.Message.Chat.Id });
                if (user == null || !user.Any())
                {
                    await _db.ExecuteAsync("INSERT INTO Users (Username, ChatId, CreatedDate) VALUES (@Username, @ChatId, @CreatedDate)", new
                    {
                        Username = update.Message.Chat.Username ?? "Unknown",
                        ChatId = update.Message.Chat.Id,
                        CreatedDate = DateTime.UtcNow
                    });

                    // Retrieve the newly created user
                    user = await _db.QueryAsync<WeatherBot.API.Models.User>("SELECT * FROM Users WHERE ChatId = @ChatId", new { ChatId = update.Message.Chat.Id });
                }

                // Store weather history
                await _db.ExecuteAsync("INSERT INTO WeatherHistory (UserId, City, WeatherData, RequestDate) VALUES (@UserId, @City, @WeatherData, @RequestDate)", new
                {
                    UserId = user.First().UserId,
                    City = city,
                    WeatherData = weather,
                    RequestDate = DateTime.UtcNow
                });

                await botClient.SendTextMessageAsync(update.Message.Chat.Id, weather, replyMarkup: new ReplyKeyboardRemove());
            }
            else if (update.Message?.Text.StartsWith("/weather ") ?? false)
            {
                // Handle /weather command
                var city = update.Message.Text.Split(' ')[1];
                var weather = await _weatherService.GetWeatherAsync(city);

                // Store user information if not already stored
                var user = await _db.QueryAsync<WeatherBot.API.Models.User>("SELECT * FROM Users WHERE ChatId = @ChatId", new { ChatId = update.Message.Chat.Id });
                if (user == null || !user.Any())
                {
                    await _db.ExecuteAsync("INSERT INTO Users (Username, ChatId, CreatedDate) VALUES (@Username, @ChatId, @CreatedDate)", new
                    {
                        Username = update.Message.Chat.Username ?? "Unknown",
                        ChatId = update.Message.Chat.Id,
                        CreatedDate = DateTime.UtcNow
                    });

                    // Retrieve the newly created user
                    user = await _db.QueryAsync<WeatherBot.API.Models.User>("SELECT * FROM Users WHERE ChatId = @ChatId", new { ChatId = update.Message.Chat.Id });
                }

                // Store weather history
                await _db.ExecuteAsync("INSERT INTO WeatherHistory (UserId, City, WeatherData, RequestDate) VALUES (@UserId, @City, @WeatherData, @RequestDate)", new
                {
                    UserId = user.First().UserId,
                    City = city,
                    WeatherData = weather,
                    RequestDate = DateTime.UtcNow
                });

                await botClient.SendTextMessageAsync(update.Message.Chat.Id, weather);
            }
        }

        private Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(exception.Message);
            return Task.CompletedTask;
        }
    }
}