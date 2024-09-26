using System.Globalization;
using AINewsAPI.Application.DTOs;
using AINewsAPI.Application.Interfaces;
using AINewsAPI.Domain.Interfaces;

namespace AINewsAPI.Application.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;

        public NewsService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<IEnumerable<NewsItemDto>> GetLatestNewsAsync()
        {
            var news = await _newsRepository.GetLatestNewsAsync();
            return news.Select(n => new NewsItemDto
            {
                Title = n.Title,
                Description = n.Description,
                Url = n.Url,
                PublishedAt = n.PublishedAt,
                FormattedPublishedDate = n.PublishedAt.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture)
            });
        }
    }
}
