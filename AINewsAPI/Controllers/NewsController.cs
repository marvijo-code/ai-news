using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AINewsAPI.Application.Interfaces;
using AINewsAPI.Application.DTOs;

namespace AINewsAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;
        private readonly ILogger<NewsController> _logger;

        public NewsController(INewsService newsService, ILogger<NewsController> logger)
        {
            _newsService = newsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsItemDto>>> GetLatestNews()
        {
            try
            {
                _logger.LogInformation("Fetching latest news");
                var news = await _newsService.GetLatestNewsAsync();
                if (news == null || !news.Any())
                {
                    _logger.LogWarning("No news items available");
                    return NotFound("No news items available.");
                }
                foreach (var item in news)
                {
                    _logger.LogInformation("News item: Title: {Title}, FormattedPublishedDate: {FormattedPublishedDate}", 
                        item.Title, item.FormattedPublishedDate);
                }
                _logger.LogInformation("Successfully retrieved {Count} news items", news.Count());
                return Ok(news);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching news items");
                return StatusCode(500, "An error occurred while fetching news items.");
            }
        }
    }
}
