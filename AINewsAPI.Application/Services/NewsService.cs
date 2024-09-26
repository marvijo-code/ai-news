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
                FormattedPublishedDate = FormatPublishedDate(n.PublishedAt)
            });
        }

        private string FormatPublishedDate(DateTime publishedAt)
        {
            if (publishedAt == default)
            {
                return "Date not available";
            }

            var now = DateTime.UtcNow;
            var difference = now - publishedAt;

            if (difference.TotalHours < 24)
            {
                return $"{(int)difference.TotalHours} hours ago";
            }
            else if (difference.TotalDays < 30)
            {
                return $"{(int)difference.TotalDays} days ago";
            }
            else
            {
                return publishedAt.ToString("MMMM dd, yyyy", CultureInfo.InvariantCulture);
            }
        }
    }
}
