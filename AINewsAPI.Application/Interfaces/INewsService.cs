using AINewsAPI.Application.DTOs;

namespace AINewsAPI.Application.Interfaces
{
    public interface INewsService
    {
        Task<IEnumerable<NewsItemDto>> GetLatestNewsAsync();
    }
}
