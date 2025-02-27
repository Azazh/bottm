namespace WeatherBot.API.Models
{
    public class WeatherHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string City { get; set; }
        public string WeatherData { get; set; } // Store the entire weather response as JSON
        public DateTime RequestDate { get; set; }
    }
}