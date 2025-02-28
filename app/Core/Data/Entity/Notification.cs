namespace Core.Data.Entity
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SenderId { get; set; }
        public int? NotificationTypeId { get; set; }
        public bool Lu { get; set; }
        public DateTime Timestamp { get; set; }
        public NotificationType? NotificationType { get; set; }
    }

    public class NotificationType
    {
        public int Id { get; set; }
        public string Libelle { get; set; } = string.Empty;
    }
}