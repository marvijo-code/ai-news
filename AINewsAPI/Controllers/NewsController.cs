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
            var news = await _newsService.GetLatestNewsAsync();
            return Ok(news);
        }
    }
}
