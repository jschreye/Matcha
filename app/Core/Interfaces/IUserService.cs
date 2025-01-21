using Core.Data.DTOs;

namespace Core.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<bool> ValidateUser(string username, string password);
        Task CreateSessionAsync(int userId, string sessionToken, DateTime expiresAt);
        Task<bool> ValidateSessionAsync(string sessionToken);
        Task DeleteSessionAsync(string sessionToken);
    }
}