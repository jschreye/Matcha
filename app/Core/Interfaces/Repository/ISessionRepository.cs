using Core.Data.Entity;

namespace Core.Interfaces.Repository
{
    public interface ISessionRepository
    {
        Task CreateSessionAsync(int userId, string sessionToken, DateTime expiresAt);
        Task<bool> ValidateSessionAsync(string sessionToken);
        Task DeleteSessionAsync(string sessionToken);
        Task<bool> HasActiveSessionAsync(int userId);
        Task DeleteSessionsByUserIdAsync(int userId);
    }
}