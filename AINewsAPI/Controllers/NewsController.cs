using Microsoft.AspNetCore.Mvc;
using AINewsAPI.Application.Interfaces;
using AINewsAPI.Application.DTOs;

namespace AINewsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsItemDto>>> GetLatestNews()
        {
            try
            {
                var news = await _newsService.GetLatestNewsAsync();
                if (news == null || !news.Any())
                {
                    return NotFound("No news items available.");
                }
                return Ok(news);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "An error occurred while fetching news items.");
            }
        }
    }
}
