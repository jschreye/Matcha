using Core.Data.Entity;
using Core.Data.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces.Repository
{
    public interface INotificationRepository
    {
        Task SaveNotificationAsync(Notification notification);
        Task<List<NotificationDto>> GetNotificationsForUserAsync(int userId);
        Task DeleteNotificationAsync(int notificationId);
        Task MarkAsReadAsync(int notificationId);
        Task DeleteNotificationsByUserIdAsync(int userId);
        Task<int> CountUnreadLikesAsync(int userId);
        Task<int> CountUnreadMessagesAsync(int userId);
        Task MarkMessagesAsReadAsync(int userId, int senderId, int notificationTypeId);
        Task DeleteMessageNotificationAsync(int userId, int senderId);
        Task DeleteNotificationsByTypeAsync(int userId, string typeLibelle);
    }
}