using Core.Data.DTOs;

namespace Core.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<List<UserDto>> GetMatchesAsync(int userId);
        Task<List<ConversationDto>> GetConversationsAsync(int userId);
        Task<NotificationStats> GetNotificationStatsAsync(int userId);
        Task<List<UserDto>> GetVisitedProfilesAsync(int userId);
        Task<List<UserDto>> GetProfileVisitorsAsync(int userId);
        Task<List<UserDto>> GetRecentLikesAsync(int userId);
        Task<List<ConversationDto>> GetUnreadMessagesAsync(int userId);
    }

}