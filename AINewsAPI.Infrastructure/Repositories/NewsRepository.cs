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
                _logger.LogInformation("Fetching news from {Url}", baseUrl);

                var html = await _httpClient.GetStringAsync(baseUrl);
                _logger.LogDebug("Received HTML content of length: {Length}", html.Length);

                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);

                var articleNodes = TryGetArticleNodes(htmlDocument);

                if (articleNodes == null || !articleNodes.Any())
                {
                    _logger.LogWarning("No article nodes found in the HTML. Content snippet: {HtmlSnippet}", html.Substring(0, Math.Min(1000, html.Length)));
                    return new List<NewsItem> { CreateDefaultNewsItem() };
                }

                _logger.LogInformation("Found {Count} article nodes", articleNodes.Count);

                var newsItems = new List<NewsItem>();

                foreach (var articleNode in articleNodes)
                {
                    var newsItem = ExtractNewsItem(articleNode);
                    if (newsItem != null)
                    {
                        newsItems.Add(newsItem);
                        if (newsItems.Count >= 10) break; // Limit to 10 news items
                    }
                }

                if (!newsItems.Any())
                {
                    _logger.LogWarning("No valid news items were extracted. HTML content snippet: {HtmlSnippet}", html.Substring(0, Math.Min(1000, html.Length)));
                    return new List<NewsItem> { CreateDefaultNewsItem() };
                }

                _logger.LogInformation("Successfully extracted {Count} news items", newsItems.Count);
                return newsItems;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while fetching news");
                return new List<NewsItem> { CreateDefaultNewsItem("Network error occurred. Please try again later.") };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching news");
                return new List<NewsItem> { CreateDefaultNewsItem() };
            }
        }

        private HtmlNodeCollection TryGetArticleNodes(HtmlDocument htmlDocument)
        {
            var selectors = new[]
            {
                "//article[contains(@class, 'post-block')]",
                "//div[contains(@class, 'post-block')]",
                "//div[contains(@class, 'article')]",
                "//div[contains(@class, 'post')]"
            };

            foreach (var selector in selectors)
            {
                var nodes = htmlDocument.DocumentNode.SelectNodes(selector);
                if (nodes != null && nodes.Any())
                {
                    _logger.LogInformation("Found article nodes using selector: {Selector}", selector);
                    return nodes;
                }
            }

            _logger.LogWarning("No article nodes found using any of the predefined selectors.");
            return null;
        }

        private NewsItem ExtractNewsItem(HtmlNode articleNode)
        {
            var titleNode = articleNode.SelectSingleNode(".//h2[contains(@class, 'post-block__title')]/a") ??
                            articleNode.SelectSingleNode(".//h2[contains(@class, 'title')]/a") ??
                            articleNode.SelectSingleNode(".//h3[contains(@class, 'title')]/a");

            var descriptionNode = articleNode.SelectSingleNode(".//div[contains(@class, 'post-block__content')]") ??
                                  articleNode.SelectSingleNode(".//div[contains(@class, 'excerpt')]") ??
                                  articleNode.SelectSingleNode(".//p[contains(@class, 'excerpt')]");

            var dateNode = articleNode.SelectSingleNode(".//time") ??
                           articleNode.SelectSingleNode(".//span[contains(@class, 'date')]");

            if (titleNode == null || descriptionNode == null || dateNode == null)
            {
                _logger.LogWarning("Missing required nodes for article. Article HTML: {ArticleHtml}", articleNode.OuterHtml);
                return null;
            }

            var title = titleNode.InnerText.Trim();
            var description = descriptionNode.InnerText.Trim();
            var articleUrl = titleNode.GetAttributeValue("href", "");
            var dateString = dateNode.GetAttributeValue("datetime", "") ?? dateNode.InnerText.Trim();

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description) || 
                string.IsNullOrWhiteSpace(articleUrl) || string.IsNullOrWhiteSpace(dateString))
            {
                _logger.LogWarning("Invalid article data. Title: {Title}, Description: {Description}, URL: {Url}, Date: {Date}",
                    title, description, articleUrl, dateString);
                return null;
            }

            if (!DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var publishedAt))
            {
                _logger.LogWarning("Failed to parse date: {DateString}", dateString);
                publishedAt = DateTime.UtcNow; // Use current time as fallback
            }

            return new NewsItem
            {
                Title = title,
                Description = description,
                Url = articleUrl,
                PublishedAt = publishedAt.ToUniversalTime()
            };
        }

        private NewsItem CreateDefaultNewsItem(string description = "Unable to fetch news at this time. Please try again later.")
        {
            return new NewsItem
            {
                Title = "No news available",
                Description = description,
                Url = "https://techcrunch.com/category/artificial-intelligence/",
                PublishedAt = DateTime.UtcNow
            };
        }
    }
}
