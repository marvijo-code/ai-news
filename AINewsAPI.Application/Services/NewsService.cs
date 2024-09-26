using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AINewsAPI.Application.DTOs;
using AINewsAPI.Application.Interfaces;
using AINewsAPI.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AINewsAPI.Application.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
        private readonly ILogger<NewsService> _logger;

        public NewsService(INewsRepository newsRepository, ILogger<NewsService> logger)
        {
            _newsRepository = newsRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<NewsItemDto>> GetLatestNewsAsync()
        {
            var news = await _newsRepository.GetLatestNewsAsync();
            _logger.LogInformation("Retrieved {Count} news items from repository", news.Count());

            return news.Select(n =>
            {
                var formattedDate = FormatPublishedDate(n.PublishedAt);
                _logger.LogInformation("Formatting date for news item. Original: {OriginalDate}, Formatted: {FormattedDate}", n.PublishedAt, formattedDate);

                return new NewsItemDto
                {
                    Title = n.Title,
                    Description = n.Description,
                    Url = n.Url,
                    PublishedAt = n.PublishedAt,
                    FormattedPublishedDate = formattedDate
                };
            });
        }

        private string FormatPublishedDate(DateTime publishedAt)
        {
            _logger.LogInformation("Formatting date: {PublishedAt}", publishedAt);

            if (publishedAt == default || publishedAt == DateTime.MinValue)
            {
                _logger.LogWarning("Published date is default or minimum value");
                return "Date not available";
            }

            var now = DateTime.UtcNow;
            var difference = now - publishedAt;

            _logger.LogInformation("Time difference: {Difference}", difference);

            if (difference.TotalMinutes < 1)
            {
                return "Just now";
            }
            else if (difference.TotalMinutes < 60)
            {
                return $"{(int)difference.TotalMinutes} minute{(difference.TotalMinutes < 2 ? "" : "s")} ago";
            }
            else if (difference.TotalHours < 24)
            {
                return $"{(int)difference.TotalHours} hour{(difference.TotalHours < 2 ? "" : "s")} ago";
            }
            else if (difference.TotalDays < 30)
            {
                return $"{(int)difference.TotalDays} day{(difference.TotalDays < 2 ? "" : "s")} ago";
            }
            else
            {
                return publishedAt.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture);
            }
        }
    }
}
