using Microsoft.AspNetCore.Mvc;
using WeatherBot.API.Repositories;
using WeatherBot.API.Models;
using System.Threading.Tasks;
using WeatherBot.API.Services;
using Telegram.Bot;

namespace WeatherBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly DatabaseHelper _db;
        private readonly OpenWeatherMapService _weatherService; // Add OpenWeatherMapService
        private readonly TelegramBotClient _botClient; // Add TelegramBotClient

        public WeatherController(DatabaseHelper db, OpenWeatherMapService weatherService, TelegramBotClient botClient)
        {
            _db = db;
            _weatherService = weatherService; // Initialize OpenWeatherMapService
            _botClient = botClient; // Initialize TelegramBotClient
        }

        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetWeatherHistory(int userId)
        {
            var history = await _db.QueryAsync<WeatherHistory>("SELECT * FROM WeatherHistory WHERE UserId = @UserId", new { UserId = userId });
            return Ok(history);
        }

        [HttpPost("sendWeatherToAll")]
        public async Task<IActionResult> SendWeatherToAll()
        {
            var users = await _db.QueryAsync<User>("SELECT * FROM Users");
            foreach (var user in users)
            {
                // Send weather information to each user
                var weather = await _weatherService.GetWeatherAsync("London"); // Example city
                await _botClient.SendTextMessageAsync(user.ChatId, weather);
            }
            return Ok();
        }
    }
}