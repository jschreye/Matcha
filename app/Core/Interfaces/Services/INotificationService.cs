using Core.Data.Entity;
using Core.Data.DTOs;
using System;
using System.Threading.Tasks;

public interface INotificationService
{
    event Action<int>? OnNotify;
    Task NotifyMessageReceivedAsync(int receiverId, int senderId);
    Task<List<NotificationDto>> GetNotificationsForUserAsync(int userId);
    Task DeleteNotificationAsync(int notificationId);
}

