using Core.Data.Entity;

namespace Core.Interfaces.Services
{
    public interface ITagService
    {
        Task<List<Tag>> GetTagsAsync();
        Task SaveUserTagsAsync(int userId, List<int> tagIds);
        Task<List<int>> GetTagUserAsync(int userId);
        Task<List<string>> GetTagNamesForUserAsync(int userId);
    }
}