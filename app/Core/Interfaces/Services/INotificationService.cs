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
    Task NotifyProfileLikedAsync(int likedUserId, int likerUserId);
    Task NotifyProfileUnLikedAsync(int unLikedUserId, int unLikerUserId);
    Task NotifyVisitPofileAsync(int visitedUserId, int visitUserId);
    Task NotifyMatchAsync(int matchedUserId, int matchUserId);
    Task ClearAllNotificationsAsync(int userId);
}