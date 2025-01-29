namespace Core.Data.Entity
{
    public class Like
    {
        public int UserId { get; set; }
        public int LikedUserId { get; set; }
        public DateTime Timestamp { get; set; }
    }
}