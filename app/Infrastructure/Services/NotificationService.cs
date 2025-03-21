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
        // Optionnellement, vous pouvez notifier que la liste a été mise à jour
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
}