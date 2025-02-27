using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json; // Use Newtonsoft.Json for deserialization
using Newtonsoft.Json.Linq;

namespace WeatherBot.API.Services
{
    public class OpenWeatherMapService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public OpenWeatherMapService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public async Task<string> GetWeatherAsync(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return "Please provide a valid city name.";
            }

            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("API Error Response: " + errorResponse);

                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return "Sorry, the API key is invalid. Please check the API key and try again.";
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return "Sorry, the city was not found. Please check the city name and try again.";
                    }
                    else
                    {
                        return "Sorry, I couldn't fetch the weather data. Please try again later.";
                    }
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine("API Response: " + json);

                // Deserialize the JSON response using Newtonsoft.Json
                var weatherData = JsonConvert.DeserializeObject<OpenWeatherMapResponse>(json);

                if (weatherData == null ||
                    string.IsNullOrEmpty(weatherData.Name) ||
                    weatherData.Main?.Temp == null ||
                    weatherData.Weather == null ||
                    weatherData.Weather.Length == 0)
                {
                    Console.WriteLine("Deserialized object is invalid or incomplete.");
                    return "Sorry, I couldn't fetch the weather data. Please check the city name and try again.";
                }

                return FormatWeatherResponse(weatherData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return "An unexpected error occurred. Please try again later.";
            }
        }

        private string FormatWeatherResponse(OpenWeatherMapResponse weatherData)
        {
            return $"Weather in {weatherData.Name}:\n" +
                   $"- Temperature: {weatherData.Main.Temp}°C\n" +
                   $"- Feels Like: {weatherData.Main.Feels_Like}°C\n" +
                   $"- Humidity: {weatherData.Main.Humidity}%\n" +
                   $"- Wind: {weatherData.Wind?.Speed ?? 0} km/h\n" +
                   $"- Description: {weatherData.Weather[0].Description}";
        }

        public class OpenWeatherMapResponse
        {
            [JsonProperty("name")] // Use JsonProperty instead of JsonPropertyName
            public string Name { get; set; }

            [JsonProperty("main")]
            public MainData Main { get; set; }

            [JsonProperty("wind")]
            public WindData Wind { get; set; }

            [JsonProperty("weather")]
            public WeatherData[] Weather { get; set; }

            public class MainData
            {
                [JsonProperty("temp")]
                public float Temp { get; set; }

                [JsonProperty("feels_like")]
                public float Feels_Like { get; set; }

                [JsonProperty("humidity")]
                public int Humidity { get; set; }

                [JsonProperty("temp_min")]
                public float Temp_Min { get; set; }

                [JsonProperty("temp_max")]
                public float Temp_Max { get; set; }

                [JsonProperty("pressure")]
                public int Pressure { get; set; }
            }

            public class WindData
            {
                [JsonProperty("speed")]
                public float Speed { get; set; }

                [JsonProperty("deg")]
                public int Deg { get; set; }
            }

            public class WeatherData
            {
                [JsonProperty("description")]
                public string Description { get; set; }
            }
        }
    }
}