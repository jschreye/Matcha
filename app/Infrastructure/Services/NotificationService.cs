using Core.Data.Entity;
using Core.Data.DTOs;
using Core.Interfaces.Repository;

public class NotificationService : INotificationService
{
    public event Action<int>? OnNotify;
    // Nouvel événement pour notifier une mise à jour des notifications (ajout ou suppression)
    public event Action<int>? OnNotificationsUpdated;
    
    private readonly INotificationRepository _notificationRepository;

    public NotificationService(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task NotifyMessageReceivedAsync(int receiverId, int senderId)
    {
        int messageTypeId = 1;
        var notification = new Notification
        {
            UserId = receiverId,
            SenderId = senderId,
            NotificationTypeId = messageTypeId,
            Lu = false,
            Timestamp = DateTime.Now
        };

        await _notificationRepository.SaveNotificationAsync(notification);
        OnNotify?.Invoke(receiverId);
        OnNotificationsUpdated?.Invoke(receiverId);
    }
    
    public async Task<List<NotificationDto>> GetNotificationsForUserAsync(int userId)
    {
        return await _notificationRepository.GetNotificationsForUserAsync(userId);
    }

    public async Task DeleteNotificationAsync(int notificationId, int userId)
    {
        await _notificationRepository.DeleteNotificationAsync(notificationId);

        OnNotificationsUpdated?.Invoke(userId);
    }

    public async Task DeleteNotificationsByUserIdAsync(int userId)
    {
        await _notificationRepository.DeleteNotificationsByUserIdAsync(userId);

        OnNotificationsUpdated?.Invoke(userId);
    }
    public async Task NotifyProfileLikedAsync(int likedUserId, int likerUserId)
    {
        int notificationTypeId = 2;

        var notification = new Notification
        {
            UserId = likedUserId,
            SenderId = likerUserId,
            NotificationTypeId = notificationTypeId,
            Lu = false,
            Timestamp = DateTime.Now
        };

        await _notificationRepository.SaveNotificationAsync(notification);
        OnNotify?.Invoke(likedUserId);
        OnNotificationsUpdated?.Invoke(likedUserId);
    }
    public async Task NotifyProfileUnLikedAsync(int unLikedUserId, int unLikerUserId)
    {
        int notificationTypeId = 3;

        var notification = new Notification
        {
            UserId = unLikedUserId,
            SenderId = unLikerUserId,
            NotificationTypeId = notificationTypeId,
            Lu = false,
            Timestamp = DateTime.Now
        };

        await _notificationRepository.SaveNotificationAsync(notification);
        OnNotify?.Invoke(unLikedUserId);
        OnNotificationsUpdated?.Invoke(unLikedUserId);
    }
    public async Task NotifyVisitPofileAsync(int visitedUserId, int visitUserId)
    {
        int notificationTypeId = 4;

        var notification = new Notification
        {
            UserId = visitedUserId,
            SenderId = visitUserId,
            NotificationTypeId = notificationTypeId,
            Lu = false,
            Timestamp = DateTime.Now
        };

        await _notificationRepository.SaveNotificationAsync(notification);
        OnNotify?.Invoke(visitedUserId);
        OnNotificationsUpdated?.Invoke(visitedUserId);
    }
    public async Task NotifyMatchAsync(int matchedUserId, int matchUserId)
    {
        int notificationTypeId = 5;

        var notification = new Notification
        {
            UserId = matchedUserId,
            SenderId = matchUserId,
            NotificationTypeId = notificationTypeId,
            Lu = false,
            Timestamp = DateTime.Now
        };

        await _notificationRepository.SaveNotificationAsync(notification);
        OnNotify?.Invoke(matchedUserId);
        OnNotificationsUpdated?.Invoke(matchedUserId);
    }
    public async Task ClearAllNotificationsAsync(int userId)
    {
        await _notificationRepository.DeleteNotificationsByUserIdAsync(userId);
    }
}