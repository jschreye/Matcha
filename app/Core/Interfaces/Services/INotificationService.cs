using Core.Data.Entity;
using Core.Data.DTOs;
using System;
using System.Threading.Tasks;

public interface INotificationService
{
    event Action<int>? OnNotify;
    event Action<int>? OnNotificationsUpdated;
    Task NotifyMessageReceivedAsync(int receiverId, int senderId);
    Task<List<NotificationDto>> GetNotificationsForUserAsync(int userId);
    Task DeleteNotificationAsync(int notificationId, int userId);
}

