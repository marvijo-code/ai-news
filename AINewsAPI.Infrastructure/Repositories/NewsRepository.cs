using AINewsAPI.Domain.Entities;
using AINewsAPI.Domain.Interfaces;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace AINewsAPI.Infrastructure.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<NewsRepository> _logger;

        public NewsRepository(HttpClient httpClient, ILogger<NewsRepository> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<NewsItem>> GetLatestNewsAsync()
        {
            try
            {
                var baseUrl = "https://techcrunch.com/category/artificial-intelligence/";
                var html = await _httpClient.GetStringAsync(baseUrl);

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var articleNodes = htmlDocument.DocumentNode.SelectNodes("//article[contains(@class, 'post-block')]");

                var newsItems = new List<NewsItem>();

                if (articleNodes != null)
                {
                    foreach (var articleNode in articleNodes)
                    {
                        var titleNode = articleNode.SelectSingleNode(".//h2[contains(@class, 'post-block__title')]/a");
                        var descriptionNode = articleNode.SelectSingleNode(".//div[contains(@class, 'post-block__content')]");
                        var dateNode = articleNode.SelectSingleNode(".//time");

                        if (titleNode != null && descriptionNode != null && dateNode != null)
                        {
                            var title = titleNode.InnerText.Trim();
                            var description = descriptionNode.InnerText.Trim();
                            var articleUrl = titleNode.GetAttributeValue("href", "");
                            var dateString = dateNode.GetAttributeValue("datetime", "");

                            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var publishedAt)
                                && !string.IsNullOrWhiteSpace(title)
                                && !string.IsNullOrWhiteSpace(description)
                                && !string.IsNullOrWhiteSpace(articleUrl))
                            {
                                newsItems.Add(new NewsItem
                                {
                                    Title = title,
                                    Description = description,
                                    Url = articleUrl,
                                    PublishedAt = publishedAt.ToUniversalTime()
                                });
                            }
                        }

                        if (newsItems.Count >= 10) break; // Limit to 10 news items
                    }
                }

                if (!newsItems.Any())
                {
                    _logger.LogWarning("No news items were found. HTML content: {HtmlContent}", html);
                    newsItems.Add(CreateDefaultNewsItem());
                }

                return newsItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching news");
                return new List<NewsItem> { CreateDefaultNewsItem() };
            }
        }

        private NewsItem CreateDefaultNewsItem()
        {
            return new NewsItem
            {
                Title = "No news available",
                Description = "Unable to fetch news at this time. Please try again later.",
                Url = "https://techcrunch.com/category/artificial-intelligence/",
                PublishedAt = DateTime.UtcNow
            };
        }
    }
}
