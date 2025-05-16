using Core.Interfaces.Services;
using Core.Interfaces.Repository;
using Core.Data.DTOs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DashboardService : IDashboardService
{
    private readonly IMatchRepository _matchRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IVisitRepository _visitRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DashboardService> _logger;
    private readonly IPhotoService _photoService;

    public DashboardService(
        IMatchRepository matchRepository,
        IMessageRepository messageRepository,
        INotificationRepository notificationRepository,
        IVisitRepository visitRepository,
        IUserRepository userRepository,
        ILogger<DashboardService> logger,
        IPhotoService photoService)
    {
        _matchRepository = matchRepository;
        _messageRepository = messageRepository;
        _notificationRepository = notificationRepository;
        _visitRepository = visitRepository;
        _userRepository = userRepository;
        _logger = logger;
        _photoService = photoService;
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

    public async Task<List<UserDto>> GetRecentLikesAsync(int userId)
    {
        try
        {
            // Obtenir les utilisateurs qui ont récemment liké l'utilisateur courant
            var likes = await _matchRepository.GetRecentLikesAsync(userId);

            foreach (var like in likes)
            {
                var profilePhoto = await _photoService.GetProfilePhotoAsync(like.Id);
                if (profilePhoto != null && profilePhoto.ImageData != null)
                {
                    like.Photo = $"data:image/jpeg;base64,{Convert.ToBase64String(profilePhoto.ImageData)}";
                }
            }

            return likes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des likes récents pour l'utilisateur {UserId}", userId);
            return new List<UserDto>();
        }
    }

    public async Task<List<ConversationDto>> GetUnreadMessagesAsync(int userId)
    {
        try
        {
            // Obtenir les utilisateurs qui ont envoyé des messages non lus
            var unreadMessages = await _messageRepository.GetUnreadMessagesAsync(userId);

            foreach (var message in unreadMessages)
            {
                var profilePhoto = await _photoService.GetProfilePhotoAsync(message.UserId);
                if (profilePhoto != null && profilePhoto.ImageData != null)
                {
                    message.Photo = $"data:image/jpeg;base64,{Convert.ToBase64String(profilePhoto.ImageData)}";
                }
            }

            return unreadMessages;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des messages non lus pour l'utilisateur {UserId}", userId);
            return new List<ConversationDto>();
        }
    }
}
