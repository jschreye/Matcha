using System.Threading.Tasks;
using Core.Data.Entity;
using System.Collections.Generic;

namespace Core.Interfaces.Repository
{
    public interface IMatchRepository
    {
        Task<bool> CreateMatchAsync(int userId1, int userId2);
        Task DeleteMatchAsync(int userId1, int userId2);
        Task<List<int>> GetMatchedUserIdsAsync(int userId);
        Task<bool> HasMatchAsync(int userId1, int userId2);
        Task<List<int>> GetUserMatchesAsync(int userId);
    }
}

