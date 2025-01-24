namespace Core.Data.Entity
{
    public class Session
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SessionToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}