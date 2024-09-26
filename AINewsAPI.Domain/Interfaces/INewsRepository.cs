using AINewsAPI.Domain.Entities;

namespace AINewsAPI.Domain.Interfaces
{
    public interface INewsRepository
    {
        Task<IEnumerable<NewsItem>> GetLatestNewsAsync();
    }
}
