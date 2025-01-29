namespace Core.Data.Entity
{
    public class Notification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Type { get; set; }
        public string? Contenu { get; set; }
        public bool Lu { get; set; }
        public DateTime Timestamp { get; set; }
    }
}