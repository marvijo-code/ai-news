using AINewsAPI.Domain.Entities;
using AINewsAPI.Domain.Interfaces;
using HtmlAgilityPack;
using System.Globalization;

namespace AINewsAPI.Infrastructure.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly HttpClient _httpClient;

        public NewsRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<NewsItem>> GetLatestNewsAsync()
        {
            var baseUrl = "https://techcrunch.com/category/artificial-intelligence/";
            var html = await _httpClient.GetStringAsync(baseUrl);

            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            var articleNodes = htmlDocument.DocumentNode.SelectNodes("//div[contains(@class, 'post-block')]");

            var newsItems = new List<NewsItem>();

            if (articleNodes != null)
            {
                foreach (var articleNode in articleNodes)
                {
                    var titleNode = articleNode.SelectSingleNode(".//h2[@class='post-block__title']/a");
                    var descriptionNode = articleNode.SelectSingleNode(".//div[@class='post-block__content']");
                    var dateNode = articleNode.SelectSingleNode(".//time");

                    if (titleNode != null && descriptionNode != null && dateNode != null)
                    {
                        var title = titleNode.InnerText.Trim();
                        var description = descriptionNode.InnerText.Trim();
                        var articleUrl = titleNode.GetAttributeValue("href", "");
                        var dateString = dateNode.GetAttributeValue("datetime", "");

                        if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var publishedAt))
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

            return newsItems;
            {
                var titleNode = articleNode.SelectSingleNode(".//h2[@class='post-block__title']/a");
                var descriptionNode = articleNode.SelectSingleNode(".//div[@class='post-block__content']");
                var dateNode = articleNode.SelectSingleNode(".//time");

                if (titleNode != null && descriptionNode != null && dateNode != null)
                {
                    var title = titleNode.InnerText.Trim();
                    var description = descriptionNode.InnerText.Trim();
                    var articleUrl = titleNode.GetAttributeValue("href", "");
                    var dateString = dateNode.GetAttributeValue("datetime", "");

                    if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var publishedAt))
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

            return newsItems;
        }
    }
}
