namespace WeatherBot.API.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public long ChatId { get; set; } // Telegram chat ID
        public DateTime CreatedDate { get; set; }
    }
}