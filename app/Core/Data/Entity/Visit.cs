namespace Core.Data.Entity
{
    public class Visit
    {
        public int UserId { get; set; }
        public int VisitedUserId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}