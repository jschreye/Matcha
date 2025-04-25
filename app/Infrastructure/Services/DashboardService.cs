using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Core.Data.DTOs;

public class DashboardService : IDashboardService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IUserRepository _userRepository;

    public DashboardService(
        IMatchRepository matchRepository,
        IMessageRepository messageRepository,
        INotificationRepository notificationRepository,
        IVisitRepository visitRepository,
        IUserRepository userRepository)
    {
        _matchRepository = matchRepository;
        _messageRepository = messageRepository;
        _notificationRepository = notificationRepository;
        _visitRepository = visitRepository;
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> GetMatchesAsync(int userId)
    {
        var matchIds = await _matchRepository.GetMatchedUserIdsAsync(userId);
        return await _userRepository.GetUsersByIdsAsync(matchIds);
    }

    public async Task<List<ConversationDto>> GetConversationsAsync(int userId)
    {
        var allConvos = await _messageRepository.GetConversationsForUserAsync(userId);

        var matchedIds = await _matchRepository.GetMatchedUserIdsAsync(userId);

        var activeConvos = allConvos
            .Where(c => matchedIds.Contains(c.UserId))
            .ToList();

        return activeConvos;
    }

    public async Task<NotificationStats> GetNotificationStatsAsync(int userId)
    {
        int likes = await _notificationRepository.CountUnreadLikesAsync(userId);
        int messages = await _notificationRepository.CountUnreadMessagesAsync(userId);

        return new NotificationStats
        {
            Likes = likes,
            Messages = messages
        };
    }

    public async Task<List<UserDto>> GetVisitedProfilesAsync(int userId)
    {
        var visitedIds = await _visitRepository.GetVisitedProfileIdsAsync(userId);
        return await _userRepository.GetUsersByIdsAsync(visitedIds);
    }

    public async Task<List<UserDto>> GetProfileVisitorsAsync(int userId)
    {
        var visitorIds = await _visitRepository.GetProfileVisitorsIdsAsync(userId);
        return await _userRepository.GetUsersByIdsAsync(visitorIds);
    }
}
