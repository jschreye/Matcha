using Core.Data.DTOs;

namespace Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
        Task<bool> ValidateUser(string username, string password);
        Task<bool> RegisterUser(RegisterDto registerDto);
        Task CreateSessionAsync(int userId, string sessionToken, DateTime expiresAt);
        Task<bool> ValidateSessionAsync(string sessionToken);
        Task DeleteSessionAsync(string sessionToken);
        Task<UserProfileDto?> GetUserProfileAsync(int userId);
        Task<bool> UpdateUserProfileAsync(UserProfileDto profileDto);
        Task<bool> UpdatePasswordAsync(int userId, string newPassword);
        Task UpdateLocationAsync(int userId, double latitude, double longitude, bool localisationIsActive = false);
        Task<bool> IsUserOnlineAsync(int userId);
        Task<bool> HasMatchWithUserAsync(int currentUserId, int otherUserId);
        Task<DateTime?> GetLastActivityAsync(int userId);
        Task DeleteSessionsByUserIdAsync(int userId);
    }
}