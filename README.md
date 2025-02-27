
# WeatherBot ???

WeatherBot is a Telegram bot that provides users with real-time weather information for any city. It uses the **OpenWeatherMap API** to fetch weather data and stores user information and weather history in a **Microsoft SQL Server** database. The bot is built using **ASP.NET Core Web API**, **Dapper**, and the **Telegram.Bot** library.

---

## Features

- **Get Weather Information**: Users can request weather data for any city by sending `/weather {city}` or clicking the "Get Weather" button.
- **User Management**: The bot stores user information (e.g., Telegram chat ID, username) in a database.
- **Weather History**: The bot keeps a history of weather requests for each user.
- **API Endpoints**: The project includes RESTful API endpoints for retrieving user information and weather history.

---

## Prerequisites

Before running the project, ensure you have the following:

1. **.NET 6 SDK**: [Download .NET 6](https://dotnet.microsoft.com/download/dotnet/6.0)
2. **SQL Server**: Install [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).
3. **Telegram Bot Token**: Create a bot using [BotFather](https://core.telegram.org/bots#botfather) and obtain the API token.
4. **OpenWeatherMap API Key**: Sign up at [OpenWeatherMap](https://openweathermap.org/api) and get an API key.

---

## Setup

### 1. Clone the Repository

```bash
git clone https://github.com/Azazh/bottm.git
cd WeatherBot
2. Set Up the Database
Create a database named WeatherBotDB in SQL Server.

Run the following SQL scripts to create the required tables:

sql
Copy
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL,
    ChatId BIGINT NOT NULL,
    CreatedDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE WeatherHistory (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    City NVARCHAR(100) NOT NULL,
    WeatherData NVARCHAR(MAX) NOT NULL,
    RequestDate DATETIME DEFAULT GETDATE()
);
3. Configure Environment Variables
Create a .env file in the root of the project with the following content:

env
Copy
TELEGRAM_BOT_TOKEN=your-telegram-bot-token
OPENWEATHERMAP_API_KEY=your-openweathermap-api-key
DATABASE_CONNECTION_STRING=Server=your-server-name;Database=WeatherBotDB;Trusted_Connection=True;
4. Run the Application
bash
Copy
dotnet run
The application will start, and the Swagger UI will open automatically in your browser at http://localhost:5000/swagger.

Usage
Telegram Bot
Open Telegram and search for your bot.

Send /start to begin interacting with the bot.

Use the /weather {city} command or click the "Get Weather" button to get weather information for a specific city.

API Endpoints
The project includes the following API endpoints:

GET /users/{userId}: Get user information by ID.

GET /weather/history/{userId}: Get weather history for a user.

POST /sendWeatherToAll: Send weather information to all users.

You can test these endpoints using the Swagger UI at http://localhost:5000/swagger.

Project Structure
Copy
WeatherBot/
??? WeatherBot.API/
?   ??? Controllers/          # API controllers
?   ??? Models/               # Data models (User, WeatherHistory)
?   ??? Repositories/         # Database helper and queries
?   ??? Services/             # Business logic (TelegramBotService, OpenWeatherMapService)
?   ??? appsettings.json      # Configuration file
?   ??? Program.cs            # Application entry point
?   ??? WeatherBot.API.csproj # Project file
??? docs/                     # Documentation (Swagger files)
??? .env                      # Environment variables
??? .gitignore                # Git ignore rules
??? README.md                 # Project documentation
??? WeatherBot.sln            # Solution file
Technologies Used
.NET 6: The framework for building the application.

ASP.NET Core Web API: For building RESTful APIs.

Dapper: For database access and query execution.

Telegram.Bot: For interacting with the Telegram Bot API.

OpenWeatherMap API: For fetching weather data.

Swagger: For API documentation and testing.

License
This project is licensed under the MIT License. See the LICENSE file for details.

Contributing
Contributions are welcome! If you'd like to contribute, please follow these steps:

Fork the repository.

Create a new branch (git checkout -b feature/YourFeatureName).

Commit your changes (git commit -m 'Add some feature').

Push to the branch (git push origin feature/YourFeatureName).

Open a pull request.

Acknowledgments
OpenWeatherMap for providing the weather data API.

Telegram.Bot for the Telegram Bot library.

Dapper for simplifying database access.






