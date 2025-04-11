using Core.Data.Entity;

namespace Core.Interfaces.Repository
{
    public interface IMatchRepository
    {
        Task CreateMatchAsync(int userId1, int userId2);
        Task DeleteMatchAsync(int userId1, int userId2);
        Task<List<int>> GetMatchedUserIdsAsync(int userId);
    }
}

