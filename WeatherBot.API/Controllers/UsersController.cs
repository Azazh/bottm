using Microsoft.AspNetCore.Mvc;
using WeatherBot.API.Repositories;
using WeatherBot.API.Models;
using System.Threading.Tasks;

namespace WeatherBot.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseHelper _db;

        public UsersController(DatabaseHelper db)
        {
            _db = db;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var user = await _db.QueryAsync<User>("SELECT * FROM Users WHERE UserId = @UserId", new { UserId = userId });
            return Ok(user);
        }
    }
}