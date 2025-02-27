// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using WeatherBot.API.Models;

namespace WeatherBot.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(string connectionString) : base()
        {
            Database.SetConnectionString(connectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<WeatherHistory> WeatherHistories { get; set; }
    }
}