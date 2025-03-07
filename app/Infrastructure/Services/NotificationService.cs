using Core.Data.Entity;
using Core.Data.DTOs;

using Core.Interfaces.Repository;
public class NotificationService : INotificationService
{
    public event Action<int>? OnNotify;
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
    }
    
    public async Task<List<NotificationDto>> GetNotificationsForUserAsync(int userId)
    {
        return await _notificationRepository.GetNotificationsForUserAsync(userId);
    }

    public async Task DeleteNotificationAsync(int notificationId)
    {
        await _notificationRepository.DeleteNotificationAsync(notificationId);
    }
}