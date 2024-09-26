using AINewsAPI.Domain.Entities;
using AINewsAPI.Domain.Interfaces;
using System.Text.Json;

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
            var response = await _httpClient.GetAsync("https://newsapi.org/v2/everything?q=AI&from=" + DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + "&sortBy=publishedAt&apiKey=YOUR_API_KEY");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var newsResponse = JsonSerializer.Deserialize<NewsApiResponse>(content);

            return newsResponse?.Articles.Select(a => new NewsItem
            {
                Title = a.Title,
                Description = a.Description,
                Url = a.Url,
                PublishedAt = a.PublishedAt
            }) ?? Enumerable.Empty<NewsItem>();
        }
    }

    public class NewsApiResponse
    {
        public List<Article> Articles { get; set; } = new List<Article>();
    }

    public class Article
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime PublishedAt { get; set; }
    }
}
