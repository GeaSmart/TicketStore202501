using Microsoft.AspNetCore.Mvc;

namespace MusicStore.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenresController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetGenres()
        {
            // This is a placeholder implementation.
            // In a real application, you would retrieve genres from a database or other data source.
            var genres = new[]
            {
                new { Id = 1, Name = "Rock", Status = true },
                new { Id = 2, Name = "Jazz", Status = true },
                new { Id = 3, Name = "Classical", Status = true }
            };
            return Ok(genres);
        }
    }
}
