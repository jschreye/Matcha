namespace Core.Data.DTOs
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public int SenderId { get; set; }
        public string SenderUsername { get; set; } = string.Empty; 
        public string TypeLibelle { get; set; } = string.Empty; 
        public bool Lu { get; set; }
        public DateTime Timestamp { get; set; } 
    }
}