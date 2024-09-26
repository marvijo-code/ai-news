using System.Net;
using System.Net.Http.Json;
using AINewsAPI.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AINewsAPI.IntegrationTests
{
    public class NewsApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public NewsApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetLatestNews_ReturnsSuccessStatusCode()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/News");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Actual status code: {response.StatusCode}");
        }

        [Fact]
        public async Task GetLatestNews_ReturnsNewsItems()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/News");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Actual status code: {response.StatusCode}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                var newsItems = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<NewsItemDto>>(content);

                Assert.NotNull(newsItems);
                Assert.NotEmpty(newsItems);
            }
        }

        [Fact]
        public async Task GetLatestNews_ReturnsValidNewsItems()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/News");

            // Assert
            Assert.True(response.IsSuccessStatusCode, $"Actual status code: {response.StatusCode}");

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                var newsItems = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<NewsItemDto>>(content);

                Assert.NotNull(newsItems);
                Assert.All(newsItems, item =>
                {
                    Assert.NotNull(item.Title);
                    Assert.NotEmpty(item.Title);
                    Assert.NotNull(item.Description);
                    Assert.NotNull(item.Url);
                    Assert.NotEmpty(item.Url);
                    Assert.True(item.PublishedAt != default);
                });
            }
        }
    }
}
