using Core.Data.Entity;

namespace Core.Repository
{
    public interface ISessionRepository
    {
        Task CreateSessionAsync(int userId, string sessionToken, DateTime expiresAt);
        Task<bool> ValidateSessionAsync(string sessionToken);
        Task DeleteSessionAsync(string sessionToken);
    }
}