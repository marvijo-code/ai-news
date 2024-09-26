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
                            articleNode.SelectSingleNode(".//h3[contains(@class, 'title')]/a") ??
                            articleNode.SelectSingleNode(".//a[contains(@class, 'post-block__title__link')]") ??
                            articleNode.SelectSingleNode(".//h2[contains(@class, 'wp-block-post-title')]/a");

            var descriptionNode = articleNode.SelectSingleNode(".//div[contains(@class, 'post-block__content')]") ??
                                  articleNode.SelectSingleNode(".//div[contains(@class, 'excerpt')]") ??
                                  articleNode.SelectSingleNode(".//p[contains(@class, 'excerpt')]") ??
                                  articleNode.SelectSingleNode(".//div[contains(@class, 'wp-block-post-excerpt')]") ??
                                  articleNode.SelectSingleNode(".//p[contains(@class, 'wp-block-post-excerpt__excerpt')]");

            var dateNode = articleNode.SelectSingleNode(".//time") ??
                           articleNode.SelectSingleNode(".//span[contains(@class, 'date')]") ??
                           articleNode.SelectSingleNode(".//time[contains(@class, 'wp-block-post-date')]") ??
                           articleNode.SelectSingleNode(".//div[contains(@class, 'wp-block-post-date')]");

            if (titleNode == null)
            {
                _logger.LogWarning("Missing title node for article. Article HTML: {ArticleHtml}", articleNode.OuterHtml);
            }

            if (descriptionNode == null)
            {
                _logger.LogWarning("Missing description node for article. Article HTML: {ArticleHtml}", articleNode.OuterHtml);
            }

            if (dateNode == null)
            {
                _logger.LogWarning("Missing date node for article. Article HTML: {ArticleHtml}", articleNode.OuterHtml);
            }

            if (titleNode == null && descriptionNode == null && dateNode == null)
            {
                _logger.LogWarning("Missing all required nodes for article. Article HTML: {ArticleHtml}", articleNode.OuterHtml);
                return null;
            }

            var title = titleNode != null ? HtmlEntity.DeEntitize(titleNode.InnerText.Trim()) : "Title not available";
            var description = descriptionNode != null ? HtmlEntity.DeEntitize(descriptionNode.InnerText.Trim()) : "Description not available";
            var articleUrl = titleNode?.GetAttributeValue("href", "") ?? "";
            
            var dateString = dateNode?.GetAttributeValue("datetime", "") ?? 
                             dateNode?.InnerText.Trim() ?? 
                             DateTime.UtcNow.ToString("o");

            _logger.LogInformation("Extracted date string: {DateString}", dateString);

            if (string.IsNullOrWhiteSpace(articleUrl))
            {
                _logger.LogWarning("Invalid article URL. Title: {Title}, Description: {Description}, URL: {Url}, Date: {Date}",
                    title, description, articleUrl, dateString);
                return null;
            }

            var publishedAt = ParsePublishedDate(dateString);

            _logger.LogInformation("Extracted news item. Title: {Title}, URL: {Url}, Date string: {DateString}, Parsed date: {ParsedDate}",
                title, articleUrl, dateString, publishedAt);

            return new NewsItem
            {
                Title = title,
                Description = description,
                Url = articleUrl,
                PublishedAt = publishedAt
            };
        }

        private DateTime ParsePublishedDate(string dateString)
        {
            _logger.LogInformation("Attempting to parse date string: {DateString}", dateString);

            if (string.IsNullOrWhiteSpace(dateString))
            {
                _logger.LogWarning("Date string is null or empty. Using current UTC time.");
                return DateTime.UtcNow;
            }

            // Try parsing with specific formats
            string[] formats = { "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ssZ", "ddd MMM dd HH:mm:ss +ffff yyyy" };
            if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsedDate))
            {
                _logger.LogInformation("Successfully parsed date using specific format: {ParsedDate}", parsedDate);
                return parsedDate;
            }

            // Try parsing with invariant culture
            if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out parsedDate))
            {
                _logger.LogInformation("Successfully parsed date using invariant culture: {ParsedDate}", parsedDate);
                return parsedDate;
            }

            // Handle relative time strings
            if (dateString.Contains("ago", StringComparison.OrdinalIgnoreCase))
            {
                var parts = dateString.Split(' ');
                if (parts.Length >= 2 && int.TryParse(parts[0], out int value))
                {
                    DateTime result = DateTime.UtcNow;
                    if (dateString.Contains("minute", StringComparison.OrdinalIgnoreCase))
                    {
                        result = result.AddMinutes(-value);
                    }
                    else if (dateString.Contains("hour", StringComparison.OrdinalIgnoreCase))
                    {
                        result = result.AddHours(-value);
                    }
                    else if (dateString.Contains("day", StringComparison.OrdinalIgnoreCase))
                    {
                        result = result.AddDays(-value);
                    }
                    else if (dateString.Contains("week", StringComparison.OrdinalIgnoreCase))
                    {
                        result = result.AddDays(-value * 7);
                    }
                    else if (dateString.Contains("month", StringComparison.OrdinalIgnoreCase))
                    {
                        result = result.AddMonths(-value);
                    }
                    else if (dateString.Contains("year", StringComparison.OrdinalIgnoreCase))
                    {
                        result = result.AddYears(-value);
                    }
                    _logger.LogInformation("Successfully parsed relative date: {ParsedDate}", result);
                    return result;
                }
            }

            _logger.LogWarning("Failed to parse date: {DateString}. Using current UTC time.", dateString);
            return DateTime.UtcNow; // Use current time as fallback
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
