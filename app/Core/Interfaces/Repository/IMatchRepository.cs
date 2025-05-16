using System.Threading.Tasks;
using Core.Data.Entity;
using System.Collections.Generic;
using Core.Data.DTOs;

namespace Core.Interfaces.Repository
{
    public interface IMatchRepository
    {
        Task<bool> CreateMatchAsync(int userId1, int userId2);
        Task DeleteMatchAsync(int userId1, int userId2);
        Task<List<int>> GetMatchedUserIdsAsync(int userId);
        Task<bool> HasMatchAsync(int userId1, int userId2);
        Task<List<int>> GetUserMatchesAsync(int userId);
        Task<List<UserDto>> GetMatchesAsync(int userId);
        Task<bool> AddMatchAsync(int user1Id, int user2Id);
        Task<bool> AddLikeAsync(int userId, int likedUserId);
        Task<bool> HasLikeAsync(int userId, int likedUserId);
        Task<List<UserDto>> GetRecentLikesAsync(int userId);
    }
}

