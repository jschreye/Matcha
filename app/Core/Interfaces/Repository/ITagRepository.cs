using Core.Data.Entity;

namespace Core.Interfaces.Repository
{
    public interface ITagRepository
    {

        Task<List<Tag>> GetTagsAsync();
        Task SaveUserTagsAsync(int userId, List<int> tagIds);
        Task<List<int>> GetTagUserAsync(int userId);
    }
}